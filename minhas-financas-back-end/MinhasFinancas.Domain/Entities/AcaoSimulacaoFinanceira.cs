using MinhasFinancas.CrossCutting.Util.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class AcaoSimulacaoFinanceira
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("SimulacaoFinanceiraId")]
        public Guid SimulacaoFinanceiraId { get; set; }
        public SimulacaoFinanceira? SimulacaoFinanceira { get; set; }

        public EnumTipoAcaoSimulacaoFinanceira TipoAcao { get; set; }

        public string Descricao { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime? DataFinal { get; set; }

        public int? QuantidadeParcelas { get; set; }

        public string Observacao { get; set; } = string.Empty;

        public bool Ativa { get; set; } = true;
    }
}
