using MinhasFinancas.Application.DTOs.Passivo;
using MinhasFinancas.Application.Interfaces.baseInterface;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IPassivoAppService : IAppService<CadastrarPassivoDTO, EditarPassivoDTO>
    {
    }
}
