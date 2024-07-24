namespace MinhasFinancas.Application.Interfaces
{
    public interface IRelatoriosAppService
    {
        Task<RetornoGenerico> RelatoriosPorCategoriaLancamento(string usuarioId);

        Task<RetornoGenerico> RelatoriosValoresAnoPorAno(string usuarioId);
    }
}
