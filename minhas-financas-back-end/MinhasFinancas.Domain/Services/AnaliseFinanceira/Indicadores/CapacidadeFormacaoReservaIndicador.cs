using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    public class CapacidadeFormacaoReservaIndicador : ICalculadorIndicadorFinanceiro
    {
        private const decimal MesesExcelente = 4m;
        private const decimal MesesBom = 8m;
        private const decimal MesesAtencao = 18m;
        private const decimal ValorNaoProjetavel = 999m;

        public CodigoIndicadorFinanceiro Codigo => CodigoIndicadorFinanceiro.CapacidadeFormacaoReserva;

        public IndicadorFinanceiro Calcular(ContextoAnaliseFinanceira contexto, DadosReferenciaAnaliseFinanceira dadosReferencia)
        {
            var mesesParaFormarReserva = dadosReferencia.PossuiCapacidadeFormacaoReserva
                ? dadosReferencia.MesesParaFormarReservaIdeal
                : ValorNaoProjetavel;
            var percentual = dadosReferencia.PossuiCapacidadeFormacaoReserva && mesesParaFormarReserva > 0m
                ? Math.Min((MesesExcelente / mesesParaFormarReserva) * 100m, 100m)
                : (dadosReferencia.ReservaIdealRestante <= 0m ? 100m : 0m);
            var status = dadosReferencia.ReservaIdealRestante <= 0m
                ? StatusIndicadorFinanceiro.Excelente
                : dadosReferencia.EconomiaMensalAtual <= 0m
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
                Descricao = "Estima em quantos meses a sobra mensal atual conseguiria completar a reserva de emergência ideal restante.",
                Observacao = dadosReferencia.ReservaIdealRestante <= 0m
                    ? "A reserva ideal já está completa. Não existe saldo restante para formar."
                    : dadosReferencia.EconomiaMensalAtual <= 0m
                        ? $"Com o fluxo atual não é possível formar a reserva de emergência. Ainda faltam {dadosReferencia.ReservaIdealRestante:N2} para atingir a proteção desejada."
                        : $"Faltam {dadosReferencia.ReservaIdealRestante:N2} para atingir a reserva ideal. No ritmo atual, isso levaria cerca de {mesesParaFormarReserva:N2} mês(es).",
                Formato = FormatoValorIndicadorFinanceiro.Meses
            };
        }
    }
}
