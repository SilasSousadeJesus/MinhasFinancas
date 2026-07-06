using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira
{
    public interface ISaudeFinanceiraService
    {
        PainelSaudeFinanceira GerarPainel(PainelIndicadoresFinanceiros indicadores);
    }
}
