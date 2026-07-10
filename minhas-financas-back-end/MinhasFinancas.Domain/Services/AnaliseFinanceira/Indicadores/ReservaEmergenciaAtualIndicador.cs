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
                Status = ResolutorStatusIndicadorFinanceiro.ResolverFaixaCrescente(
                    dadosReferencia.CoberturaReservaEmMeses,
                    6m,
                    4m,
                    2m),
                Descricao = "Valor atual reservado em ativos líquidos, considerando dinheiro em conta e investimentos.",
                Observacao = dadosReferencia.ReservaEmergenciaAtual <= 0 && dadosReferencia.PossuiCapacidadeFormacaoReserva
                    ? $"Cobertura atual estimada: {dadosReferencia.CoberturaReservaEmMeses:N2} mês(es) de despesas. Apesar da reserva zerada, a formação projetada pode acontecer em cerca de {dadosReferencia.MesesParaFormarReservaIdeal:N2} mês(es) com a sobra mensal atual."
                    : $"Cobertura atual estimada: {dadosReferencia.CoberturaReservaEmMeses:N2} mês(es) de despesas.",
                Formato = FormatoValorIndicadorFinanceiro.Moeda
            };
        }
    }
}
