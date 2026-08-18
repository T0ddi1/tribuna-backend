using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Api.DTOs;

public class ComentarioCreateDto
{
    // Nome/e-mail não vêm mais do formulário — comentar exige login, e a
    // identidade é lida do usuário autenticado (ver ComentariosController.Criar).
    [Required(ErrorMessage = "O campo {0} é obrigatório."), MinLength(3, ErrorMessage = "O campo {0} deve ter pelo menos {1} caracteres."), MaxLength(2000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
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
