using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira
{
    public interface IResumoFinanceiroIAService
    {
        ResumoFinanceiroIA GerarResumo(
            DateTime dataReferencia,
            PainelSaudeFinanceira painelSaudeFinanceira,
            PainelInsightsFinanceiros painelInsightsFinanceiros);
    }
}
