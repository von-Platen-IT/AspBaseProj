using System.Text;
using AspBaseProj.Application.DependencyInjection;
using AspBaseProj.Domain.Enums;
using AspBaseProj.Infrastructure.DependencyInjection;
using AspBaseProj.Presentation.Authorization;
using AspBaseProj.Presentation.Options;
using AspBaseProj.Presentation.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AspBaseProj.Presentation.Extensions;

/// <summary>
/// Extension methods for configuring services in the Presentation layer.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Application layer services (MediatR, FluentValidation, pipeline behaviors).
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddApplication();
        return services;
    }

    /// <summary>
    /// Registers Infrastructure layer services (EF Core, Identity, repositories, services).
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        return services;
    }

    /// <summary>
    /// Configures API versioning with the Asp.Versioning library.
    /// </summary>
    public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    /// <summary>
    /// Configures dual authentication: Cookie for web (Razor Pages) and JWT Bearer for API.
    /// </summary>
    public static IServiceCollection AddWebAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();
        var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "SmartAuth";
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            options.DefaultChallengeScheme = "SmartAuth";
        })
        .AddPolicyScheme("SmartAuth", "Cookie or JWT", options =>
        {
            options.ForwardDefaultSelector = context =>
            {
                // Use JWT for API paths, Cookie for everything else
                var path = context.Request.Path;
                return path.StartsWithSegments("/api")
                    ? JwtBearerDefaults.AuthenticationScheme
                    : CookieAuthenticationDefaults.AuthenticationScheme;
            };
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.LoginPath = "/account/login";
            options.LogoutPath = "/account/logout";
            options.AccessDeniedPath = "/account/accessdenied";
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
            options.SlidingExpiration = true;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.RequireHttpsMetadata = !string.IsNullOrEmpty(configuration["ASPNETCORE_ENVIRONMENT"])
                && configuration["ASPNETCORE_ENVIRONMENT"] != "Development";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });

        return services;
    }

    /// <summary>
    /// Configures authorization policies for role-based access control.
    /// </summary>
    public static IServiceCollection AddWebAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.Author, policy =>
                policy.Requirements.Add(new GroupRequirement(UserGroup.Author)))
            .AddPolicy(Policies.Admin, policy =>
                policy.Requirements.Add(new GroupRequirement(UserGroup.Admin)))
            .AddPolicy(Policies.Root, policy =>
                policy.Requirements.Add(new GroupRequirement(UserGroup.Admin, requireRoot: true)))
            .AddPolicy(Policies.AuthorOrAdmin, policy =>
                policy.Requirements.Add(new GroupRequirement(UserGroup.Author, UserGroup.Admin)));

        services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, GroupRequirementHandler>();

        return services;
    }

    /// <summary>
    /// Configures native .NET OpenAPI documentation.
    /// </summary>
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        return services;
    }
}
