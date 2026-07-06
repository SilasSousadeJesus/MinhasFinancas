using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IInteligenciaFinanceiraAppService
    {
        Task<RetornoGenerico> BuscarInsightsFinanceiros(string usuarioId);
        Task<RetornoGenerico> BuscarResumoFinanceiroIA(string usuarioId);
        Task<ResumoFinanceiroIA?> BuscarResumoFinanceiroIAInternoAsync(string usuarioId);
    }
}
