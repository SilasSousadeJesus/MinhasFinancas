using Microsoft.EntityFrameworkCore;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class BemMaterialRepository : IBemMaterialRepository
    {

        private readonly ApplicationDbContext _context;

        public BemMaterialRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BemPatrimonial>> BuscarTodosOsElementosAsync(string id)
        {
            var listaDeBens =await _context.Set<BemPatrimonial>()
                .Include(x => x.DataPermanencia)
                 .Where(b => b.UsuarioId == id)
                 .ToListAsync();

            return listaDeBens;
        }

        public async Task<BemPatrimonial> BuscarUmElementoAsync(string idPatrono, Guid id)
        {
            return await _context.Set<BemPatrimonial>().Where(x => x.UsuarioId == idPatrono && x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CadastrarElementoAsync(BemPatrimonial elemento)
        {
            await _context.Set<BemPatrimonial>().AddAsync(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarElementoAsync(BemPatrimonial elemento)
        {
            _context.Set<BemPatrimonial>().Remove(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task EditarElementoAsync(BemPatrimonial elemento)
        {
            var existingEntity = _context.Set<BemPatrimonial>().Local.FirstOrDefault(b => b.Id == elemento.Id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            _context.Set<BemPatrimonial>().Update(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task CadastrarElementoAsync(List<BemPatrimonial> listaElemento)
        {
            await _context.Set<BemPatrimonial>().AddRangeAsync(listaElemento);
            await _context.SaveChangesAsync();
        }

    }
}
