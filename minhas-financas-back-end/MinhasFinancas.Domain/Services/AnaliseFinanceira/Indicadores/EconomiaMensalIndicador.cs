using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    public class EconomiaMensalIndicador : ICalculadorIndicadorFinanceiro
    {
        public CodigoIndicadorFinanceiro Codigo => CodigoIndicadorFinanceiro.EconomiaMensal;

        public IndicadorFinanceiro Calcular(ContextoAnaliseFinanceira contexto, DadosReferenciaAnaliseFinanceira dadosReferencia)
        {
            var percentualDesejado = contexto.ConfiguracaoPerfilFinanceiro?.PercentualEconomiaMensalDesejado ?? 0m;
            var valorIdeal = dadosReferencia.ReceitaMensalAtual * (percentualDesejado / 100m);

            return new IndicadorFinanceiro
            {
                Codigo = Codigo,
                Nome = "Economia mensal",
                ValorAtual = dadosReferencia.EconomiaMensalAtual,
                ValorIdeal = valorIdeal,
                Percentual = dadosReferencia.PercentualEconomiaAtual,
                Status = ResolutorStatusIndicadorFinanceiro.ResolverMetaMinima(dadosReferencia.EconomiaMensalAtual, valorIdeal),
                Descricao = "Saldo mensal obtido pela diferença entre receitas e despesas do mês atual.",
                Observacao = percentualDesejado > 0
                    ? $"Meta configurada no perfil financeiro: economizar {percentualDesejado:N2}% da renda mensal."
                    : "Sem percentual desejado de economia configurado no perfil financeiro.",
                Formato = FormatoValorIndicadorFinanceiro.Moeda
            };
        }
    }
}
