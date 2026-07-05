using MinhasFinancas.Application.DTOs.SimulacaoFinanceira;

namespace MinhasFinancas.Application.Interfaces
{
    public interface ISimulacaoFinanceiraAppService
    {
        Task<RetornoGenerico> BuscarTodasAsync(string usuarioId);
        Task<RetornoGenerico> BuscarUmaAsync(string usuarioId, Guid simulacaoId);
        Task<RetornoGenerico> CadastrarAsync(CadastrarSimulacaoFinanceiraDTO simulacaoDTO);
        Task<RetornoGenerico> EditarAsync(string usuarioId, Guid simulacaoId, EditarSimulacaoFinanceiraDTO simulacaoDTO);
        Task<RetornoGenerico> InativarAsync(string usuarioId, Guid simulacaoId);
        Task<RetornoGenerico> CalcularAsync(string usuarioId, Guid simulacaoId);
    }
}
