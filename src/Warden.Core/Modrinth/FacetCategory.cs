using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace Warden.Core.Modrinth;

[EnumExtensions(MetadataSource = MetadataSource.DescriptionAttribute)]
public enum FacetCategory
{
    [Description("babric")]
    Babric,

    [Description("bta-babric")]
    BtaBabric,

    [Description("bukkit")]
    Bukkit,

    [Description("bungeecord")]
    Bungeecord,

    [Description("canvas")]
    Canvas,

    [Description("datapack")]
    Datapack,

    [Description("fabric")]
    Fabric,

    [Description("folia")]
    Folia,

    [Description("forge")]
    Forge,

    [Description("geyser")]
    Geyser,

    [Description("iris")]
    Iris,

    [Description("java-agent")]
    JavaAgent,

    [Description("legacy-fabric")]
    LegacyFabric,

    [Description("liteloader")]
    Liteloader,

    [Description("minecraft")]
    Minecraft,

    [Description("modloader")]
    Modloader,

    [Description("neoforge")]
    Neoforge,

    [Description("nilloader")]
    Nilloader,

    [Description("optifine")]
    Optifine,

    [Description("ornithe")]
    Ornithe,

    [Description("paper")]
    Paper,

    [Description("purpur")]
    Purpur,

    [Description("quilt")]
    Quilt,

    [Description("rift")]
    Rift,

    [Description("spigot")]
    Spigot,

    [Description("sponge")]
    Sponge,

    [Description("vanilla")]
    Vanilla,

    [Description("velocity")]
    Velocity,

    [Description("waterfall")]
    Waterfall,
}
