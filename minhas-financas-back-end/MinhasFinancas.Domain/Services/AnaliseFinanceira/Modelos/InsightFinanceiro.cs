using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class InsightFinanceiro
    {
        public CodigoIndicadorFinanceiro? CodigoIndicadorRelacionado { get; set; }
        public TipoInsightFinanceiro Tipo { get; set; }
        public PrioridadeInsightFinanceiro Prioridade { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string AcaoSugerida { get; set; } = string.Empty;
    }
}
