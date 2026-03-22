using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace Warden.Core.Modrinth;

/// <summary>
///     The type of facet
/// </summary>
[EnumExtensions(MetadataSource = MetadataSource.DescriptionAttribute)]
public enum FacetType
{
    /// <summary>
    ///     The facet is for filtering by category
    /// </summary>
    [Description("categories")]
    Categories,

    /// <summary>
    ///     The facet is for filtering by Minecraft version
    /// </summary>
    [Description("versions")]
    Versions,

    /// <summary>
    ///     The facet is for filtering by license
    /// </summary>
    [Description("license")]
    License,

    /// <summary>
    ///     The facet is for filtering by project type
    /// </summary>
    [Description("project_type")]
    ProjectType,
}
