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
                Nome = "Endividamento patrimonial",
                ValorAtual = dadosReferencia.EndividamentoAtual,
                ValorIdeal = percentualMaximo,
                Percentual = percentualMaximo > 0 ? (dadosReferencia.EndividamentoAtual / percentualMaximo) * 100m : 0m,
                Status = ResolutorStatusIndicadorFinanceiro.ResolverFaixaDecrescente(
                    dadosReferencia.EndividamentoAtual,
                    15m,
                    30m,
                    50m),
                Descricao = "Relação entre passivos patrimoniais e a base patrimonial ativa disponível.",
                Observacao = percentualMaximo > 0
                    ? $"Limite configurado no perfil financeiro: {percentualMaximo:N2}% dos ativos."
                    : "Sem limite máximo de endividamento patrimonial configurado no perfil financeiro.",
                Formato = FormatoValorIndicadorFinanceiro.Percentual
            };
        }
    }
}
