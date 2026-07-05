using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Application.DTOs.SimulacaoFinanceira
{
    public class AcaoSimulacaoFinanceiraDTO
    {
        public EnumTipoAcaoSimulacaoFinanceira TipoAcao { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public int? QuantidadeParcelas { get; set; }
        public string Observacao { get; set; } = string.Empty;
    }
}
