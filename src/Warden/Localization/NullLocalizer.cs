using System.Globalization;
using Volo.Abp.Localization;

namespace Warden.Localization;

public sealed class NullLocalizer : BaseLocalizer
{
    public static readonly NullLocalizer Instance = new();
    public override ILanguageInfo FallbackLanguage { get; set; } =
        new CultureInfo("en").ToLanguageInfo();
    protected override ILanguageInfo CurrentLanguage { get; set; } =
        new CultureInfo("en").ToLanguageInfo();

    protected override void OnLanguageChanged() { }

    public override void Reload() { }

    public override string Get(string key)
    {
        return string.Empty;
    }
}
