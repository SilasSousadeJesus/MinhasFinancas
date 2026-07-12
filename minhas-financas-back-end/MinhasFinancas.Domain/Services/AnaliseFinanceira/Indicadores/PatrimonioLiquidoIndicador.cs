using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    public class PatrimonioLiquidoIndicador : ICalculadorIndicadorFinanceiro
    {
        public CodigoIndicadorFinanceiro Codigo => CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual;

        public IndicadorFinanceiro Calcular(ContextoAnaliseFinanceira contexto, DadosReferenciaAnaliseFinanceira dadosReferencia)
        {
            var patrimonioAlvo = dadosReferencia.PatrimonioAlvo;
            var status = dadosReferencia.PontoPartidaPatrimonialNeutro
                ? StatusIndicadorFinanceiro.Atencao
                : dadosReferencia.PatrimonioLiquidoAtual < 0m
                    ? StatusIndicadorFinanceiro.Critico
                    : ResolutorStatusIndicadorFinanceiro.ResolverFaixaCrescente(
                        dadosReferencia.PercentualPatrimonioLiquidoSobreAtivos,
                        70m,
                        40m,
                        10m);

            return new IndicadorFinanceiro
            {
                Codigo = Codigo,
                Nome = "Patrimônio líquido atual",
                ValorAtual = dadosReferencia.PatrimonioLiquidoAtual,
                ValorIdeal = patrimonioAlvo,
                Percentual = dadosReferencia.PercentualPatrimonioLiquidoSobreAtivos,
                Status = status,
                Descricao = "Representa a situação patrimonial real do usuário no momento, considerando ativos, passivos e o patrimônio líquido efetivamente disponível.",
                Observacao = dadosReferencia.PontoPartidaPatrimonialNeutro
                    ? "Ativos e passivos ainda estão zerados. O sistema trata esse cenário como ponto de partida patrimonial neutro, e não como insolvência."
                    : $"Ativos: {dadosReferencia.TotalAtivos:N2}. Passivos: {dadosReferencia.TotalPassivos:N2}. O patrimônio líquido atual representa {dadosReferencia.PercentualPatrimonioLiquidoSobreAtivos:N2}% da base patrimonial.",
                Formato = FormatoValorIndicadorFinanceiro.Moeda
            };
        }
    }
}
