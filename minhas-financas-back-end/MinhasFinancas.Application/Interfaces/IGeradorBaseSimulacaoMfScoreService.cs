using MinhasFinancas.Application.DTOs.MfScoreLaboratorio;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IGeradorBaseSimulacaoMfScoreService
    {
        Task<ResultadoGeracaoBaseSimulacaoMfScoreDTO> GerarAsync();
        Task<ResultadoLimpezaBaseSimulacaoMfScoreDTO> LimparAsync();
    }
}
