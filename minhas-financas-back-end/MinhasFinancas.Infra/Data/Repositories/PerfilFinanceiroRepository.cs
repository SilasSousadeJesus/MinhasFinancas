using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class PerfilFinanceiroRepository : IPerfilFinanceiroRepository
    {
        private readonly ApplicationDbContext _context;

        public PerfilFinanceiroRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PerfilFinanceiro?> BuscarPorUsuarioAsync(string usuarioId)
        {
            return await _context.Set<PerfilFinanceiro>()
                .Include(x => x.Configuracoes)
                .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.Ativo);
        }

        public async Task<PerfilFinanceiro?> BuscarPorUsuarioLeituraAsync(string usuarioId)
        {
            return await _context.Set<PerfilFinanceiro>()
                .AsNoTracking()
                .Include(x => x.Configuracoes)
                .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.Ativo);
        }

        public async Task CadastrarAsync(PerfilFinanceiro perfilFinanceiro)
        {
            await _context.Set<PerfilFinanceiro>().AddAsync(perfilFinanceiro);
            await _context.SaveChangesAsync();
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
