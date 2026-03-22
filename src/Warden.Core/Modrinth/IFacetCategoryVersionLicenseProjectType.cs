namespace Warden.Core.Modrinth;

public interface IFacetCategoryVersionLicenseProjectType
{
    IFacetOrAndBuild Category(FacetCategory category, FacetOperator facetOperator = 0);

    IFacetOrAndBuild Version(string value, FacetOperator facetOperator = 0);

    IFacetOrAndBuild License(string value, FacetOperator facetOperator = 0);

    IFacetOrAndBuild ProjectType(ProjectType projectType, FacetOperator facetOperator = 0);
}
