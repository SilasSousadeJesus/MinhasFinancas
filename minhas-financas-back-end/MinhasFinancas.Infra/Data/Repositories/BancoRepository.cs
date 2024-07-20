using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class BancoRepository : IBancoRepository
    {
        private readonly ApplicationDbContext _context;

        public BancoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Banco>> BuscarTodosOsElementosAsync()
        {
            return await _context.Set<Banco>().ToListAsync();
        }

        public async Task<Banco> BuscarUmElementoAsync(int id)
        {
            return await _context.Set<Banco>().FindAsync(id);
        }

        public async Task CadastrarElementoAsync(Banco elemento)
        {
            await _context.Set<Banco>().AddAsync(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarElementoAsync(Banco elemento)
        {
            _context.Set<Banco>().Update(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarElementoAsync(Banco elemento)
        {
            _context.Set<Banco>().Remove(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task EditarElementoAsync(Banco elemento)
        {
            _context.Set<Banco>().Update(elemento);
            await _context.SaveChangesAsync();
        }
    }
}
