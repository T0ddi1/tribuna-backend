using Ganss.Xss;

namespace NewsPortal.Api.Services;

/// <summary>
/// Sanitiza o HTML gerado pelo editor rico (Quill) antes de persistir.
/// Mesmo sendo só Admin/Editor que escrevem artigos, uma conta comprometida
/// ou um editor descuidado não deve conseguir injetar &lt;script&gt;, handlers
/// inline (onerror, onclick) ou iframes — o conteúdo é lido por qualquer
/// visitante do site via [innerHTML] no frontend.
/// </summary>
public class ConteudoSanitizerService
{
    private readonly HtmlSanitizer _sanitizer;

    public ConteudoSanitizerService()
    {
        _sanitizer = new HtmlSanitizer();

        _sanitizer.AllowedTags.Clear();
        foreach (var tag in new[]
        {
            "p", "br", "strong", "b", "em", "i", "u", "s", "a",
            "h1", "h2", "h3", "h4", "blockquote", "ul", "ol", "li",
            "code", "pre", "img", "span", "figure", "figcaption",
        })
        {
            _sanitizer.AllowedTags.Add(tag);
        }

        _sanitizer.AllowedAttributes.Clear();
        foreach (var attr in new[] { "href", "src", "alt", "class", "target", "rel" })
        {
            _sanitizer.AllowedAttributes.Add(attr);
        }

        // AllowedCssClasses fica vazio de propósito: sem restrição, para preservar
        // as classes utilitárias do Quill (ex.: ql-align-center, ql-indent-1).

        _sanitizer.AllowedSchemes.Clear();
        _sanitizer.AllowedSchemes.Add("http");
        _sanitizer.AllowedSchemes.Add("https");
    }

    public string Sanitizar(string htmlBruto) => _sanitizer.Sanitize(htmlBruto);
}
