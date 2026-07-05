using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IPerfilFinanceiroRepository
    {
        Task<PerfilFinanceiro?> BuscarPorUsuarioAsync(string usuarioId);
        Task<PerfilFinanceiro?> BuscarPorUsuarioLeituraAsync(string usuarioId);
        Task CadastrarAsync(PerfilFinanceiro perfilFinanceiro);
        Task SalvarAlteracoesAsync();
    }
}
