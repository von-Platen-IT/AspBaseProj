using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Interfaces;
using AspBaseProj.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MediaEntity = AspBaseProj.Domain.Entities.Media;

namespace AspBaseProj.Application.Features.Media;

/// <summary>
/// Command to upload a media file (image) to the database.
/// </summary>
public sealed record UploadMediaCommand : IRequest<MediaDto>
{
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public byte[] Data { get; init; } = [];
    public Guid UploadedById { get; init; }
    public Guid? PostId { get; init; }
}

/// <summary>
/// Validator for <see cref="UploadMediaCommand"/>.
/// </summary>
public sealed class UploadMediaCommandValidator : AbstractValidator<UploadMediaCommand>
{
    private static readonly HashSet<string> AllowedContentTypes =
    ["image/png", "image/jpeg", "image/gif", "image/webp", "image/svg+xml"];

    public UploadMediaCommandValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required.")
            .MaximumLength(255).WithMessage("File name must not exceed 255 characters.");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content type is required.")
            .Must(ct => AllowedContentTypes.Contains(ct))
            .WithMessage("Only image files (PNG, JPEG, GIF, WebP, SVG) are allowed.");

        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("File data is required.")
            .Must(d => d.Length <= 10 * 1024 * 1024)
            .WithMessage("File size must not exceed 10 MB.");

        RuleFor(x => x.UploadedById)
            .NotEmpty().WithMessage("Uploader ID is required.");
    }
}

/// <summary>
/// Handler for <see cref="UploadMediaCommand"/>.
/// </summary>
public sealed class UploadMediaCommandHandler : IRequestHandler<UploadMediaCommand, MediaDto>
{
    private readonly IApplicationDbContext _context;

    public UploadMediaCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MediaDto> Handle(UploadMediaCommand request, CancellationToken cancellationToken)
    {
        var media = new MediaEntity
        {
            Id = Guid.NewGuid(),
            FileName = request.FileName,
            ContentType = request.ContentType,
            Data = request.Data,
            FileSize = request.Data.Length,
            UploadedById = request.UploadedById,
            PostId = request.PostId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Media.Add(media);
        await _context.SaveChangesAsync(cancellationToken);

        return new MediaDto
        {
            Id = media.Id,
            FileName = media.FileName,
            ContentType = media.ContentType,
            FileSize = media.FileSize,
            UploadedById = media.UploadedById,
            PostId = media.PostId,
            CreatedAt = media.CreatedAt
        };
    }
}

/// <summary>
/// Query to get media binary data by ID (for rendering images).
/// </summary>
public sealed record GetMediaDataQuery : IRequest<(byte[] Data, string ContentType, string FileName)?>
{
    public Guid MediaId { get; init; }
}

/// <summary>
/// Handler for <see cref="GetMediaDataQuery"/>.
/// </summary>
public sealed class GetMediaDataQueryHandler : IRequestHandler<GetMediaDataQuery, (byte[] Data, string ContentType, string FileName)?>
{
    private readonly IApplicationDbContext _context;

    public GetMediaDataQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(byte[] Data, string ContentType, string FileName)?> Handle(GetMediaDataQuery request, CancellationToken cancellationToken)
    {
        var media = await _context.Media
            .AsNoTracking()
            .Where(m => m.Id == request.MediaId)
            .Select(m => new { m.Data, m.ContentType, m.FileName })
            .FirstOrDefaultAsync(cancellationToken);

        return media is not null ? (media.Data, media.ContentType, media.FileName) : null;
    }
}
