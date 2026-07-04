using MinhasFinancas.Application.DTOs.Lancamento;
using MinhasFinancas.Application.Interfaces.baseInterface;

namespace MinhasFinancas.Application.Interfaces
{
    public interface ILancamentoAppService : IAppService<CadastrarLancamentoDTO, EditarLancamentoDTO>
    {
        Task<RetornoGenerico> BuscarTodosOsElementosAsync(string id, FiltroListagemLancamentoDTO filtro);
        Task<RetornoGenerico> BuscarLancamentosPorCategoriaAsync(string usuarioId);
    }
}
