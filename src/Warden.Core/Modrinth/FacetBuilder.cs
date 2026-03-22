namespace Warden.Core.Modrinth;

internal class FacetBuilder : IFacetBuilder, IFacetOrAndBuild
{
    private readonly FacetCollection _groups = [];
    private readonly List<Facet> _currentGroup = [];

    public void Facet(FacetType type, string value, FacetOperator facetOperator) =>
        _currentGroup.Add(new Facet(type, value, facetOperator));

    public IFacetOrAndBuild Category(FacetCategory category, FacetOperator facetOperator = 0)
    {
        _currentGroup.Add(Modrinth.Facet.Category(category, facetOperator));
        return this;
    }

    public IFacetOrAndBuild Version(string value, FacetOperator facetOperator = 0)
    {
        _currentGroup.Add(Modrinth.Facet.Version(value, facetOperator));
        return this;
    }

    public IFacetOrAndBuild License(string value, FacetOperator facetOperator = 0)
    {
        _currentGroup.Add(Modrinth.Facet.License(value, facetOperator));
        return this;
    }

    public IFacetOrAndBuild ProjectType(ProjectType projectType, FacetOperator facetOperator = 0)
    {
        _currentGroup.Add(Modrinth.Facet.ProjectType(projectType, facetOperator));
        return this;
    }

    IFacetOrAndBuild IFacetCategoryVersionLicenseProjectType.Category(
        FacetCategory category,
        FacetOperator facetOperator
    )
    {
        Category(category, facetOperator);
        return this;
    }

    IFacetOrAndBuild IFacetCategoryVersionLicenseProjectType.Version(
        string value,
        FacetOperator facetOperator
    )
    {
        Version(value, facetOperator);
        return this;
    }

    IFacetOrAndBuild IFacetCategoryVersionLicenseProjectType.License(
        string value,
        FacetOperator facetOperator
    )
    {
        License(value, facetOperator);
        return this;
    }

    IFacetOrAndBuild IFacetCategoryVersionLicenseProjectType.ProjectType(
        ProjectType projectType,
        FacetOperator facetOperator
    )
    {
        ProjectType(projectType, facetOperator);
        return this;
    }

    IFacetCategoryVersionLicenseProjectType IFacetOrAndBuild.Or()
    {
        return this;
    }

    IFacetCategoryVersionLicenseProjectType IFacetOrAndBuild.And()
    {
        _groups.Add([.. _currentGroup]);
        _currentGroup.Clear();
        return this;
    }

    string IFacetOrAndBuild.Build()
    {
        if (_currentGroup.Count > 0)
            _groups.Add([.. _currentGroup]);
        return _groups.ToString();
    }
}
