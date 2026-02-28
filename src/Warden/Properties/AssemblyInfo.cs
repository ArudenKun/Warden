using Avalonia.Metadata;
using R3.ObservableEvents;

[assembly: GenerateStaticEventObservables(typeof(TaskScheduler))]
[assembly: XmlnsDefinition("https://github.com/arudenkun/warden", "Warden")]
[assembly: XmlnsDefinition("https://github.com/arudenkun/warden", "Warden.Converters")]
[assembly: XmlnsDefinition("https://github.com/arudenkun/warden", "Warden.Models")]
[assembly: XmlnsDefinition("https://github.com/arudenkun/warden", "Warden.Utilities")]
[assembly: XmlnsDefinition("https://github.com/arudenkun/warden", "Warden.ViewModels")]
[assembly: XmlnsDefinition("https://github.com/arudenkun/warden", "Warden.ViewModels.Components")]
[assembly: XmlnsDefinition("https://github.com/arudenkun/warden", "Warden.ViewModels.Dialogs")]
[assembly: XmlnsDefinition("https://github.com/arudenkun/warden", "Warden.Views")]
[assembly: XmlnsDefinition("https://github.com/arudenkun/warden", "Warden.Views.Components")]
[assembly: XmlnsDefinition("https://github.com/arudenkun/warden", "Warden.Views.Dialogs")]
[assembly: XmlnsDefinition("https://github.com/arudenkun/warden", "Warden.Controls")]
[assembly: XmlnsDefinition("https://github.com/arudenkun/warden", "Warden.Controls.WebView")]

[assembly: XmlnsDefinition("https://github.com/arudenkun/warden", "Warden.Core")]
[assembly: XmlnsPrefix("https://github.com/arudenkun/warden", "warden")]
