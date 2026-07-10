using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    public class CapacidadeFormacaoReservaIndicador : ICalculadorIndicadorFinanceiro
    {
        private const decimal MesesExcelente = 3m;
        private const decimal MesesBom = 6m;
        private const decimal MesesAtencao = 12m;
        private const decimal ValorSemCapacidade = 999m;

        public CodigoIndicadorFinanceiro Codigo => CodigoIndicadorFinanceiro.CapacidadeFormacaoReserva;

        public IndicadorFinanceiro Calcular(ContextoAnaliseFinanceira contexto, DadosReferenciaAnaliseFinanceira dadosReferencia)
        {
            var mesesParaFormarReserva = dadosReferencia.PossuiCapacidadeFormacaoReserva
                ? dadosReferencia.MesesParaFormarReservaIdeal
                : ValorSemCapacidade;
            var percentual = dadosReferencia.PossuiCapacidadeFormacaoReserva && mesesParaFormarReserva > 0
                ? Math.Min((MesesExcelente / mesesParaFormarReserva) * 100m, 100m)
                : (dadosReferencia.ReservaIdealRestante <= 0 ? 100m : 0m);
            var status = dadosReferencia.ReservaIdealRestante <= 0
                ? StatusIndicadorFinanceiro.Excelente
                : dadosReferencia.EconomiaMensalAtual <= 0
                    ? StatusIndicadorFinanceiro.Critico
                    : ResolutorStatusIndicadorFinanceiro.ResolverFaixaDecrescente(
                        mesesParaFormarReserva,
                        MesesExcelente,
                        MesesBom,
                        MesesAtencao);

            return new IndicadorFinanceiro
            {
                Codigo = Codigo,
                Nome = "Capacidade de formação de reserva",
                ValorAtual = mesesParaFormarReserva,
                ValorIdeal = MesesExcelente,
                Percentual = percentual,
                Status = status,
                Descricao = "Estima em quantos meses a economia mensal atual conseguiria completar a reserva de emergência ideal restante.",
                Observacao = dadosReferencia.ReservaIdealRestante <= 0
                    ? "A reserva ideal já está completa. Não existe saldo restante para formar."
                    : dadosReferencia.EconomiaMensalAtual <= 0
                        ? $"Sem sobra mensal positiva, a reserva ideal restante de {dadosReferencia.ReservaIdealRestante:N2} não consegue ser formada no ritmo atual."
                        : $"Faltam {dadosReferencia.ReservaIdealRestante:N2} para atingir a reserva ideal. No ritmo atual, isso levaria cerca de {mesesParaFormarReserva:N2} mês(es).",
                Formato = FormatoValorIndicadorFinanceiro.Meses
            };
        }
    }
}
