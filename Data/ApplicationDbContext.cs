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
    public DbSet<Vertical> Verticais => Set<Vertical>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Comentario> Comentarios => Set<Comentario>();
    public DbSet<ComentarioLike> ComentarioLikes => Set<ComentarioLike>();
    public DbSet<Favorito> Favoritos => Set<Favorito>();
    public DbSet<NewsletterAssinante> NewsletterAssinantes => Set<NewsletterAssinante>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Vertical>()
            .HasIndex(v => v.Slug)
            .IsUnique();

        builder.Entity<Categoria>()
            .HasIndex(c => c.Slug)
            .IsUnique();

        builder.Entity<Artigo>()
            .HasIndex(a => a.Slug)
            .IsUnique();

        builder.Entity<Artigo>()
            .HasOne(a => a.Vertical)
            .WithMany(v => v.Artigos)
            .HasForeignKey(a => a.VerticalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Artigo>()
            .HasOne(a => a.Categoria)
            .WithMany(c => c.Artigos)
            .HasForeignKey(a => a.CategoriaId)
            .OnDelete(DeleteBehavior.SetNull);

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

        builder.Entity<Comentario>()
            .HasOne(c => c.Usuario)
            .WithMany()
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ComentarioLike>()
            .HasIndex(l => new { l.ComentarioId, l.UsuarioId })
            .IsUnique();

        builder.Entity<ComentarioLike>()
            .HasOne(l => l.Comentario)
            .WithMany()
            .HasForeignKey(l => l.ComentarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ComentarioLike>()
            .HasOne(l => l.Usuario)
            .WithMany()
            .HasForeignKey(l => l.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Favorito>()
            .HasIndex(f => new { f.UsuarioId, f.ArtigoId })
            .IsUnique();

        builder.Entity<Favorito>()
            .HasOne(f => f.Usuario)
            .WithMany()
            .HasForeignKey(f => f.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Favorito>()
            .HasOne(f => f.Artigo)
            .WithMany()
            .HasForeignKey(f => f.ArtigoId)
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
