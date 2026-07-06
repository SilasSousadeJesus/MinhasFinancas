using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    public interface ICalculadorIndicadorFinanceiro
    {
        CodigoIndicadorFinanceiro Codigo { get; }
        IndicadorFinanceiro Calcular(ContextoAnaliseFinanceira contexto, DadosReferenciaAnaliseFinanceira dadosReferencia);
    }
}
