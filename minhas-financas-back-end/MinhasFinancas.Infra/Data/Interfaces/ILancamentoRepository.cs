using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface ILancamentoRepository : IRepository<Lancamento>
    {
        Task<List<Lancamento>> BuscarLancamentosPorCategoriaAsync(string usuarioId);
        Task CadastrarElementosAsync(List<Lancamento> elementos);
    }
}
