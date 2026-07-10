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
            var avaliacaoHistoricoAtrasos = AvaliarHistoricoAtrasos(lancamentos, dataReferencia);
            var totalParametrosPlanejamentoEsperados = 5;
            var quantidadeParametrosPlanejamentoConfigurados = CalcularQuantidadeParametrosPlanejamentoConfigurados(contexto.ConfiguracaoPerfilFinanceiro);
            var avaliacaoPlanoEstrategico = AvaliarPlanoEstrategico(contexto.PlanoEstrategicoFinanceiroVigente);
            var avaliacaoCompromissos = AvaliarCompromissosFinanceiros(contexto.CompromissosFinanceiros);

            return new ContextoComplementarMfScoreFinanceiro
            {
                PossuiFluxoMensalNegativoAtual = fluxoMensalAtual < 0m,
                MesesConsecutivosFluxoNegativo = mesesConsecutivos,
                PossuiInadimplencia = avaliacaoInadimplencia.PossuiInadimplencia,
                NivelInadimplencia = avaliacaoInadimplencia.Nivel,
                DiasMaximosAtraso = avaliacaoInadimplencia.DiasMaximosAtraso,
                ValorTotalEmAtraso = avaliacaoInadimplencia.ValorTotalEmAtraso,
                PercentualValorEmAtrasoSobreRenda = avaliacaoInadimplencia.PercentualValorEmAtrasoSobreRenda,
                QuantidadeParametrosPlanejamentoConfigurados = quantidadeParametrosPlanejamentoConfigurados,
                TotalParametrosPlanejamentoEsperados = totalParametrosPlanejamentoEsperados,
                PerfilFinanceiroBasicoCompleto = quantidadeParametrosPlanejamentoConfigurados >= totalParametrosPlanejamentoEsperados,
                NotaConfiguracaoPlanejamento = CalcularNotaConfiguracaoPlanejamento(quantidadeParametrosPlanejamentoConfigurados),
                PossuiPlanoEstrategicoVigente = avaliacaoPlanoEstrategico.PossuiPlano,
                QuantidadeObjetivosPlanoAtivos = avaliacaoPlanoEstrategico.QuantidadeObjetivosAtivos,
                QuantidadeObjetivosPlanoAltaPrioridade = avaliacaoPlanoEstrategico.QuantidadeObjetivosAltaPrioridade,
                QuantidadeObjetivosPlanoConcluidos = avaliacaoPlanoEstrategico.QuantidadeObjetivosConcluidos,
                NotaPlanoEstrategico = avaliacaoPlanoEstrategico.Nota,
                PossuiCompromissosFinanceiros = avaliacaoCompromissos.PossuiCompromissos,
                QuantidadeCompromissosEmAndamento = avaliacaoCompromissos.QuantidadeEmAndamento,
                QuantidadeCompromissosConcluidos = avaliacaoCompromissos.QuantidadeConcluidos,
                QuantidadeCompromissosCancelados = avaliacaoCompromissos.QuantidadeCancelados,
                NotaCompromissosFinanceiros = avaliacaoCompromissos.Nota,
                PossuiCuraRecenteInadimplencia = avaliacaoHistoricoAtrasos.PossuiCuraRecente,
                QuantidadeOcorrenciasAtrasoRecente = avaliacaoHistoricoAtrasos.QuantidadeOcorrencias,
                QuantidadeMesesComOcorrenciaAtrasoRecente = avaliacaoHistoricoAtrasos.QuantidadeMesesDistintos,
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

        private static int CalcularQuantidadeParametrosPlanejamentoConfigurados(ConfiguracaoPerfilFinanceiro? configuracao)
        {
            if (configuracao is null)
            {
                return 0;
            }

            var quantidade = 0;

            if (configuracao.PercentualEconomiaMensalDesejado > 0m)
            {
                quantidade++;
            }

            if (configuracao.PercentualReservaEmergenciaDesejado > 0m)
            {
                quantidade++;
            }

            if (configuracao.MesesReservaEmergenciaDesejados > 0)
            {
                quantidade++;
            }

            if (configuracao.PercentualMaximoComprometimentoRenda > 0m)
            {
                quantidade++;
            }

            if (configuracao.PercentualMaximoEndividamento > 0m)
            {
                quantidade++;
            }

            return quantidade;
        }

        private static int CalcularNotaConfiguracaoPlanejamento(int quantidadeParametrosPlanejamentoConfigurados)
        {
            return quantidadeParametrosPlanejamentoConfigurados switch
            {
                >= 5 => 100,
                4 => 75,
                3 => 55,
                2 => 40,
                1 => 25,
                _ => 10
            };
        }

        private static AvaliacaoPlanoEstrategicoMfScoreFinanceiro AvaliarPlanoEstrategico(PlanoEstrategicoFinanceiro? plano)
        {
            if (plano is null || !plano.Ativo)
            {
                return new AvaliacaoPlanoEstrategicoMfScoreFinanceiro();
            }

            var objetivosAtivos = plano.Objetivos
                .Where(x => x.Status != EnumStatusObjetivoPlanoEstrategico.Cancelado)
                .ToList();

            var quantidadeObjetivosAtivos = objetivosAtivos.Count;
            var quantidadeAltaPrioridade = objetivosAtivos.Count(x =>
                x.Prioridade is EnumPrioridadeObjetivoPlanoEstrategico.Alta or EnumPrioridadeObjetivoPlanoEstrategico.Critica);
            var quantidadeConcluidos = objetivosAtivos.Count(x => x.Status == EnumStatusObjetivoPlanoEstrategico.Concluido);
            var quantidadeEmAndamento = objetivosAtivos.Count(x => x.Status == EnumStatusObjetivoPlanoEstrategico.EmAndamento);

            var nota = quantidadeObjetivosAtivos == 0
                ? 45m
                : 55m
                    + (quantidadeAltaPrioridade > 0 ? 15m : 0m)
                    + (quantidadeEmAndamento > 0 ? 15m : 0m)
                    + (quantidadeObjetivosAtivos > 0 ? Math.Min(15m, (quantidadeConcluidos / (decimal)quantidadeObjetivosAtivos) * 15m) : 0m);

            return new AvaliacaoPlanoEstrategicoMfScoreFinanceiro
            {
                PossuiPlano = true,
                QuantidadeObjetivosAtivos = quantidadeObjetivosAtivos,
                QuantidadeObjetivosAltaPrioridade = quantidadeAltaPrioridade,
                QuantidadeObjetivosConcluidos = quantidadeConcluidos,
                Nota = Math.Clamp((int)Math.Round(nota), 0, 100)
            };
        }

        private static AvaliacaoCompromissosFinanceirosMfScoreFinanceiro AvaliarCompromissosFinanceiros(
            IReadOnlyCollection<CompromissoFinanceiro> compromissos)
        {
            var compromissosAtivos = compromissos
                .Where(x => x.Ativo)
                .ToList();

            if (compromissosAtivos.Count == 0)
            {
                return new AvaliacaoCompromissosFinanceirosMfScoreFinanceiro();
            }

            var quantidadeConcluidos = compromissosAtivos.Count(x => x.Status == EnumStatusCompromissoFinanceiro.Concluido);
            var quantidadeEmAndamento = compromissosAtivos.Count(x => x.Status == EnumStatusCompromissoFinanceiro.EmAndamento);
            var quantidadeCancelados = compromissosAtivos.Count(x => x.Status == EnumStatusCompromissoFinanceiro.Cancelado);
            var total = compromissosAtivos.Count;

            var percentualConcluidos = total > 0 ? quantidadeConcluidos / (decimal)total : 0m;
            var percentualCancelados = total > 0 ? quantidadeCancelados / (decimal)total : 0m;

            var nota = 50m
                + Math.Min(30m, percentualConcluidos * 30m)
                + (quantidadeEmAndamento > 0 ? 10m : 0m)
                - Math.Min(20m, percentualCancelados * 20m);

            return new AvaliacaoCompromissosFinanceirosMfScoreFinanceiro
            {
                PossuiCompromissos = true,
                QuantidadeConcluidos = quantidadeConcluidos,
                QuantidadeEmAndamento = quantidadeEmAndamento,
                QuantidadeCancelados = quantidadeCancelados,
                Nota = Math.Clamp((int)Math.Round(nota), 0, 100)
            };
        }

        private static AvaliacaoHistoricoAtrasosMfScoreFinanceiro AvaliarHistoricoAtrasos(
            IReadOnlyCollection<Lancamento> lancamentos,
            DateTime dataReferencia)
        {
            var janelaReincidencia = dataReferencia.AddDays(-180);
            var janelaCura = dataReferencia.AddDays(-90);

            var pendentesEmAtraso = lancamentos
                .Where(x =>
                    x.Tipo == EnumTipoLancamento.Despesa &&
                    x.StatusLancamento == EnumStatusLancamento.Pendente &&
                    x.DataVencimento.Date < dataReferencia &&
                    x.DataVencimento.Date >= janelaReincidencia)
                .ToList();

            var pagosEmAtrasoRecentes = lancamentos
                .Where(x =>
                    x.Tipo == EnumTipoLancamento.Despesa &&
                    x.StatusLancamento == EnumStatusLancamento.Pago &&
                    x.DataEfetivacao.HasValue &&
                    x.DataEfetivacao.Value.Date > x.DataVencimento.Date &&
                    x.DataEfetivacao.Value.Date >= janelaCura)
                .ToList();

            var referenciasMensais = pendentesEmAtraso
                .Select(x => new DateTime(x.DataVencimento.Year, x.DataVencimento.Month, 1))
                .Concat(pagosEmAtrasoRecentes.Select(x => new DateTime(x.DataVencimento.Year, x.DataVencimento.Month, 1)))
                .Distinct()
                .Count();

            return new AvaliacaoHistoricoAtrasosMfScoreFinanceiro
            {
                PossuiCuraRecente = !pendentesEmAtraso.Any() && pagosEmAtrasoRecentes.Count > 0,
                QuantidadeOcorrencias = pendentesEmAtraso.Count + pagosEmAtrasoRecentes.Count,
                QuantidadeMesesDistintos = referenciasMensais
            };
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

        private sealed class AvaliacaoPlanoEstrategicoMfScoreFinanceiro
        {
            public bool PossuiPlano { get; set; }
            public int QuantidadeObjetivosAtivos { get; set; }
            public int QuantidadeObjetivosAltaPrioridade { get; set; }
            public int QuantidadeObjetivosConcluidos { get; set; }
            public int? Nota { get; set; }
        }

        private sealed class AvaliacaoCompromissosFinanceirosMfScoreFinanceiro
        {
            public bool PossuiCompromissos { get; set; }
            public int QuantidadeEmAndamento { get; set; }
            public int QuantidadeConcluidos { get; set; }
            public int QuantidadeCancelados { get; set; }
            public int? Nota { get; set; }
        }

        private sealed class AvaliacaoHistoricoAtrasosMfScoreFinanceiro
        {
            public bool PossuiCuraRecente { get; set; }
            public int QuantidadeOcorrencias { get; set; }
            public int QuantidadeMesesDistintos { get; set; }
        }
    }
}
