using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class LancamentoParcelado
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; } = decimal.Zero;
        public int NumeroParcela { get; set; } = 0;
        public DateTime DataVencimento { get; set; }

        [ForeignKey("LancamentoId")]
        public Guid LancamentoId { get; set; }
    }
}