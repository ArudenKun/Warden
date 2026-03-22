namespace Warden.Core.Modrinth;

public interface IFacetOrAndBuild
{
    IFacetCategoryVersionLicenseProjectType Or();

    IFacetCategoryVersionLicenseProjectType And();

    string Build();
}
