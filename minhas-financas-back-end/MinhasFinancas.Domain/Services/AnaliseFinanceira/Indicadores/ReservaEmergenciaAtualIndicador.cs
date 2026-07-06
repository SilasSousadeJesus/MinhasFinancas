using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    public class ReservaEmergenciaAtualIndicador : ICalculadorIndicadorFinanceiro
    {
        public CodigoIndicadorFinanceiro Codigo => CodigoIndicadorFinanceiro.ReservaEmergenciaAtual;

        public IndicadorFinanceiro Calcular(ContextoAnaliseFinanceira contexto, DadosReferenciaAnaliseFinanceira dadosReferencia)
        {
            return new IndicadorFinanceiro
            {
                Codigo = Codigo,
                Nome = "Reserva de emergência atual",
                ValorAtual = dadosReferencia.ReservaEmergenciaAtual,
                ValorIdeal = dadosReferencia.ReservaEmergenciaIdealConfigurada,
                Percentual = dadosReferencia.ReservaEmergenciaIdealConfigurada > 0
                    ? (dadosReferencia.ReservaEmergenciaAtual / dadosReferencia.ReservaEmergenciaIdealConfigurada) * 100m
                    : 0m,
                Status = ResolutorStatusIndicadorFinanceiro.ResolverMetaMinima(
                    dadosReferencia.ReservaEmergenciaAtual,
                    dadosReferencia.ReservaEmergenciaIdealConfigurada),
                Descricao = "Valor atual reservado em ativos líquidos, considerando dinheiro em conta e investimentos.",
                Observacao = $"Cobertura atual estimada: {dadosReferencia.CoberturaReservaEmMeses:N2} mês(es) de despesas.",
                Formato = FormatoValorIndicadorFinanceiro.Moeda
            };
        }
    }
}
