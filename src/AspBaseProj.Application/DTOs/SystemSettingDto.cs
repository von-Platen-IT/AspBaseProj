namespace AspBaseProj.Application.DTOs;

/// <summary>
/// DTO for a system setting key-value pair.
/// </summary>
public sealed record SystemSettingDto
{
    public int Id { get; init; }
    public string Key { get; init; } = string.Empty;
    public string? Value { get; init; }
    public string? Description { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// DTO for updating a system setting.
/// </summary>
public sealed record UpdateSettingDto
{
    public string Key { get; init; } = string.Empty;
    public string? Value { get; init; }
}

/// <summary>
/// DTO for a media file reference.
/// </summary>
public sealed record MediaDto
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public Guid UploadedById { get; init; }
    public Guid? PostId { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Paginated result wrapper.
/// </summary>
public sealed record PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
