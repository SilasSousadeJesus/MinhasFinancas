using MinhasFinancas.Application.DTOs.BemPatrimonial;
using MinhasFinancas.Application.Interfaces.baseInterface;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IBemPatrimonialAppService : IAppService<CadastrarBemPatrimonialDTO, EditarBemPatrimonialDTO>
    {
    }
}
