using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class MetaRepository : IMetaRepository
    {
        private readonly ApplicationDbContext _context;

        public MetaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Meta>> BuscarTodosOsElementosAsync(string id)
        {
            return await _context.Set<Meta>()
                        .Where(b => b.UsuarioId == id)
                        .ToListAsync();
        }

        public async Task<Meta> BuscarUmElementoAsync(string idPatrono, Guid id)
        {
            return await _context.Set<Meta>().Where(x => x.UsuarioId == idPatrono && x.Id == id).Include(x => x.AportesMeta).FirstOrDefaultAsync();
        }

        public async Task CadastrarElementoAsync(Meta elemento)
        {
            await _context.Set<Meta>().AddAsync(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarElementoAsync(Meta elemento)
        {
            _context.Set<Meta>().Remove(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task EditarElementoAsync(Meta elemento)
        {
            var existingMeta = await _context.Set<Meta>().FindAsync(elemento.Id);
            var existingAportes = await _context.Set<AporteMeta>().Where(x => x.MetaId == elemento.Id).ToListAsync();

            if (existingMeta != null)
            {
                _context.Entry(existingMeta).State = EntityState.Detached;
            }

            _context.Set<Meta>().Update(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAndamentoMetaAsync(Meta elemento)
        {
            var existingEntity = _context.Set<Meta>().Local.FirstOrDefault(b => b.Id == elemento.Id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            _context.Set<Meta>().Update(elemento);
            await _context.SaveChangesAsync();
        }


        public async Task CadastrarNovoAporteAsync(AporteMeta elemento)
        {
            await _context.Set<AporteMeta>().AddAsync(elemento);
            await _context.SaveChangesAsync();
        }
    }
}
