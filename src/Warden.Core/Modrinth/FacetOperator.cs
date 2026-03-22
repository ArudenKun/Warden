using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace Warden.Core.Modrinth;

/// <summary>
///     The operator to use for filtering
/// </summary>
[EnumExtensions(MetadataSource = MetadataSource.DescriptionAttribute)]
public enum FacetOperator
{
    /// <summary>
    ///     Equals (=)
    /// </summary>
    [Description(":")]
    Equals,

    /// <summary>
    ///     Not equals (!=)
    /// </summary>
    [Description("!=")]
    NotEquals,

    /// <summary>
    ///     Greater than (>)
    /// </summary>
    [Description(">")]
    GreaterThan,

    /// <summary>
    ///     Greater than or equal (>=)
    /// </summary>
    [Description(">=")]
    GreaterThanOrEqual,

    /// <summary>
    ///     Less than (&lt;)
    /// </summary>
    [Description("<")]
    LessThan,

    /// <summary>
    ///     Less than or equal (&lt;=)
    /// </summary>
    [Description("<=")]
    LessThanOrEqual,
}
