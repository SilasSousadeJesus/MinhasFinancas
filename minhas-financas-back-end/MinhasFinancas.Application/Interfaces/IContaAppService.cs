using MinhasFinancas.Application.DTOs.Banco;
using MinhasFinancas.Application.Interfaces.baseInterface;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IContaAppService : IAppService<CadastrarContaDTO, EditarContaDTO>
    {
    }
}
