namespace MinhasFinancas.Domain.Services.DashBoard.ClassesDoDashboard
{
    public class FluxoCaixaProximos30DiasDashboard
    {
        public string ReceitasPrevistas { get; set; } = string.Empty;
        public string DespesasPrevistas { get; set; } = string.Empty;
        public string SaldoPrevisto { get; set; } = string.Empty;
        public List<FluxoCaixaTimelineItemDashboard> LinhaDoTempo { get; set; } = new();
    }
}
