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
            var status = dadosReferencia.PontoPartidaPatrimonialNeutro
                ? StatusIndicadorFinanceiro.Atencao
                : patrimonioAlvo > 0m
                    ? ResolutorStatusIndicadorFinanceiro.ResolverProgresso(dadosReferencia.PercentualPatrimonioAlvoAtual)
                    : dadosReferencia.PatrimonioLiquidoAtual > 0m
                        ? StatusIndicadorFinanceiro.Bom
                        : dadosReferencia.PatrimonioLiquidoAtual == 0m
                            ? StatusIndicadorFinanceiro.Atencao
                            : StatusIndicadorFinanceiro.Critico;

            return new IndicadorFinanceiro
            {
                Codigo = Codigo,
                Nome = "Evolução em relação ao patrimônio alvo",
                ValorAtual = dadosReferencia.PercentualPatrimonioAlvoAtual,
                ValorIdeal = 100m,
                Percentual = dadosReferencia.PercentualPatrimonioAlvoAtual,
                Status = status,
                Descricao = "Mostra o avanço do patrimônio líquido em relação à meta de longo prazo, sem substituir a leitura da situação patrimonial real.",
                Observacao = dadosReferencia.PontoPartidaPatrimonialNeutro
                    ? "O acompanhamento da meta patrimonial começa a partir do ponto de partida neutro."
                    : patrimonioAlvo > 0m
                        ? $"Patrimônio alvo configurado: {patrimonioAlvo:N2}."
                        : "Sem patrimônio líquido alvo configurado. Neste caso, a meta patrimonial é tratada apenas como evolução futura, sem derrubar a leitura do patrimônio atual.",
                Formato = FormatoValorIndicadorFinanceiro.Percentual
            };
        }
    }
}
