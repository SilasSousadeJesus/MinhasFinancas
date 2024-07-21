using MinhasFinancas.Application.DTOs.Categoria;
using MinhasFinancas.Application.Interfaces.baseInterface;

namespace MinhasFinancas.Application.Interfaces
{
    public interface ICategoriaAppService : IAppService<CadastrarCategoria, EditarCategoria>
    {
    }
}
