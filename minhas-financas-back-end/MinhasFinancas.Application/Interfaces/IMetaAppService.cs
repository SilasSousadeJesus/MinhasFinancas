using MinhasFinancas.Application.DTOs.Meta;
using MinhasFinancas.Application.Interfaces.baseInterface;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IMetaAppService : IAppService<CadastrarMetaDTO, EditarMetalDTO>
    {
        Task<RetornoGenerico> AtualizarAndamentoMetaAsync(string idPatrono, Guid elementoId, decimal valor);
    }
}
