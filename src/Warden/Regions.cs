using System.Diagnostics.CodeAnalysis;

namespace Warden;

public static class Regions
{
    private const string Prefix = "Region.";

    public const string Main = Prefix + "Main";

    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    public static class Setup
    {
        private const string Prefix = Regions.Prefix + "Setup.";

        public const string Main = Prefix + "Main";
    }
}
