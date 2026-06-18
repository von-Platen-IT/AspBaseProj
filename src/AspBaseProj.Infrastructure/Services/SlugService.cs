using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AspBaseProj.Infrastructure.Services;

/// <summary>
/// Implementation of <see cref="ISlugService"/> for generating URL-friendly slugs.
/// </summary>
public sealed class SlugService : ISlugService
{
    public string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        // Normalize to lowercase and remove diacritics
        var normalized = title.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var withoutDiacritics = new string(normalized
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());

        // Replace spaces and special characters with hyphens
        var slug = Regex.Replace(withoutDiacritics, @"[^a-z0-9\s-]", string.Empty);
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');

        return slug;
    }

    public async Task<string> GenerateUniqueSlugAsync(
        string title,
        Func<string, Task<bool>> slugExists,
        CancellationToken cancellationToken = default)
    {
        var baseSlug = GenerateSlug(title);
        if (string.IsNullOrEmpty(baseSlug))
            baseSlug = "post";

        var slug = baseSlug;
        var suffix = 1;

        while (await slugExists(slug))
        {
            slug = $"{baseSlug}-{suffix}";
            suffix++;
        }

        return slug;
    }
}
