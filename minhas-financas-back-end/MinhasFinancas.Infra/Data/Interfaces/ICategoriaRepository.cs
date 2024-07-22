using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task CadastrarListaDeCategoriasAsync(List<Categoria> listaCategoria);
    }
}
