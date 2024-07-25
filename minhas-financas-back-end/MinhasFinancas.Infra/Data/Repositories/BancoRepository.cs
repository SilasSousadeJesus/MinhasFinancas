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

        public async Task<List<Conta>> BuscarTodosOsElementosAsync(string id)
        {
            return await _context.Set<Conta>()
                        .Where(b => b.UsuarioId == id)
                        .ToListAsync();
        }

        public async Task<Conta> BuscarUmElementoAsync(string idPatrono, Guid id)
        {
            return await _context.Set<Conta>().Where(x => x.UsuarioId == idPatrono && x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CadastrarElementoAsync(Conta elemento)
        {
            await _context.Set<Conta>().AddAsync(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarElementoAsync(Conta elemento)
        {
            _context.Set<Conta>().Remove(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task EditarElementoAsync(Conta elemento)
        {
            var existingEntity = _context.Set<Conta>().Local.FirstOrDefault(b => b.Id == elemento.Id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            _context.Set<Conta>().Update(elemento);
            await _context.SaveChangesAsync();
        }
    }
}
