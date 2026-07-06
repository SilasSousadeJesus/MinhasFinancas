namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class PainelInsightsFinanceiros
    {
        public List<InsightFinanceiro> Todos { get; set; } = [];
        public List<InsightFinanceiro> Prioritarios { get; set; } = [];
        public List<InsightFinanceiro> DestaquesPositivos { get; set; } = [];
    }
}
