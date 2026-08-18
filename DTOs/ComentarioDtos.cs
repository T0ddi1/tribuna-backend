using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Api.DTOs;

public class ComentarioCreateDto
{
    // Nome/e-mail não vêm mais do formulário — comentar exige login, e a
    // identidade é lida do usuário autenticado (ver ComentariosController.Criar).
    [Required, MaxLength(2000), MinLength(3)]
    public string Texto { get; set; } = string.Empty;
}

public class ComentarioResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public int Curtidas { get; set; }
    public bool CurtidoPeloUsuarioAtual { get; set; }
}

public class ComentarioModeracaoDto : ComentarioResponseDto
{
    public string Email { get; set; } = string.Empty;
    public bool Aprovado { get; set; }
    public int ArtigoId { get; set; }
    public string ArtigoTitulo { get; set; } = string.Empty;
}
