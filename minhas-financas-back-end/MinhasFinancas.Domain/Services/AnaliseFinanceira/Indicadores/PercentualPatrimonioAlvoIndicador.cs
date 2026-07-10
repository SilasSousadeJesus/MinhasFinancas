using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    public class PercentualPatrimonioAlvoIndicador : ICalculadorIndicadorFinanceiro
    {
        public CodigoIndicadorFinanceiro Codigo => CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo;

        public IndicadorFinanceiro Calcular(ContextoAnaliseFinanceira contexto, DadosReferenciaAnaliseFinanceira dadosReferencia)
        {
            var patrimonioAlvo = dadosReferencia.PatrimonioAlvo;

            return new IndicadorFinanceiro
            {
                Codigo = Codigo,
                Nome = "Percentual do patrimônio alvo",
                ValorAtual = dadosReferencia.PercentualPatrimonioAlvoAtual,
                ValorIdeal = 100m,
                Percentual = dadosReferencia.PercentualPatrimonioAlvoAtual,
                Status = dadosReferencia.PontoPartidaPatrimonialNeutro
                    ? StatusIndicadorFinanceiro.Atencao
                    : patrimonioAlvo > 0
                        ? ResolutorStatusIndicadorFinanceiro.ResolverProgresso(dadosReferencia.PercentualPatrimonioAlvoAtual)
                        : StatusIndicadorFinanceiro.Atencao,
                Descricao = "Percentual do patrimônio alvo já alcançado com base no patrimônio líquido atual.",
                Observacao = dadosReferencia.PontoPartidaPatrimonialNeutro
                    ? "Ainda não existe patrimônio acumulado nem passivos registrados. O avanço patrimonial começa a ser medido a partir desse ponto neutro."
                    : patrimonioAlvo > 0
                        ? $"Patrimônio alvo configurado: {patrimonioAlvo:N2}."
                        : "Sem patrimônio líquido alvo configurado no perfil financeiro.",
                Formato = FormatoValorIndicadorFinanceiro.Percentual
            };
        }
    }
}
