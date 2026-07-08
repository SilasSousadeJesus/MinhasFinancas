using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Provedores
{
    public class OpenAIProvider : IProvedorIA
    {
        private const string NomeProvedor = "OpenAI";
        private const string OrigemErroConfiguracao = "Configuracao.OpenAI";
        private const string OrigemErroTimeout = "OpenAI.Timeout";
        private const string OrigemErroAutenticacao = "OpenAI.Autenticacao";
        private const string OrigemErroPermissao = "OpenAI.Permissao";
        private const string OrigemErroLimite = "OpenAI.Limite";
        private const string OrigemErroTransiente = "OpenAI.Transiente";
        private const string OrigemErroResposta = "OpenAI.Resposta";

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenAIProvider> _logger;
        private readonly ConfiguracaoOpenAI _configuracao;

        public OpenAIProvider(
            HttpClient httpClient,
            IOptions<ConfiguracaoOpenAI> configuracao,
            ILogger<OpenAIProvider> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuracao = configuracao.Value;
        }

        public async Task<RespostaIA> GerarRespostaAsync(RequisicaoIA requisicao, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_configuracao.ApiKey))
            {
                return CriarFalha(
                    mensagemTecnica: "A chave da OpenAI não foi configurada.",
                    mensagemAmigavel: "A integração com IA ainda não foi configurada neste ambiente.",
                    origemErro: OrigemErroConfiguracao,
                    categoriaErro: CategoriaErroIA.Configuracao);
            }

            var promptCompleto = requisicao.PromptCompleto ?? string.Empty;
            var caracteresOriginais = promptCompleto.Length;
            var promptTruncado = TruncarSeNecessario(promptCompleto, _configuracao.MaxInputCharacters, out var entradaFoiTruncada);
            var tokensEntradaEstimados = EstimarTokens(promptTruncado);
            var tentativasMaximas = Math.Max(1, _configuracao.RetryCount + 1);
            var timeout = TimeSpan.FromSeconds(Math.Max(5, _configuracao.TimeoutSeconds));
            var modelo = string.IsNullOrWhiteSpace(requisicao.ModeloSugerido)
                ? _configuracao.Model
                : requisicao.ModeloSugerido;

            _logger.LogInformation(
                "Iniciando chamada {Provedor}. Modelo={Modelo}. CaracteresEntrada={CaracteresEntrada}. TokensEntradaEstimados={TokensEntradaEstimados}. EntradaTruncada={EntradaTruncada}. TentativasMaximas={TentativasMaximas}.",
                NomeProvedor,
                modelo,
                caracteresOriginais,
                tokensEntradaEstimados,
                entradaFoiTruncada,
                tentativasMaximas);

            for (var tentativa = 1; tentativa <= tentativasMaximas; tentativa++)
            {
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    using var attemptCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    attemptCts.CancelAfter(timeout);

                    using var requestMessage = CriarRequestMessage(modelo, promptTruncado);
                    using var response = await _httpClient.SendAsync(
                        requestMessage,
                        HttpCompletionOption.ResponseHeadersRead,
                        attemptCts.Token);

                    var conteudoResposta = await response.Content.ReadAsStringAsync(attemptCts.Token);

                    if (response.IsSuccessStatusCode)
                    {
                        using var document = JsonDocument.Parse(conteudoResposta);
                        var texto = ExtrairTextoResposta(document.RootElement);
                        var metricas = ExtrairMetricasUso(document.RootElement, tokensEntradaEstimados, texto);
                        var custoEstimado = CalcularCustoEstimado(metricas, _configuracao);

                        if (string.IsNullOrWhiteSpace(texto))
                        {
                            stopwatch.Stop();

                            _logger.LogWarning(
                                "Resposta vazia do provedor {Provedor}. Modelo={Modelo}. Tentativa={Tentativa}. TempoTotalMs={TempoTotalMs}. TokensEntrada={TokensEntrada}. TokensSaida={TokensSaida}. TokensTotais={TokensTotais}.",
                                NomeProvedor,
                                modelo,
                                tentativa,
                                stopwatch.ElapsedMilliseconds,
                                metricas.TokensEntradaUtilizados,
                                metricas.TokensSaidaUtilizados,
                                metricas.TokensTotaisUtilizados);

                            return CriarFalha(
                                mensagemTecnica: "A API da OpenAI respondeu sem texto utilizável.",
                                mensagemAmigavel: "A IA não retornou um texto válido para esta análise.",
                                origemErro: OrigemErroResposta,
                                categoriaErro: CategoriaErroIA.RespostaInvalida,
                                statusHttpProvedor: (int)response.StatusCode,
                                modelo: modelo,
                                tentativasRealizadas: tentativa,
                                caracteresEntrada: caracteresOriginais,
                                tokensEntradaEstimados: tokensEntradaEstimados,
                                tokensEntradaUtilizados: metricas.TokensEntradaUtilizados,
                                tokensSaidaUtilizados: metricas.TokensSaidaUtilizados,
                                tokensRaciocinioUtilizados: metricas.TokensRaciocinioUtilizados,
                                tokensTotaisUtilizados: metricas.TokensTotaisUtilizados,
                                tokensReaisDisponiveis: metricas.TokensReaisDisponiveis,
                                entradaFoiTruncada: entradaFoiTruncada,
                                tempoTotalMs: stopwatch.ElapsedMilliseconds,
                                custoEstimadoUsd: custoEstimado,
                                precoEntradaPorMilhaoTokensUsd: _configuracao.InputPricePerMillionTokens,
                                precoSaidaPorMilhaoTokensUsd: _configuracao.OutputPricePerMillionTokens);
                        }

                        stopwatch.Stop();

                        _logger.LogInformation(
                            "Chamada {Provedor} concluída com sucesso. Modelo={Modelo}. Tentativa={Tentativa}. TempoTotalMs={TempoTotalMs}. TokensEntrada={TokensEntrada}. TokensSaida={TokensSaida}. TokensRaciocinio={TokensRaciocinio}. TokensTotais={TokensTotais}. TokensReaisDisponiveis={TokensReaisDisponiveis}. CustoEstimadoUsd={CustoEstimadoUsd}.",
                            NomeProvedor,
                            modelo,
                            tentativa,
                            stopwatch.ElapsedMilliseconds,
                            metricas.TokensEntradaUtilizados,
                            metricas.TokensSaidaUtilizados,
                            metricas.TokensRaciocinioUtilizados,
                            metricas.TokensTotaisUtilizados,
                            metricas.TokensReaisDisponiveis,
                            custoEstimado);

                        var sugestaoCompromisso = ExtrairSugestaoCompromissoFinanceiro(texto);

                        return new RespostaIA
                        {
                            Sucesso = true,
                            Provedor = NomeProvedor,
                            Modelo = modelo,
                            Conteudo = texto.Trim(),
                            SugestaoCompromissoFinanceiro = sugestaoCompromisso,
                            FoiSimulada = false,
                            ObservacaoInfraestrutura = "Resposta real gerada pela OpenAI a partir do contexto preparado pelo sistema.",
                            MensagemTecnica = "Resposta gerada com sucesso.",
                            MensagemAmigavel = "Análise técnica gerada com sucesso.",
                            OrigemErro = string.Empty,
                            CategoriaErro = CategoriaErroIA.Nenhum,
                            StatusHttpProvedor = (int)response.StatusCode,
                            TentativasRealizadas = tentativa,
                            CaracteresEntrada = caracteresOriginais,
                            TokensEntradaEstimados = tokensEntradaEstimados,
                            TokensEntradaUtilizados = metricas.TokensEntradaUtilizados,
                            TokensSaidaUtilizados = metricas.TokensSaidaUtilizados,
                            TokensRaciocinioUtilizados = metricas.TokensRaciocinioUtilizados,
                            TokensTotaisUtilizados = metricas.TokensTotaisUtilizados,
                            TokensReaisDisponiveis = metricas.TokensReaisDisponiveis,
                            EntradaFoiTruncada = entradaFoiTruncada,
                            TempoTotalMs = stopwatch.ElapsedMilliseconds,
                            CustoEstimadoUsd = custoEstimado,
                            PrecoEntradaPorMilhaoTokensUsd = _configuracao.InputPricePerMillionTokens,
                            PrecoSaidaPorMilhaoTokensUsd = _configuracao.OutputPricePerMillionTokens
                        };
                    }

                    if (DeveTentarNovamente(response.StatusCode) && tentativa < tentativasMaximas)
                    {
                        stopwatch.Stop();

                        _logger.LogWarning(
                            "Falha transitória ao chamar {Provedor}. Modelo={Modelo}. Status={Status}. Tentativa={Tentativa}. TempoTotalMs={TempoTotalMs}. Nova tentativa será executada.",
                            NomeProvedor,
                            modelo,
                            (int)response.StatusCode,
                            tentativa,
                            stopwatch.ElapsedMilliseconds);

                        await Task.Delay(TimeSpan.FromSeconds(tentativa), cancellationToken);
                        continue;
                    }

                    stopwatch.Stop();

                    _logger.LogWarning(
                        "Chamada {Provedor} falhou. Modelo={Modelo}. Status={Status}. Tentativa={Tentativa}. TempoTotalMs={TempoTotalMs}.",
                        NomeProvedor,
                        modelo,
                        (int)response.StatusCode,
                        tentativa,
                        stopwatch.ElapsedMilliseconds);

                    return CriarFalhaPorStatus(
                        statusCode: response.StatusCode,
                        corpoResposta: conteudoResposta,
                        modelo: modelo,
                        tentativa: tentativa,
                        caracteresEntrada: caracteresOriginais,
                        tokensEntradaEstimados: tokensEntradaEstimados,
                        entradaFoiTruncada: entradaFoiTruncada,
                        tempoTotalMs: stopwatch.ElapsedMilliseconds,
                        configuracao: _configuracao);
                }
                catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
                {
                    stopwatch.Stop();

                    if (tentativa < tentativasMaximas)
                    {
                        _logger.LogWarning(
                            ex,
                            "Timeout ao chamar {Provedor}. Modelo={Modelo}. Tentativa={Tentativa}. TempoTotalMs={TempoTotalMs}. Nova tentativa será executada.",
                            NomeProvedor,
                            modelo,
                            tentativa,
                            stopwatch.ElapsedMilliseconds);

                        await Task.Delay(TimeSpan.FromSeconds(tentativa), cancellationToken);
                        continue;
                    }

                    _logger.LogError(
                        ex,
                        "Timeout definitivo ao chamar {Provedor}. Modelo={Modelo}. Tentativas={Tentativas}. TempoTotalMs={TempoTotalMs}.",
                        NomeProvedor,
                        modelo,
                        tentativa,
                        stopwatch.ElapsedMilliseconds);

                    return CriarFalha(
                        mensagemTecnica: "A chamada para a OpenAI excedeu o tempo limite configurado.",
                        mensagemAmigavel: "A IA demorou mais do que o esperado para responder. Tente novamente em instantes.",
                        origemErro: OrigemErroTimeout,
                        categoriaErro: CategoriaErroIA.Timeout,
                        modelo: modelo,
                        tentativasRealizadas: tentativa,
                        caracteresEntrada: caracteresOriginais,
                        tokensEntradaEstimados: tokensEntradaEstimados,
                        tokensEntradaUtilizados: tokensEntradaEstimados,
                        tokensSaidaUtilizados: 0,
                        tokensRaciocinioUtilizados: 0,
                        tokensTotaisUtilizados: tokensEntradaEstimados,
                        tokensReaisDisponiveis: false,
                        entradaFoiTruncada: entradaFoiTruncada,
                        tempoTotalMs: stopwatch.ElapsedMilliseconds,
                        custoEstimadoUsd: CalcularCustoEstimado(tokensEntradaEstimados, 0, _configuracao),
                        precoEntradaPorMilhaoTokensUsd: _configuracao.InputPricePerMillionTokens,
                        precoSaidaPorMilhaoTokensUsd: _configuracao.OutputPricePerMillionTokens);
                }
                catch (HttpRequestException ex)
                {
                    stopwatch.Stop();

                    if (tentativa < tentativasMaximas)
                    {
                        _logger.LogWarning(
                            ex,
                            "Falha de rede ao chamar {Provedor}. Modelo={Modelo}. Tentativa={Tentativa}. TempoTotalMs={TempoTotalMs}. Nova tentativa será executada.",
                            NomeProvedor,
                            modelo,
                            tentativa,
                            stopwatch.ElapsedMilliseconds);

                        await Task.Delay(TimeSpan.FromSeconds(tentativa), cancellationToken);
                        continue;
                    }

                    _logger.LogError(
                        ex,
                        "Falha de rede definitiva ao chamar {Provedor}. Modelo={Modelo}. Tentativas={Tentativas}. TempoTotalMs={TempoTotalMs}.",
                        NomeProvedor,
                        modelo,
                        tentativa,
                        stopwatch.ElapsedMilliseconds);

                    return CriarFalha(
                        mensagemTecnica: $"Falha de rede ao chamar OpenAI: {ex.Message}",
                        mensagemAmigavel: "Não foi possível se comunicar com o provedor de IA neste momento.",
                        origemErro: OrigemErroTransiente,
                        categoriaErro: CategoriaErroIA.Transiente,
                        modelo: modelo,
                        tentativasRealizadas: tentativa,
                        caracteresEntrada: caracteresOriginais,
                        tokensEntradaEstimados: tokensEntradaEstimados,
                        tokensEntradaUtilizados: tokensEntradaEstimados,
                        tokensSaidaUtilizados: 0,
                        tokensRaciocinioUtilizados: 0,
                        tokensTotaisUtilizados: tokensEntradaEstimados,
                        tokensReaisDisponiveis: false,
                        entradaFoiTruncada: entradaFoiTruncada,
                        tempoTotalMs: stopwatch.ElapsedMilliseconds,
                        custoEstimadoUsd: CalcularCustoEstimado(tokensEntradaEstimados, 0, _configuracao),
                        precoEntradaPorMilhaoTokensUsd: _configuracao.InputPricePerMillionTokens,
                        precoSaidaPorMilhaoTokensUsd: _configuracao.OutputPricePerMillionTokens);
                }
                catch (JsonException ex)
                {
                    stopwatch.Stop();

                    _logger.LogError(
                        ex,
                        "Erro ao interpretar resposta do provedor {Provedor}. Modelo={Modelo}. Tentativa={Tentativa}. TempoTotalMs={TempoTotalMs}.",
                        NomeProvedor,
                        modelo,
                        tentativa,
                        stopwatch.ElapsedMilliseconds);

                    return CriarFalha(
                        mensagemTecnica: $"Não foi possível interpretar a resposta da OpenAI: {ex.Message}",
                        mensagemAmigavel: "A resposta da IA veio em um formato inesperado.",
                        origemErro: OrigemErroResposta,
                        categoriaErro: CategoriaErroIA.RespostaInvalida,
                        modelo: modelo,
                        tentativasRealizadas: tentativa,
                        caracteresEntrada: caracteresOriginais,
                        tokensEntradaEstimados: tokensEntradaEstimados,
                        tokensEntradaUtilizados: tokensEntradaEstimados,
                        tokensSaidaUtilizados: 0,
                        tokensRaciocinioUtilizados: 0,
                        tokensTotaisUtilizados: tokensEntradaEstimados,
                        tokensReaisDisponiveis: false,
                        entradaFoiTruncada: entradaFoiTruncada,
                        tempoTotalMs: stopwatch.ElapsedMilliseconds,
                        custoEstimadoUsd: CalcularCustoEstimado(tokensEntradaEstimados, 0, _configuracao),
                        precoEntradaPorMilhaoTokensUsd: _configuracao.InputPricePerMillionTokens,
                        precoSaidaPorMilhaoTokensUsd: _configuracao.OutputPricePerMillionTokens);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();

                    _logger.LogError(
                        ex,
                        "Erro inesperado ao chamar {Provedor}. Modelo={Modelo}. Tentativa={Tentativa}. TempoTotalMs={TempoTotalMs}.",
                        NomeProvedor,
                        modelo,
                        tentativa,
                        stopwatch.ElapsedMilliseconds);

                    return CriarFalha(
                        mensagemTecnica: $"Erro inesperado ao chamar OpenAI: {ex.Message}",
                        mensagemAmigavel: "A integração com IA encontrou um erro inesperado.",
                        origemErro: OrigemErroTransiente,
                        categoriaErro: CategoriaErroIA.Desconhecido,
                        modelo: modelo,
                        tentativasRealizadas: tentativa,
                        caracteresEntrada: caracteresOriginais,
                        tokensEntradaEstimados: tokensEntradaEstimados,
                        tokensEntradaUtilizados: tokensEntradaEstimados,
                        tokensSaidaUtilizados: 0,
                        tokensRaciocinioUtilizados: 0,
                        tokensTotaisUtilizados: tokensEntradaEstimados,
                        tokensReaisDisponiveis: false,
                        entradaFoiTruncada: entradaFoiTruncada,
                        tempoTotalMs: stopwatch.ElapsedMilliseconds,
                        custoEstimadoUsd: CalcularCustoEstimado(tokensEntradaEstimados, 0, _configuracao),
                        precoEntradaPorMilhaoTokensUsd: _configuracao.InputPricePerMillionTokens,
                        precoSaidaPorMilhaoTokensUsd: _configuracao.OutputPricePerMillionTokens);
                }
            }

            return CriarFalha(
                mensagemTecnica: "Fluxo de retry encerrado sem resposta utilizável.",
                mensagemAmigavel: "A integração com IA não conseguiu concluir a análise.",
                origemErro: OrigemErroTransiente,
                categoriaErro: CategoriaErroIA.Transiente,
                modelo: modelo,
                tentativasRealizadas: tentativasMaximas,
                caracteresEntrada: caracteresOriginais,
                tokensEntradaEstimados: tokensEntradaEstimados,
                tokensEntradaUtilizados: tokensEntradaEstimados,
                tokensSaidaUtilizados: 0,
                tokensRaciocinioUtilizados: 0,
                tokensTotaisUtilizados: tokensEntradaEstimados,
                tokensReaisDisponiveis: false,
                entradaFoiTruncada: entradaFoiTruncada,
                tempoTotalMs: 0,
                custoEstimadoUsd: CalcularCustoEstimado(tokensEntradaEstimados, 0, _configuracao),
                precoEntradaPorMilhaoTokensUsd: _configuracao.InputPricePerMillionTokens,
                precoSaidaPorMilhaoTokensUsd: _configuracao.OutputPricePerMillionTokens);
        }

        private HttpRequestMessage CriarRequestMessage(string modelo, string promptCompleto)
        {
            var payload = new
            {
                model = modelo,
                input = promptCompleto,
                max_output_tokens = _configuracao.MaxTokens
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "responses");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _configuracao.ApiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(
                JsonSerializer.Serialize(payload, JsonOptions),
                Encoding.UTF8,
                "application/json");

            return request;
        }

        private static bool DeveTentarNovamente(HttpStatusCode statusCode)
        {
            return statusCode is HttpStatusCode.RequestTimeout
                or HttpStatusCode.TooManyRequests
                or HttpStatusCode.InternalServerError
                or HttpStatusCode.BadGateway
                or HttpStatusCode.ServiceUnavailable
                or HttpStatusCode.GatewayTimeout;
        }

        private static string TruncarSeNecessario(string texto, int maximoCaracteres, out bool truncado)
        {
            truncado = false;

            if (maximoCaracteres <= 0 || texto.Length <= maximoCaracteres)
            {
                return texto;
            }

            truncado = true;
            return texto[..maximoCaracteres];
        }

        private static int EstimarTokens(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return 0;
            }

            return (int)Math.Ceiling(texto.Length / 4.0);
        }

        private static string ExtrairTextoResposta(JsonElement root)
        {
            if (root.TryGetProperty("output_text", out var outputText) &&
                outputText.ValueKind == JsonValueKind.String)
            {
                return outputText.GetString() ?? string.Empty;
            }

            if (!root.TryGetProperty("output", out var output) || output.ValueKind != JsonValueKind.Array)
            {
                return string.Empty;
            }

            var textos = new List<string>();

            foreach (var item in output.EnumerateArray())
            {
                if (!item.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (var contentItem in content.EnumerateArray())
                {
                    if (contentItem.TryGetProperty("text", out var textProperty) &&
                        textProperty.ValueKind == JsonValueKind.String)
                    {
                        var texto = textProperty.GetString();

                        if (!string.IsNullOrWhiteSpace(texto))
                        {
                            textos.Add(texto);
                        }
                    }
                }
            }

            return string.Join(Environment.NewLine + Environment.NewLine, textos);
        }

        private static MetricasUsoResposta ExtrairMetricasUso(JsonElement root, int tokensEntradaEstimados, string textoResposta)
        {
            var tokensSaidaEstimados = EstimarTokens(textoResposta);

            if (!root.TryGetProperty("usage", out var usage) || usage.ValueKind != JsonValueKind.Object)
            {
                return new MetricasUsoResposta(
                    tokensEntradaEstimados,
                    tokensSaidaEstimados,
                    0,
                    tokensEntradaEstimados + tokensSaidaEstimados,
                    false);
            }

            var tokensEntrada = LerInt(usage, "input_tokens");
            var tokensSaida = LerInt(usage, "output_tokens");
            var tokensTotais = LerInt(usage, "total_tokens");

            var tokensRaciocinio = 0;

            if (usage.TryGetProperty("output_tokens_details", out var outputTokensDetails) &&
                outputTokensDetails.ValueKind == JsonValueKind.Object)
            {
                tokensRaciocinio = LerInt(outputTokensDetails, "reasoning_tokens") ?? 0;
            }

            var entradaUtilizada = tokensEntrada ?? tokensEntradaEstimados;
            var saidaUtilizada = tokensSaida ?? tokensSaidaEstimados;
            var totalUtilizado = tokensTotais ?? (entradaUtilizada + saidaUtilizada);
            var tokensReaisDisponiveis = tokensEntrada.HasValue || tokensSaida.HasValue || tokensTotais.HasValue;

            return new MetricasUsoResposta(
                entradaUtilizada,
                saidaUtilizada,
                tokensRaciocinio,
                totalUtilizado,
                tokensReaisDisponiveis);
        }

        private static int? LerInt(JsonElement element, string propriedade)
        {
            if (!element.TryGetProperty(propriedade, out var value))
            {
                return null;
            }

            if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var numero))
            {
                return numero;
            }

            return null;
        }

        private static decimal CalcularCustoEstimado(MetricasUsoResposta metricas, ConfiguracaoOpenAI configuracao)
        {
            return CalcularCustoEstimado(
                metricas.TokensEntradaUtilizados,
                metricas.TokensSaidaUtilizados,
                configuracao);
        }

        private static decimal CalcularCustoEstimado(int tokensEntrada, int tokensSaida, ConfiguracaoOpenAI configuracao)
        {
            var custoEntrada = (tokensEntrada / 1_000_000m) * configuracao.InputPricePerMillionTokens;
            var custoSaida = (tokensSaida / 1_000_000m) * configuracao.OutputPricePerMillionTokens;
            return decimal.Round(custoEntrada + custoSaida, 8, MidpointRounding.AwayFromZero);
        }

        private static RespostaIA CriarFalhaPorStatus(
            HttpStatusCode statusCode,
            string corpoResposta,
            string modelo,
            int tentativa,
            int caracteresEntrada,
            int tokensEntradaEstimados,
            bool entradaFoiTruncada,
            long tempoTotalMs,
            ConfiguracaoOpenAI configuracao)
        {
            return statusCode switch
            {
                HttpStatusCode.Unauthorized => CriarFalha(
                    mensagemTecnica: $"OpenAI retornou 401 Unauthorized. Corpo resumido: {ResumirCorpo(corpoResposta)}",
                    mensagemAmigavel: "A chave da integração com IA foi rejeitada pelo provedor.",
                    origemErro: OrigemErroAutenticacao,
                    categoriaErro: CategoriaErroIA.Autenticacao,
                    statusHttpProvedor: (int)statusCode,
                    modelo: modelo,
                    tentativasRealizadas: tentativa,
                    caracteresEntrada: caracteresEntrada,
                    tokensEntradaEstimados: tokensEntradaEstimados,
                    tokensEntradaUtilizados: tokensEntradaEstimados,
                    tokensSaidaUtilizados: 0,
                    tokensRaciocinioUtilizados: 0,
                    tokensTotaisUtilizados: tokensEntradaEstimados,
                    tokensReaisDisponiveis: false,
                    entradaFoiTruncada: entradaFoiTruncada,
                    tempoTotalMs: tempoTotalMs,
                    custoEstimadoUsd: CalcularCustoEstimado(tokensEntradaEstimados, 0, configuracao),
                    precoEntradaPorMilhaoTokensUsd: configuracao.InputPricePerMillionTokens,
                    precoSaidaPorMilhaoTokensUsd: configuracao.OutputPricePerMillionTokens),

                HttpStatusCode.Forbidden => CriarFalha(
                    mensagemTecnica: $"OpenAI retornou 403 Forbidden. Corpo resumido: {ResumirCorpo(corpoResposta)}",
                    mensagemAmigavel: "A integração com IA não possui permissão para executar esta chamada.",
                    origemErro: OrigemErroPermissao,
                    categoriaErro: CategoriaErroIA.Permissao,
                    statusHttpProvedor: (int)statusCode,
                    modelo: modelo,
                    tentativasRealizadas: tentativa,
                    caracteresEntrada: caracteresEntrada,
                    tokensEntradaEstimados: tokensEntradaEstimados,
                    tokensEntradaUtilizados: tokensEntradaEstimados,
                    tokensSaidaUtilizados: 0,
                    tokensRaciocinioUtilizados: 0,
                    tokensTotaisUtilizados: tokensEntradaEstimados,
                    tokensReaisDisponiveis: false,
                    entradaFoiTruncada: entradaFoiTruncada,
                    tempoTotalMs: tempoTotalMs,
                    custoEstimadoUsd: CalcularCustoEstimado(tokensEntradaEstimados, 0, configuracao),
                    precoEntradaPorMilhaoTokensUsd: configuracao.InputPricePerMillionTokens,
                    precoSaidaPorMilhaoTokensUsd: configuracao.OutputPricePerMillionTokens),

                HttpStatusCode.TooManyRequests => CriarFalha(
                    mensagemTecnica: $"OpenAI retornou 429 Too Many Requests. Corpo resumido: {ResumirCorpo(corpoResposta)}",
                    mensagemAmigavel: "A IA atingiu um limite temporário de uso. Tente novamente em instantes.",
                    origemErro: OrigemErroLimite,
                    categoriaErro: CategoriaErroIA.Limite,
                    statusHttpProvedor: (int)statusCode,
                    modelo: modelo,
                    tentativasRealizadas: tentativa,
                    caracteresEntrada: caracteresEntrada,
                    tokensEntradaEstimados: tokensEntradaEstimados,
                    tokensEntradaUtilizados: tokensEntradaEstimados,
                    tokensSaidaUtilizados: 0,
                    tokensRaciocinioUtilizados: 0,
                    tokensTotaisUtilizados: tokensEntradaEstimados,
                    tokensReaisDisponiveis: false,
                    entradaFoiTruncada: entradaFoiTruncada,
                    tempoTotalMs: tempoTotalMs,
                    custoEstimadoUsd: CalcularCustoEstimado(tokensEntradaEstimados, 0, configuracao),
                    precoEntradaPorMilhaoTokensUsd: configuracao.InputPricePerMillionTokens,
                    precoSaidaPorMilhaoTokensUsd: configuracao.OutputPricePerMillionTokens),

                _ => CriarFalha(
                    mensagemTecnica: $"OpenAI retornou {(int)statusCode}. Corpo resumido: {ResumirCorpo(corpoResposta)}",
                    mensagemAmigavel: "A integração com IA não conseguiu concluir a solicitação.",
                    origemErro: OrigemErroTransiente,
                    categoriaErro: CategoriaErroIA.Transiente,
                    statusHttpProvedor: (int)statusCode,
                    modelo: modelo,
                    tentativasRealizadas: tentativa,
                    caracteresEntrada: caracteresEntrada,
                    tokensEntradaEstimados: tokensEntradaEstimados,
                    tokensEntradaUtilizados: tokensEntradaEstimados,
                    tokensSaidaUtilizados: 0,
                    tokensRaciocinioUtilizados: 0,
                    tokensTotaisUtilizados: tokensEntradaEstimados,
                    tokensReaisDisponiveis: false,
                    entradaFoiTruncada: entradaFoiTruncada,
                    tempoTotalMs: tempoTotalMs,
                    custoEstimadoUsd: CalcularCustoEstimado(tokensEntradaEstimados, 0, configuracao),
                    precoEntradaPorMilhaoTokensUsd: configuracao.InputPricePerMillionTokens,
                    precoSaidaPorMilhaoTokensUsd: configuracao.OutputPricePerMillionTokens)
            };
        }

        private static RespostaIA CriarFalha(
            string mensagemTecnica,
            string mensagemAmigavel,
            string origemErro,
            CategoriaErroIA categoriaErro,
            int? statusHttpProvedor = null,
            string modelo = "",
            int tentativasRealizadas = 0,
            int caracteresEntrada = 0,
            int tokensEntradaEstimados = 0,
            int tokensEntradaUtilizados = 0,
            int tokensSaidaUtilizados = 0,
            int tokensRaciocinioUtilizados = 0,
            int tokensTotaisUtilizados = 0,
            bool tokensReaisDisponiveis = false,
            bool entradaFoiTruncada = false,
            long tempoTotalMs = 0,
            decimal custoEstimadoUsd = 0,
            decimal precoEntradaPorMilhaoTokensUsd = 0,
            decimal precoSaidaPorMilhaoTokensUsd = 0)
        {
            return new RespostaIA
            {
                Sucesso = false,
                Provedor = NomeProvedor,
                Modelo = modelo,
                Conteudo = string.Empty,
                SugestaoCompromissoFinanceiro = null,
                FoiSimulada = false,
                ObservacaoInfraestrutura = "A integração técnica com a OpenAI foi executada, mas a resposta não pôde ser concluída com sucesso.",
                MensagemTecnica = mensagemTecnica,
                MensagemAmigavel = mensagemAmigavel,
                OrigemErro = origemErro,
                CategoriaErro = categoriaErro,
                StatusHttpProvedor = statusHttpProvedor,
                TentativasRealizadas = tentativasRealizadas,
                CaracteresEntrada = caracteresEntrada,
                TokensEntradaEstimados = tokensEntradaEstimados,
                TokensEntradaUtilizados = tokensEntradaUtilizados,
                TokensSaidaUtilizados = tokensSaidaUtilizados,
                TokensRaciocinioUtilizados = tokensRaciocinioUtilizados,
                TokensTotaisUtilizados = tokensTotaisUtilizados,
                TokensReaisDisponiveis = tokensReaisDisponiveis,
                EntradaFoiTruncada = entradaFoiTruncada,
                TempoTotalMs = tempoTotalMs,
                CustoEstimadoUsd = custoEstimadoUsd,
                PrecoEntradaPorMilhaoTokensUsd = precoEntradaPorMilhaoTokensUsd,
                PrecoSaidaPorMilhaoTokensUsd = precoSaidaPorMilhaoTokensUsd
            };
        }

        private static string ResumirCorpo(string corpoResposta)
        {
            if (string.IsNullOrWhiteSpace(corpoResposta))
            {
                return "(vazio)";
            }

            const int limite = 300;
            return corpoResposta.Length <= limite
                ? corpoResposta
                : corpoResposta[..limite];
        }

        private static string? ExtrairSugestaoCompromissoFinanceiro(string conteudo)
        {
            if (string.IsNullOrWhiteSpace(conteudo))
            {
                return null;
            }

            var regex = new Regex(
                @"(?:^|\n)\s*(?:#{2,3}\s*)?Sugest[aã]o de compromisso\s*:?\s*(?<texto>.+?)(?=\n#{2,3}\s|\n---|\s*$)",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            var match = regex.Match(conteudo);
            if (!match.Success)
            {
                return null;
            }

            var texto = match.Groups["texto"].Value.Trim();
            return string.IsNullOrWhiteSpace(texto) ? null : texto;
        }

        private sealed class MetricasUsoResposta
        {
            public MetricasUsoResposta(
                int tokensEntradaUtilizados,
                int tokensSaidaUtilizados,
                int tokensRaciocinioUtilizados,
                int tokensTotaisUtilizados,
                bool tokensReaisDisponiveis)
            {
                TokensEntradaUtilizados = tokensEntradaUtilizados;
                TokensSaidaUtilizados = tokensSaidaUtilizados;
                TokensRaciocinioUtilizados = tokensRaciocinioUtilizados;
                TokensTotaisUtilizados = tokensTotaisUtilizados;
                TokensReaisDisponiveis = tokensReaisDisponiveis;
            }

            public int TokensEntradaUtilizados { get; }
            public int TokensSaidaUtilizados { get; }
            public int TokensRaciocinioUtilizados { get; }
            public int TokensTotaisUtilizados { get; }
            public bool TokensReaisDisponiveis { get; }
        }
    }
}
