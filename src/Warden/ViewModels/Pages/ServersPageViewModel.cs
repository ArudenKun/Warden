using Lucide.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace Warden.ViewModels.Pages;

[Dependency(ServiceLifetime.Singleton)]
public sealed partial class ServersPageViewModel : PageViewModel
{
    public override int Index => 1;
    public override LucideIconKind IconKind => LucideIconKind.Server;
}
