using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Reports;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Services.AnaliseFinanceira;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.AuditoriaMfScore;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class MfScoreAuditoriaAppService : IMfScoreAuditoriaAppService
    {
        private const string VersaoMfScore = MfScoreCalculoAppService.VersaoModeloAtual;

        private readonly IEnumerable<IPersonaMfScore> _personas;
        private readonly IIndicadoresFinanceirosService _indicadoresFinanceirosService;
        private readonly ISaudeFinanceiraService _saudeFinanceiraService;
        private readonly IExcelReport<MfScoreAuditoriaExcelReportData> _excelReport;
        private readonly IExcelReport<MfScoreAuditoriaHumanaExcelReportData> _excelReportAuditoriaHumana;

        public MfScoreAuditoriaAppService(
            IEnumerable<IPersonaMfScore> personas,
            IIndicadoresFinanceirosService indicadoresFinanceirosService,
            ISaudeFinanceiraService saudeFinanceiraService,
            IExcelReport<MfScoreAuditoriaExcelReportData> excelReport,
            IExcelReport<MfScoreAuditoriaHumanaExcelReportData> excelReportAuditoriaHumana)
        {
            _personas = personas;
            _indicadoresFinanceirosService = indicadoresFinanceirosService;
            _saudeFinanceiraService = saudeFinanceiraService;
            _excelReport = excelReport;
            _excelReportAuditoriaHumana = excelReportAuditoriaHumana;
        }

        public Task<RetornoGenerico> GerarPlanilhaAsync()
        {
            try
            {
                var resultados = CalcularResultados();
                var dataGeracao = DateTime.Now;
                var dadosRelatorio = new MfScoreAuditoriaExcelReportData
                {
                    NomeArquivo = $"mf-score-auditoria-{dataGeracao:yyyyMMdd-HHmmss}.xlsx",
                    DataGeracao = dataGeracao,
                    VersaoMfScore = VersaoMfScore,
                    Cenarios = resultados
                        .Select(MapearParaAuditoriaAutomatica)
                        .ToList()
                };

                var arquivo = _excelReport.Gerar(dadosRelatorio);

                return Task.FromResult(new RetornoGenerico(
                    true,
                    "Planilha de auditoria do MF Score gerada com sucesso.",
                    "Planilha de auditoria do MF Score gerada com sucesso.",
                    HttpStatusCode.OK,
                    arquivo));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new RetornoGenerico(
                    false,
                    ex.ToString(),
                    "Nao foi possivel gerar a auditoria do MF Score.",
                    HttpStatusCode.InternalServerError,
                    null));
            }
        }

        public Task<RetornoGenerico> GerarPlanilhaAuditoriaHumanaAsync()
        {
            try
            {
                var resultados = CalcularResultados();
                var dataGeracao = DateTime.Now;
                var dadosRelatorio = new MfScoreAuditoriaHumanaExcelReportData
                {
                    NomeArquivo = $"mf-score-auditoria-humana-{dataGeracao:yyyyMMdd-HHmmss}.xlsx",
                    DataGeracao = dataGeracao,
                    VersaoMfScore = VersaoMfScore,
                    Personas = resultados
                        .Select(MapearParaAuditoriaHumana)
                        .ToList()
                };

                var arquivo = _excelReportAuditoriaHumana.Gerar(dadosRelatorio);

                return Task.FromResult(new RetornoGenerico(
                    true,
                    "Planilha de auditoria humana do MF Score gerada com sucesso.",
                    "Planilha de auditoria humana do MF Score gerada com sucesso.",
                    HttpStatusCode.OK,
                    arquivo));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new RetornoGenerico(
                    false,
                    ex.ToString(),
                    "Nao foi possivel gerar a auditoria humana do MF Score.",
                    HttpStatusCode.InternalServerError,
                    null));
            }
        }

        private List<ResultadoCenarioAuditoriaMfScore> CalcularResultados()
        {
            return _personas
                .Select(persona => persona.CriarCenario())
                .Select(CalcularResultadoCenario)
                .ToList();
        }

        private ResultadoCenarioAuditoriaMfScore CalcularResultadoCenario(CenarioMfScore cenario)
        {
            var painelIndicadores = _indicadoresFinanceirosService.Calcular(cenario.Contexto);
            var contextoComplementar = ConstrutorContextoComplementarMfScoreFinanceiro.Construir(cenario.Contexto);
            var painelSaude = _saudeFinanceiraService.GerarPainel(painelIndicadores, contextoComplementar);
            var mfScore = painelSaude.Resumo.MfScore;

            return new ResultadoCenarioAuditoriaMfScore
            {
                Cenario = cenario,
                PainelIndicadores = painelIndicadores,
                MfScore = mfScore
            };
        }

        private static MfScoreAuditoriaCenarioExcelReportData MapearParaAuditoriaAutomatica(ResultadoCenarioAuditoriaMfScore resultado)
        {
            var cenario = resultado.Cenario;
            var mfScore = resultado.MfScore;
            var painelIndicadores = resultado.PainelIndicadores;

            return new MfScoreAuditoriaCenarioExcelReportData
            {
                Persona = cenario.Nome,
                Descricao = cenario.Descricao,
                ScoreEsperadoMin = cenario.ScoreEsperadoMin,
                ScoreEsperadoMax = cenario.ScoreEsperadoMax,
                ScoreObtido = mfScore.PontuacaoFinal,
                Status = mfScore.PontuacaoFinal >= cenario.ScoreEsperadoMin && mfScore.PontuacaoFinal <= cenario.ScoreEsperadoMax
                    ? "OK"
                    : "FALHA",
                Classificacao = mfScore.Classificacao,
                Risco = mfScore.Risco,
                Justificativa = cenario.Justificativa,
                Observacoes = cenario.Observacoes,
                FluxoDeCaixa = BuscarNotaPilar(mfScore, CodigoPilarMfScoreFinanceiro.FluxoDeCaixa),
                LiquidezEReserva = BuscarNotaPilar(mfScore, CodigoPilarMfScoreFinanceiro.LiquidezEReserva),
                EndividamentoEObrigacoes = BuscarNotaPilar(mfScore, CodigoPilarMfScoreFinanceiro.EndividamentoEObrigacoes),
                Patrimonio = BuscarNotaPilar(mfScore, CodigoPilarMfScoreFinanceiro.Patrimonio),
                PlanejamentoEDisciplina = BuscarNotaPilar(mfScore, CodigoPilarMfScoreFinanceiro.PlanejamentoEDisciplina),
                IndicadoresCriticos = MapearIndicadoresCriticos(cenario, mfScore, painelIndicadores),
                DadosEntrada = new MfScoreAuditoriaDadosEntradaExcelReportData
                {
                    Renda = cenario.DadosEntrada.Renda,
                    Despesas = cenario.DadosEntrada.Despesas,
                    Reserva = cenario.DadosEntrada.Reserva,
                    Patrimonio = cenario.DadosEntrada.Patrimonio,
                    Passivos = cenario.DadosEntrada.Passivos,
                    ObrigacoesFuturas30Dias = cenario.DadosEntrada.ObrigacoesFuturas30Dias,
                    ObrigacoesFuturas90Dias = cenario.DadosEntrada.ObrigacoesFuturas90Dias,
                    ObrigacoesFuturas180Dias = cenario.DadosEntrada.ObrigacoesFuturas180Dias,
                    ObrigacoesFuturas12Meses = cenario.DadosEntrada.ObrigacoesFuturas12Meses
                }
            };
        }

        private static MfScoreAuditoriaHumanaPersonaExcelReportData MapearParaAuditoriaHumana(ResultadoCenarioAuditoriaMfScore resultado)
        {
            var cenario = resultado.Cenario;
            var mfScore = resultado.MfScore;
            var painelIndicadores = resultado.PainelIndicadores;
            var somaPesos = mfScore.Pilares.Sum(pilar => pilar.Peso);

            return new MfScoreAuditoriaHumanaPersonaExcelReportData
            {
                Persona = cenario.Nome,
                Objetivo = cenario.Justificativa,
                Descricao = cenario.Descricao,
                ScoreCalculado = mfScore.PontuacaoFinal,
                ClassificacaoCalculada = mfScore.Classificacao,
                RiscoCalculado = mfScore.Risco,
                FluxoDeCaixa = BuscarNotaPilar(mfScore, CodigoPilarMfScoreFinanceiro.FluxoDeCaixa),
                LiquidezEReserva = BuscarNotaPilar(mfScore, CodigoPilarMfScoreFinanceiro.LiquidezEReserva),
                EndividamentoEObrigacoes = BuscarNotaPilar(mfScore, CodigoPilarMfScoreFinanceiro.EndividamentoEObrigacoes),
                Patrimonio = BuscarNotaPilar(mfScore, CodigoPilarMfScoreFinanceiro.Patrimonio),
                PlanejamentoEDisciplina = BuscarNotaPilar(mfScore, CodigoPilarMfScoreFinanceiro.PlanejamentoEDisciplina),
                PenalidadeTotal = mfScore.PenalidadeTotal,
                IndicadoresCriticosResumo = string.Join(" | ", mfScore.IndicadoresCriticos.Select(x => x.Nome).Distinct()),
                PenalizacoesAplicadasResumo = string.Join(" | ", mfScore.RegrasCriticasAplicadas),
                ScoreEsperadoMinAtual = cenario.ScoreEsperadoMin,
                ScoreEsperadoMaxAtual = cenario.ScoreEsperadoMax,
                DadosEntrada = MapearDadosEntradaHumana(cenario),
                Indicadores = painelIndicadores.Todos
                    .Select(indicador => new MfScoreAuditoriaHumanaIndicadorExcelReportData
                    {
                        Persona = cenario.Nome,
                        Indicador = indicador.Nome,
                        ValorAtual = indicador.ValorAtual,
                        ValorIdeal = indicador.ValorIdeal,
                        Percentual = indicador.Percentual,
                        Status = indicador.Status.ToString(),
                        Descricao = indicador.Descricao,
                        Observacao = indicador.Observacao,
                        PilarRelacionado = BuscarPilarRelacionado(mfScore, indicador.Nome)
                    })
                    .ToList(),
                Pilares = mfScore.Pilares
                    .Select(pilar => new MfScoreAuditoriaHumanaPilarExcelReportData
                    {
                        Persona = cenario.Nome,
                        Pilar = pilar.Nome,
                        NotaPilar = pilar.Nota,
                        PesoPilar = pilar.Peso,
                        ContribuicaoScoreBase = somaPesos > 0 ? (pilar.Nota * pilar.Peso) / somaPesos : 0m,
                        Observacao = pilar.Descricao
                    })
                    .ToList(),
                Penalizacoes = MapearPenalizacoesHumanas(cenario, mfScore, painelIndicadores)
            };
        }

        private static MfScoreAuditoriaHumanaDadosEntradaExcelReportData MapearDadosEntradaHumana(CenarioMfScore cenario)
        {
            var receitasNoPeriodo = cenario.Contexto.Lancamentos
                .Where(lancamento => lancamento.Tipo == EnumTipoLancamento.Receita)
                .Sum(lancamento => lancamento.Valor);

            var patrimonioLiquido = cenario.DadosEntrada.Patrimonio - cenario.DadosEntrada.Passivos;
            var plano = cenario.Contexto.PlanoEstrategicoFinanceiroVigente;
            var compromissos = cenario.Contexto.CompromissosFinanceiros
                .Where(compromisso => compromisso.Ativo)
                .ToList();

            var descricaoPlano = plano is null
                ? "Não existe plano estratégico neste cenário."
                : $"Plano vigente com {plano.Objetivos.Count(x => x.Status != EnumStatusObjetivoPlanoEstrategico.Cancelado)} objetivo(s) ativo(s).";

            var descricaoCompromissos = compromissos.Count == 0
                ? "Não existem compromissos financeiros neste cenário."
                : $"{compromissos.Count} compromisso(s): {compromissos.Count(x => x.Status == EnumStatusCompromissoFinanceiro.Concluido)} concluído(s), {compromissos.Count(x => x.Status == EnumStatusCompromissoFinanceiro.EmAndamento)} em andamento e {compromissos.Count(x => x.Status == EnumStatusCompromissoFinanceiro.Cancelado)} cancelado(s).";

            return new MfScoreAuditoriaHumanaDadosEntradaExcelReportData
            {
                RendaMensal = cenario.DadosEntrada.Renda,
                ReceitasNoPeriodo = receitasNoPeriodo,
                DespesasMensais = cenario.DadosEntrada.Despesas,
                DespesasFuturas30Dias = cenario.DadosEntrada.ObrigacoesFuturas30Dias,
                DespesasFuturas90Dias = cenario.DadosEntrada.ObrigacoesFuturas90Dias,
                DespesasFuturas180Dias = cenario.DadosEntrada.ObrigacoesFuturas180Dias,
                DespesasFuturas12Meses = cenario.DadosEntrada.ObrigacoesFuturas12Meses,
                Reserva = cenario.DadosEntrada.Reserva,
                PatrimonioBruto = cenario.DadosEntrada.Patrimonio,
                Passivos = cenario.DadosEntrada.Passivos,
                PatrimonioLiquido = patrimonioLiquido,
                PerfilFinanceiroConfigurado = cenario.Contexto.ConfiguracaoPerfilFinanceiro is null ? "Nao" : "Sim",
                PlanoEstrategico = descricaoPlano,
                Compromissos = descricaoCompromissos,
                Observacoes = cenario.Observacoes
            };
        }

        private static List<MfScoreAuditoriaIndicadorCriticoExcelReportData> MapearIndicadoresCriticos(
            CenarioMfScore cenario,
            MfScoreFinanceiro mfScore,
            PainelIndicadoresFinanceiros painelIndicadores)
        {
            var valoresIndicadores = painelIndicadores.Todos.ToDictionary(indicador => indicador.Codigo, indicador => indicador.ValorAtual);

            return mfScore.IndicadoresCriticos
                .Select(indicadorCritico => new MfScoreAuditoriaIndicadorCriticoExcelReportData
                {
                    Persona = cenario.Nome,
                    Indicador = indicadorCritico.Nome,
                    Valor = valoresIndicadores.TryGetValue(indicadorCritico.CodigoIndicador, out var valor) ? valor : 0m,
                    Penalidade = indicadorCritico.Penalidade,
                    Observacao = indicadorCritico.Motivo
                })
                .ToList();
        }

        private static List<MfScoreAuditoriaHumanaPenalizacaoExcelReportData> MapearPenalizacoesHumanas(
            CenarioMfScore cenario,
            MfScoreFinanceiro mfScore,
            PainelIndicadoresFinanceiros painelIndicadores)
        {
            var valoresIndicadores = painelIndicadores.Todos.ToDictionary(indicador => indicador.Codigo, indicador => indicador.ValorAtual);

            if (mfScore.IndicadoresCriticos.Count == 0)
            {
                return
                [
                    new MfScoreAuditoriaHumanaPenalizacaoExcelReportData
                    {
                        Persona = cenario.Nome,
                        RegraCritica = "Nenhuma regra critica aplicada",
                        IndicadorRelacionado = "-",
                        Valor = 0m,
                        Penalizacao = 0m,
                        Justificativa = "Nenhuma penalizacao aplicada neste cenario."
                    }
                ];
            }

            return mfScore.IndicadoresCriticos
                .Select(indicadorCritico => new MfScoreAuditoriaHumanaPenalizacaoExcelReportData
                {
                    Persona = cenario.Nome,
                    RegraCritica = indicadorCritico.Nome,
                    IndicadorRelacionado = indicadorCritico.CodigoIndicador.ToString(),
                    Valor = valoresIndicadores.TryGetValue(indicadorCritico.CodigoIndicador, out var valor) ? valor : 0m,
                    Penalizacao = indicadorCritico.Penalidade,
                    Justificativa = indicadorCritico.Motivo
                })
                .ToList();
        }

        private static int BuscarNotaPilar(MfScoreFinanceiro mfScore, CodigoPilarMfScoreFinanceiro codigo)
        {
            return mfScore.Pilares.FirstOrDefault(pilar => pilar.Codigo == codigo)?.Nota ?? 0;
        }

        private static string BuscarPilarRelacionado(MfScoreFinanceiro mfScore, string nomeIndicador)
        {
            return mfScore.Pilares.FirstOrDefault(pilar => pilar.Indicadores.Contains(nomeIndicador))?.Nome ?? "Nao mapeado";
        }

        private sealed class ResultadoCenarioAuditoriaMfScore
        {
            public required CenarioMfScore Cenario { get; init; }
            public required PainelIndicadoresFinanceiros PainelIndicadores { get; init; }
            public required MfScoreFinanceiro MfScore { get; init; }
        }
    }
}
