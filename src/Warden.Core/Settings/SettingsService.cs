using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using Antelcat.AutoGen.ComponentModel.Diagnostic;
using Volo.Abp.DependencyInjection;

namespace Warden.Core.Settings;

[AutoExtractInterface(Interfaces = [typeof(IDisposable)])]
public class SettingsService : ISettingsService
{
    private readonly ConcurrentDictionary<Type, Lazy<object>> _settings = new();
    private readonly ConcurrentDictionary<Type, JsonTypeInfo> _settingsJsonTypeInfo = new();
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private int _isDisposed;

    /// <summary>Initializes a new instance of the SettingsService.</summary>
    /// <summary>
    /// Initializes an instance of <see cref="SettingsService" />.
    /// </summary>
    /// <remarks>
    /// If you are relying on compile-time serialization, the <paramref name="jsonSerializerOptions" /> instance
    /// must have a valid <see cref="JsonSerializerOptions.TypeInfoResolver"/> set.
    /// </remarks>
    protected SettingsService(string filePath, JsonSerializerOptions jsonSerializerOptions)
    {
        FilePath = filePath;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    /// <summary>
    /// Initializes an instance of <see cref="SettingsService" />.
    /// </summary>
    protected SettingsService(string filePath, IJsonTypeInfoResolver jsonTypeInfoResolver)
        : this(
            filePath,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                TypeInfoResolver = jsonTypeInfoResolver,
            }
        ) { }

    /// <summary>
    /// Initializes an instance of <see cref="SettingsService" />.
    /// </summary>
    [RequiresUnreferencedCode(
        "This constructor initializes the settings manager with reflection-based serialization, which is incompatible with assembly trimming."
    )]
    [RequiresDynamicCode(
        "This constructor initializes the settings manager with reflection-based serialization, which is incompatible with ahead-of-time compilation."
    )]
    protected SettingsService(string filePath)
        : this(filePath, new DefaultJsonTypeInfoResolver()) { }

    public string FilePath { get; }

    public event EventHandler<SettingsErrorEventArgs>? ErrorOccurred;

    public T Get<T>()
        where T : class, new() =>
        (T)_settings.GetOrAdd(typeof(T), key => new Lazy<object>(() => Load(key))).Value;

    public void Save()
    {
        var settingsList = _settings.ToArray(); // Snapshot of current loaded settings
        if (settingsList.Length < 1)
            return;

        try
        {
            JsonObject? rootNode = null;

            // 1. Try to load existing file to preserve settings not currently in memory
            if (File.Exists(FilePath))
            {
                try
                {
                    using var stream = new FileStream(
                        FilePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read
                    );
                    // Parse into a mutable JsonNode
                    rootNode = JsonNode.Parse(stream)?.AsObject();
                }
                catch (Exception ex)
                {
                    // _logger.LogError(ex, "Error reading settings file");
                    OnErrorOccurred(
                        new SettingsErrorEventArgs(ex, SettingsServiceAction.Save, FilePath)
                    );
                }
            }
            else
            {
                Directory.CreateDirectory(
                    Path.GetDirectoryName(FilePath)
                        ?? throw new DirectoryNotFoundException("Directory not found: " + FilePath)
                );
            }

            // 2. Initialize if null (new file or corrupted read)
            rootNode ??= new JsonObject();

            // 3. Update the JsonObject with current in-memory settings
            try
            {
                UpdateJsonNode(rootNode, settingsList);
            }
            catch (Exception ex)
            {
                OnErrorOccurred(
                    new SettingsErrorEventArgs(ex, SettingsServiceAction.Save, FilePath)
                );
                // If update failed, reset to empty to ensure we at least save the current state
                rootNode = new JsonObject();
                UpdateJsonNode(rootNode, settingsList);
            }

            // 4. Write back to disk
            using (
                var stream = new FileStream(
                    FilePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.Write
                )
            )
            {
                using var writer = new Utf8JsonWriter(
                    stream,
                    new JsonWriterOptions { Indented = true }
                );
                rootNode.WriteTo(writer, _jsonSerializerOptions);
            }
        }
        catch (Exception ex)
        {
            OnErrorOccurred(new SettingsErrorEventArgs(ex, SettingsServiceAction.Save, FilePath));
        }
    }

    private void UpdateJsonNode(JsonObject root, KeyValuePair<Type, Lazy<object>>[] settingsList)
    {
        foreach (var kvp in settingsList)
        {
            // Ensure we only save values that have actually been instantiated
            if (kvp.Value.IsValueCreated)
            {
                var typeKey = GetTypeKey(kvp.Key);
                var settingObj = kvp.Value.Value;
                var jsonTypeInfo = _settingsJsonTypeInfo.GetOrAdd(
                    kvp.Key,
                    k => _jsonSerializerOptions.GetTypeInfo(k)
                );

                // Serialize the specific setting object to a node
                var settingNode = JsonSerializer.SerializeToNode(settingObj, jsonTypeInfo);

                // Update or Add to the root object
                root[typeKey] = settingNode;
            }
        }
    }

    /// <summary>Calls save to ensure that the latest changes are persisted.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) != 0)
            return;
        if (disposing)
            Save();
    }

    private void OnErrorOccurred(SettingsErrorEventArgs e)
    {
        // _logger.LogError(e.Error, "An error occurred on [{Action}]", e.Action);
        ErrorOccurred?.Invoke(this, e);
    }

    private object Load(Type type)
    {
        object? settingObject = null;
        try
        {
            settingObject = LoadCore(type);
        }
        catch (Exception ex)
        {
            OnErrorOccurred(new SettingsErrorEventArgs(ex, SettingsServiceAction.Open, FilePath));
        }

        return settingObject ?? Activator.CreateInstance(type)!;
    }

    private object? LoadCore(Type type)
    {
        if (!File.Exists(FilePath))
            return null;

        var jsonTypeInfo = _settingsJsonTypeInfo.GetOrAdd(
            type,
            k => _jsonSerializerOptions.GetTypeInfo(k)
        );

        try
        {
            using var stream = new FileStream(
                FilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read
            );

            // Parse the whole file structure
            var rootNode = JsonNode.Parse(stream);
            if (rootNode is JsonObject rootObj)
            {
                var typeKey = GetTypeKey(type);

                // Find the key corresponding to this type
                if (rootObj.TryGetPropertyValue(typeKey, out var section) && section != null)
                {
                    return section.Deserialize(jsonTypeInfo);
                }
            }
        }
        catch (JsonException)
        {
            // JSON might be corrupted; return null so a default instance is created.
            // The Save() method will handle overwriting the bad file later.
        }

        return null;
    }

    /// <summary>
    /// Generates a stable key for the dictionary based on the Type.
    /// Using FullName handles nested types correctly.
    /// </summary>
    private static string GetTypeKey(Type type)
    {
        string? name = null;
        while (type.IsNested)
        {
            name = name == null ? type.Name : type.Name + "." + name;
            type = type.DeclaringType!;
        }

        name = name == null ? type.Name : type.Name + "." + name;
        return name;
    }
}
