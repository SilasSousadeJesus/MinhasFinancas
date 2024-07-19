using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class LancamentoFixo
    {
        [Key]
        public Guid Id { get; set; }
        public string Valor { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        [ForeignKey("LancamentoId")]
        public Guid LancamentoId { get; set; }
        public virtual Lancamento? Lancamento { get; set; }
    }
}
