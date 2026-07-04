namespace MinhasFinancas.Domain.Services.DashBoard.ClassesDoDashboard
{
    public class FluxoCaixaTimelineItemDashboard
    {
        public DateTime Data { get; set; }
        public string Receita { get; set; } = string.Empty;
        public string Despesa { get; set; } = string.Empty;
        public string Saldo { get; set; } = string.Empty;
    }
}
