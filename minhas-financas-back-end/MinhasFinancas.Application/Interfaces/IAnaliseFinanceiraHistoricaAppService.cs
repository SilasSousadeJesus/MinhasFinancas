using MinhasFinancas.Application.DTOs.AnaliseFinanceiraHistorica;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IAnaliseFinanceiraHistoricaAppService
    {
        Task<RetornoGenerico> BuscarTodasAsync(string usuarioId, int pagina = 1, int tamanhoPagina = 5);
        Task<RetornoGenerico> BuscarDetalheAsync(string usuarioId, Guid analiseId);
        Task<RetornoGenerico> ExcluirAsync(string usuarioId, Guid analiseId);
        Task<Guid?> RegistrarAsync(RegistrarAnaliseFinanceiraHistoricaDTO dto);
        Task<List<AnaliseFinanceiraHistoricaResumidaDTO>> BuscarUltimasAnalisesResumidasAsync(string usuarioId, int quantidade);
    }
}
