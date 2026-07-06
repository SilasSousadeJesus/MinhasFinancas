using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IAnaliseFinanceiraAppService
    {
        Task<RetornoGenerico> BuscarIndicadoresFinanceiros(string usuarioId);
        Task<PainelIndicadoresFinanceiros?> BuscarPainelIndicadoresInternoAsync(string usuarioId);
    }
}
