using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IAnaliseFinanceiraHistoricaRepository : IRepository<AnaliseFinanceiraHistorica>
    {
        Task<List<AnaliseFinanceiraHistorica>> BuscarUltimasAnalisesAsync(string usuarioId, int quantidade);
        Task<(List<AnaliseFinanceiraHistorica> Itens, int TotalItens)> BuscarPaginaAsync(string usuarioId, int pagina, int tamanhoPagina);
    }
}
