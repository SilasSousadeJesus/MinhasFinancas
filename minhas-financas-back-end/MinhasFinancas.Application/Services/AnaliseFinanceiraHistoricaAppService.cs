using System.Net;
using System.Reflection;
using System.Text.Json;
using MinhasFinancas.Application.DTOs.AnaliseFinanceiraHistorica;
using MinhasFinancas.Application.DTOs.Lancamento;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Application.Services
{
    public class AnaliseFinanceiraHistoricaAppService : IAnaliseFinanceiraHistoricaAppService
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly IUsuarioAppService _usuarioAppService;
        private readonly IAnaliseFinanceiraHistoricaRepository _repository;
        private readonly IPerfilFinanceiroRepository _perfilFinanceiroRepository;

        public AnaliseFinanceiraHistoricaAppService(
            IUsuarioAppService usuarioAppService,
            IAnaliseFinanceiraHistoricaRepository repository,
            IPerfilFinanceiroRepository perfilFinanceiroRepository)
        {
            _usuarioAppService = usuarioAppService;
            _repository = repository;
            _perfilFinanceiroRepository = perfilFinanceiroRepository;
        }

        public async Task<RetornoGenerico> BuscarTodasAsync(string usuarioId, int pagina = 1, int tamanhoPagina = 5)
        {
            try
            {
                var validacaoUsuario = await ValidarUsuarioAsync(usuarioId);
                if (validacaoUsuario != null)
                {
                    return validacaoUsuario;
                }

                var paginaFinal = pagina < 1 ? 1 : pagina;
                var tamanhoPaginaFinal = tamanhoPagina < 1 ? 5 : tamanhoPagina;

                var resultado = await _repository.BuscarPaginaAsync(usuarioId, paginaFinal, tamanhoPaginaFinal);
                var dados = new ResultadoPaginadoDTO<AnaliseFinanceiraHistoricaListaDTO>
                {
                    Itens = resultado.Itens.Select(MapearLista).ToList(),
                    PaginaAtual = paginaFinal,
                    TamanhoPagina = tamanhoPaginaFinal,
                    TotalItens = resultado.TotalItens,
                    TotalPaginas = resultado.TotalItens == 0
                        ? 1
                        : (int)Math.Ceiling(resultado.TotalItens / (double)tamanhoPaginaFinal)
                };

                return new RetornoGenerico(
                    true,
                    $"{dados.TotalItens} análise(s) histórica(s) encontrada(s).",
                    $"{dados.Itens.Count} análise(s) histórica(s) carregada(s) com sucesso.",
                    HttpStatusCode.OK,
                    dados);
            }
            catch (Exception ex)
            {
                return new RetornoGenerico(false, ex.ToString(), "Não foi possível carregar o histórico de análises.", HttpStatusCode.InternalServerError, null);
            }
        }

        public async Task<RetornoGenerico> BuscarDetalheAsync(string usuarioId, Guid analiseId)
        {
            try
            {
                var validacaoUsuario = await ValidarUsuarioAsync(usuarioId);
                if (validacaoUsuario != null)
                {
                    return validacaoUsuario;
                }

                var analise = await _repository.BuscarUmElementoAsync(usuarioId, analiseId);

                if (analise == null)
                {
                    return new RetornoGenerico(false, "Análise histórica não encontrada.", "Não foi possível localizar a análise histórica.", HttpStatusCode.NotFound, null);
                }

                return new RetornoGenerico(
                    true,
                    "Análise histórica carregada com sucesso.",
                    "Análise histórica carregada com sucesso.",
                    HttpStatusCode.OK,
                    MapearDetalhe(analise));
            }
            catch (Exception ex)
            {
                return new RetornoGenerico(false, ex.ToString(), "Não foi possível carregar a análise histórica.", HttpStatusCode.InternalServerError, null);
            }
        }

        public async Task<RetornoGenerico> ExcluirAsync(string usuarioId, Guid analiseId)
        {
            try
            {
                var validacaoUsuario = await ValidarUsuarioAsync(usuarioId);
                if (validacaoUsuario != null)
                {
                    return validacaoUsuario;
                }

                var analise = await _repository.BuscarUmElementoAsync(usuarioId, analiseId);

                if (analise == null)
                {
                    return new RetornoGenerico(false, "Análise histórica não encontrada.", "Não foi possível localizar a análise histórica.", HttpStatusCode.NotFound, null);
                }

                analise.Ativa = false;
                await _repository.EditarElementoAsync(analise);

                return new RetornoGenerico(
                    true,
                    "Análise histórica inativada com sucesso.",
                    "Análise removida do histórico com sucesso.",
                    HttpStatusCode.OK,
                    null);
            }
            catch (Exception ex)
            {
                return new RetornoGenerico(false, ex.ToString(), "Não foi possível excluir a análise histórica.", HttpStatusCode.InternalServerError, null);
            }
        }

        public async Task<Guid?> RegistrarAsync(RegistrarAnaliseFinanceiraHistoricaDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UsuarioId))
            {
                return null;
            }

            var perfilFinanceiroVigente = await BuscarPerfilFinanceiroVigenteAsync(dto.UsuarioId);
            var indicadoresResumidos = dto.ResumoFinanceiroIA.Indicadores.Todos
                .Select(indicador => new IndicadorFinanceiroResumidoDTO
                {
                    Nome = indicador.Nome,
                    Status = FormatarStatus(indicador.Status),
                    Observacao = indicador.Observacao
                })
                .ToList();

            var insightsResumidos = dto.ResumoFinanceiroIA.Insights.Todos
                .Select(insight => new InsightFinanceiroResumidoDTO
                {
                    Tipo = insight.Tipo.ToString(),
                    Titulo = insight.Titulo,
                    AcaoSugerida = insight.AcaoSugerida
                })
                .ToList();

            var principaisRiscos = dto.ResumoFinanceiroIA.Insights.Prioritarios
                .Select(insight => insight.Titulo)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Take(4)
                .ToList();

            var principaisPontosPositivos = dto.ResumoFinanceiroIA.DestaquesPositivos
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Take(4)
                .ToList();

            if (principaisPontosPositivos.Count == 0)
            {
                principaisPontosPositivos = dto.ResumoFinanceiroIA.Insights.DestaquesPositivos
                    .Select(insight => insight.Titulo)
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Take(4)
                    .ToList();
            }

            var principaisRecomendacoes = dto.ResumoFinanceiroIA.Insights.Prioritarios
                .Select(insight => insight.AcaoSugerida)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Take(4)
                .ToList();

            var periodoReferencia = new DateTime(
                dto.ResumoFinanceiroIA.DataReferencia.Year,
                dto.ResumoFinanceiroIA.DataReferencia.Month,
                1);

            var entidade = new AnaliseFinanceiraHistorica
            {
                Id = Guid.NewGuid(),
                UsuarioId = dto.UsuarioId,
                DataGeracao = DateTime.UtcNow,
                PeriodoReferencia = periodoReferencia,
                PontuacaoSaudeFinanceira = dto.ResumoFinanceiroIA.SaudeFinanceira.PontuacaoGeral,
                ClassificacaoSaudeFinanceira = dto.ResumoFinanceiroIA.SaudeFinanceira.Classificacao,
                ResumoExecutivoSistema = dto.ResumoFinanceiroIA.ResumoExecutivo,
                ContextoResumoFinanceiroIAJson = JsonSerializer.Serialize(dto.ResumoFinanceiroIA, JsonOptions),
                IndicadoresResumidosJson = JsonSerializer.Serialize(indicadoresResumidos, JsonOptions),
                InsightsResumidosJson = JsonSerializer.Serialize(insightsResumidos, JsonOptions),
                PerfilFinanceiroVigenteJson = JsonSerializer.Serialize(perfilFinanceiroVigente, JsonOptions),
                PrincipaisRiscosJson = JsonSerializer.Serialize(principaisRiscos, JsonOptions),
                PrincipaisPontosPositivosJson = JsonSerializer.Serialize(principaisPontosPositivos, JsonOptions),
                PrincipaisRecomendacoesJson = JsonSerializer.Serialize(principaisRecomendacoes, JsonOptions),
                PrioridadesJson = JsonSerializer.Serialize(dto.ResumoFinanceiroIA.PrioridadesImediatas, JsonOptions),
                PerguntaUsuario = dto.ContextoAssistenteFinanceiro.PerguntaUsuario ?? string.Empty,
                RespostaIA = dto.RespostaIA.Conteudo ?? string.Empty,
                ProvedorIA = dto.RespostaIA.Provedor ?? string.Empty,
                ModeloIA = dto.RespostaIA.Modelo ?? dto.RequisicaoIA.ModeloSugerido ?? string.Empty,
                VersaoPrompt = dto.RequisicaoIA.VersaoPrompt ?? string.Empty,
                VersaoSistema = ObterVersaoSistema(),
                TokensEntrada = dto.RespostaIA.TokensEntradaUtilizados > 0 ? dto.RespostaIA.TokensEntradaUtilizados : dto.RespostaIA.TokensEntradaEstimados,
                TokensSaida = dto.RespostaIA.TokensSaidaUtilizados,
                TokensTotais = dto.RespostaIA.TokensTotaisUtilizados > 0
                    ? dto.RespostaIA.TokensTotaisUtilizados
                    : (dto.RespostaIA.TokensEntradaUtilizados > 0 ? dto.RespostaIA.TokensEntradaUtilizados : dto.RespostaIA.TokensEntradaEstimados) + dto.RespostaIA.TokensSaidaUtilizados,
                CustoEstimadoUsd = dto.RespostaIA.CustoEstimadoUsd,
                TempoTotalMs = dto.RespostaIA.TempoTotalMs,
                Sucesso = dto.RespostaIA.Sucesso,
                MensagemErro = dto.RespostaIA.Sucesso
                    ? string.Empty
                    : string.IsNullOrWhiteSpace(dto.RespostaIA.MensagemTecnica)
                        ? dto.RespostaIA.MensagemAmigavel
                        : dto.RespostaIA.MensagemTecnica,
                Ativa = true
            };

            await _repository.CadastrarElementoAsync(entidade);
            return entidade.Id;
        }

        public async Task<List<AnaliseFinanceiraHistoricaResumidaDTO>> BuscarUltimasAnalisesResumidasAsync(string usuarioId, int quantidade)
        {
            var analises = await _repository.BuscarUltimasAnalisesAsync(usuarioId, quantidade);
            return analises.Select(MapearResumida).ToList();
        }

        private async Task<PerfilFinanceiroVigenteResumidoDTO?> BuscarPerfilFinanceiroVigenteAsync(string usuarioId)
        {
            var perfil = await _perfilFinanceiroRepository.BuscarPorUsuarioLeituraAsync(usuarioId);
            var vigente = perfil?.Configuracoes
                .Where(x => x.DataFimVigencia == null)
                .OrderByDescending(x => x.DataInicioVigencia)
                .ThenByDescending(x => x.DataCriacao)
                .FirstOrDefault();

            if (vigente == null)
            {
                return null;
            }

            return new PerfilFinanceiroVigenteResumidoDTO
            {
                PercentualEconomiaMensalDesejado = vigente.PercentualEconomiaMensalDesejado,
                PercentualReservaEmergenciaDesejado = vigente.PercentualReservaEmergenciaDesejado,
                MesesReservaEmergenciaDesejados = vigente.MesesReservaEmergenciaDesejados,
                PercentualMaximoComprometimentoRenda = vigente.PercentualMaximoComprometimentoRenda,
                PercentualMaximoEndividamento = vigente.PercentualMaximoEndividamento,
                PercentualMinimoInvestimento = vigente.PercentualMinimoInvestimento,
                PatrimonioLiquidoAlvo = vigente.PatrimonioLiquidoAlvo
            };
        }

        private async Task<RetornoGenerico?> ValidarUsuarioAsync(string usuarioId)
        {
            var buscaPorUsuario = await _usuarioAppService.BuscarUmUsuario(usuarioId);
            if (buscaPorUsuario.Sucesso)
            {
                return null;
            }

            return new RetornoGenerico
            {
                Sucesso = false,
                HttpStatusCode = HttpStatusCode.NotFound,
                MensagemSistema = buscaPorUsuario.MensagemSistema,
                MensagemUsuario = buscaPorUsuario.MensagemUsuario,
                Dados = null
            };
        }

        private static string ObterVersaoSistema()
        {
            return Assembly.GetEntryAssembly()?.GetName().Version?.ToString()
                ?? Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? "desconhecida";
        }

        private static AnaliseFinanceiraHistoricaListaDTO MapearLista(AnaliseFinanceiraHistorica entidade)
        {
            return new AnaliseFinanceiraHistoricaListaDTO
            {
                Id = entidade.Id,
                DataGeracao = entidade.DataGeracao,
                PeriodoReferencia = entidade.PeriodoReferencia,
                PontuacaoSaudeFinanceira = entidade.PontuacaoSaudeFinanceira,
                ClassificacaoSaudeFinanceira = entidade.ClassificacaoSaudeFinanceira,
                ResumoExecutivoSistema = entidade.ResumoExecutivoSistema,
                PerguntaUsuario = entidade.PerguntaUsuario,
                ProvedorIA = entidade.ProvedorIA,
                ModeloIA = entidade.ModeloIA,
                Sucesso = entidade.Sucesso,
                TempoTotalMs = entidade.TempoTotalMs,
                CustoEstimadoUsd = entidade.CustoEstimadoUsd,
                CompromissoFinanceiroId = entidade.CompromissoFinanceiroId
            };
        }

        private static AnaliseFinanceiraHistoricaDetalheDTO MapearDetalhe(AnaliseFinanceiraHistorica entidade)
        {
            return new AnaliseFinanceiraHistoricaDetalheDTO
            {
                Id = entidade.Id,
                UsuarioId = entidade.UsuarioId,
                DataGeracao = entidade.DataGeracao,
                PeriodoReferencia = entidade.PeriodoReferencia,
                PontuacaoSaudeFinanceira = entidade.PontuacaoSaudeFinanceira,
                ClassificacaoSaudeFinanceira = entidade.ClassificacaoSaudeFinanceira,
                ResumoExecutivoSistema = entidade.ResumoExecutivoSistema,
                ContextoResumoFinanceiroIAJson = entidade.ContextoResumoFinanceiroIAJson,
                IndicadoresResumidosJson = entidade.IndicadoresResumidosJson,
                InsightsResumidosJson = entidade.InsightsResumidosJson,
                PerfilFinanceiroVigenteJson = entidade.PerfilFinanceiroVigenteJson,
                PrincipaisRiscosJson = entidade.PrincipaisRiscosJson,
                PrincipaisPontosPositivosJson = entidade.PrincipaisPontosPositivosJson,
                PrincipaisRecomendacoesJson = entidade.PrincipaisRecomendacoesJson,
                PrioridadesJson = entidade.PrioridadesJson,
                PerguntaUsuario = entidade.PerguntaUsuario,
                RespostaIA = entidade.RespostaIA,
                ProvedorIA = entidade.ProvedorIA,
                ModeloIA = entidade.ModeloIA,
                VersaoPrompt = entidade.VersaoPrompt,
                VersaoSistema = entidade.VersaoSistema,
                TokensEntrada = entidade.TokensEntrada,
                TokensSaida = entidade.TokensSaida,
                TokensTotais = entidade.TokensTotais,
                CustoEstimadoUsd = entidade.CustoEstimadoUsd,
                TempoTotalMs = entidade.TempoTotalMs,
                Sucesso = entidade.Sucesso,
                MensagemErro = entidade.MensagemErro,
                CompromissoFinanceiroId = entidade.CompromissoFinanceiroId,
                Ativa = entidade.Ativa
            };
        }

        private static AnaliseFinanceiraHistoricaResumidaDTO MapearResumida(AnaliseFinanceiraHistorica entidade)
        {
            return new AnaliseFinanceiraHistoricaResumidaDTO
            {
                Id = entidade.Id,
                DataGeracao = entidade.DataGeracao,
                PeriodoReferencia = entidade.PeriodoReferencia,
                PontuacaoSaudeFinanceira = entidade.PontuacaoSaudeFinanceira,
                ClassificacaoSaudeFinanceira = entidade.ClassificacaoSaudeFinanceira,
                ResumoExecutivoSistema = entidade.ResumoExecutivoSistema,
                PrincipaisRiscos = DeserializarListaTexto(entidade.PrincipaisRiscosJson),
                PrincipaisPontosPositivos = DeserializarListaTexto(entidade.PrincipaisPontosPositivosJson),
                PrincipaisRecomendacoes = DeserializarListaTexto(entidade.PrincipaisRecomendacoesJson),
                Prioridades = DeserializarListaTexto(entidade.PrioridadesJson),
                Sucesso = entidade.Sucesso
            };
        }

        private static List<string> DeserializarListaTexto(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return [];
            }

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json, JsonOptions) ?? [];
            }
            catch
            {
                return [];
            }
        }

        private static string FormatarStatus(StatusIndicadorFinanceiro status)
        {
            return status switch
            {
                StatusIndicadorFinanceiro.Excelente => "Excelente",
                StatusIndicadorFinanceiro.Bom => "Bom",
                StatusIndicadorFinanceiro.Atencao => "Atenção",
                _ => "Crítico"
            };
        }
    }
}
