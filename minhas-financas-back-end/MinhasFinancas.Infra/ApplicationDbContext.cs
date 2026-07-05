using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }
        public DbSet<Conta> Conta { get; set; }
        public DbSet<Cartao> Cartao { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<SubCategoria> SubCategoria { get; set; }
        public DbSet<Lancamento> Lancamento { get; set; }
        public DbSet<LancamentoFixo> LancamentoFixo { get; set; }
        public DbSet<LancamentoParcelado> LancamentoParcelado { get; set; }
        public DbSet<BemPatrimonial> BemPatrimonial { get; set; }
        public DbSet<Meta> Meta { get; set; }
        public DbSet<PermanenciaBemMaterial> PermanenciaBemMaterial { get; set; }
        public DbSet<Passivo> Passivo { get; set; }
        public DbSet<SnapshotPatrimonial> SnapshotPatrimonial { get; set; }
        public DbSet<Projecao> Projecao { get; set; }
        public DbSet<RendaProjecao> RendaProjecao { get; set; }
        public DbSet<RendaExtraProjecaoMensal> RendaExtraProjecaoMensal { get; set; }
        public DbSet<DividaManualProjecaoMensal> DividaManualProjecaoMensal { get; set; }
        public DbSet<SimulacaoFinanceira> SimulacaoFinanceira { get; set; }
        public DbSet<AcaoSimulacaoFinanceira> AcaoSimulacaoFinanceira { get; set; }

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


            // deletes em cascata

            modelBuilder.Entity<SubCategoria>()
                    .HasOne<Categoria>()
                    .WithMany(c => c.SubCategorias)
                    .HasForeignKey(sc => sc.CategoriaId)
                    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Projecao>()
                .HasMany(x => x.Rendas)
                .WithOne(x => x.Projecao)
                .HasForeignKey(x => x.ProjecaoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Projecao>()
                .HasMany(x => x.RendasExtrasMensais)
                .WithOne(x => x.Projecao)
                .HasForeignKey(x => x.ProjecaoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Projecao>()
                .HasMany(x => x.DividasManuaisMensais)
                .WithOne(x => x.Projecao)
                .HasForeignKey(x => x.ProjecaoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SimulacaoFinanceira>()
                .HasMany(x => x.Acoes)
                .WithOne(x => x.SimulacaoFinanceira)
                .HasForeignKey(x => x.SimulacaoFinanceiraId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuração das chaves primárias para as entidades do Identity
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(x => new { x.LoginProvider, x.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(x => new { x.UserId, x.LoginProvider, x.Name });

        }
    }
}

