using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Provedores
{
    public class OpenAIProvider : IProvedorIA
    {
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
                    mensagemTecnica: "A chave da OpenAI nÃ£o foi configurada.",
                    mensagemAmigavel: "A integraÃ§Ã£o com IA ainda nÃ£o foi configurada neste ambiente.",
                    origemErro: OrigemErroConfiguracao);
            }

            var promptCompleto = requisicao.PromptCompleto ?? string.Empty;
            var caracteresOriginais = promptCompleto.Length;
            var promptTruncado = TruncarSeNecessario(promptCompleto, _configuracao.MaxInputCharacters, out var entradaFoiTruncada);
            var tokensEntradaEstimados = EstimarTokens(promptTruncado);
            var tentativasMaximas = Math.Max(1, _configuracao.RetryCount + 1);
            var timeout = TimeSpan.FromSeconds(Math.Max(5, _configuracao.TimeoutSeconds));

            var stopwatch = Stopwatch.StartNew();
            string modelo = string.IsNullOrWhiteSpace(requisicao.ModeloSugerido)
                ? _configuracao.Model
                : requisicao.ModeloSugerido;

            _logger.LogInformation(
                "Iniciando chamada OpenAI. Modelo={Modelo}. CaracteresEntrada={CaracteresEntrada}. TokensEstimados={TokensEstimados}. EntradaTruncada={EntradaTruncada}.",
                modelo,
                caracteresOriginais,
                tokensEntradaEstimados,
                entradaFoiTruncada);

            for (var tentativa = 1; tentativa <= tentativasMaximas; tentativa++)
            {
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
                        var texto = ExtrairTextoResposta(conteudoResposta);

                        if (string.IsNullOrWhiteSpace(texto))
                        {
                            _logger.LogWarning(
                                "OpenAI retornou resposta vazia. Modelo={Modelo}. Tentativa={Tentativa}. DuraÃ§Ã£oMs={DuracaoMs}.",
                                modelo,
                                tentativa,
                                stopwatch.ElapsedMilliseconds);

                            return CriarFalha(
                                mensagemTecnica: "A API da OpenAI respondeu sem texto utilizÃ¡vel.",
                                mensagemAmigavel: "A IA nÃ£o retornou um texto vÃ¡lido para esta anÃ¡lise.",
                                origemErro: OrigemErroResposta,
                                statusHttpProvedor: (int)response.StatusCode,
                                tentativasRealizadas: tentativa,
                                caracteresEntrada: caracteresOriginais,
                                tokensEntradaEstimados: tokensEntradaEstimados,
                                entradaFoiTruncada: entradaFoiTruncada);
                        }

                        stopwatch.Stop();

                        _logger.LogInformation(
                            "Chamada OpenAI concluÃ­da com sucesso. Modelo={Modelo}. Tentativa={Tentativa}. DuraÃ§Ã£oMs={DuracaoMs}.",
                            modelo,
                            tentativa,
                            stopwatch.ElapsedMilliseconds);

                        return new RespostaIA
                        {
                            Sucesso = true,
                            Provedor = "OpenAI",
                            Modelo = modelo,
                            Conteudo = texto.Trim(),
                            FoiSimulada = false,
                            ObservacaoInfraestrutura = "Resposta real gerada pela OpenAI a partir do contexto preparado pelo sistema.",
                            MensagemTecnica = "Resposta gerada com sucesso.",
                            MensagemAmigavel = "AnÃ¡lise tÃ©cnica gerada com sucesso.",
                            TentativasRealizadas = tentativa,
                            CaracteresEntrada = caracteresOriginais,
                            TokensEntradaEstimados = tokensEntradaEstimados,
                            EntradaFoiTruncada = entradaFoiTruncada,
                            StatusHttpProvedor = (int)response.StatusCode
                        };
                    }

                    if (DeveTentarNovamente(response.StatusCode) && tentativa < tentativasMaximas)
                    {
                        _logger.LogWarning(
                            "Falha transitÃ³ria ao chamar OpenAI. Modelo={Modelo}. Status={Status}. Tentativa={Tentativa}.",
                            modelo,
                            (int)response.StatusCode,
                            tentativa);

                        await Task.Delay(TimeSpan.FromSeconds(tentativa), cancellationToken);
                        continue;
                    }

                    stopwatch.Stop();

                    _logger.LogWarning(
                        "Chamada OpenAI falhou. Modelo={Modelo}. Status={Status}. Tentativa={Tentativa}. DuraÃ§Ã£oMs={DuracaoMs}.",
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
                        entradaFoiTruncada: entradaFoiTruncada);
                }
                catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
                {
                    if (tentativa < tentativasMaximas)
                    {
                        _logger.LogWarning(
                            ex,
                            "Timeout ao chamar OpenAI. Modelo={Modelo}. Tentativa={Tentativa}. Nova tentativa serÃ¡ executada.",
                            modelo,
                            tentativa);

                        await Task.Delay(TimeSpan.FromSeconds(tentativa), cancellationToken);
                        continue;
                    }

                    stopwatch.Stop();

                    _logger.LogError(
                        ex,
                        "Timeout definitivo ao chamar OpenAI. Modelo={Modelo}. Tentativas={Tentativas}. DuraÃ§Ã£oMs={DuracaoMs}.",
                        modelo,
                        tentativa,
                        stopwatch.ElapsedMilliseconds);

                    return CriarFalha(
                        mensagemTecnica: "A chamada para a OpenAI excedeu o tempo limite configurado.",
                        mensagemAmigavel: "A IA demorou mais do que o esperado para responder. Tente novamente em instantes.",
                        origemErro: OrigemErroTimeout,
                        tentativasRealizadas: tentativa,
                        caracteresEntrada: caracteresOriginais,
                        tokensEntradaEstimados: tokensEntradaEstimados,
                        entradaFoiTruncada: entradaFoiTruncada);
                }
                catch (HttpRequestException ex)
                {
                    if (tentativa < tentativasMaximas)
                    {
                        _logger.LogWarning(
                            ex,
                            "Falha de rede ao chamar OpenAI. Modelo={Modelo}. Tentativa={Tentativa}. Nova tentativa serÃ¡ executada.",
                            modelo,
                            tentativa);

                        await Task.Delay(TimeSpan.FromSeconds(tentativa), cancellationToken);
                        continue;
                    }

                    stopwatch.Stop();

                    _logger.LogError(
                        ex,
                        "Falha de rede definitiva ao chamar OpenAI. Modelo={Modelo}. Tentativas={Tentativas}. DuraÃ§Ã£oMs={DuracaoMs}.",
                        modelo,
                        tentativa,
                        stopwatch.ElapsedMilliseconds);

                    return CriarFalha(
                        mensagemTecnica: $"Falha de rede ao chamar OpenAI: {ex.Message}",
                        mensagemAmigavel: "NÃ£o foi possÃ­vel se comunicar com o provedor de IA neste momento.",
                        origemErro: OrigemErroTransiente,
                        tentativasRealizadas: tentativa,
                        caracteresEntrada: caracteresOriginais,
                        tokensEntradaEstimados: tokensEntradaEstimados,
                        entradaFoiTruncada: entradaFoiTruncada);
                }
                catch (JsonException ex)
                {
                    stopwatch.Stop();

                    _logger.LogError(
                        ex,
                        "Erro ao interpretar resposta da OpenAI. Modelo={Modelo}. Tentativa={Tentativa}. DuraÃ§Ã£oMs={DuracaoMs}.",
                        modelo,
                        tentativa,
                        stopwatch.ElapsedMilliseconds);

                    return CriarFalha(
                        mensagemTecnica: $"NÃ£o foi possÃ­vel interpretar a resposta da OpenAI: {ex.Message}",
                        mensagemAmigavel: "A resposta da IA veio em um formato inesperado.",
                        origemErro: OrigemErroResposta,
                        tentativasRealizadas: tentativa,
                        caracteresEntrada: caracteresOriginais,
                        tokensEntradaEstimados: tokensEntradaEstimados,
                        entradaFoiTruncada: entradaFoiTruncada);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();

                    _logger.LogError(
                        ex,
                        "Erro inesperado ao chamar OpenAI. Modelo={Modelo}. Tentativa={Tentativa}. DuraÃ§Ã£oMs={DuracaoMs}.",
                        modelo,
                        tentativa,
                        stopwatch.ElapsedMilliseconds);

                    return CriarFalha(
                        mensagemTecnica: $"Erro inesperado ao chamar OpenAI: {ex.Message}",
                        mensagemAmigavel: "A integraÃ§Ã£o com IA encontrou um erro inesperado.",
                        origemErro: OrigemErroTransiente,
                        tentativasRealizadas: tentativa,
                        caracteresEntrada: caracteresOriginais,
                        tokensEntradaEstimados: tokensEntradaEstimados,
                        entradaFoiTruncada: entradaFoiTruncada);
                }
            }

            return CriarFalha(
                mensagemTecnica: "Fluxo de retry encerrado sem resposta utilizÃ¡vel.",
                mensagemAmigavel: "A integraÃ§Ã£o com IA nÃ£o conseguiu concluir a anÃ¡lise.",
                origemErro: OrigemErroTransiente,
                tentativasRealizadas: tentativasMaximas,
                caracteresEntrada: caracteresOriginais,
                tokensEntradaEstimados: tokensEntradaEstimados,
                entradaFoiTruncada: entradaFoiTruncada);
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

        private static string ExtrairTextoResposta(string json)
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

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

        private static RespostaIA CriarFalhaPorStatus(
            HttpStatusCode statusCode,
            string corpoResposta,
            string modelo,
            int tentativa,
            int caracteresEntrada,
            int tokensEntradaEstimados,
            bool entradaFoiTruncada)
        {
            return statusCode switch
            {
                HttpStatusCode.Unauthorized => CriarFalha(
                    mensagemTecnica: $"OpenAI retornou 401 Unauthorized. Corpo resumido: {ResumirCorpo(corpoResposta)}",
                    mensagemAmigavel: "A chave da integraÃ§Ã£o com IA foi rejeitada pelo provedor.",
                    origemErro: OrigemErroAutenticacao,
                    statusHttpProvedor: (int)statusCode,
                    modelo: modelo,
                    tentativasRealizadas: tentativa,
                    caracteresEntrada: caracteresEntrada,
                    tokensEntradaEstimados: tokensEntradaEstimados,
                    entradaFoiTruncada: entradaFoiTruncada),

                HttpStatusCode.Forbidden => CriarFalha(
                    mensagemTecnica: $"OpenAI retornou 403 Forbidden. Corpo resumido: {ResumirCorpo(corpoResposta)}",
                    mensagemAmigavel: "A integraÃ§Ã£o com IA nÃ£o possui permissÃ£o para executar esta chamada.",
                    origemErro: OrigemErroPermissao,
                    statusHttpProvedor: (int)statusCode,
                    modelo: modelo,
                    tentativasRealizadas: tentativa,
                    caracteresEntrada: caracteresEntrada,
                    tokensEntradaEstimados: tokensEntradaEstimados,
                    entradaFoiTruncada: entradaFoiTruncada),

                HttpStatusCode.TooManyRequests => CriarFalha(
                    mensagemTecnica: $"OpenAI retornou 429 Too Many Requests. Corpo resumido: {ResumirCorpo(corpoResposta)}",
                    mensagemAmigavel: "A IA atingiu um limite temporÃ¡rio de uso. Tente novamente em instantes.",
                    origemErro: OrigemErroLimite,
                    statusHttpProvedor: (int)statusCode,
                    modelo: modelo,
                    tentativasRealizadas: tentativa,
                    caracteresEntrada: caracteresEntrada,
                    tokensEntradaEstimados: tokensEntradaEstimados,
                    entradaFoiTruncada: entradaFoiTruncada),

                _ => CriarFalha(
                    mensagemTecnica: $"OpenAI retornou {(int)statusCode}. Corpo resumido: {ResumirCorpo(corpoResposta)}",
                    mensagemAmigavel: "A integraÃ§Ã£o com IA nÃ£o conseguiu concluir a solicitaÃ§Ã£o.",
                    origemErro: OrigemErroTransiente,
                    statusHttpProvedor: (int)statusCode,
                    modelo: modelo,
                    tentativasRealizadas: tentativa,
                    caracteresEntrada: caracteresEntrada,
                    tokensEntradaEstimados: tokensEntradaEstimados,
                    entradaFoiTruncada: entradaFoiTruncada)
            };
        }

        private static RespostaIA CriarFalha(
            string mensagemTecnica,
            string mensagemAmigavel,
            string origemErro,
            int? statusHttpProvedor = null,
            string modelo = "",
            int tentativasRealizadas = 0,
            int caracteresEntrada = 0,
            int tokensEntradaEstimados = 0,
            bool entradaFoiTruncada = false)
        {
            return new RespostaIA
            {
                Sucesso = false,
                Provedor = "OpenAI",
                Modelo = modelo,
                Conteudo = string.Empty,
                FoiSimulada = false,
                ObservacaoInfraestrutura = "A integraÃ§Ã£o tÃ©cnica com a OpenAI foi executada, mas a resposta nÃ£o pÃ´de ser concluÃ­da com sucesso.",
                MensagemTecnica = mensagemTecnica,
                MensagemAmigavel = mensagemAmigavel,
                OrigemErro = origemErro,
                StatusHttpProvedor = statusHttpProvedor,
                TentativasRealizadas = tentativasRealizadas,
                CaracteresEntrada = caracteresEntrada,
                TokensEntradaEstimados = tokensEntradaEstimados,
                EntradaFoiTruncada = entradaFoiTruncada
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
    }
}
