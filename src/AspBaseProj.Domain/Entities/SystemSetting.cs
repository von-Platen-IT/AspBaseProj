namespace AspBaseProj.Domain.Entities;

/// <summary>
/// Key-value store for configurable system settings such as blog title,
/// moderation process toggles, and other administrative options.
/// </summary>
public class SystemSetting
{
    /// <summary>
    /// Primary key (auto-increment).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Unique setting key (e.g., 'BlogTitle', 'ModerationEnabled').
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Setting value as string.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Human-readable description of the setting.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Last modification timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// FK to AppUser who last changed the setting.
    /// </summary>
    public Guid? UpdatedById { get; set; }

    // --- Navigation properties ---

    /// <summary>
    /// The user who last updated this setting (optional).
    /// </summary>
    public AppUser? UpdatedBy { get; set; }
}
