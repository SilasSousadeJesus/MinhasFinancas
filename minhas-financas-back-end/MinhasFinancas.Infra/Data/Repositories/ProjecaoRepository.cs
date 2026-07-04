using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class ProjecaoRepository : IProjecaoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjecaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Projecao>> BuscarTodosOsElementosAsync(string id)
        {
            return await _context.Set<Projecao>()
                .Where(x => x.UsuarioId == id)
                .Include(x => x.Rendas)
                .OrderByDescending(x => x.DataAtualizacao)
                .ToListAsync();
        }

        public async Task<Projecao> BuscarUmElementoAsync(string idPatrono, Guid id)
        {
            return await _context.Set<Projecao>()
                .Where(x => x.UsuarioId == idPatrono && x.Id == id)
                .Include(x => x.Rendas.OrderBy(r => r.Nome))
                .FirstOrDefaultAsync();
        }

        public async Task CadastrarElementoAsync(Projecao elemento)
        {
            await _context.Set<Projecao>().AddAsync(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarElementoAsync(Projecao elemento)
        {
            _context.Set<Projecao>().Remove(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task EditarElementoAsync(Projecao elemento)
        {
            var existente = await _context.Set<Projecao>()
                .Include(x => x.Rendas)
                .FirstOrDefaultAsync(x => x.Id == elemento.Id);

            if (existente == null)
            {
                return;
            }

            existente.Nome = elemento.Nome;
            existente.DataInicial = elemento.DataInicial;
            existente.ValorAcumuladoInicial = elemento.ValorAcumuladoInicial;
            existente.ValorObjetivo = elemento.ValorObjetivo;
            existente.MesesLimite = elemento.MesesLimite;
            existente.DataAtualizacao = elemento.DataAtualizacao;

            _context.Set<RendaProjecao>().RemoveRange(existente.Rendas);
            await _context.SaveChangesAsync();

            await _context.Set<RendaProjecao>().AddRangeAsync(elemento.Rendas);

            await _context.SaveChangesAsync();
        }
    }
}
