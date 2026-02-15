using System.Security.Cryptography;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Warden.Messaging;

namespace Warden.ViewModels;

[Dependency(ServiceLifetime.Singleton)]
public partial class MainWindowViewModel
    : ViewModel,
        IRequest<MainWindowViewModel, string>,
        IRecipient<string>
{
    public string Greeting =>
        $"Welcome to Avalonia! {WeakReferenceMessenger.Default.IsRegistered<RandomNumberGenerator>(this)}";

    public string Request(MainWindowViewModel instance)
    {
        return instance.Greeting;
    }

    public void Receive(string message) { }
}
