using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira
{
    public interface IIndicadoresFinanceirosService
    {
        PainelIndicadoresFinanceiros Calcular(ContextoAnaliseFinanceira contexto);
    }
}
