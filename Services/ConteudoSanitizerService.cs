using System.Text.RegularExpressions;
using Ganss.Xss;

namespace NewsPortal.Api.Services;

/// <summary>
/// Sanitiza o HTML gerado pelo editor rico (Quill) antes de persistir.
/// Mesmo sendo só Admin/Editor que escrevem artigos, uma conta comprometida
/// ou um editor descuidado não deve conseguir injetar &lt;script&gt;, handlers
/// inline (onerror, onclick) ou iframes de origem arbitrária — o conteúdo é
/// lido por qualquer visitante do site via [innerHTML] no frontend.
/// </summary>
public class ConteudoSanitizerService
{
    // Hosts que podem aparecer como src de <iframe> (embed de vídeo do editor).
    // Qualquer coisa fora disso — incluindo domínios parecidos tipo
    // "youtube.com.evil.com" — é removida por completo no pós-processamento
    // abaixo, então isso não depende só do sanitizador permitir a tag.
    private static readonly HashSet<string> HostsDeEmbedPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "www.youtube.com",
        "youtube.com",
        "www.youtube-nocookie.com",
        "youtube-nocookie.com",
        "player.vimeo.com",
    };

    private static readonly Regex RegexIframe = new("<iframe\\b[^>]*>.*?</iframe>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
    private static readonly Regex RegexSrc = new("src\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase);

    private readonly HtmlSanitizer _sanitizer;

    public ConteudoSanitizerService()
    {
        _sanitizer = new HtmlSanitizer();

        _sanitizer.AllowedTags.Clear();
        foreach (var tag in new[]
        {
            "p", "br", "strong", "b", "em", "i", "u", "s", "a",
            "h1", "h2", "h3", "h4", "blockquote", "ul", "ol", "li",
            "code", "pre", "img", "span", "figure", "figcaption", "iframe",
        })
        {
            _sanitizer.AllowedTags.Add(tag);
        }

        _sanitizer.AllowedAttributes.Clear();
        foreach (var attr in new[] { "href", "src", "alt", "class", "target", "rel", "frameborder", "allowfullscreen" })
        {
            _sanitizer.AllowedAttributes.Add(attr);
        }

        // AllowedCssClasses fica vazio de propósito: sem restrição, para preservar
        // as classes utilitárias do Quill (ex.: ql-align-center, ql-indent-1, ql-video).

        _sanitizer.AllowedSchemes.Clear();
        _sanitizer.AllowedSchemes.Add("http");
        _sanitizer.AllowedSchemes.Add("https");
    }

    public string Sanitizar(string htmlBruto)
    {
        var sanitizado = _sanitizer.Sanitize(htmlBruto);
        return RegexIframe.Replace(sanitizado, RemoverIframeNaoPermitido);
    }

    private static string RemoverIframeNaoPermitido(Match match)
    {
        var matchSrc = RegexSrc.Match(match.Value);
        if (!matchSrc.Success)
        {
            return string.Empty;
        }

        if (!Uri.TryCreate(matchSrc.Groups[1].Value, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
        {
            return string.Empty;
        }

        return HostsDeEmbedPermitidos.Contains(uri.Host) ? match.Value : string.Empty;
    }
}
