using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    public class ComprometimentoFinanceiroFuturo180DiasIndicador : ICalculadorIndicadorFinanceiro
    {
        public CodigoIndicadorFinanceiro Codigo => CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias;

        public IndicadorFinanceiro Calcular(ContextoAnaliseFinanceira contexto, DadosReferenciaAnaliseFinanceira dadosReferencia)
        {
            var percentualMaximo = contexto.ConfiguracaoPerfilFinanceiro?.PercentualMaximoComprometimentoRenda ?? 0m;

            return new IndicadorFinanceiro
            {
                Codigo = Codigo,
                Nome = "Comprometimento financeiro futuro - 180 dias",
                ValorAtual = dadosReferencia.ComprometimentoFinanceiroFuturo180DiasAtual,
                ValorIdeal = percentualMaximo,
                Percentual = percentualMaximo > 0
                    ? (dadosReferencia.ComprometimentoFinanceiroFuturo180DiasAtual / percentualMaximo) * 100m
                    : 0m,
                Status = ResolutorStatusIndicadorFinanceiro.ResolverMetaMaxima(
                    dadosReferencia.ComprometimentoFinanceiroFuturo180DiasAtual,
                    percentualMaximo),
                Descricao = "Percentual da renda que já está comprometido com despesas pendentes nos próximos 180 dias.",
                Observacao = percentualMaximo > 0
                    ? $"Considera {dadosReferencia.ObrigacoesFinanceirasFuturas180Dias:N2} em despesas pendentes com vencimento nos próximos 180 dias."
                    : "Sem limite máximo de comprometimento da renda configurado no perfil financeiro.",
                Formato = FormatoValorIndicadorFinanceiro.Percentual
            };
        }
    }
}
