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
                Nome = "Comprometimento financeiro futuro - 30 dias",
                ValorAtual = dadosReferencia.ComprometimentoFinanceiroFuturoAtual,
                ValorIdeal = percentualMaximo,
                Percentual = percentualMaximo > 0
                    ? (dadosReferencia.ComprometimentoFinanceiroFuturoAtual / percentualMaximo) * 100m
                    : 0m,
                ValorObrigacoesPrevistas = dadosReferencia.ObrigacoesFinanceirasFuturas30Dias,
                ValorReceitaPrevista = dadosReferencia.ReceitaPrevista30Dias,
                PercentualComprometimento = dadosReferencia.ComprometimentoFinanceiroFuturoAtual,
                Status = ResolutorStatusIndicadorFinanceiro.ResolverFaixaDecrescente(
                    dadosReferencia.ComprometimentoFinanceiroFuturoAtual,
                    25m,
                    40m,
                    55m),
                Descricao = "Percentual da renda prevista para os próximos 30 dias que já está comprometido com despesas e obrigações futuras.",
                Observacao = percentualMaximo > 0
                    ? $"Considera {dadosReferencia.ObrigacoesFinanceirasFuturas30Dias:N2} em despesas pendentes sobre {dadosReferencia.ReceitaPrevista30Dias:N2} de receita prevista para os próximos 30 dias."
                    : "Sem limite máximo de comprometimento da renda configurado no perfil financeiro.",
                Formato = FormatoValorIndicadorFinanceiro.Percentual
            };
        }
    }
}
