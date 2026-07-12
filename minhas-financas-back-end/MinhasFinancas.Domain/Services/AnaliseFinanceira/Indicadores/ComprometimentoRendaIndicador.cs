using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    public class ComprometimentoRendaIndicador : ICalculadorIndicadorFinanceiro
    {
        public CodigoIndicadorFinanceiro Codigo => CodigoIndicadorFinanceiro.ComprometimentoRenda;

        public IndicadorFinanceiro Calcular(ContextoAnaliseFinanceira contexto, DadosReferenciaAnaliseFinanceira dadosReferencia)
        {
            var percentualMaximo = contexto.ConfiguracaoPerfilFinanceiro?.PercentualMaximoComprometimentoRenda ?? 0m;

            return new IndicadorFinanceiro
            {
                Codigo = Codigo,
                Nome = "Comprometimento da renda",
                ValorAtual = dadosReferencia.ComprometimentoRendaAtual,
                ValorIdeal = percentualMaximo,
                Percentual = percentualMaximo > 0 ? (dadosReferencia.ComprometimentoRendaAtual / percentualMaximo) * 100m : 0m,
                Status = ResolutorStatusIndicadorFinanceiro.ResolverFaixaDecrescente(
                    dadosReferencia.ComprometimentoRendaAtual,
                    55m,
                    75m,
                    95m),
                Descricao = "Mostra quanto da renda mensal já está comprometido com despesas no mês atual.",
                Observacao = percentualMaximo > 0
                    ? $"Limite configurado no perfil financeiro: {percentualMaximo:N2}% da renda."
                    : "Sem limite máximo de comprometimento configurado no perfil financeiro.",
                Formato = FormatoValorIndicadorFinanceiro.Percentual
            };
        }
    }
}
