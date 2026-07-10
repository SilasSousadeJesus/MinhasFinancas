using MinhasFinancas.Application.DTOs.MfScore;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IMfScoreCalculoAppService
    {
        Task<ResultadoCalculoMfScoreInternoDTO?> CalcularAsync(string usuarioId, DateTime? dataReferencia = null);
        Task<RetornoGenerico> GerarHistoricoMensalAsync(DateTime? competenciaReferencia = null);
    }
}
