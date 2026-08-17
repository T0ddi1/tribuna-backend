using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Artigo> Artigos => Set<Artigo>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Comentario> Comentarios => Set<Comentario>();
    public DbSet<NewsletterAssinante> NewsletterAssinantes => Set<NewsletterAssinante>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Categoria>()
            .HasIndex(c => c.Slug)
            .IsUnique();

        builder.Entity<Artigo>()
            .HasIndex(a => a.Slug)
            .IsUnique();

        builder.Entity<Artigo>()
            .HasOne(a => a.Categoria)
            .WithMany(c => c.Artigos)
            .HasForeignKey(a => a.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Artigo>()
            .HasOne(a => a.Autor)
            .WithMany()
            .HasForeignKey(a => a.AutorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Comentario>()
            .HasOne(c => c.Artigo)
            .WithMany(a => a.Comentarios)
            .HasForeignKey(c => c.ArtigoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<NewsletterAssinante>()
            .HasIndex(n => n.Email)
            .IsUnique();

        builder.Entity<RefreshToken>()
            .HasIndex(r => r.TokenHash)
            .IsUnique();

        builder.Entity<RefreshToken>()
            .HasOne(r => r.Usuario)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
