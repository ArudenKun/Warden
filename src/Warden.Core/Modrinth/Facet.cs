using Warden.Core.Modrinth.Extensions;

namespace Warden.Core.Modrinth;

/// <summary>
///     A facet for the filtering of results
/// </summary>
// [FluentApi("{Name}Builder")]
public sealed class Facet
{
    internal Facet(FacetType type, string value, FacetOperator facetOperator = FacetOperator.Equals)
    {
        Type = type;
        Value = value;
        Operator = facetOperator;
    }

    public static IFacetBuilder Create() => new FacetBuilder();

    /// <summary>
    ///     The type of the facet
    /// </summary>
    public FacetType Type { get; }

    /// <summary>
    ///     The value of the facet
    /// </summary>
    public string Value { get; }

    /// <summary>
    ///     The operator to use for filtering
    /// </summary>
    public FacetOperator Operator { get; }

    /// <summary>
    ///     Creates a new facet for the filtering by category or loader
    /// </summary>
    /// <param name="category"> The loader or category to filter the results from </param>
    /// <param name="facetOperator"> The operator to use for filtering (defaults to equals) </param>
    /// <returns> The created facet </returns>
    internal static Facet Category(
        FacetCategory category,
        FacetOperator facetOperator = FacetOperator.Equals
    )
    {
        return new Facet(FacetType.Categories, category.ToStringFast(true), facetOperator);
    }

    /// <summary>
    ///     Creates a new facet for the filtering by Minecraft version
    /// </summary>
    /// <param name="value"> The minecraft version to filter the results from </param>
    /// <param name="facetOperator"> The operator to use for filtering (defaults to equals) </param>
    /// <returns> The created facet </returns>
    internal static Facet Version(string value, FacetOperator facetOperator = FacetOperator.Equals)
    {
        return new Facet(FacetType.Versions, value, facetOperator);
    }

    /// <summary>
    ///     Creates a new facet for the filtering by license
    /// </summary>
    /// <param name="value"> The license ID to filter the results from </param>
    /// <param name="facetOperator"> The operator to use for filtering (defaults to equals) </param>
    /// <returns> The created facet </returns>
    internal static Facet License(string value, FacetOperator facetOperator = FacetOperator.Equals)
    {
        return new Facet(FacetType.License, value, facetOperator);
    }

    /// <summary>
    ///     Creates a new facet for the filtering by project type
    /// </summary>
    /// <param name="projectType"> The project type to filter the results from </param>
    /// <param name="facetOperator"> The operator to use for filtering (defaults to equals) </param>
    /// <returns> The created facet </returns>
    internal static Facet ProjectType(
        ProjectType projectType,
        FacetOperator facetOperator = FacetOperator.Equals
    )
    {
        return new Facet(FacetType.ProjectType, projectType.ToModrinthString(), facetOperator);
    }

    /// <summary>
    ///     Returns a string representation of the facet, so that it is usable in API requests
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var facetKey = Type.ToStringFast(true);
        var operatorStr = Operator.ToStringFast(true);
        return $"{facetKey}{operatorStr}{Value}";
    }
}
