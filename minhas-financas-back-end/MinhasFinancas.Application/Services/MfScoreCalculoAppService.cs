using System.Text.Json;
using MinhasFinancas.Application.DTOs.MfScore;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Services.AnaliseFinanceira;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Application.Services
{
    public class MfScoreCalculoAppService : IMfScoreCalculoAppService
    {
        public const string VersaoModeloAtual = "mf-score-v2.5-1000";

        private readonly IAnaliseFinanceiraAppService _analiseFinanceiraAppService;
        private readonly ISaudeFinanceiraService _saudeFinanceiraService;
        private readonly IHistoricoMfScoreRepository _historicoMfScoreRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public MfScoreCalculoAppService(
            IAnaliseFinanceiraAppService analiseFinanceiraAppService,
            ISaudeFinanceiraService saudeFinanceiraService,
            IHistoricoMfScoreRepository historicoMfScoreRepository,
            IUsuarioRepository usuarioRepository)
        {
            _analiseFinanceiraAppService = analiseFinanceiraAppService;
            _saudeFinanceiraService = saudeFinanceiraService;
            _historicoMfScoreRepository = historicoMfScoreRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<ResultadoCalculoMfScoreInternoDTO?> CalcularAsync(string usuarioId, DateTime? dataReferencia = null)
        {
            var contexto = await _analiseFinanceiraAppService.BuscarContextoAnaliseInternoAsync(usuarioId, dataReferencia);
            if (contexto is null)
            {
                return null;
            }

            var painelIndicadores = await _analiseFinanceiraAppService.BuscarPainelIndicadoresInternoAsync(usuarioId, dataReferencia);
            if (painelIndicadores is null)
            {
                return null;
            }

            var historicoPontuacoes = await _historicoMfScoreRepository.BuscarRecentesPorUsuarioAsync(usuarioId, 6);
            var contextoComplementar = ConstrutorContextoComplementarMfScoreFinanceiro.Construir(
                contexto,
                historicoPontuacoes
                    .OrderBy(x => x.CompetenciaAno)
                    .ThenBy(x => x.CompetenciaMes)
                    .Select(x => x.MfScoreFinal)
                    .ToList());
            var painelSaude = _saudeFinanceiraService.GerarPainel(painelIndicadores, contextoComplementar);

            return new ResultadoCalculoMfScoreInternoDTO
            {
                UsuarioId = usuarioId,
                DataReferencia = contexto.DataReferencia,
                ContextoAnalise = contexto,
                PainelIndicadores = painelIndicadores,
                PainelSaude = painelSaude,
                ContextoComplementar = contextoComplementar
            };
        }

        public async Task<RetornoGenerico> GerarHistoricoMensalAsync(DateTime? competenciaReferencia = null)
        {
            var competencia = NormalizarCompetencia(competenciaReferencia ?? DateTime.Today.AddMonths(-1));
            var usuarios = await _usuarioRepository.BuscarIdsUsuariosAtivosAsync();
            var resultado = new ResultadoGeracaoHistoricoMfScoreDTO
            {
                CompetenciaAno = competencia.Year,
                CompetenciaMes = competencia.Month,
                VersaoModelo = VersaoModeloAtual,
                UsuariosProcessados = usuarios.Count
            };

            foreach (var usuarioId in usuarios)
            {
                try
                {
                    var existente = await _historicoMfScoreRepository.BuscarPorCompetenciaAsync(
                        usuarioId,
                        competencia.Year,
                        competencia.Month,
                        VersaoModeloAtual);

                    if (existente is not null)
                    {
                        resultado.HistoricosJaExistentes++;
                        continue;
                    }

                    var calculo = await CalcularAsync(usuarioId, competencia);
                    if (calculo is null)
                    {
                        resultado.Falhas.Add($"Usuário {usuarioId}: não foi possível calcular o MF Score.");
                        continue;
                    }

                    var historico = CriarHistorico(calculo);
                    await _historicoMfScoreRepository.AdicionarAsync(historico);
                    resultado.HistoricosCriados++;
                }
                catch (Exception ex)
                {
                    resultado.Falhas.Add($"Usuário {usuarioId}: {ex.Message}");
                }
            }

            await _historicoMfScoreRepository.SalvarAlteracoesAsync();

            return new RetornoGenerico(
                true,
                "Histórico mensal do MF Score processado com sucesso.",
                "Histórico mensal do MF Score processado com sucesso.",
                System.Net.HttpStatusCode.OK,
                resultado);
        }

        private static HistoricoMfScore CriarHistorico(ResultadoCalculoMfScoreInternoDTO calculo)
        {
            var mfScore = calculo.PainelSaude.Resumo.MfScore;
            var resumo = new
            {
                DataReferencia = calculo.DataReferencia,
                PontuacaoGeral = calculo.PainelSaude.Resumo.PontuacaoGeral,
                calculo.PainelSaude.Resumo.Classificacao,
                PontosAtencao = calculo.PainelSaude.Resumo.PontosAtencao.Select(x => new { x.Nome, x.Status, x.Descricao, x.Observacao }),
                ContextoTemporal = calculo.ContextoComplementar
            };

            return new HistoricoMfScore
            {
                Id = Guid.NewGuid(),
                UsuarioId = calculo.UsuarioId,
                CompetenciaAno = calculo.DataReferencia.Year,
                CompetenciaMes = calculo.DataReferencia.Month,
                MfScoreBase = mfScore.PontuacaoBase,
                MfScoreFinal = mfScore.PontuacaoFinal,
                Classificacao = mfScore.Classificacao,
                Risco = mfScore.Risco,
                PenalidadeTotal = mfScore.PenalidadeTotal,
                DataCalculo = DateTime.UtcNow,
                VersaoModelo = VersaoModeloAtual,
                JsonPilares = JsonSerializer.Serialize(mfScore.Pilares),
                JsonIndicadoresCriticos = JsonSerializer.Serialize(mfScore.IndicadoresCriticos),
                JsonResumo = JsonSerializer.Serialize(resumo),
                CriadoEm = DateTime.UtcNow
            };
        }

        private static DateTime NormalizarCompetencia(DateTime data)
        {
            return new DateTime(data.Year, data.Month, 1);
        }
    }
}
