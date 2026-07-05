using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class PatrimonioRepository : IPatrimonioRepository
    {
        private readonly ApplicationDbContext _context;

        public PatrimonioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SnapshotPatrimonial>> BuscarSnapshotsAsync(string usuarioId)
        {
            return await _context.Set<SnapshotPatrimonial>()
                .AsNoTracking()
                .Where(x => x.UsuarioId == usuarioId)
                .OrderByDescending(x => x.DataReferencia)
                .ThenByDescending(x => x.DataCriacao)
                .ToListAsync();
        }

        public async Task CadastrarSnapshotAsync(SnapshotPatrimonial snapshot)
        {
            await _context.Set<SnapshotPatrimonial>().AddAsync(snapshot);
            await _context.SaveChangesAsync();
        }
    }
}
