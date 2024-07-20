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

        public async Task<List<Banco>> BuscarTodosOsElementosAsync(string id)
        {
            return await _context.Set<Banco>()
                        .Where(b => b.UsuarioId == id)
                        .ToListAsync();
        }

        public async Task<Banco> BuscarUmElementoAsync(string idPatrono, Guid id)
        {
            return await _context.Set<Banco>().Where(x => x.UsuarioId == idPatrono && x.Id == id).FirstAsync();
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
            var existingEntity = _context.Set<Banco>().Local.FirstOrDefault(b => b.Id == elemento.Id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            _context.Set<Banco>().Update(elemento);
            await _context.SaveChangesAsync();
        }
    }
}
