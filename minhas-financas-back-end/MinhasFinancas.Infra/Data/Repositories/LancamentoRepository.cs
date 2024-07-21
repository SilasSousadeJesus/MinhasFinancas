using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Infra.Data.Repositories
{
    public class LancamentoRepository : ILancamentoRepository
    {

        private readonly ApplicationDbContext _context;

        public LancamentoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public Task<List<Lancamento>> BuscarTodosOsElementosAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Lancamento> BuscarUmElementoAsync(string idPatrono, Guid id)
        {
            throw new NotImplementedException();
        }

        public Task CadastrarElementoAsync(Lancamento elemento)
        {
            throw new NotImplementedException();
        }

        public Task DeletarElementoAsync(Lancamento elemento)
        {
            throw new NotImplementedException();
        }

        public Task EditarElementoAsync(Lancamento elemento)
        {
            throw new NotImplementedException();
        }
    }
}
