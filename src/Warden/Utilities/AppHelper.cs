using Warden.Core.Extensions;

namespace Warden.Utilities;

public static class AppHelper
{
    public static bool IsDebug
#if DEBUG
        => true;
#else
        => false;
#endif
    public static Version Version =>
        typeof(AppHelper).Assembly.GetName().Version ?? new Version(0, 1, 0);

    public static string VersionString => Version.ToString();

    public static string AppDir => AppDomain.CurrentDomain.BaseDirectory;

    public static string ContentRootDir =>
        AppContext.BaseDirectory[
            ..AppContext.BaseDirectory.LastIndexOf("bin", StringComparison.OrdinalIgnoreCase)
        ];

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
                return RoamingDir.CombinePath(AppConsts.Name);
            var dataDir = AppDir.CombinePath("data");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }

            return dataDir;
        }
    }

    public static string LogsDir => DataDir.CombinePath("Logs");
    public static string SettingsPath => DataDir.CombinePath(AppConsts.SettingsFileName);
    public static string ToolsDir => DataDir.CombinePath("Tools");

    public static string BackgroundJobsWorkersConnectionString =>
        DataDir.CombinePath("background_jobs_workers.db");
}
