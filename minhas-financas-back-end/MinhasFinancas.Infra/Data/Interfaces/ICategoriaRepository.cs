using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task CadastrarListaDeCategoriasAsync(List<Categoria> listaCategoria);
        Task<bool> UsuarioPossuiCategoriasAsync(string usuarioId);
        Task<bool> ExisteCategoriaComNomeAsync(string usuarioId, string nomeCategoria, Guid? ignorarCategoriaId = null);
        Task<List<SubCategoria>> BuscarTodosAsSubCategoriasAsync(string usuarioId, Guid categoriaId);
        Task<bool> ExisteSubCategoriaComNomeAsync(Guid categoriaId, string nomeSubCategoria, Guid? ignorarSubCategoriaId = null);
        Task<SubCategoria?> BuscarUmaSubCategoriaAsync(Guid categoriaId, Guid subCategoriaId);
        Task CadastrarSubCategoriaAsync(SubCategoria subCategoria);
        Task EditarSubCategoriaAsync(SubCategoria subCategoria);
        Task DeletarSubCategoriaAsync(SubCategoria subCategoria);
    }
}
