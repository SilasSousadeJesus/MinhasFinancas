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
                Nome = "Pressão financeira acumulada - 12 meses",
                ValorAtual = dadosReferencia.ComprometimentoFinanceiroFuturo365DiasAtual,
                ValorIdeal = percentualMaximo,
                Percentual = percentualMaximo > 0m
                    ? (dadosReferencia.ComprometimentoFinanceiroFuturo365DiasAtual / percentualMaximo) * 100m
                    : 0m,
                ValorObrigacoesPrevistas = dadosReferencia.ObrigacoesFinanceirasFuturas365Dias,
                ValorReceitaPrevista = dadosReferencia.ReceitaPrevista365Dias,
                PercentualComprometimento = dadosReferencia.ComprometimentoFinanceiroFuturo365DiasAtual,
                Status = ResolutorStatusIndicadorFinanceiro.ResolverFaixaDecrescente(
                    dadosReferencia.ComprometimentoFinanceiroFuturo365DiasAtual,
                    85m,
                    110m,
                    130m),
                Descricao = "Percentual da renda prevista para os próximos 12 meses que já está comprometido por despesas e obrigações futuras.",
                Observacao = percentualMaximo > 0m
                    ? $"Considera {dadosReferencia.ObrigacoesFinanceirasFuturas365Dias:N2} em obrigações futuras sobre {dadosReferencia.ReceitaPrevista365Dias:N2} de receita prevista para os próximos 12 meses."
                    : "Sem limite máximo configurado no perfil financeiro para medir a pressão financeira acumulada de 12 meses.",
                Formato = FormatoValorIndicadorFinanceiro.Percentual
            };
        }
    }
}
