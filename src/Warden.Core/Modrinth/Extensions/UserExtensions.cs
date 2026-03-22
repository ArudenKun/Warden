namespace Warden.Core.Modrinth.Extensions;

public static class UserExtensions
{
    extension(User user)
    {
        public string Url => user.GetDirectUrl();
    }
}
