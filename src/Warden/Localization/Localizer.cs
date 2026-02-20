using JetBrains.Annotations;
using Volo.Abp.Localization;

namespace Warden.Localization;

// https://github.com/tifish/Jeek.Avalonia.Localization
/// <summary>
///
/// </summary>
[PublicAPI]
public static class Localizer
{
    public static ILocalizer Current { get; private set; } = NullLocalizer.Instance;

    public static void SetLocalizer(ILocalizer localizer)
    {
        Current = localizer;
    }

    public static IReadOnlyList<ILanguageInfo> Languages => Current.Languages;

    public static ILanguageInfo Language
    {
        get => Current.Language;
        set => Current.Language = value;
    }

    public static int LanguageIndex
    {
        get => Current.LanguageIndex;
        set => Current.LanguageIndex = value;
    }

    public static string Get(string key) => Current.Get(key);

    public static event EventHandler? LanguageChanged
    {
        add => Current.LanguageChanged += value;
        remove => Current.LanguageChanged -= value;
    }
}
