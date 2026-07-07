using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class AnaliseFinanceiraHistoricaRepository : IAnaliseFinanceiraHistoricaRepository
    {
        private readonly ApplicationDbContext _context;

        public AnaliseFinanceiraHistoricaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AnaliseFinanceiraHistorica>> BuscarTodosOsElementosAsync(string id)
        {
            return await _context.Set<AnaliseFinanceiraHistorica>()
                .AsNoTracking()
                .Where(x => x.UsuarioId == id && x.Ativa)
                .OrderByDescending(x => x.DataGeracao)
                .ToListAsync();
        }

        public async Task<AnaliseFinanceiraHistorica> BuscarUmElementoAsync(string idPatrono, Guid id)
        {
            return await _context.Set<AnaliseFinanceiraHistorica>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UsuarioId == idPatrono && x.Id == id && x.Ativa);
        }

        public async Task<List<AnaliseFinanceiraHistorica>> BuscarUltimasAnalisesAsync(string usuarioId, int quantidade)
        {
            var quantidadeFinal = quantidade <= 0 ? 5 : quantidade;

            return await _context.Set<AnaliseFinanceiraHistorica>()
                .AsNoTracking()
                .Where(x => x.UsuarioId == usuarioId && x.Ativa)
                .OrderByDescending(x => x.DataGeracao)
                .Take(quantidadeFinal)
                .ToListAsync();
        }

        public async Task CadastrarElementoAsync(AnaliseFinanceiraHistorica elemento)
        {
            await _context.Set<AnaliseFinanceiraHistorica>().AddAsync(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarElementoAsync(AnaliseFinanceiraHistorica elemento)
        {
            _context.Set<AnaliseFinanceiraHistorica>().Remove(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task EditarElementoAsync(AnaliseFinanceiraHistorica elemento)
        {
            _context.Set<AnaliseFinanceiraHistorica>().Update(elemento);
            await _context.SaveChangesAsync();
        }
    }
}
