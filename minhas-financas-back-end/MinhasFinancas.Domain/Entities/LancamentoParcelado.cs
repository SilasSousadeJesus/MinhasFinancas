using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class LancamentoParcelado
    {
        [Key]
        public Guid Id { get; set; }
        public string Valor { get; set; } = string.Empty;
        public string NumeroParcela { get; set; } = string.Empty;
        public DateTime DataVencimento { get; set; }



        [ForeignKey("LancamentoId")]
        public Guid LancamentoId { get; set; }
        public virtual Lancamento? Lancamento { get; set; }
    }
}