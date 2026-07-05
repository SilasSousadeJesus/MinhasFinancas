using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class SimulacaoFinanceiraRepository : ISimulacaoFinanceiraRepository
    {
        private readonly ApplicationDbContext _context;

        public SimulacaoFinanceiraRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SimulacaoFinanceira>> BuscarTodosOsElementosAsync(string id)
        {
            return await _context.Set<SimulacaoFinanceira>()
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.UsuarioId == id && x.Ativa)
                .Include(x => x.Acoes.Where(a => a.Ativa).OrderBy(a => a.DataInicial))
                .OrderByDescending(x => x.DataAtualizacao)
                .ToListAsync();
        }

        public async Task<SimulacaoFinanceira> BuscarUmElementoAsync(string idPatrono, Guid id)
        {
            return await _context.Set<SimulacaoFinanceira>()
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.UsuarioId == idPatrono && x.Id == id)
                .Include(x => x.Acoes.Where(a => a.Ativa).OrderBy(a => a.DataInicial))
                .FirstOrDefaultAsync();
        }

        public async Task CadastrarElementoAsync(SimulacaoFinanceira elemento)
        {
            await _context.Set<SimulacaoFinanceira>().AddAsync(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarElementoAsync(SimulacaoFinanceira elemento)
        {
            _context.Set<SimulacaoFinanceira>().Remove(elemento);
            await _context.SaveChangesAsync();
        }

        public async Task EditarElementoAsync(SimulacaoFinanceira elemento)
        {
            var existente = await _context.Set<SimulacaoFinanceira>()
                .Include(x => x.Acoes)
                .FirstOrDefaultAsync(x => x.Id == elemento.Id);

            if (existente == null)
            {
                return;
            }

            existente.Nome = elemento.Nome;
            existente.Descricao = elemento.Descricao;
            existente.DataInicial = elemento.DataInicial;
            existente.QuantidadeMeses = elemento.QuantidadeMeses;
            existente.DataAtualizacao = elemento.DataAtualizacao;
            existente.Ativa = elemento.Ativa;

            _context.Set<AcaoSimulacaoFinanceira>().RemoveRange(existente.Acoes);
            await _context.SaveChangesAsync();

            await _context.Set<AcaoSimulacaoFinanceira>().AddRangeAsync(elemento.Acoes);
            await _context.SaveChangesAsync();
        }
    }
}
