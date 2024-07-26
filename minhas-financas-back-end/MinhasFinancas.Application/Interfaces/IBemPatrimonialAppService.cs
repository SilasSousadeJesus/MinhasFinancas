using MinhasFinancas.Application.DTOs.BemPatrimonial;
using MinhasFinancas.Application.Interfaces.baseInterface;
using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IBemPatrimonialAppService : IAppService<CadastrarBemPatrimonialDTO, EditarBemPatrimonialDTO>
    {
        Task<RetornoGenerico> BuscarUltimaDataPermanencia(Guid ultimaDataPermanente);
        Task EditarUltimaDataPermanencia(PermanenciaBemMaterial permanencia);
    }
}
