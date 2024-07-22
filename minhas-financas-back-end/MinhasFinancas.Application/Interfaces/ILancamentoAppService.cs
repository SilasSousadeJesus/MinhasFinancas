using MinhasFinancas.Application.DTOs.Lancamento;
using MinhasFinancas.Application.Interfaces.baseInterface;

namespace MinhasFinancas.Application.Interfaces
{
    public interface ILancamentoAppService : IAppService<CadastrarLancamento, EditarLancamento>
    {
        Task<RetornoGenerico> BuscarLancamentosPorCategoriaAsync(string usuarioId);
    }
}
