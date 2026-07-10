using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;



namespace MinhasFinancas.Infra.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {

        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteUsuarioAsync(string usuarioId)
        {
            return await _context.Users.AsNoTracking().AnyAsync(x => x.Id == usuarioId);
        }

        public async Task<List<string>> BuscarIdsUsuariosAtivosAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Select(x => x.Id)
                .ToListAsync();
        }

        public async Task<List<Usuario>> BuscarUsuariosParaLaboratorioAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .OrderBy(x => x.Nome ?? x.Email)
                .ThenBy(x => x.Email)
                .ToListAsync();
        }

        public async Task<Usuario?> BuscarResumoUsuarioAsync(string usuarioId)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == usuarioId);
        }


        public async Task DeletarUsuarioESeusDados(Usuario usuario)
        {
            var categorias = await _context.Categoria.Where(x=> x.UsuarioId == usuario.Id).ToListAsync();
            var cartoes = await _context.Cartao.Where(x=> x.UsuarioId == usuario.Id).ToListAsync();
            var contas = await _context.Conta.Where(x=> x.UsuarioId == usuario.Id).ToListAsync();
            var lancamentos = await _context.Lancamento.Where(x=> x.UsuarioId == usuario.Id).ToListAsync();
            var bemPatrimonial = await _context.BemPatrimonial.Where(x=> x.UsuarioId == usuario.Id).ToListAsync();
            var metas = await _context.Meta.Where(x=> x.UsuarioId == usuario.Id).ToListAsync();
               
            if (usuario != null)
            {
                _context.Meta.RemoveRange(metas);
                _context.BemPatrimonial.RemoveRange(bemPatrimonial);
                _context.Lancamento.RemoveRange(lancamentos);
                _context.Categoria.RemoveRange(categorias);
                _context.Cartao.RemoveRange(cartoes);
                _context.Conta.RemoveRange(contas);
                _context.Users.Remove(usuario);

                await _context.SaveChangesAsync();
            }
        }
    }  
}
