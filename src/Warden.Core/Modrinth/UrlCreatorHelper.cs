using Warden.Core.Modrinth.Extensions;

namespace Warden.Core.Modrinth;

internal static class UrlCreatorHelper
{
    /// <summary>
    ///     Base Modrinth URL
    /// </summary>
    public const string ModrinthUrl = "https://modrinth.com";

    /// <summary>
    ///     Returns direct link to the project on Modrinth
    /// </summary>
    /// <param name="project"> The project to get the link for </param>
    /// <returns></returns>
    public static string GetDirectUrl(this Project project)
    {
        return $"{ModrinthUrl}/{project.ProjectType.ToModrinthString()}/{project.Id}";
    }

    /// <summary>
    ///     Returns direct link to the user on Modrinth
    /// </summary>
    /// <param name="user"> The user to get the link for </param>
    /// <returns></returns>
    public static string GetDirectUrl(this User user)
    {
        return $"{ModrinthUrl}/user/{user.Id}";
    }

    /// <summary>
    ///     Returns direct link to the project of this search result on Modrinth
    /// </summary>
    /// <param name="projectResult"> The search result to get the link for </param>
    /// <returns></returns>
    public static string GetDirectUrl(this ProjectResult projectResult)
    {
        return $"{ModrinthUrl}/{projectResult.ProjectType.ToModrinthString()}/{projectResult.ProjectId}";
    }
}
