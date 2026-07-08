using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Domain.Entities
{
    public class ObjetivoPlanoEstrategico
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PlanoEstrategicoFinanceiroId { get; set; }

        [MaxLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string? Descricao { get; set; }

        public EnumPrioridadeObjetivoPlanoEstrategico Prioridade { get; set; } = EnumPrioridadeObjetivoPlanoEstrategico.Media;

        public EnumStatusObjetivoPlanoEstrategico Status { get; set; } = EnumStatusObjetivoPlanoEstrategico.Planejado;

        public int Ordem { get; set; }

        public DateTime? DataAlvo { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ValorAlvo { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ValorAtual { get; set; }

        [MaxLength(4000)]
        public string? Observacao { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public PlanoEstrategicoFinanceiro? PlanoEstrategicoFinanceiro { get; set; }
    }
}
