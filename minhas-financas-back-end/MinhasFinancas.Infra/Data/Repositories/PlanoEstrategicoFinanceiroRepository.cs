using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class PlanoEstrategicoFinanceiroRepository : IPlanoEstrategicoFinanceiroRepository
    {
        private readonly ApplicationDbContext _context;

        public PlanoEstrategicoFinanceiroRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlanoEstrategicoFinanceiro>> BuscarTodosOsElementosAsync(string usuarioId)
        {
            return await _context.Set<PlanoEstrategicoFinanceiro>()
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.UsuarioId == usuarioId)
                .Include(x => x.Objetivos.OrderBy(o => o.Ordem))
                .OrderByDescending(x => x.NumeroVersao)
                .ThenByDescending(x => x.DataAtualizacao)
                .ToListAsync();
        }

        public async Task<PlanoEstrategicoFinanceiro?> BuscarUmElementoAsync(string usuarioId, Guid planoId)
        {
            return await _context.Set<PlanoEstrategicoFinanceiro>()
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.UsuarioId == usuarioId && x.Id == planoId)
                .Include(x => x.Objetivos.OrderBy(o => o.Ordem))
                .FirstOrDefaultAsync();
        }

        public async Task<PlanoEstrategicoFinanceiro?> BuscarUmElementoGerenciadoAsync(string usuarioId, Guid planoId)
        {
            return await _context.Set<PlanoEstrategicoFinanceiro>()
                .AsSplitQuery()
                .Where(x => x.UsuarioId == usuarioId && x.Id == planoId)
                .Include(x => x.Objetivos.OrderBy(o => o.Ordem))
                .FirstOrDefaultAsync();
        }

        public async Task<PlanoEstrategicoFinanceiro?> BuscarVigenteAsync(string usuarioId)
        {
            return await _context.Set<PlanoEstrategicoFinanceiro>()
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.UsuarioId == usuarioId && x.Ativo)
                .Include(x => x.Objetivos.OrderBy(o => o.Ordem))
                .OrderByDescending(x => x.NumeroVersao)
                .ThenByDescending(x => x.DataAtualizacao)
                .FirstOrDefaultAsync();
        }

        public async Task AdicionarAsync(PlanoEstrategicoFinanceiro plano)
        {
            await _context.Set<PlanoEstrategicoFinanceiro>().AddAsync(plano);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
