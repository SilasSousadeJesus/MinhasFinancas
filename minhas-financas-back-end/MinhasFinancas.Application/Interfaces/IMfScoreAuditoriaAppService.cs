using System.Threading.Tasks;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IMfScoreAuditoriaAppService
    {
        Task<RetornoGenerico> GerarPlanilhaAsync();
        Task<RetornoGenerico> GerarPlanilhaAuditoriaHumanaAsync();
    }
}
