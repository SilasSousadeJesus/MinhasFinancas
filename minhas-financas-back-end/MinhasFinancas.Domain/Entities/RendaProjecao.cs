using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class RendaProjecao
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Nome { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorMensal { get; set; } = decimal.Zero;

        [ForeignKey("ProjecaoId")]
        public Guid ProjecaoId { get; set; }
        public Projecao? Projecao { get; set; }
    }
}
