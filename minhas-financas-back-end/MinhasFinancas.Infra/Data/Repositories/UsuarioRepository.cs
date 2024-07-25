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


        public async Task DeletarUsuarioESeusDados(Usuario usuario)
        {
            var categorias = await _context.Categoria.Where(x=> x.UsuarioId == usuario.Id).ToListAsync();
            var cartoes = await _context.Cartao.Where(x=> x.UsuarioId == usuario.Id).ToListAsync();
            var contas = await _context.Conta.Where(x=> x.UsuarioId == usuario.Id).ToListAsync();
            var lancamentos = await _context.Lancamento.Where(x=> x.UsuarioId == usuario.Id).ToListAsync();
            var bemPatrimonial = await _context.BemPatrimonial.Where(x=> x.UsuarioId == usuario.Id).ToListAsync();
               
            if (usuario != null)
            {
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
