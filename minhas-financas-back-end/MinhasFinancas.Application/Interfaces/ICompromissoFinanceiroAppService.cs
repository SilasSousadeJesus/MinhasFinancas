using MinhasFinancas.Application.DTOs.CompromissoFinanceiro;
using MinhasFinancas.Application.Interfaces.baseInterface;

namespace MinhasFinancas.Application.Interfaces
{
    public interface ICompromissoFinanceiroAppService : IAppService<SalvarCompromissoFinanceiroDTO, SalvarCompromissoFinanceiroDTO>
    {
        Task<RetornoGenerico> ConcluirAsync(string usuarioId, Guid compromissoId);
        Task<RetornoGenerico> CancelarAsync(string usuarioId, Guid compromissoId);
        Task<RetornoGenerico> ExcluirAsync(string usuarioId, Guid compromissoId);
    }
}
