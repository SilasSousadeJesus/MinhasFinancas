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

            return new IndicadorFinanceiro
            {
                Codigo = Codigo,
                Nome = "Patrimônio líquido atual",
                ValorAtual = dadosReferencia.PatrimonioLiquidoAtual,
                ValorIdeal = patrimonioAlvo,
                Percentual = dadosReferencia.PercentualPatrimonioAlvoAtual,
                Status = dadosReferencia.PontoPartidaPatrimonialNeutro
                    ? StatusIndicadorFinanceiro.Atencao
                    : patrimonioAlvo > 0
                        ? ResolutorStatusIndicadorFinanceiro.ResolverMetaMinima(dadosReferencia.PatrimonioLiquidoAtual, patrimonioAlvo)
                        : (dadosReferencia.PatrimonioLiquidoAtual >= 0 ? StatusIndicadorFinanceiro.Bom : StatusIndicadorFinanceiro.Critico),
                Descricao = "Diferença entre o total de ativos e o total de passivos atualmente registrados.",
                Observacao = dadosReferencia.PontoPartidaPatrimonialNeutro
                    ? "Ativos e passivos ainda estão zerados. O sistema trata esse cenário como ponto de partida patrimonial neutro, e não como insolvência."
                    : patrimonioAlvo > 0
                        ? "Comparado ao patrimônio líquido alvo definido no perfil financeiro."
                        : "Sem patrimônio líquido alvo configurado no perfil financeiro.",
                Formato = FormatoValorIndicadorFinanceiro.Moeda
            };
        }
    }
}
