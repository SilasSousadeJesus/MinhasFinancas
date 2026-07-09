using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    public class ComprometimentoFinanceiroFuturo90DiasIndicador : ICalculadorIndicadorFinanceiro
    {
        public CodigoIndicadorFinanceiro Codigo => CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias;

        public IndicadorFinanceiro Calcular(ContextoAnaliseFinanceira contexto, DadosReferenciaAnaliseFinanceira dadosReferencia)
        {
            var percentualMaximo = contexto.ConfiguracaoPerfilFinanceiro?.PercentualMaximoComprometimentoRenda ?? 0m;

            return new IndicadorFinanceiro
            {
                Codigo = Codigo,
                Nome = "Comprometimento financeiro futuro - 90 dias",
                ValorAtual = dadosReferencia.ComprometimentoFinanceiroFuturo90DiasAtual,
                ValorIdeal = percentualMaximo,
                Percentual = percentualMaximo > 0
                    ? (dadosReferencia.ComprometimentoFinanceiroFuturo90DiasAtual / percentualMaximo) * 100m
                    : 0m,
                Status = ResolutorStatusIndicadorFinanceiro.ResolverMetaMaxima(
                    dadosReferencia.ComprometimentoFinanceiroFuturo90DiasAtual,
                    percentualMaximo),
                Descricao = "Percentual da renda que já está comprometido com despesas pendentes nos próximos 90 dias.",
                Observacao = percentualMaximo > 0
                    ? $"Considera {dadosReferencia.ObrigacoesFinanceirasFuturas90Dias:N2} em despesas pendentes com vencimento nos próximos 90 dias."
                    : "Sem limite máximo de comprometimento da renda configurado no perfil financeiro.",
                Formato = FormatoValorIndicadorFinanceiro.Percentual
            };
        }
    }
}
