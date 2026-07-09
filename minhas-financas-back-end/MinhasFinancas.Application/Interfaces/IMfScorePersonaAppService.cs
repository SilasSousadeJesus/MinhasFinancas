using MinhasFinancas.Application.DTOs.MfScorePersona;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IMfScorePersonaAppService
    {
        Task<RetornoGenerico> BuscarTodasAsync();
        Task<RetornoGenerico> BuscarUmaAsync(Guid personaId);
        Task<RetornoGenerico> CadastrarAsync(SalvarMfScorePersonaDTO dto);
        Task<RetornoGenerico> EditarAsync(Guid personaId, SalvarMfScorePersonaDTO dto);
        Task<RetornoGenerico> InativarAsync(Guid personaId);
        Task<RetornoGenerico> RodarScoreAsync(Guid personaId);
        Task<RetornoGenerico> MarcarAuditadaAsync(Guid personaId);
        Task<RetornoGenerico> MarcarCasoCanonicoAsync(Guid personaId);
    }
}
