using Warden.Core.Options;

namespace Warden.Options;

[Option("Test2")]
public class Test2Options
{
    public string Text { get; set; } = string.Empty;
}
