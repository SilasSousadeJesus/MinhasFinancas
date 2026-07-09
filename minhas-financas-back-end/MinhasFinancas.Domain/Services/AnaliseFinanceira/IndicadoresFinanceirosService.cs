using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira
{
    public class IndicadoresFinanceirosService : IIndicadoresFinanceirosService
    {
        private readonly IReadOnlyCollection<ICalculadorIndicadorFinanceiro> _calculadores;

        public IndicadoresFinanceirosService(IEnumerable<ICalculadorIndicadorFinanceiro> calculadores)
        {
            _calculadores = calculadores.ToList();
        }

        public PainelIndicadoresFinanceiros Calcular(ContextoAnaliseFinanceira contexto)
        {
            var dadosReferencia = ConstruirDadosReferencia(contexto);
            var indicadores = _calculadores
                .Select(calculador => calculador.Calcular(contexto, dadosReferencia))
                .OrderBy(indicador => indicador.Codigo)
                .ToList();

            return new PainelIndicadoresFinanceiros
            {
                EconomiaMensal = BuscarIndicador(indicadores, CodigoIndicadorFinanceiro.EconomiaMensal),
                PercentualEconomia = BuscarIndicador(indicadores, CodigoIndicadorFinanceiro.PercentualEconomia),
                ReservaEmergenciaAtual = BuscarIndicador(indicadores, CodigoIndicadorFinanceiro.ReservaEmergenciaAtual),
                ReservaEmergenciaIdeal = BuscarIndicador(indicadores, CodigoIndicadorFinanceiro.ReservaEmergenciaIdeal),
                ComprometimentoRenda = BuscarIndicador(indicadores, CodigoIndicadorFinanceiro.ComprometimentoRenda),
                ComprometimentoFinanceiroFuturo = BuscarIndicador(indicadores, CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo),
                Endividamento = BuscarIndicador(indicadores, CodigoIndicadorFinanceiro.Endividamento),
                PatrimonioLiquidoAtual = BuscarIndicador(indicadores, CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual),
                PercentualPatrimonioAlvo = BuscarIndicador(indicadores, CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo),
                Todos = indicadores
            };
        }

        private static IndicadorFinanceiro BuscarIndicador(
            IEnumerable<IndicadorFinanceiro> indicadores,
            CodigoIndicadorFinanceiro codigo)
        {
            return indicadores.First(indicador => indicador.Codigo == codigo);
        }

        private static DadosReferenciaAnaliseFinanceira ConstruirDadosReferencia(ContextoAnaliseFinanceira contexto)
        {
            var dataReferencia = contexto.DataReferencia == default ? DateTime.Today : contexto.DataReferencia.Date;
            var lancamentosValidos = contexto.Lancamentos
                .Where(lancamento => lancamento.StatusLancamento != EnumStatusLancamento.Cancelado)
                .ToList();

            var lancamentosMesAtual = lancamentosValidos
                .Where(lancamento =>
                    lancamento.DataVencimento.Year == dataReferencia.Year &&
                    lancamento.DataVencimento.Month == dataReferencia.Month)
                .ToList();

            var receitaMensalAtual = lancamentosMesAtual
                .Where(lancamento => lancamento.Tipo == EnumTipoLancamento.Receita)
                .Sum(lancamento => lancamento.Valor);

            var despesaMensalAtual = lancamentosMesAtual
                .Where(lancamento => lancamento.Tipo == EnumTipoLancamento.Despesa)
                .Sum(lancamento => lancamento.Valor);

            var economiaMensalAtual = receitaMensalAtual - despesaMensalAtual;
            var percentualEconomiaAtual = receitaMensalAtual > 0
                ? (economiaMensalAtual / receitaMensalAtual) * 100m
                : 0m;

            var totalAtivos = contexto.Ativos.Sum(ObterValorAtualAtivo);
            var totalPassivos = contexto.Passivos.Sum(ObterValorAtualPassivo);
            var patrimonioLiquidoAtual = totalAtivos - totalPassivos;

            var reservaEmergenciaAtual = contexto.Ativos
                .Where(ativo =>
                    ativo.Tipo == EnumBemPatrimonial.DinheiroEmConta ||
                    ativo.Tipo == EnumBemPatrimonial.Investimento)
                .Sum(ObterValorAtualAtivo);

            var configuracao = contexto.ConfiguracaoPerfilFinanceiro;
            var mesesReservaDesejados = configuracao?.MesesReservaEmergenciaDesejados ?? 0;
            var percentualReservaDesejado = configuracao?.PercentualReservaEmergenciaDesejado ?? 0m;

            var baseReservaEmergenciaIntegral = despesaMensalAtual * mesesReservaDesejados;
            var reservaEmergenciaIdealConfigurada = baseReservaEmergenciaIntegral * (percentualReservaDesejado / 100m);
            var coberturaReservaEmMeses = despesaMensalAtual > 0
                ? reservaEmergenciaAtual / despesaMensalAtual
                : 0m;

            var comprometimentoRendaAtual = receitaMensalAtual > 0
                ? (despesaMensalAtual / receitaMensalAtual) * 100m
                : (despesaMensalAtual > 0 ? 100m : 0m);

            var obrigacoesFinanceirasFuturas30Dias = lancamentosValidos
                .Where(lancamento =>
                    lancamento.Tipo == EnumTipoLancamento.Despesa &&
                    lancamento.StatusLancamento == EnumStatusLancamento.Pendente &&
                    lancamento.DataVencimento.Date >= dataReferencia &&
                    lancamento.DataVencimento.Date <= dataReferencia.AddDays(30))
                .Sum(lancamento => lancamento.Valor);

            var comprometimentoFinanceiroFuturoAtual = receitaMensalAtual > 0
                ? (obrigacoesFinanceirasFuturas30Dias / receitaMensalAtual) * 100m
                : (obrigacoesFinanceirasFuturas30Dias > 0 ? 100m : 0m);

            var endividamentoAtual = totalAtivos > 0
                ? (totalPassivos / totalAtivos) * 100m
                : (totalPassivos > 0 ? 100m : 0m);

            var patrimonioAlvo = configuracao?.PatrimonioLiquidoAlvo ?? 0m;
            var percentualPatrimonioAlvoAtual = patrimonioAlvo > 0
                ? (patrimonioLiquidoAtual / patrimonioAlvo) * 100m
                : 0m;

            return new DadosReferenciaAnaliseFinanceira
            {
                ReceitaMensalAtual = receitaMensalAtual,
                DespesaMensalAtual = despesaMensalAtual,
                EconomiaMensalAtual = economiaMensalAtual,
                PercentualEconomiaAtual = percentualEconomiaAtual,
                TotalAtivos = totalAtivos,
                TotalPassivos = totalPassivos,
                PatrimonioLiquidoAtual = patrimonioLiquidoAtual,
                ReservaEmergenciaAtual = reservaEmergenciaAtual,
                BaseReservaEmergenciaIntegral = baseReservaEmergenciaIntegral,
                ReservaEmergenciaIdealConfigurada = reservaEmergenciaIdealConfigurada,
                CoberturaReservaEmMeses = coberturaReservaEmMeses,
                ComprometimentoRendaAtual = comprometimentoRendaAtual,
                ObrigacoesFinanceirasFuturas30Dias = obrigacoesFinanceirasFuturas30Dias,
                ComprometimentoFinanceiroFuturoAtual = comprometimentoFinanceiroFuturoAtual,
                EndividamentoAtual = endividamentoAtual,
                PatrimonioAlvo = patrimonioAlvo,
                PercentualPatrimonioAlvoAtual = percentualPatrimonioAlvoAtual
            };
        }

        private static decimal ObterValorAtualAtivo(BemPatrimonial ativo)
        {
            return ativo.DataPermanencia?
                .OrderByDescending(item => item.DataPermanencia)
                .FirstOrDefault()?.Valor ?? 0m;
        }

        private static decimal ObterValorAtualPassivo(Passivo passivo)
        {
            return passivo.DataPermanencia?
                .OrderByDescending(item => item.DataPermanencia)
                .FirstOrDefault()?.Valor ?? 0m;
        }
    }
}
