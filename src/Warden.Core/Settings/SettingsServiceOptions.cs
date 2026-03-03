using System.Text.Json;
using Warden.Core.Extensions;

namespace Warden.Core.Settings;

public class SettingsServiceOptions
{
    public string FilePath { get; set; } = AppContext.BaseDirectory.CombinePath("settings.json");

    public JsonSerializerOptions JsonSerializerOptions { get; set; } =
        JsonSerializerOptions.Default;
}
