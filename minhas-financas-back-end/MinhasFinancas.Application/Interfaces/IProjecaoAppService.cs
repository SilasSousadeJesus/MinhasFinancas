using MinhasFinancas.Application.DTOs.Projecao;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IProjecaoAppService
    {
        Task<RetornoGenerico> BuscarTodosAsync(string usuarioId);
        Task<RetornoGenerico> BuscarUmAsync(string usuarioId, Guid projecaoId);
        Task<RetornoGenerico> CadastrarAsync(CadastrarProjecaoDTO projecaoDTO);
        Task<RetornoGenerico> EditarAsync(string usuarioId, Guid projecaoId, EditarProjecaoDTO projecaoDTO);
        Task<RetornoGenerico> DeletarAsync(string usuarioId, Guid projecaoId);
        Task<RetornoGenerico> CalcularAsync(string usuarioId, CalcularProjecaoDTO calcularProjecaoDTO);
        Task<RetornoGenerico> CalcularAsync(string usuarioId, Guid projecaoId);
    }
}
