using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class MfScorePersonaRepository : IMfScorePersonaRepository
    {
        private readonly ApplicationDbContext _context;

        public MfScorePersonaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PersonaMfScore>> BuscarTodasAsync()
        {
            return await _context.Set<PersonaMfScore>()
                .AsNoTracking()
                .OrderByDescending(x => x.EhCasoCanonico)
                .ThenBy(x => x.Status)
                .ThenByDescending(x => x.DataAtualizacao)
                .ToListAsync();
        }

        public async Task<PersonaMfScore?> BuscarUmaAsync(Guid personaId)
        {
            return await _context.Set<PersonaMfScore>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == personaId);
        }

        public async Task<PersonaMfScore?> BuscarUmaGerenciadaAsync(Guid personaId)
        {
            return await _context.Set<PersonaMfScore>()
                .FirstOrDefaultAsync(x => x.Id == personaId);
        }

        public async Task AdicionarAsync(PersonaMfScore persona)
        {
            await _context.Set<PersonaMfScore>().AddAsync(persona);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
