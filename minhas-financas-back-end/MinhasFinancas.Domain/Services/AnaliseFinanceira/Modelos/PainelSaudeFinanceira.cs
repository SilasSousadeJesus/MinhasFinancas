namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class PainelSaudeFinanceira
    {
        public ResumoSaudeFinanceira Resumo { get; set; } = new();
        public PainelIndicadoresFinanceiros Indicadores { get; set; } = new();
    }
}
