using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira
{
    public interface IInsightsFinanceirosService
    {
        PainelInsightsFinanceiros GerarPainel(PainelSaudeFinanceira painelSaudeFinanceira);
    }
}
