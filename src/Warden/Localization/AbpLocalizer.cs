using System.Globalization;
using Microsoft.Extensions.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Threading;

namespace Warden.Localization;

public sealed class AbpLocalizer<TResource> : BaseLocalizer
    where TResource : class
{
    private readonly ILanguageProvider _languageProvider;
    private readonly IStringLocalizer<TResource> _stringLocalizer;

    public AbpLocalizer(
        ILanguageProvider languageProvider,
        IStringLocalizer<TResource> stringLocalizer
    )
    {
        _languageProvider = languageProvider;
        _stringLocalizer = stringLocalizer;

        var languages = AsyncHelper.RunSync(languageProvider.GetLanguagesAsync);
        FallbackLanguage = languages.FirstOrDefault() ?? new LanguageInfo("en", "en");
        CurrentLanguage =
            languages.FindByCulture(
                CultureInfo.CurrentCulture.Name,
                CultureInfo.CurrentUICulture.Name
            ) ?? FallbackLanguage;
    }

    public override ILanguageInfo FallbackLanguage { get; set; }
    protected override ILanguageInfo CurrentLanguage { get; set; }
    public override IReadOnlyList<ILanguageInfo> Languages =>
        AsyncHelper.RunSync(_languageProvider.GetLanguagesAsync);

    protected override void OnLanguageChanged() => Reload();

    public override void Reload()
    {
        Thread.CurrentThread.CurrentCulture = CurrentLanguage.ToCultureInfo();
        Thread.CurrentThread.CurrentUICulture = CurrentLanguage.ToCultureInfo();
        CultureInfo.CurrentCulture = CurrentLanguage.ToCultureInfo();
        CultureInfo.CurrentUICulture = CurrentLanguage.ToCultureInfo();
        CultureInfo.DefaultThreadCurrentCulture = CurrentLanguage.ToCultureInfo();
        CultureInfo.DefaultThreadCurrentUICulture = CurrentLanguage.ToCultureInfo();
        HasLoaded = true;
    }

    public override string Get(string key)
    {
        if (!HasLoaded)
            Reload();

        return _stringLocalizer.GetString(key);
    }
}
