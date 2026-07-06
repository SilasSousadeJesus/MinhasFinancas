using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    public class PercentualPatrimonioAlvoIndicador : ICalculadorIndicadorFinanceiro
    {
        public CodigoIndicadorFinanceiro Codigo => CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo;

        public IndicadorFinanceiro Calcular(ContextoAnaliseFinanceira contexto, DadosReferenciaAnaliseFinanceira dadosReferencia)
        {
            return new IndicadorFinanceiro
            {
                Codigo = Codigo,
                Nome = "Percentual do patrimônio alvo",
                ValorAtual = dadosReferencia.PercentualPatrimonioAlvoAtual,
                ValorIdeal = 100m,
                Percentual = dadosReferencia.PercentualPatrimonioAlvoAtual,
                Status = ResolutorStatusIndicadorFinanceiro.ResolverProgresso(dadosReferencia.PercentualPatrimonioAlvoAtual),
                Descricao = "Percentual do patrimônio alvo já alcançado com base no patrimônio líquido atual.",
                Observacao = dadosReferencia.PatrimonioAlvo > 0
                    ? $"Patrimônio alvo configurado: {dadosReferencia.PatrimonioAlvo:N2}."
                    : "Sem patrimônio líquido alvo configurado no perfil financeiro.",
                Formato = FormatoValorIndicadorFinanceiro.Percentual
            };
        }
    }
}
