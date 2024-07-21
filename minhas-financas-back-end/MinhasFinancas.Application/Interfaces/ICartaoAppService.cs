using MinhasFinancas.Application.DTOs.Banco;
using MinhasFinancas.Application.DTOs.Cartao;
using MinhasFinancas.Application.Interfaces.baseInterface;

namespace MinhasFinancas.Application.Interfaces
{
    public interface ICartaoAppService : IAppService<CadastrarCartaoDTO, EditarCartaoDTO>
    {
    }
}
