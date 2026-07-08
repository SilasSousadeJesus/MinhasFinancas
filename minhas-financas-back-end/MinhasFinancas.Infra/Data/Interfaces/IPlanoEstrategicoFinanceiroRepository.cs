using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IPlanoEstrategicoFinanceiroRepository
    {
        Task<List<PlanoEstrategicoFinanceiro>> BuscarTodosOsElementosAsync(string usuarioId);
        Task<PlanoEstrategicoFinanceiro?> BuscarUmElementoAsync(string usuarioId, Guid planoId);
        Task<PlanoEstrategicoFinanceiro?> BuscarUmElementoGerenciadoAsync(string usuarioId, Guid planoId);
        Task<PlanoEstrategicoFinanceiro?> BuscarVigenteAsync(string usuarioId);
        Task AdicionarAsync(PlanoEstrategicoFinanceiro plano);
        Task SalvarAlteracoesAsync();
    }
}
