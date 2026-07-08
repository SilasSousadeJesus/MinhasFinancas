using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface ICompromissoFinanceiroRepository
    {
        Task<List<CompromissoFinanceiro>> BuscarTodosOsElementosAsync(string usuarioId);
        Task<List<CompromissoFinanceiro>> BuscarCompromissosAtivosAsync(string usuarioId);
        Task<CompromissoFinanceiro?> BuscarUmElementoAsync(string usuarioId, Guid compromissoId);
        Task<CompromissoFinanceiro?> BuscarUmElementoGerenciadoAsync(string usuarioId, Guid compromissoId);
        Task AdicionarAsync(CompromissoFinanceiro compromisso);
        Task SalvarAlteracoesAsync();
    }
}
