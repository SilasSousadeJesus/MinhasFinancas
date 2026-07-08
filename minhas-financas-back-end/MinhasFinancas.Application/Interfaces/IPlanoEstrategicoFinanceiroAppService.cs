using MinhasFinancas.Application.DTOs.PlanoEstrategicoFinanceiro;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IPlanoEstrategicoFinanceiroAppService
    {
        Task<RetornoGenerico> BuscarTodosAsync(string usuarioId);
        Task<RetornoGenerico> BuscarVigenteAsync(string usuarioId);
        Task<RetornoGenerico> BuscarUmAsync(string usuarioId, Guid planoId);
        Task<RetornoGenerico> CadastrarAsync(string usuarioId, SalvarPlanoEstrategicoFinanceiroDTO dto);
        Task<RetornoGenerico> AtualizarVersaoAsync(string usuarioId, Guid planoId, SalvarPlanoEstrategicoFinanceiroDTO dto);
        Task<RetornoGenerico> InativarAsync(string usuarioId, Guid planoId);
    }
}
