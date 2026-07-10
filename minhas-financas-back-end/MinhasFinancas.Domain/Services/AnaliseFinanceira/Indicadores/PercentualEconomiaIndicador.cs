using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    public class PercentualEconomiaIndicador : ICalculadorIndicadorFinanceiro
    {
        public CodigoIndicadorFinanceiro Codigo => CodigoIndicadorFinanceiro.PercentualEconomia;

        public IndicadorFinanceiro Calcular(ContextoAnaliseFinanceira contexto, DadosReferenciaAnaliseFinanceira dadosReferencia)
        {
            var valorIdeal = contexto.ConfiguracaoPerfilFinanceiro?.PercentualEconomiaMensalDesejado ?? 0m;

            return new IndicadorFinanceiro
            {
                Codigo = Codigo,
                Nome = "Percentual de economia",
                ValorAtual = dadosReferencia.PercentualEconomiaAtual,
                ValorIdeal = valorIdeal,
                Percentual = valorIdeal > 0 ? (dadosReferencia.PercentualEconomiaAtual / valorIdeal) * 100m : 0m,
                Status = ResolutorStatusIndicadorFinanceiro.ResolverFaixaCrescente(
                    dadosReferencia.PercentualEconomiaAtual,
                    20m,
                    10m,
                    0m),
                Descricao = "Percentual da renda mensal que realmente sobra após as despesas do mês atual.",
                Observacao = valorIdeal > 0
                    ? $"Objetivo desejado no perfil financeiro: {valorIdeal:N2}%."
                    : "Sem objetivo percentual de economia configurado no perfil financeiro.",
                Formato = FormatoValorIndicadorFinanceiro.Percentual
            };
        }
    }
}
