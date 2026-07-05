using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface ILancamentoRepository : IRepository<Lancamento>
    {
        Task<List<Lancamento>> BuscarLancamentosPorCategoriaAsync(string usuarioId);
        Task<List<Lancamento>> BuscarPorPeriodoVencimentoAsync(string usuarioId, DateTime dataInicial, DateTime dataFinal);
        Task<List<Lancamento>> BuscarPorGrupoParcelamentoAsync(string usuarioId, Guid grupoParcelamentoId);
        Task CadastrarElementosAsync(List<Lancamento> elementos);
        Task EditarElementosAsync(List<Lancamento> elementos);
    }
}
