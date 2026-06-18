using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Interfaces;
using AspBaseProj.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AspBaseProj.Application.Features.Users;

/// <summary>
/// Query to get all users (admin/root).
/// </summary>
public sealed record GetUsersQuery : IRequest<List<UserDto>>;

/// <summary>
/// Handler for <see cref="GetUsersQuery"/>.
/// </summary>
public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .OrderBy(u => u.CreatedAt)
            .Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName!,
                Email = u.Email!,
                EmailConfirmed = u.EmailConfirmed,
                Group = u.Group,
                IsRoot = u.IsRoot,
                DisplayName = u.DisplayName,
                AvatarImageId = u.AvatarImageId,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                IsActive = u.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}

/// <summary>
/// Query to get a user by ID.
/// </summary>
public sealed record GetUserByIdQuery : IRequest<UserDto>
{
    public Guid UserId { get; init; }
}

/// <summary>
/// Handler for <see cref="GetUserByIdQuery"/>.
/// </summary>
public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IApplicationDbContext _context;

    public GetUserByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == request.UserId)
            .Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName!,
                Email = u.Email!,
                EmailConfirmed = u.EmailConfirmed,
                Group = u.Group,
                IsRoot = u.IsRoot,
                DisplayName = u.DisplayName,
                AvatarImageId = u.AvatarImageId,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                IsActive = u.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException($"User with ID {request.UserId} not found.");

        return user;
    }
}

/// <summary>
/// Command to change a user's group (root-only).
/// </summary>
public sealed record ChangeUserGroupCommand : IRequest
{
    public Guid UserId { get; init; }
    public UserGroup Group { get; init; }
}

/// <summary>
/// Validator for <see cref="ChangeUserGroupCommand"/>.
/// </summary>
public sealed class ChangeUserGroupCommandValidator : AbstractValidator<ChangeUserGroupCommand>
{
    public ChangeUserGroupCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Group)
            .IsInEnum().WithMessage("Invalid user group.");
    }
}

/// <summary>
/// Handler for <see cref="ChangeUserGroupCommand"/>.
/// </summary>
public sealed class ChangeUserGroupCommandHandler : IRequestHandler<ChangeUserGroupCommand>
{
    private readonly IApplicationDbContext _context;

    public ChangeUserGroupCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ChangeUserGroupCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User with ID {request.UserId} not found.");

        if (user.IsRoot)
            throw new InvalidOperationException("Cannot change the group of the root user.");

        user.Group = request.Group;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Command to activate/deactivate a user.
/// </summary>
public sealed record UpdateUserStatusCommand : IRequest
{
    public Guid UserId { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// Handler for <see cref="UpdateUserStatusCommand"/>.
/// </summary>
public sealed class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User with ID {request.UserId} not found.");

        if (user.IsRoot)
            throw new InvalidOperationException("Cannot deactivate the root user.");

        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Command to update user profile (display name, email, avatar).
/// </summary>
public sealed record UpdateProfileCommand : IRequest
{
    public Guid UserId { get; init; }
    public string? DisplayName { get; init; }
    public string Email { get; init; } = string.Empty;
    public Guid? AvatarImageId { get; init; }
}

/// <summary>
/// Validator for <see cref="UpdateProfileCommand"/>.
/// </summary>
public sealed class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters.");

        RuleFor(x => x.DisplayName)
            .MaximumLength(100).WithMessage("Display name must not exceed 100 characters.");
    }
}

/// <summary>
/// Handler for <see cref="UpdateProfileCommand"/>.
/// </summary>
public sealed class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateProfileCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User with ID {request.UserId} not found.");

        user.DisplayName = request.DisplayName;
        user.Email = request.Email;
        user.AvatarImageId = request.AvatarImageId;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Query to get admin dashboard statistics.
/// </summary>
public sealed record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;

/// <summary>
/// DTO for admin dashboard statistics.
/// </summary>
public sealed record DashboardStatsDto
{
    public int TotalUsers { get; init; }
    public int TotalPosts { get; init; }
    public int PublishedPosts { get; init; }
    public int PendingComments { get; init; }
}

/// <summary>
/// Handler for <see cref="GetDashboardStatsQuery"/>.
/// </summary>
public sealed class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var totalUsers = await _context.Users.CountAsync(cancellationToken);
        var totalPosts = await _context.Posts.CountAsync(cancellationToken);
        var publishedPosts = await _context.Posts.CountAsync(p => p.IsPublished, cancellationToken);
        var pendingComments = await _context.Comments
            .CountAsync(c => !c.IsApproved && !c.IsRejected && c.UserId == null, cancellationToken);

        return new DashboardStatsDto
        {
            TotalUsers = totalUsers,
            TotalPosts = totalPosts,
            PublishedPosts = publishedPosts,
            PendingComments = pendingComments
        };
    }
}
