using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Reports;
using MinhasFinancas.Domain.Services.AnaliseFinanceira;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.AuditoriaMfScore;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class MfScoreAuditoriaAppService : IMfScoreAuditoriaAppService
    {
        private const string VersaoMfScore = "Nao versionado";

        private readonly IEnumerable<IPersonaMfScore> _personas;
        private readonly IIndicadoresFinanceirosService _indicadoresFinanceirosService;
        private readonly ISaudeFinanceiraService _saudeFinanceiraService;
        private readonly IExcelReport<MfScoreAuditoriaExcelReportData> _excelReport;

        public MfScoreAuditoriaAppService(
            IEnumerable<IPersonaMfScore> personas,
            IIndicadoresFinanceirosService indicadoresFinanceirosService,
            ISaudeFinanceiraService saudeFinanceiraService,
            IExcelReport<MfScoreAuditoriaExcelReportData> excelReport)
        {
            _personas = personas;
            _indicadoresFinanceirosService = indicadoresFinanceirosService;
            _saudeFinanceiraService = saudeFinanceiraService;
            _excelReport = excelReport;
        }

        public Task<RetornoGenerico> GerarPlanilhaAsync()
        {
            try
            {
                var cenarios = _personas
                    .Select(persona => persona.CriarCenario())
                    .ToList();

                var dataGeracao = DateTime.Now;
                var dadosRelatorio = new MfScoreAuditoriaExcelReportData
                {
                    NomeArquivo = $"mf-score-auditoria-{dataGeracao:yyyyMMdd-HHmmss}.xlsx",
                    DataGeracao = dataGeracao,
                    VersaoMfScore = VersaoMfScore,
                    Cenarios = cenarios
                        .Select(CalcularCenario)
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

        private MfScoreAuditoriaCenarioExcelReportData CalcularCenario(CenarioMfScore cenario)
        {
            var painelIndicadores = _indicadoresFinanceirosService.Calcular(cenario.Contexto);
            var painelSaude = _saudeFinanceiraService.GerarPainel(painelIndicadores);
            var mfScore = painelSaude.Resumo.MfScore;

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

        private static int BuscarNotaPilar(MfScoreFinanceiro mfScore, CodigoPilarMfScoreFinanceiro codigo)
        {
            return mfScore.Pilares.FirstOrDefault(pilar => pilar.Codigo == codigo)?.Nota ?? 0;
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
    }
}
