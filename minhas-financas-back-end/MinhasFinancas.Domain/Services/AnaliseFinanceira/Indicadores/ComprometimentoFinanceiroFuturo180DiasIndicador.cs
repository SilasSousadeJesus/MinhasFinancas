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
                Nome = "Pressão financeira acumulada - 180 dias",
                ValorAtual = dadosReferencia.ComprometimentoFinanceiroFuturo180DiasAtual,
                ValorIdeal = percentualMaximo,
                Percentual = percentualMaximo > 0m
                    ? (dadosReferencia.ComprometimentoFinanceiroFuturo180DiasAtual / percentualMaximo) * 100m
                    : 0m,
                ValorObrigacoesPrevistas = dadosReferencia.ObrigacoesFinanceirasFuturas180Dias,
                ValorReceitaPrevista = dadosReferencia.ReceitaPrevista180Dias,
                PercentualComprometimento = dadosReferencia.ComprometimentoFinanceiroFuturo180DiasAtual,
                Status = ResolutorStatusIndicadorFinanceiro.ResolverFaixaDecrescente(
                    dadosReferencia.ComprometimentoFinanceiroFuturo180DiasAtual,
                    45m,
                    65m,
                    100m),
                Descricao = "Percentual da renda prevista para os próximos 180 dias que já está comprometido por despesas e obrigações futuras.",
                Observacao = percentualMaximo > 0m
                    ? $"Considera {dadosReferencia.ObrigacoesFinanceirasFuturas180Dias:N2} em obrigações futuras sobre {dadosReferencia.ReceitaPrevista180Dias:N2} de receita prevista para os próximos 180 dias."
                    : "Sem limite máximo configurado no perfil financeiro para medir a pressão financeira acumulada de 180 dias.",
                Formato = FormatoValorIndicadorFinanceiro.Percentual
            };
        }
    }
}
