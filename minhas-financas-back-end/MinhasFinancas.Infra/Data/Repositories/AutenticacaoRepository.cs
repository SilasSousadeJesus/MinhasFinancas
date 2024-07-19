
using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class AutenticacaoRepository : IAutenticacaoRepository
    {
        private readonly ApplicationDbContext _context;
        public AutenticacaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Usuario?> BuscarUsuarioPorEmail(string email)
        {
            throw new NotImplementedException();
        }


        //public async  Task<ApplicationUser?> BuscarUsuarioPorEmail(string email)
        //{
        //    try
        //    {
        //        var usuario = await _context.AspNetUsers
        //                        .Where(x => x.Email == email)
        //                        .FirstOrDefaultAsync();

        //        return usuario;
        //    }
        //    catch(Exception ex) {
        //        Console.WriteLine(ex);
        //        return null;
        //    }
        //}
    }
}
