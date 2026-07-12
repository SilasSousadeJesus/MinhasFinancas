using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    public class EndividamentoIndicador : ICalculadorIndicadorFinanceiro
    {
        public CodigoIndicadorFinanceiro Codigo => CodigoIndicadorFinanceiro.Endividamento;

        public IndicadorFinanceiro Calcular(ContextoAnaliseFinanceira contexto, DadosReferenciaAnaliseFinanceira dadosReferencia)
        {
            var percentualMaximo = contexto.ConfiguracaoPerfilFinanceiro?.PercentualMaximoEndividamento ?? 0m;

            return new IndicadorFinanceiro
            {
                Codigo = Codigo,
                Nome = "Exposição a dívidas e passivos",
                ValorAtual = dadosReferencia.EndividamentoAtual,
                ValorIdeal = percentualMaximo,
                Percentual = percentualMaximo > 0m
                    ? (dadosReferencia.EndividamentoAtual / percentualMaximo) * 100m
                    : 0m,
                Status = ResolutorStatusIndicadorFinanceiro.ResolverFaixaDecrescente(
                    dadosReferencia.EndividamentoAtual,
                    15m,
                    30m,
                    50m),
                Descricao = "Mede a pressão estrutural dos passivos sobre a base patrimonial, distinguindo dívida de consumo, financiamento patrimonial e obrigações estruturais.",
                Observacao = percentualMaximo > 0m
                    ? $"Composição atual: consumo {dadosReferencia.TotalPassivosConsumo:N2}, financiamento patrimonial {dadosReferencia.TotalPassivosPatrimoniais:N2} e obrigações estruturais {dadosReferencia.TotalPassivosObrigacoesEstruturais:N2}. Limite configurado no perfil financeiro: {percentualMaximo:N2}% dos ativos."
                    : $"Composição atual: consumo {dadosReferencia.TotalPassivosConsumo:N2}, financiamento patrimonial {dadosReferencia.TotalPassivosPatrimoniais:N2} e obrigações estruturais {dadosReferencia.TotalPassivosObrigacoesEstruturais:N2}.",
                Formato = FormatoValorIndicadorFinanceiro.Percentual
            };
        }
    }
}
