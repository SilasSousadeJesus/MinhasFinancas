using MinhasFinancas.Application.DTOs.Patrimonio;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IPatrimonioAppService
    {
        Task<RetornoGenerico> BuscarVisaoGeralAsync(string usuarioId);
        Task<RetornoGenerico> GerarSnapshotAsync(string usuarioId, CadastrarSnapshotPatrimonialDTO snapshotDTO);
    }
}
