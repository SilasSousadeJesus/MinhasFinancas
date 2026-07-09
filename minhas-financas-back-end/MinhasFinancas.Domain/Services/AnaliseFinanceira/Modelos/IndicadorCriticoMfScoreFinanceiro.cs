using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class IndicadorCriticoMfScoreFinanceiro
    {
        public CodigoIndicadorFinanceiro CodigoIndicador { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public decimal Penalidade { get; set; }
        public string PilarRelacionado { get; set; } = string.Empty;
    }
}
