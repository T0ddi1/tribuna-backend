namespace NewsPortal.Api.Models;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Editor = "Editor";

    public static readonly string[] Todas = [Admin, Editor];
}
