namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class ResumoFinanceiroIA
    {
        public DateTime DataReferencia { get; set; }
        public ResumoSaudeFinanceira SaudeFinanceira { get; set; } = new();
        public PainelIndicadoresFinanceiros Indicadores { get; set; } = new();
        public PainelInsightsFinanceiros Insights { get; set; } = new();
        public string ResumoExecutivo { get; set; } = string.Empty;
        public List<string> PrioridadesImediatas { get; set; } = [];
        public List<string> DestaquesPositivos { get; set; } = [];
    }
}
