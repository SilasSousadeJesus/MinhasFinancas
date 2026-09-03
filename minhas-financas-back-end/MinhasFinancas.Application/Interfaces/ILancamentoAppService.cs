using MinhasFinancas.Application.DTOs.Lancamento;
using MinhasFinancas.Application.Interfaces.baseInterface;
using MinhasFinancas.CrossCutting;

namespace MinhasFinancas.Application.Interfaces
{
    public interface ILancamentoAppService : IAppService<CadastrarLancamentoDTO, EditarLancamentoDTO>
    {
        Task<RetornoGenerico> BuscarTodosOsElementosAsync(string id, FiltroListagemLancamentoDTO filtro);
        Task<RetornoGenerico> BuscarLancamentosPorCategoriaAsync(string usuarioId);
        Task<RetornoGenerico> EfetivarLancamentoAsync(string usuarioId, Guid lancamentoId);
        Task<RetornoGenerico> BuscarFluxoCaixaSimplesAsync(string usuarioId, int ano, int mes);
        Task<RetornoGenerico> ExportarLancamentosExcelAsync(string usuarioId, FiltroListagemLancamentoDTO filtro);
        Task<RetornoGenerico> BaixarModeloImportacaoLancamentosExcelAsync(string usuarioId);
        Task<RetornoGenerico> ImportarLancamentosExcelAsync(string usuarioId, Stream arquivo);
        Task<RetornoGenerico> ExportarFluxoCaixaSimplesExcelAsync(string usuarioId, ExportarFluxoCaixaSimplesExcelDTO filtro);
        Task<RetornoGenerico> BuscarParcelamentoAsync(string usuarioId, Guid grupoParcelamentoId);
        Task<RetornoGenerico> EditarParcelamentoEmLoteAsync(string usuarioId, Guid grupoParcelamentoId, EditarParcelamentoEmLoteDTO dto);
    }
}
