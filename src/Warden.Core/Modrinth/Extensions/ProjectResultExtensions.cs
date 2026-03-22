namespace Warden.Core.Modrinth.Extensions;

public static class ProjectResultExtensions
{
    extension(ProjectResult projectResult)
    {
        public string Url => projectResult.GetDirectUrl();
    }
}
