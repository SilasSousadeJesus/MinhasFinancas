using MinhasFinancas.Application.DTOs.Categoria;
using MinhasFinancas.Application.Interfaces.baseInterface;

namespace MinhasFinancas.Application.Interfaces
{
    public interface ICategoriaAppService : IAppService<CadastrarCategoriaDTO, EditarCategoriaDTO>
    {
        Task<RetornoGenerico> CadastrarSubCategoriaAsync(string usuarioId, Guid categoriaId, CadastrarSubCategoriaDTO cadastrarSubCategoriaDTO);
        Task<RetornoGenerico> EditarSubCategoriaAsync(string usuarioId, Guid categoriaId, Guid subCategoriaId, EditarSubCategoriaDTO editarSubCategoriaDTO);
        Task<RetornoGenerico> BuscarTodosAsSubCategoriaAsync(string usuarioId, Guid categoriaId);
        Task<RetornoGenerico> BuscarUmaSubCategoriaAsync(Guid categoriaId, Guid subCategoriaId);
        Task<RetornoGenerico> DeletarSubCategoriaAsync(string usuarioId, Guid categoriaId, Guid subCategoriaId);
    }
}
