using System.Globalization;
using Volo.Abp.Localization;

namespace Warden.Localization;

public static class LocalizationExtensions
{
    public static CultureInfo ToCultureInfo(this ILanguageInfo languageInfo)
    {
        return CultureInfo.GetCultureInfo(languageInfo.CultureName);
    }

    public static LanguageInfo ToLanguageInfo(this CultureInfo cultureInfo)
    {
        return new LanguageInfo(cultureInfo.Name, cultureInfo.Name, cultureInfo.DisplayName);
    }
}
