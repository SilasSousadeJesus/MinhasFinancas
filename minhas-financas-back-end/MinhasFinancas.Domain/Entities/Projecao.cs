using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class Projecao
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Nome { get; set; } = string.Empty;

        public DateTime DataInicial { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorAcumuladoInicial { get; set; } = decimal.Zero;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorObjetivo { get; set; } = decimal.Zero;

        public int MesesLimite { get; set; } = 60;
        public bool AtreladaADespesas { get; set; } = true;

        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

        [ForeignKey("UsuarioId")]
        public string UsuarioId { get; set; } = string.Empty;
        public Usuario? Usuario { get; set; }

        public List<RendaProjecao> Rendas { get; set; } = new();
        public List<RendaExtraProjecaoMensal> RendasExtrasMensais { get; set; } = new();
        public List<DividaManualProjecaoMensal> DividasManuaisMensais { get; set; } = new();
    }
}
