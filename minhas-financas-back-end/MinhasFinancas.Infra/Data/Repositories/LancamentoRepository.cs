using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class LancamentoRepository : ILancamentoRepository
    {

        private readonly ApplicationDbContext _context;

        public LancamentoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Lancamento>> BuscarLancamentosPorCategoriaAsync(string usuarioId)
        {
            return await _context.Set<Lancamento>()
                                           .AsNoTracking()
                                           .Where(b => b.UsuarioId == usuarioId)
                                           .Include(l => l.Categoria)
                                           .ToListAsync();
        }

        public async Task<List<Lancamento>> BuscarPorPeriodoVencimentoAsync(string usuarioId, DateTime dataInicial, DateTime dataFinal)
        {
            return await _context.Set<Lancamento>()
                .AsNoTracking()
                .Where(x =>
                    x.UsuarioId == usuarioId &&
                    x.DataVencimento.Date >= dataInicial.Date &&
                    x.DataVencimento.Date <= dataFinal.Date)
                .Include(x => x.Categoria)
                .OrderBy(x => x.DataVencimento)
                .ThenBy(x => x.Descricao)
                .ToListAsync();
        }

        public async Task<List<Lancamento>> BuscarTodosOsElementosAsync(string id)
        {
            return await _context.Set<Lancamento>()
               .AsNoTracking()
               .Where(b => b.UsuarioId == id)
                .Include(l => l.Categoria)
               .ToListAsync();
        }

        public async Task<Lancamento> BuscarUmElementoAsync(string idPatrono, Guid id)
        {
            return await _context.Set<Lancamento>()
                .AsNoTracking()
                .Where(x => x.UsuarioId == idPatrono && x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task CadastrarElementoAsync(Lancamento elemento)
        {
            await _context.Set<Lancamento>().AddAsync(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task CadastrarElementosAsync(List<Lancamento> elementos)
        {
            await _context.Set<Lancamento>().AddRangeAsync(elementos);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarElementoAsync(Lancamento elemento)
        {
            _context.Set<Lancamento>().Remove(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task EditarElementoAsync(Lancamento elemento)
        {
            var existingEntity = _context.Set<Lancamento>().Local.FirstOrDefault(b => b.Id == elemento.Id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            _context.Set<Lancamento>().Update(elemento);
            await _context.SaveChangesAsync();
        }
    }
}
