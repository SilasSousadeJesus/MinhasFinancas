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
                CapacidadeFormacaoReserva = BuscarIndicador(indicadores, CodigoIndicadorFinanceiro.CapacidadeFormacaoReserva),
                ComprometimentoRenda = BuscarIndicador(indicadores, CodigoIndicadorFinanceiro.ComprometimentoRenda),
                ComprometimentoFinanceiroFuturo = BuscarIndicador(indicadores, CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo),
                ComprometimentoFinanceiroFuturo90Dias = BuscarIndicador(indicadores, CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias),
                ComprometimentoFinanceiroFuturo180Dias = BuscarIndicador(indicadores, CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias),
                ComprometimentoFinanceiroFuturo365Dias = BuscarIndicador(indicadores, CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias),
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
            var totalPassivosConsumo = contexto.Passivos
                .Where(EhPassivoDeConsumo)
                .Sum(ObterValorAtualPassivo);
            var totalPassivosPatrimoniais = contexto.Passivos
                .Where(EhPassivoPatrimonial)
                .Sum(ObterValorAtualPassivo);
            var totalPassivosObrigacoesEstruturais = contexto.Passivos
                .Where(EhObrigacaoEstrutural)
                .Sum(ObterValorAtualPassivo);
            var patrimonioLiquidoAtual = totalAtivos - totalPassivos;
            var percentualPatrimonioLiquidoSobreAtivos = totalAtivos > 0m
                ? (patrimonioLiquidoAtual / totalAtivos) * 100m
                : (patrimonioLiquidoAtual > 0m ? 100m : 0m);

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
            var reservaIdealRestante = Math.Max(reservaEmergenciaIdealConfigurada - reservaEmergenciaAtual, 0m);
            var coberturaReservaEmMeses = despesaMensalAtual > 0
                ? reservaEmergenciaAtual / despesaMensalAtual
                : 0m;
            var possuiCapacidadeFormacaoReserva = economiaMensalAtual > 0m;
            var mesesParaFormarReservaIdeal = reservaIdealRestante <= 0m
                ? 0m
                : possuiCapacidadeFormacaoReserva
                    ? reservaIdealRestante / economiaMensalAtual
                    : 999m;

            var comprometimentoRendaAtual = receitaMensalAtual > 0
                ? (despesaMensalAtual / receitaMensalAtual) * 100m
                : (despesaMensalAtual > 0 ? 100m : 0m);

            var obrigacoesFinanceirasFuturas30Dias = CalcularObrigacoesFinanceirasFuturas(lancamentosValidos, dataReferencia, 30);
            var obrigacoesFinanceirasFuturas90Dias = CalcularObrigacoesFinanceirasFuturas(lancamentosValidos, dataReferencia, 90);
            var obrigacoesFinanceirasFuturas180Dias = CalcularObrigacoesFinanceirasFuturas(lancamentosValidos, dataReferencia, 180);
            var obrigacoesFinanceirasFuturas365Dias = CalcularObrigacoesFinanceirasFuturas(lancamentosValidos, dataReferencia, 365);

            var receitaPrevista30Dias = CalcularReceitaPrevistaFutura(lancamentosValidos, dataReferencia, 30);
            var receitaPrevista90Dias = CalcularReceitaPrevistaFutura(lancamentosValidos, dataReferencia, 90);
            var receitaPrevista180Dias = CalcularReceitaPrevistaFutura(lancamentosValidos, dataReferencia, 180);
            var receitaPrevista365Dias = CalcularReceitaPrevistaFutura(lancamentosValidos, dataReferencia, 365);

            var comprometimentoFinanceiroFuturoAtual = CalcularPercentualComprometimentoFinanceiroFuturo(obrigacoesFinanceirasFuturas30Dias, receitaPrevista30Dias);
            var comprometimentoFinanceiroFuturo90DiasAtual = CalcularPercentualPressaoFinanceiraAcumulada(obrigacoesFinanceirasFuturas90Dias, receitaPrevista90Dias);
            var comprometimentoFinanceiroFuturo180DiasAtual = CalcularPercentualPressaoFinanceiraAcumulada(obrigacoesFinanceirasFuturas180Dias, receitaPrevista180Dias);
            var comprometimentoFinanceiroFuturo365DiasAtual = CalcularPercentualPressaoFinanceiraAcumulada(obrigacoesFinanceirasFuturas365Dias, receitaPrevista365Dias);

            var exposicaoConsumoPonderada = totalPassivosConsumo
                + (totalPassivosObrigacoesEstruturais * 0.60m)
                + (totalPassivosPatrimoniais * 0.35m);
            var endividamentoAtual = totalAtivos > 0m
                ? (exposicaoConsumoPonderada / totalAtivos) * 100m
                : (exposicaoConsumoPonderada > 0m ? 100m : 0m);
            var endividamentoConsumoAtual = totalAtivos > 0m
                ? (totalPassivosConsumo / totalAtivos) * 100m
                : (totalPassivosConsumo > 0m ? 100m : 0m);
            var endividamentoPatrimonialAtual = totalAtivos > 0m
                ? (totalPassivosPatrimoniais / totalAtivos) * 100m
                : (totalPassivosPatrimoniais > 0m ? 100m : 0m);

            var patrimonioAlvo = configuracao?.PatrimonioLiquidoAlvo ?? 0m;
            var percentualPatrimonioAlvoAtual = patrimonioAlvo > 0
                ? (patrimonioLiquidoAtual / patrimonioAlvo) * 100m
                : 0m;
            var pontoPartidaPatrimonialNeutro = totalAtivos == 0m
                && totalPassivos == 0m
                && patrimonioLiquidoAtual == 0m;

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
                ReservaIdealRestante = reservaIdealRestante,
                CoberturaReservaEmMeses = coberturaReservaEmMeses,
                MesesParaFormarReservaIdeal = mesesParaFormarReservaIdeal,
                PossuiCapacidadeFormacaoReserva = possuiCapacidadeFormacaoReserva,
                ComprometimentoRendaAtual = comprometimentoRendaAtual,
                ObrigacoesFinanceirasFuturas30Dias = obrigacoesFinanceirasFuturas30Dias,
                ObrigacoesFinanceirasFuturas90Dias = obrigacoesFinanceirasFuturas90Dias,
                ObrigacoesFinanceirasFuturas180Dias = obrigacoesFinanceirasFuturas180Dias,
                ObrigacoesFinanceirasFuturas365Dias = obrigacoesFinanceirasFuturas365Dias,
                ComprometimentoFinanceiroFuturoAtual = comprometimentoFinanceiroFuturoAtual,
                ComprometimentoFinanceiroFuturo90DiasAtual = comprometimentoFinanceiroFuturo90DiasAtual,
                ComprometimentoFinanceiroFuturo180DiasAtual = comprometimentoFinanceiroFuturo180DiasAtual,
                ComprometimentoFinanceiroFuturo365DiasAtual = comprometimentoFinanceiroFuturo365DiasAtual,
                ReceitaPrevista30Dias = receitaPrevista30Dias,
                ReceitaPrevista90Dias = receitaPrevista90Dias,
                ReceitaPrevista180Dias = receitaPrevista180Dias,
                ReceitaPrevista365Dias = receitaPrevista365Dias,
                TotalPassivosConsumo = totalPassivosConsumo,
                TotalPassivosPatrimoniais = totalPassivosPatrimoniais,
                TotalPassivosObrigacoesEstruturais = totalPassivosObrigacoesEstruturais,
                EndividamentoAtual = endividamentoAtual,
                EndividamentoConsumoAtual = endividamentoConsumoAtual,
                EndividamentoPatrimonialAtual = endividamentoPatrimonialAtual,
                PercentualPatrimonioLiquidoSobreAtivos = percentualPatrimonioLiquidoSobreAtivos,
                PatrimonioAlvo = patrimonioAlvo,
                PercentualPatrimonioAlvoAtual = percentualPatrimonioAlvoAtual,
                PontoPartidaPatrimonialNeutro = pontoPartidaPatrimonialNeutro
            };
        }

        private static decimal CalcularObrigacoesFinanceirasFuturas(
            IEnumerable<Lancamento> lancamentos,
            DateTime dataReferencia,
            int dias)
        {
            return lancamentos
                .Where(lancamento =>
                    lancamento.Tipo == EnumTipoLancamento.Despesa &&
                    lancamento.StatusLancamento == EnumStatusLancamento.Pendente &&
                    lancamento.DataVencimento.Date >= dataReferencia &&
                    lancamento.DataVencimento.Date <= dataReferencia.AddDays(dias))
                .Sum(lancamento => lancamento.Valor);
        }

        private static decimal CalcularPercentualComprometimentoFinanceiroFuturo(decimal obrigacoesFuturas, decimal receitaMensalAtual)
        {
            return receitaMensalAtual > 0
                ? (obrigacoesFuturas / receitaMensalAtual) * 100m
                : (obrigacoesFuturas > 0 ? 100m : 0m);
        }

        private static decimal CalcularPercentualPressaoFinanceiraAcumulada(decimal obrigacoesFuturas, decimal receitaPrevistaPeriodo)
        {
            return receitaPrevistaPeriodo > 0
                ? (obrigacoesFuturas / receitaPrevistaPeriodo) * 100m
                : (obrigacoesFuturas > 0 ? 100m : 0m);
        }

        private static decimal CalcularReceitaPrevistaFutura(
            IEnumerable<Lancamento> lancamentos,
            DateTime dataReferencia,
            int dias)
        {
            var receitaExplicita = lancamentos
                .Where(lancamento =>
                    lancamento.Tipo == EnumTipoLancamento.Receita &&
                    lancamento.StatusLancamento != EnumStatusLancamento.Cancelado &&
                    lancamento.DataVencimento.Date >= dataReferencia &&
                    lancamento.DataVencimento.Date <= dataReferencia.AddDays(dias))
                .Sum(lancamento => lancamento.Valor);

            var receitaRecorrenteProjetada = CalcularReceitaRecorrenteProjetada(
                lancamentos,
                dataReferencia,
                dias);

            return receitaExplicita + receitaRecorrenteProjetada;
        }

        private static decimal CalcularReceitaRecorrenteProjetada(
            IEnumerable<Lancamento> lancamentos,
            DateTime dataReferencia,
            int dias)
        {
            var inicioMesSeguinte = new DateTime(dataReferencia.Year, dataReferencia.Month, 1).AddMonths(1);
            var fimPeriodo = dataReferencia.AddDays(dias);

            if (fimPeriodo < inicioMesSeguinte)
            {
                return 0m;
            }

            var baseRecorrenteMensal = lancamentos
                .Where(lancamento =>
                    lancamento.Tipo == EnumTipoLancamento.Receita &&
                    lancamento.StatusLancamento != EnumStatusLancamento.Cancelado &&
                    lancamento.FrequenciaLancamento is EnumTipoFrequenciaLancamento.Fixo or EnumTipoFrequenciaLancamento.DiaUtil &&
                    lancamento.DataVencimento.Date <= dataReferencia)
                .GroupBy(lancamento => new
                {
                    lancamento.Descricao,
                    lancamento.Valor,
                    lancamento.FrequenciaLancamento,
                    Dia = lancamento.FrequenciaLancamento == EnumTipoFrequenciaLancamento.DiaUtil
                        ? lancamento.NumeroDiaUtil ?? lancamento.DataVencimento.Day
                        : lancamento.DataVencimento.Day
                })
                .Select(grupo => grupo
                    .OrderByDescending(lancamento => lancamento.DataVencimento)
                    .First())
                .ToList();

            if (baseRecorrenteMensal.Count == 0)
            {
                return 0m;
            }

            var mesesProjetados = EnumerarMesesEntre(inicioMesSeguinte, fimPeriodo)
                .ToList();

            var totalProjetado = 0m;

            foreach (var mes in mesesProjetados)
            {
                foreach (var receitaBase in baseRecorrenteMensal)
                {
                    var jaExisteReceitaExplicitaNoMes = lancamentos.Any(lancamento =>
                        lancamento.Tipo == EnumTipoLancamento.Receita &&
                        lancamento.StatusLancamento != EnumStatusLancamento.Cancelado &&
                        lancamento.FrequenciaLancamento == receitaBase.FrequenciaLancamento &&
                        lancamento.Descricao == receitaBase.Descricao &&
                        lancamento.Valor == receitaBase.Valor &&
                        lancamento.DataVencimento.Year == mes.Year &&
                        lancamento.DataVencimento.Month == mes.Month);

                    if (!jaExisteReceitaExplicitaNoMes)
                    {
                        totalProjetado += receitaBase.Valor;
                    }
                }
            }

            return totalProjetado;
        }

        private static IEnumerable<DateTime> EnumerarMesesEntre(DateTime inicio, DateTime fim)
        {
            var competencia = new DateTime(inicio.Year, inicio.Month, 1);
            var competenciaFim = new DateTime(fim.Year, fim.Month, 1);

            while (competencia <= competenciaFim)
            {
                yield return competencia;
                competencia = competencia.AddMonths(1);
            }
        }

        private static bool EhPassivoPatrimonial(Passivo passivo)
        {
            return passivo.Tipo == EnumPassivo.Financiamento;
        }

        private static bool EhPassivoDeConsumo(Passivo passivo)
        {
            return passivo.Tipo is EnumPassivo.Emprestimo or EnumPassivo.Divida or EnumPassivo.Parcelamento;
        }

        private static bool EhObrigacaoEstrutural(Passivo passivo)
        {
            return passivo.Tipo is EnumPassivo.ObrigacaoFinanceira or EnumPassivo.Outro;
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
