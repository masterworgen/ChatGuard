namespace ChatGuard.Infrastructure;

public class UserConfiguration
{
    public static string Path = nameof(UserConfiguration);
    public int ApiId { get; set; }
    public string ApiHash { get; set; } = null!;
    public string BotToken { get; set; } = null!;
};