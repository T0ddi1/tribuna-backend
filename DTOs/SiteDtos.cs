using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Api.DTOs;

// ---------- Público (formato idêntico ao que o frontend já consome) ----------

public class NavItemDto
{
    public string Label { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public List<SubmenuColumnDto>? Columns { get; set; }
}

public class SubmenuColumnDto
{
    public string Title { get; set; } = string.Empty;
    public List<SubmenuItemDto> Items { get; set; } = new();
}

public class SubmenuItemDto
{
    public string Label { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
}

public class FooterDataDto
{
    public string Tagline { get; set; } = string.Empty;
    public List<FooterLinkColumnDto> Columns { get; set; } = new();
    public List<SocialLinkDto> Social { get; set; } = new();
    public FooterNewsletterDto Newsletter { get; set; } = new();
    public List<SpecialProjectDto> SpecialProjects { get; set; } = new();
}

public class FooterLinkColumnDto
{
    public string Title { get; set; } = string.Empty;
    public List<FooterLinkItemDto> Links { get; set; } = new();
}

public class FooterLinkItemDto
{
    public string Label { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
}

public class SocialLinkDto
{
    public string Icon { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

public class FooterNewsletterDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Placeholder { get; set; } = string.Empty;
    public string CtaLabel { get; set; } = string.Empty;
}

public class SpecialProjectDto
{
    public string Icon { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
}

public class PaginaInstitucionalPublicaDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ContentHtml { get; set; } = string.Empty;
    public string? Image { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? AttachmentLabel { get; set; }
}

public class ConfiguracaoSitePublicaDto
{
    public string? LogoUrl { get; set; }
    public string? FaviconUrl { get; set; }
    public string? CoresJson { get; set; }
}

public class AvisoSitePublicoDto
{
    public int Id { get; set; }

    // Muda a cada edição — o frontend usa pra "não ver mais" valer só até o aviso ser alterado.
    public string Versao { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Mensagem { get; set; }
    public string? ImagemUrl { get; set; }
    public bool SomenteImagem { get; set; }
    public string? TextoBotao { get; set; }
    public string? Link { get; set; }
    public bool Externo { get; set; }
    public bool PermiteNaoVerMais { get; set; }
}

// ---------- Admin ----------

public class ConfiguracaoSiteSalvarDto
{
    [MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? LogoUrl { get; set; }

    [MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? FaviconUrl { get; set; }

    [MaxLength(4000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? CoresJson { get; set; }
}

public class ConfiguracaoSiteAdminDto
{
    public int Id { get; set; }
    public string? LogoUrl { get; set; }
    public string? FaviconUrl { get; set; }
    public string? CoresJson { get; set; }
}

public class ConfiguracaoRodapeSalvarDto
{
    [MaxLength(300, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Tagline { get; set; } = string.Empty;

    [MaxLength(120, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string NewsletterTitulo { get; set; } = string.Empty;

    [MaxLength(300, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string NewsletterDescricao { get; set; } = string.Empty;

    [MaxLength(80, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string NewsletterPlaceholder { get; set; } = string.Empty;

    [MaxLength(40, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string NewsletterCta { get; set; } = string.Empty;
}

public class ConfiguracaoRodapeAdminDto : ConfiguracaoRodapeSalvarDto
{
    public int Id { get; set; }
}

public class MenuItemSalvarDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(60, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Label { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Href { get; set; } = string.Empty;

    public int Ordem { get; set; }
    public bool? Ativo { get; set; }
}

public class MenuItemAdminDto
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public bool Ativo { get; set; }
}

public class MenuLinkSalvarDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecione o item do menu.")]
    public int MenuItemId { get; set; }

    [MaxLength(60, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? ColunaTitulo { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(80, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Label { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Href { get; set; } = string.Empty;

    public int Ordem { get; set; }
    public bool? Ativo { get; set; }
}

public class MenuLinkAdminDto
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public string? MenuItemLabel { get; set; }
    public string? ColunaTitulo { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public bool Ativo { get; set; }
}

public class FooterLinkSalvarDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(60, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Coluna { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(80, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Label { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Href { get; set; } = string.Empty;

    public int Ordem { get; set; }
    public bool? Ativo { get; set; }
}

public class FooterLinkAdminDto
{
    public int Id { get; set; }
    public string Coluna { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public bool Ativo { get; set; }
}

public class FooterRedeSocialSalvarDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(40, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Icone { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(60, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Label { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Href { get; set; } = string.Empty;

    public int Ordem { get; set; }
    public bool? Ativo { get; set; }
}

public class FooterRedeSocialAdminDto
{
    public int Id { get; set; }
    public string Icone { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public bool Ativo { get; set; }
}

public class FooterProjetoEspecialSalvarDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(40, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Icone { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(80, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Href { get; set; } = string.Empty;

    public int Ordem { get; set; }
    public bool? Ativo { get; set; }
}

public class FooterProjetoEspecialAdminDto
{
    public int Id { get; set; }
    public string Icone { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public bool Ativo { get; set; }
}

public class PaginaInstitucionalSalvarDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(150, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(300, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string ResumoBusca { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public string ConteudoHtml { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? ImagemUrl { get; set; }

    [MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? AnexoUrl { get; set; }

    [MaxLength(120, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? AnexoLabel { get; set; }

    public bool? Ativo { get; set; }
}

public class PaginaInstitucionalAdminDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ResumoBusca { get; set; } = string.Empty;
    public string ConteudoHtml { get; set; } = string.Empty;
    public string? ImagemUrl { get; set; }
    public string? AnexoUrl { get; set; }
    public string? AnexoLabel { get; set; }
    public bool Ativo { get; set; }
}

public class AvisoSiteSalvarDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(150, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? Mensagem { get; set; }

    [MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? ImagemUrl { get; set; }

    public bool SomenteImagem { get; set; }

    [MaxLength(60, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? TextoBotao { get; set; }

    [MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? Link { get; set; }

    public bool Externo { get; set; }
    public bool PermiteNaoVerMais { get; set; } = true;
    public DateTime? InicioEm { get; set; }
    public DateTime? FimEm { get; set; }
    public int Ordem { get; set; }
    public bool? Ativo { get; set; }
}

public class AvisoSiteAdminDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Mensagem { get; set; }
    public string? ImagemUrl { get; set; }
    public bool SomenteImagem { get; set; }
    public string? TextoBotao { get; set; }
    public string? Link { get; set; }
    public bool Externo { get; set; }
    public bool PermiteNaoVerMais { get; set; }
    public DateTime? InicioEm { get; set; }
    public DateTime? FimEm { get; set; }
    public int Ordem { get; set; }
    public bool Ativo { get; set; }
}
