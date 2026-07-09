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
                Nome = "Pressão financeira acumulada - 90 dias",
                ValorAtual = dadosReferencia.ComprometimentoFinanceiroFuturo90DiasAtual,
                ValorIdeal = percentualMaximo,
                Percentual = percentualMaximo > 0
                    ? (dadosReferencia.ComprometimentoFinanceiroFuturo90DiasAtual / percentualMaximo) * 100m
                    : 0m,
                ValorObrigacoesPrevistas = dadosReferencia.ObrigacoesFinanceirasFuturas90Dias,
                ValorReceitaPrevista = dadosReferencia.ReceitaPrevista90Dias,
                PercentualComprometimento = dadosReferencia.ComprometimentoFinanceiroFuturo90DiasAtual,
                Status = ResolutorStatusIndicadorFinanceiro.ResolverMetaMaxima(
                    dadosReferencia.ComprometimentoFinanceiroFuturo90DiasAtual,
                    percentualMaximo),
                Descricao = "Percentual da renda prevista para os próximos 90 dias que já está comprometido por despesas e obrigações futuras.",
                Observacao = percentualMaximo > 0
                    ? $"Considera {dadosReferencia.ObrigacoesFinanceirasFuturas90Dias:N2} em obrigações futuras sobre {dadosReferencia.ReceitaPrevista90Dias:N2} de receita prevista para os próximos 90 dias."
                    : "Sem limite máximo configurado no perfil financeiro para medir a pressão financeira acumulada de 90 dias.",
                Formato = FormatoValorIndicadorFinanceiro.Percentual
            };
        }
    }
}
