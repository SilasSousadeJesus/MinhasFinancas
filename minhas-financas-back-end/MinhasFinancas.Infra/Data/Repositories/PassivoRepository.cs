using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class PassivoRepository : IPassivoRepository
    {
        private readonly ApplicationDbContext _context;

        public PassivoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Passivo>> BuscarTodosOsElementosAsync(string id)
        {
            var listaDeBens = await _context.Set<Passivo>()
                .AsNoTracking()
                .Include(x => x.DataPermanencia)
                 .Where(b => b.UsuarioId == id && b.Ativo)
                 .ToListAsync();

            return listaDeBens;
        }

        public async Task<Passivo> BuscarUmElementoAsync(string idPatrono, Guid id)
        {
            return await _context.Set<Passivo>()
                    .AsNoTracking()
                    .Include(x => x.DataPermanencia)
                    .Where(x => x.UsuarioId == idPatrono && x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CadastrarElementoAsync(Passivo elemento)
        {
            await _context.Set<Passivo>().AddAsync(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarElementoAsync(Passivo elemento)
        {
            _context.Set<Passivo>().Remove(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task EditarElementoAsync(Passivo elemento)
        {
            var existingEntity = _context.Set<Passivo>().Local.FirstOrDefault(b => b.Id == elemento.Id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            _context.Set<Passivo>().Update(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task CadastrarPermanenciaAsync(PermanenciaPassivo permanencia)
        {
            await _context.Set<PermanenciaPassivo>().AddAsync(permanencia);
            await _context.SaveChangesAsync();
        }
    }
}
