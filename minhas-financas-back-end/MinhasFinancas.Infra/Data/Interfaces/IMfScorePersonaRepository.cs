using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IMfScorePersonaRepository
    {
        Task<List<PersonaMfScore>> BuscarTodasAsync();
        Task<PersonaMfScore?> BuscarUmaAsync(Guid personaId);
        Task<PersonaMfScore?> BuscarUmaGerenciadaAsync(Guid personaId);
        Task AdicionarAsync(PersonaMfScore persona);
        Task SalvarAlteracoesAsync();
    }
}
