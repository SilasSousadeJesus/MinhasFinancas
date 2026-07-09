using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    public class ComprometimentoFinanceiroFuturo365DiasIndicador : ICalculadorIndicadorFinanceiro
    {
        public CodigoIndicadorFinanceiro Codigo => CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias;

        public IndicadorFinanceiro Calcular(ContextoAnaliseFinanceira contexto, DadosReferenciaAnaliseFinanceira dadosReferencia)
        {
            var percentualMaximo = contexto.ConfiguracaoPerfilFinanceiro?.PercentualMaximoComprometimentoRenda ?? 0m;

            return new IndicadorFinanceiro
            {
                Codigo = Codigo,
                Nome = "Comprometimento financeiro futuro - 12 meses",
                ValorAtual = dadosReferencia.ComprometimentoFinanceiroFuturo365DiasAtual,
                ValorIdeal = percentualMaximo,
                Percentual = percentualMaximo > 0
                    ? (dadosReferencia.ComprometimentoFinanceiroFuturo365DiasAtual / percentualMaximo) * 100m
                    : 0m,
                Status = ResolutorStatusIndicadorFinanceiro.ResolverMetaMaxima(
                    dadosReferencia.ComprometimentoFinanceiroFuturo365DiasAtual,
                    percentualMaximo),
                Descricao = "Percentual da renda que já está comprometido com despesas pendentes nos próximos 12 meses.",
                Observacao = percentualMaximo > 0
                    ? $"Considera {dadosReferencia.ObrigacoesFinanceirasFuturas365Dias:N2} em despesas pendentes com vencimento nos próximos 12 meses."
                    : "Sem limite máximo de comprometimento da renda configurado no perfil financeiro.",
                Formato = FormatoValorIndicadorFinanceiro.Percentual
            };
        }
    }
}
