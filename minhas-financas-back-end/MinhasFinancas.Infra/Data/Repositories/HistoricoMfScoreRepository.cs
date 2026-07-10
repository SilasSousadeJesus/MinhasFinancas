using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class HistoricoMfScoreRepository : IHistoricoMfScoreRepository
    {
        private readonly ApplicationDbContext _context;

        public HistoricoMfScoreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<HistoricoMfScore>> BuscarRecentesPorUsuarioAsync(string usuarioId, int quantidade)
        {
            return await _context.Set<HistoricoMfScore>()
                .AsNoTracking()
                .Where(x => x.UsuarioId == usuarioId)
                .OrderByDescending(x => x.CompetenciaAno)
                .ThenByDescending(x => x.CompetenciaMes)
                .Take(quantidade)
                .ToListAsync();
        }

        public async Task<HistoricoMfScore?> BuscarPorCompetenciaAsync(string usuarioId, int competenciaAno, int competenciaMes, string versaoModelo)
        {
            return await _context.Set<HistoricoMfScore>()
                .FirstOrDefaultAsync(x =>
                    x.UsuarioId == usuarioId &&
                    x.CompetenciaAno == competenciaAno &&
                    x.CompetenciaMes == competenciaMes &&
                    x.VersaoModelo == versaoModelo);
        }

        public async Task AdicionarAsync(HistoricoMfScore historico)
        {
            await _context.Set<HistoricoMfScore>().AddAsync(historico);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
