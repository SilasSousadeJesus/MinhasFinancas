using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    public class ComprometimentoFinanceiroFuturoIndicador : ICalculadorIndicadorFinanceiro
    {
        public CodigoIndicadorFinanceiro Codigo => CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo;

        public IndicadorFinanceiro Calcular(ContextoAnaliseFinanceira contexto, DadosReferenciaAnaliseFinanceira dadosReferencia)
        {
            var percentualMaximo = contexto.ConfiguracaoPerfilFinanceiro?.PercentualMaximoComprometimentoRenda ?? 0m;

            return new IndicadorFinanceiro
            {
                Codigo = Codigo,
                Nome = "Comprometimento financeiro futuro",
                ValorAtual = dadosReferencia.ComprometimentoFinanceiroFuturoAtual,
                ValorIdeal = percentualMaximo,
                Percentual = percentualMaximo > 0
                    ? (dadosReferencia.ComprometimentoFinanceiroFuturoAtual / percentualMaximo) * 100m
                    : 0m,
                Status = ResolutorStatusIndicadorFinanceiro.ResolverMetaMaxima(
                    dadosReferencia.ComprometimentoFinanceiroFuturoAtual,
                    percentualMaximo),
                Descricao = "Percentual da renda que já está comprometido com despesas pendentes nos próximos 30 dias.",
                Observacao = percentualMaximo > 0
                    ? $"Considera {dadosReferencia.ObrigacoesFinanceirasFuturas30Dias:N2} em despesas pendentes com vencimento nos próximos 30 dias."
                    : "Sem limite máximo de comprometimento da renda configurado no perfil financeiro.",
                Formato = FormatoValorIndicadorFinanceiro.Percentual
            };
        }
    }
}
