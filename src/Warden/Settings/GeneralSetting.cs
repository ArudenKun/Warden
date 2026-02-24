using CommunityToolkit.Mvvm.ComponentModel;

namespace Warden.Settings;

public sealed partial class GeneralSetting : ObservableObject
{
    [ObservableProperty]
    public partial bool AutoUpdate { get; set; } = false;

    [ObservableProperty]
    public partial bool ShowConsole { get; set; } = false;

    [ObservableProperty]
    public partial bool IsSetup { get; set; } = true;
}
