using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IHistoricoMfScoreRepository
    {
        Task<List<HistoricoMfScore>> BuscarRecentesPorUsuarioAsync(string usuarioId, int quantidade);
        Task<HistoricoMfScore?> BuscarPorCompetenciaAsync(string usuarioId, int competenciaAno, int competenciaMes, string versaoModelo);
        Task AdicionarAsync(HistoricoMfScore historico);
        Task SalvarAlteracoesAsync();
    }
}
