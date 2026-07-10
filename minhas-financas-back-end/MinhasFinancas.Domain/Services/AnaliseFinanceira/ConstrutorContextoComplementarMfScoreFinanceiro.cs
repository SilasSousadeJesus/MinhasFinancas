using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira
{
    public static class ConstrutorContextoComplementarMfScoreFinanceiro
    {
        public static ContextoComplementarMfScoreFinanceiro Construir(
            ContextoAnaliseFinanceira contexto,
            IReadOnlyCollection<int>? historicoPontuacoesFinais = null)
        {
            var dataReferencia = contexto.DataReferencia.Date;
            var mesAtual = new DateTime(dataReferencia.Year, dataReferencia.Month, 1);
            var lancamentos = contexto.Lancamentos
                .Where(x => x.StatusLancamento != EnumStatusLancamento.Cancelado)
                .ToList();

            var fluxoMensalAtual = CalcularFluxoMensal(lancamentos, mesAtual);
            var mesesConsecutivos = CalcularMesesConsecutivosFluxoNegativo(lancamentos, mesAtual, 12);
            var receitaMensalAtual = CalcularReceitaMensal(lancamentos, mesAtual);
            var avaliacaoInadimplencia = AvaliarInadimplencia(lancamentos, dataReferencia, receitaMensalAtual);

            return new ContextoComplementarMfScoreFinanceiro
            {
                PossuiFluxoMensalNegativoAtual = fluxoMensalAtual < 0m,
                MesesConsecutivosFluxoNegativo = mesesConsecutivos,
                PossuiInadimplencia = avaliacaoInadimplencia.PossuiInadimplencia,
                NivelInadimplencia = avaliacaoInadimplencia.Nivel,
                DiasMaximosAtraso = avaliacaoInadimplencia.DiasMaximosAtraso,
                ValorTotalEmAtraso = avaliacaoInadimplencia.ValorTotalEmAtraso,
                PercentualValorEmAtrasoSobreRenda = avaliacaoInadimplencia.PercentualValorEmAtrasoSobreRenda,
                PossuiDadosEssenciaisInsuficientes =
                    !lancamentos.Any() ||
                    (!contexto.Ativos.Any() && !contexto.Passivos.Any()),
                HistoricoPontuacoesFinais = historicoPontuacoesFinais?.ToList() ?? []
            };
        }

        public static AvaliacaoInadimplenciaMfScoreFinanceiro AvaliarInadimplencia(
            IReadOnlyCollection<Lancamento> lancamentos,
            DateTime dataReferencia,
            decimal receitaMensalAtual)
        {
            var despesasEmAtraso = lancamentos
                .Where(x =>
                    x.Tipo == EnumTipoLancamento.Despesa &&
                    x.StatusLancamento == EnumStatusLancamento.Pendente &&
                    x.DataVencimento.Date < dataReferencia)
                .ToList();

            if (despesasEmAtraso.Count == 0)
            {
                return new AvaliacaoInadimplenciaMfScoreFinanceiro();
            }

            var valorTotalEmAtraso = despesasEmAtraso.Sum(x => x.Valor);
            var diasMaximosAtraso = despesasEmAtraso
                .Max(x => Math.Max((dataReferencia - x.DataVencimento.Date).Days, 0));

            var percentualValorEmAtrasoSobreRenda = receitaMensalAtual > 0m
                ? (valorTotalEmAtraso / receitaMensalAtual) * 100m
                : (valorTotalEmAtraso > 0m ? 100m : 0m);

            return new AvaliacaoInadimplenciaMfScoreFinanceiro
            {
                PossuiInadimplencia = true,
                Nivel = DeterminarNivelInadimplencia(diasMaximosAtraso, percentualValorEmAtrasoSobreRenda),
                DiasMaximosAtraso = diasMaximosAtraso,
                ValorTotalEmAtraso = valorTotalEmAtraso,
                PercentualValorEmAtrasoSobreRenda = percentualValorEmAtrasoSobreRenda
            };
        }

        private static int DeterminarNivelInadimplencia(int diasMaximosAtraso, decimal percentualValorEmAtrasoSobreRenda)
        {
            if (diasMaximosAtraso > 60 || percentualValorEmAtrasoSobreRenda > 50m)
            {
                return 4;
            }

            if (diasMaximosAtraso >= 31 || percentualValorEmAtrasoSobreRenda >= 25m)
            {
                return 3;
            }

            if (diasMaximosAtraso >= 8 || percentualValorEmAtrasoSobreRenda >= 10m)
            {
                return 2;
            }

            return 1;
        }

        private static decimal CalcularReceitaMensal(IReadOnlyCollection<Lancamento> lancamentos, DateTime competencia)
        {
            return lancamentos
                .Where(x =>
                    x.Tipo == EnumTipoLancamento.Receita &&
                    x.DataVencimento.Year == competencia.Year &&
                    x.DataVencimento.Month == competencia.Month)
                .Sum(x => x.Valor);
        }

        private static decimal CalcularFluxoMensal(IReadOnlyCollection<Lancamento> lancamentos, DateTime competencia)
        {
            var receitas = CalcularReceitaMensal(lancamentos, competencia);

            var despesas = lancamentos
                .Where(x =>
                    x.Tipo == EnumTipoLancamento.Despesa &&
                    x.DataVencimento.Year == competencia.Year &&
                    x.DataVencimento.Month == competencia.Month)
                .Sum(x => x.Valor);

            return receitas - despesas;
        }

        private static int CalcularMesesConsecutivosFluxoNegativo(
            IReadOnlyCollection<Lancamento> lancamentos,
            DateTime competenciaAtual,
            int limiteMeses)
        {
            var consecutivos = 0;

            for (var indice = 0; indice < limiteMeses; indice++)
            {
                var competencia = competenciaAtual.AddMonths(-indice);
                var fluxo = CalcularFluxoMensal(lancamentos, competencia);

                if (fluxo < 0m)
                {
                    consecutivos++;
                    continue;
                }

                break;
            }

            return consecutivos;
        }
    }
}
