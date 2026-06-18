using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AspBaseProj.Application.Features.Settings;

/// <summary>
/// Query to get all system settings.
/// </summary>
public sealed record GetSettingsQuery : IRequest<List<SystemSettingDto>>;

/// <summary>
/// Handler for <see cref="GetSettingsQuery"/>.
/// </summary>
public sealed class GetSettingsQueryHandler : IRequestHandler<GetSettingsQuery, List<SystemSettingDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSettingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SystemSettingDto>> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
    {
        return await _context.SystemSettings
            .AsNoTracking()
            .OrderBy(s => s.Key)
            .Select(s => new SystemSettingDto
            {
                Id = s.Id,
                Key = s.Key,
                Value = s.Value,
                Description = s.Description,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }
}

/// <summary>
/// Command to update a system setting.
/// </summary>
public sealed record UpdateSettingCommand : IRequest
{
    public string Key { get; init; } = string.Empty;
    public string? Value { get; init; }
    public Guid UpdatedById { get; init; }
}

/// <summary>
/// Validator for <see cref="UpdateSettingCommand"/>.
/// </summary>
public sealed class UpdateSettingCommandValidator : AbstractValidator<UpdateSettingCommand>
{
    public UpdateSettingCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("Setting key is required.")
            .MaximumLength(100).WithMessage("Key must not exceed 100 characters.");

        RuleFor(x => x.Value)
            .MaximumLength(2000).WithMessage("Value must not exceed 2000 characters.");
    }
}

/// <summary>
/// Handler for <see cref="UpdateSettingCommand"/>.
/// </summary>
public sealed class UpdateSettingCommandHandler : IRequestHandler<UpdateSettingCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateSettingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateSettingCommand request, CancellationToken cancellationToken)
    {
        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == request.Key, cancellationToken)
            ?? throw new KeyNotFoundException($"Setting with key '{request.Key}' not found.");

        setting.Value = request.Value;
        setting.UpdatedAt = DateTime.UtcNow;
        setting.UpdatedById = request.UpdatedById;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Query to get a single setting value by key.
/// </summary>
public sealed record GetSettingByKeyQuery : IRequest<string?>
{
    public string Key { get; init; } = string.Empty;
}

/// <summary>
/// Handler for <see cref="GetSettingByKeyQuery"/>.
/// </summary>
public sealed class GetSettingByKeyQueryHandler : IRequestHandler<GetSettingByKeyQuery, string?>
{
    private readonly IApplicationDbContext _context;

    public GetSettingByKeyQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string?> Handle(GetSettingByKeyQuery request, CancellationToken cancellationToken)
    {
        return await _context.SystemSettings
            .AsNoTracking()
            .Where(s => s.Key == request.Key)
            .Select(s => s.Value)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
