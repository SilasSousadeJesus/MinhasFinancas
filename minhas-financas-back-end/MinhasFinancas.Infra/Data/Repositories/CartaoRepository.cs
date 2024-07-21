using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class CartaoRepository : ICartaoRepository
    {

        private readonly ApplicationDbContext _context;

        public CartaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cartao>> BuscarTodosOsElementosAsync(string id)
        {
            return await _context.Set<Cartao>()
                 .Where(b => b.UsuarioId == id)
                 .ToListAsync();
        }

        public async Task<Cartao> BuscarUmElementoAsync(string idPatrono, Guid id)
        {
            return await _context.Set<Cartao>().Where(x => x.UsuarioId == idPatrono && x.Id == id).FirstAsync();
        }

        public async Task CadastrarElementoAsync(Cartao elemento)
        {
            await _context.Set<Cartao>().AddAsync(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarElementoAsync(Cartao elemento)
        {
            _context.Set<Cartao>().Remove(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task EditarElementoAsync(Cartao elemento)
        {
            var existingEntity = _context.Set<Cartao>().Local.FirstOrDefault(b => b.Id == elemento.Id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            _context.Set<Cartao>().Update(elemento);
            await _context.SaveChangesAsync();
        }
    }
}
