namespace AspBaseProj.Presentation.Authorization;

/// <summary>
/// Authorization policy names used across the application.
/// </summary>
public static class Policies
{
    /// <summary>
    /// Requires the user to be in the Author group.
    /// </summary>
    public const string Author = "AuthorPolicy";

    /// <summary>
    /// Requires the user to be in the Admin group.
    /// </summary>
    public const string Admin = "AdminPolicy";

    /// <summary>
    /// Requires the user to be root (IsRoot = true).
    /// </summary>
    public const string Root = "RootPolicy";

    /// <summary>
    /// Requires the user to be in the Author or Admin group (or root).
    /// </summary>
    public const string AuthorOrAdmin = "AuthorOrAdminPolicy";
}
