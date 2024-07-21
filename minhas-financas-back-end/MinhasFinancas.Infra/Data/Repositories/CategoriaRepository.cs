using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Categoria>> BuscarTodosOsElementosAsync(string Id)
        {
            return await _context.Set<Categoria>()
                                        .Where(b => b.UsuarioId == Id)
                                        .ToListAsync();
        }
        public async Task<Categoria> BuscarUmElementoAsync(string idPatrono, Guid id)
        {
            return await _context.Set<Categoria>().Where(x => x.UsuarioId == idPatrono && x.Id == id).FirstAsync();
        }

        public async Task CadastrarElementoAsync(Categoria elemento)
        {
            await _context.Set<Categoria>().AddAsync(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarElementoAsync(Categoria elemento)
        {
            _context.Set<Categoria>().Remove(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task EditarElementoAsync(Categoria elemento)
        {
            var existingEntity = _context.Set<Categoria>().Local.FirstOrDefault(b => b.Id == elemento.Id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            _context.Set<Categoria>().Update(elemento);
            await _context.SaveChangesAsync();
        }
    }
}
