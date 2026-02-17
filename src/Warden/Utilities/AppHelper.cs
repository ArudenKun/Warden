using Warden.Core.Extensions;

namespace Warden.Utilities;

public static class AppHelper
{
    public const string Name = nameof(Warden);
    public static bool IsDebug
#if DEBUG
        => true;
#else
        => false;
#endif
    public static string AppDir => AppDomain.CurrentDomain.BaseDirectory;

    public static string RoamingDir =>
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    public static string DataDir
    {
        get
        {
            if (
                !File.Exists(AppDir.CombinePath(".portable"))
                && !Directory.Exists(AppDir.CombinePath("data"))
                && !IsDebug
            )
                return RoamingDir.CombinePath(Name);
            var dataDir = AppDir.CombinePath("data");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }

            return dataDir;
        }
    }

    public static string LogsDir => DataDir.CombinePath("Logs");
    public const string SettingsFileName = "settings.json";
    public static string SettingsPath => DataDir.CombinePath(SettingsFileName);
    public static string ToolsDir => DataDir.CombinePath("Tools");
}
