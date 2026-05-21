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
                                        .Include(c => c.SubCategorias)
                                        .Where(b => b.UsuarioId == Id)
                                        .ToListAsync();
        }

        public async Task<bool> UsuarioPossuiCategoriasAsync(string usuarioId)
        {
            return await _context.Set<Categoria>().AnyAsync(c => c.UsuarioId == usuarioId);
        }

        public async Task<bool> ExisteCategoriaComNomeAsync(string usuarioId, string nomeCategoria, Guid? ignorarCategoriaId = null)
        {
            var nomeNormalizado = nomeCategoria.Trim().ToLower();

            return await _context.Set<Categoria>()
                .Where(c => c.UsuarioId == usuarioId)
                .Where(c => !ignorarCategoriaId.HasValue || c.Id != ignorarCategoriaId.Value)
                .AnyAsync(c => c.NomeCategoria.ToLower() == nomeNormalizado);
        }

        public async Task<Categoria> BuscarUmElementoAsync(string idPatrono, Guid id)
        {
            return await _context.Set<Categoria>()
                .Include(c => c.SubCategorias)
                .Where(x => x.UsuarioId == idPatrono && x.Id == id)
                .FirstOrDefaultAsync();
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

        public async Task CadastrarListaDeCategoriasAsync(List<Categoria> listaCategoria)
        {
            await _context.Set<Categoria>().AddRangeAsync(listaCategoria);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SubCategoria>> BuscarTodosAsSubCategoriasAsync(string usuarioId, Guid categoriaId)
        {
            return await _context.Set<SubCategoria>()
                .Join(
                    _context.Set<Categoria>(),
                    subCategoria => subCategoria.CategoriaId,
                    categoria => categoria.Id,
                    (subCategoria, categoria) => new { subCategoria, categoria }
                )
                .Where(x => x.categoria.UsuarioId == usuarioId && x.categoria.Id == categoriaId)
                .Select(x => x.subCategoria)
                .ToListAsync();
        }

        public async Task<bool> ExisteSubCategoriaComNomeAsync(Guid categoriaId, string nomeSubCategoria, Guid? ignorarSubCategoriaId = null)
        {
            var nomeNormalizado = nomeSubCategoria.Trim().ToLower();

            return await _context.Set<SubCategoria>()
                .Where(x => x.CategoriaId == categoriaId)
                .Where(x => !ignorarSubCategoriaId.HasValue || x.Id != ignorarSubCategoriaId.Value)
                .AnyAsync(x => x.NomeSubCategoria.ToLower() == nomeNormalizado);
        }

        public async Task<SubCategoria?> BuscarUmaSubCategoriaAsync(Guid categoriaId, Guid subCategoriaId)
        {
            return await _context.Set<SubCategoria>()
                .Where(x => x.CategoriaId == categoriaId && x.Id == subCategoriaId)
                .FirstOrDefaultAsync();
        }

        public async Task CadastrarSubCategoriaAsync(SubCategoria subCategoria)
        {
            await _context.Set<SubCategoria>().AddAsync(subCategoria);
            await _context.SaveChangesAsync();
        }

        public async Task EditarSubCategoriaAsync(SubCategoria subCategoria)
        {
            var existingEntity = _context.Set<SubCategoria>().Local.FirstOrDefault(x => x.Id == subCategoria.Id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            _context.Set<SubCategoria>().Update(subCategoria);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarSubCategoriaAsync(SubCategoria subCategoria)
        {
            _context.Set<SubCategoria>().Remove(subCategoria);
            await _context.SaveChangesAsync();
        }
    }
}
