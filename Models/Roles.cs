namespace NewsPortal.Api.Models;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Editor = "Editor";

    // Conta de leitor: cadastro público (favoritos, comentários, curtidas).
    // Não entra em Todas — esse array só é usado pra validar a role escolhida
    // pelo Admin ao criar contas de equipe editorial.
    public const string Leitor = "Leitor";

    public static readonly string[] Todas = [Admin, Editor];
    public static readonly string[] TodasComLeitor = [Admin, Editor, Leitor];
}
