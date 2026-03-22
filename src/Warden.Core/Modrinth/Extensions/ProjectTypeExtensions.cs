namespace Warden.Core.Modrinth.Extensions;

/// <summary>
///     Extensions for <see cref="ProjectType" />
/// </summary>
public static class ProjectTypeExtensions
{
    /// <summary>
    ///     Convert ProjectType to string for Modrinth API
    /// </summary>
    /// <param name="projectType"></param>
    /// <returns></returns>
    public static string ToModrinthString(this ProjectType projectType)
    {
        return projectType.ToStringFast(true);
    }

    /// <summary>
    ///     Convert ProjectType to string for Modrinth API
    /// </summary>
    /// <param name="projectType"></param>
    /// <returns></returns>
    public static string ToModrinthString(this ProjectType? projectType)
    {
        return projectType is null ? string.Empty : projectType.Value.ToStringFast(true);
    }
}
