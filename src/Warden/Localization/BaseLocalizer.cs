using Antelcat.AutoGen.ComponentModel.Diagnostic;
using JetBrains.Annotations;
using Volo.Abp.Localization;

namespace Warden.Localization;

// https://github.com/tifish/Jeek.Avalonia.Localization
[AutoExtractInterface(NamingTemplate = "ILocalizer")]
[PublicAPI]
public abstract class BaseLocalizer : ILocalizer
{
    // Fallback language when the current language is not found
    public abstract ILanguageInfo FallbackLanguage { get; set; }

    protected List<ILanguageInfo> CurrentLanguages { get; } = [];

    // List of available languages, e.g. ["en", "zh"]
    public virtual IReadOnlyList<ILanguageInfo> Languages
    {
        get
        {
            if (!HasLoaded)
                Reload();

            return CurrentLanguages;
        }
    }

    protected abstract ILanguageInfo CurrentLanguage { get; set; }

    // Current language, e.g. "en"
    public ILanguageInfo Language
    {
        get => CurrentLanguage;
        set
        {
            if (EqualityComparer<ILanguageInfo>.Default.Equals(CurrentLanguage, value))
                return;

            CurrentLanguage = value;
            OnLanguageChanged();
            FireLanguageChanged();
        }
    }

    // Index of current language in Languages
    public int LanguageIndex
    {
        get;
        set
        {
            if (field == value)
                return;

            if (value < 0 || value >= CurrentLanguages.Count)
                return;

            field = value;

            Language = CurrentLanguages[field];
        }
    } = -1;

    // Must be called after _language or _languages are changed
    protected virtual void ValidateLanguage()
    {
        var languageIndex = CurrentLanguages.IndexOf(CurrentLanguage);
        if (languageIndex != -1)
        {
            LanguageIndex = languageIndex;
            return;
        }

        languageIndex = CurrentLanguages.IndexOf(FallbackLanguage);
        if (languageIndex == -1)
            throw new KeyNotFoundException(CurrentLanguage.CultureName);

        LanguageIndex = languageIndex;
        CurrentLanguage = FallbackLanguage;
    }

    // Implementations deal with loading language strings
    protected abstract void OnLanguageChanged();

    protected bool HasLoaded { get; set; }

    // Reload language strings
    public abstract void Reload();

    // Get language string by key
    public abstract string Get(string key);

    public event EventHandler? LanguageChanged;

    // Fire language changed
    public void FireLanguageChanged()
    {
        LanguageChanged?.Invoke(null, EventArgs.Empty);
    }
}
