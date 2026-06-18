namespace AspBaseProj.Presentation.Options;

/// <summary>
/// Blog configuration loaded from the "Blog" configuration section.
/// </summary>
public sealed class BlogOptions
{
    public string RootUserName { get; init; } = "root";
    public string RootEmail { get; init; } = "root@localhost";
    public string RootPassword { get; init; } = "Root#12345!";
    public int PageSize { get; init; } = 10;
}
