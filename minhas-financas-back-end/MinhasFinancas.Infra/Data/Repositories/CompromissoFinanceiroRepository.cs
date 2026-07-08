using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class CompromissoFinanceiroRepository : ICompromissoFinanceiroRepository
    {
        private readonly ApplicationDbContext _context;

        public CompromissoFinanceiroRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CompromissoFinanceiro>> BuscarTodosOsElementosAsync(string usuarioId)
        {
            return await _context.Set<CompromissoFinanceiro>()
                .AsNoTracking()
                .Where(x => x.UsuarioId == usuarioId && x.Ativo)
                .OrderByDescending(x => x.DataCriacao)
                .ToListAsync();
        }

        public async Task<List<CompromissoFinanceiro>> BuscarCompromissosAtivosAsync(string usuarioId)
        {
            return await _context.Set<CompromissoFinanceiro>()
                .AsNoTracking()
                .Where(x => x.UsuarioId == usuarioId && x.Ativo && x.Status == EnumStatusCompromissoFinanceiro.EmAndamento)
                .OrderByDescending(x => x.DataCriacao)
                .ToListAsync();
        }

        public async Task<CompromissoFinanceiro?> BuscarUmElementoAsync(string usuarioId, Guid compromissoId)
        {
            return await _context.Set<CompromissoFinanceiro>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.Id == compromissoId && x.Ativo);
        }

        public async Task<CompromissoFinanceiro?> BuscarUmElementoGerenciadoAsync(string usuarioId, Guid compromissoId)
        {
            return await _context.Set<CompromissoFinanceiro>()
                .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.Id == compromissoId && x.Ativo);
        }

        public async Task AdicionarAsync(CompromissoFinanceiro compromisso)
        {
            await _context.Set<CompromissoFinanceiro>().AddAsync(compromisso);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
