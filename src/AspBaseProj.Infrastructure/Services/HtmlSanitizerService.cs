using Ganss.Xss;

namespace AspBaseProj.Infrastructure.Services;

/// <summary>
/// Implementation of <see cref="IHtmlSanitizerService"/> using the Ganss.XSS HtmlSanitizer library.
/// Allows safe HTML tags and attributes for rich-text content while removing XSS vectors.
/// </summary>
public sealed class HtmlSanitizerService : IHtmlSanitizerService
{
    private readonly HtmlSanitizer _sanitizer;

    public HtmlSanitizerService()
    {
        _sanitizer = new HtmlSanitizer();

        // Allow common rich-text formatting tags
        _sanitizer.AllowedTags.Add("img");
        _sanitizer.AllowedTags.Add("figure");
        _sanitizer.AllowedTags.Add("figcaption");
        _sanitizer.AllowedTags.Add("video");
        _sanitizer.AllowedTags.Add("source");
        _sanitizer.AllowedTags.Add("blockquote");
        _sanitizer.AllowedTags.Add("pre");
        _sanitizer.AllowedTags.Add("code");
        _sanitizer.AllowedTags.Add("hr");
        _sanitizer.AllowedTags.Add("span");
        _sanitizer.AllowedTags.Add("div");
        _sanitizer.AllowedTags.Add("p");
        _sanitizer.AllowedTags.Add("br");
        _sanitizer.AllowedTags.Add("a");
        _sanitizer.AllowedTags.Add("ul");
        _sanitizer.AllowedTags.Add("ol");
        _sanitizer.AllowedTags.Add("li");
        _sanitizer.AllowedTags.Add("strong");
        _sanitizer.AllowedTags.Add("em");
        _sanitizer.AllowedTags.Add("b");
        _sanitizer.AllowedTags.Add("i");
        _sanitizer.AllowedTags.Add("u");
        _sanitizer.AllowedTags.Add("h1");
        _sanitizer.AllowedTags.Add("h2");
        _sanitizer.AllowedTags.Add("h3");
        _sanitizer.AllowedTags.Add("h4");
        _sanitizer.AllowedTags.Add("h5");
        _sanitizer.AllowedTags.Add("h6");
        _sanitizer.AllowedTags.Add("table");
        _sanitizer.AllowedTags.Add("thead");
        _sanitizer.AllowedTags.Add("tbody");
        _sanitizer.AllowedTags.Add("tr");
        _sanitizer.AllowedTags.Add("th");
        _sanitizer.AllowedTags.Add("td");

        // Allow src, alt, title, class on images
        _sanitizer.AllowedAttributes.Add("src");
        _sanitizer.AllowedAttributes.Add("alt");
        _sanitizer.AllowedAttributes.Add("title");
        _sanitizer.AllowedAttributes.Add("class");
        _sanitizer.AllowedAttributes.Add("href");
        _sanitizer.AllowedAttributes.Add("target");
        _sanitizer.AllowedAttributes.Add("rel");
        _sanitizer.AllowedAttributes.Add("width");
        _sanitizer.AllowedAttributes.Add("height");
        _sanitizer.AllowedAttributes.Add("controls");
        _sanitizer.AllowedAttributes.Add("style");

        // Allow data attributes for embedded media references
        _sanitizer.AllowedAttributes.Add("data-media-id");

        // Allow style attribute with safe properties
        _sanitizer.AllowedCssProperties.Add("text-align");
        _sanitizer.AllowedCssProperties.Add("margin");
        _sanitizer.AllowedCssProperties.Add("padding");
        _sanitizer.AllowedCssProperties.Add("width");
        _sanitizer.AllowedCssProperties.Add("height");
        _sanitizer.AllowedCssProperties.Add("max-width");
        _sanitizer.AllowedCssProperties.Add("color");
        _sanitizer.AllowedCssProperties.Add("background-color");
        _sanitizer.AllowedCssProperties.Add("font-weight");
        _sanitizer.AllowedCssProperties.Add("font-style");
    }

    public string Sanitize(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        return _sanitizer.Sanitize(html);
    }
}
