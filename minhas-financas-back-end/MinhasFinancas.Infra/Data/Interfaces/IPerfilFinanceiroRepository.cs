using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IPerfilFinanceiroRepository
    {
        Task<PerfilFinanceiro?> BuscarPorUsuarioAsync(string usuarioId);
        Task<PerfilFinanceiro?> BuscarPorUsuarioLeituraAsync(string usuarioId);
        Task<ConfiguracaoPerfilFinanceiro?> BuscarConfiguracaoVigenteAsync(Guid perfilFinanceiroId);
        Task EncerrarConfiguracaoVigenteAsync(Guid configuracaoId, DateTime dataFimVigencia);
        Task AdicionarConfiguracaoAsync(ConfiguracaoPerfilFinanceiro configuracaoPerfilFinanceiro);
        Task CadastrarAsync(PerfilFinanceiro perfilFinanceiro);
        Task SalvarAlteracoesAsync();
    }
}
