using MinhasFinancas.Application.DTOs.PerfilFinanceiro;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IPerfilFinanceiroAppService
    {
        Task<RetornoGenerico> BuscarVisaoGeralAsync(string usuarioId);
        Task<RetornoGenerico> SalvarConfiguracaoAsync(string usuarioId, SalvarPerfilFinanceiroDTO configuracaoDTO);
    }
}
