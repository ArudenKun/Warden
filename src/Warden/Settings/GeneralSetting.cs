using CommunityToolkit.Mvvm.ComponentModel;

namespace Warden.Settings;

public sealed partial class GeneralSetting : ObservableObject
{
    [ObservableProperty]
    public partial bool AutoUpdate { get; set; } = false;

    [ObservableProperty]
    public partial bool ShowConsole { get; set; } = false;

    // public ConnectionStrings ConnectionStrings { get; set; } =
    //     new() { Default = $"Data Source={AppHelper.DataDir.CombinePath($"{AppHelper.Name}.db")}" };
}
