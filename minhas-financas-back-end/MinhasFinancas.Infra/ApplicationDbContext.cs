using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra
{
    public class ApplicationDbContext : IdentityDbContext<Usuario, IdentityRole<Guid>, Guid>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }
        public DbSet<Banco> Banco { get; set; }
        public DbSet<Cartao> Cartao { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<SubCategoria> SubCategoria { get; set; }
        public DbSet<Lancamento> Lancamento { get; set; }
        public DbSet<LancamentoFixo> LancamentoFixo { get; set; }
        public DbSet<LancamentoParcelado> LancamentoParcelado { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Lancamento>()
                    .HasOne(l => l.Categoria)
                    .WithMany()
                    .HasForeignKey(l => l.CategoriaId)
                    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Lancamento>()
                .HasOne(l => l.SubCategoria)
                .WithMany()
                .HasForeignKey(l => l.SubCategoriaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Categoria>()
                .HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade); // Permite exclusão em cascata caso o usuario seja deletado.

            // Configuração das chaves primárias para as entidades do Identity
            modelBuilder.Entity<IdentityUserLogin<Guid>>().HasKey(x => new { x.LoginProvider, x.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<IdentityUserToken<Guid>>().HasKey(x => new { x.UserId, x.LoginProvider, x.Name });


        }


    }
}

