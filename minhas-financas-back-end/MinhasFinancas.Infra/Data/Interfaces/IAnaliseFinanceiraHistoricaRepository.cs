using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IAnaliseFinanceiraHistoricaRepository : IRepository<AnaliseFinanceiraHistorica>
    {
        Task<List<AnaliseFinanceiraHistorica>> BuscarUltimasAnalisesAsync(string usuarioId, int quantidade);
    }
}
