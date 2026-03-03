using CommunityToolkit.Mvvm.ComponentModel;

namespace Warden.Options;

public sealed partial class GeneralOptions : ObservableObject
{
    [ObservableProperty]
    public partial bool AutoUpdate { get; set; } = false;

    [ObservableProperty]
    public partial bool ShowConsole { get; set; } = false;

    [ObservableProperty]
    public partial bool IsSetup { get; set; } = true;
}
