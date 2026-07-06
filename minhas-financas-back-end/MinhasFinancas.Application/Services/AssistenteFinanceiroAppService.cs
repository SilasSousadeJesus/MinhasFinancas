using System.Net;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Infra.IA;

namespace MinhasFinancas.Application.Services
{
    public class AssistenteFinanceiroAppService : IAssistenteFinanceiroAppService
    {
        private readonly IInteligenciaFinanceiraAppService _inteligenciaFinanceiraAppService;
        private readonly AssistenteFinanceiroService _assistenteFinanceiroService;

        public AssistenteFinanceiroAppService(
            IInteligenciaFinanceiraAppService inteligenciaFinanceiraAppService,
            AssistenteFinanceiroService assistenteFinanceiroService)
        {
            _inteligenciaFinanceiraAppService = inteligenciaFinanceiraAppService;
            _assistenteFinanceiroService = assistenteFinanceiroService;
        }

        public async Task<RetornoGenerico> GerarAnaliseAsync(
            string usuarioId,
            string? perguntaUsuario,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var resumo = await _inteligenciaFinanceiraAppService.BuscarResumoFinanceiroIAInternoAsync(usuarioId);

                if (resumo is null)
                {
                    return new RetornoGenerico(
                        false,
                        "ResumoFinanceiroIA nÃ£o encontrado para o usuÃ¡rio informado.",
                        "NÃ£o foi possÃ­vel preparar a anÃ¡lise financeira.",
                        HttpStatusCode.NotFound,
                        null);
                }

                var resposta = await _assistenteFinanceiroService.GerarRespostaAsync(
                    resumo,
                    perguntaUsuario,
                    cancellationToken);

                if (!resposta.Sucesso)
                {
                    return new RetornoGenerico(
                        false,
                        resposta.MensagemTecnica,
                        resposta.MensagemAmigavel,
                        MapearStatusHttp(resposta.OrigemErro, resposta.StatusHttpProvedor),
                        resposta);
                }

                return new RetornoGenerico(
                    true,
                    "AnÃ¡lise tÃ©cnica de IA gerada com sucesso.",
                    "AnÃ¡lise tÃ©cnica gerada com sucesso.",
                    HttpStatusCode.OK,
                    resposta);
            }
            catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                return new RetornoGenerico(
                    false,
                    $"Tempo limite excedido ao gerar anÃ¡lise com IA: {ex.Message}",
                    "A IA demorou mais do que o esperado para responder.",
                    HttpStatusCode.ServiceUnavailable,
                    null);
            }
            catch (Exception ex)
            {
                return new RetornoGenerico(
                    false,
                    ex.ToString(),
                    "NÃ£o foi possÃ­vel gerar a anÃ¡lise com IA.",
                    HttpStatusCode.InternalServerError,
                    null);
            }
        }

        private static HttpStatusCode MapearStatusHttp(string origemErro, int? statusHttpProvedor)
        {
            return origemErro switch
            {
                "Configuracao.OpenAI" => HttpStatusCode.InternalServerError,
                "OpenAI.Autenticacao" => HttpStatusCode.BadGateway,
                "OpenAI.Permissao" => HttpStatusCode.BadGateway,
                "OpenAI.Timeout" => HttpStatusCode.ServiceUnavailable,
                "OpenAI.Limite" => HttpStatusCode.ServiceUnavailable,
                "OpenAI.Transiente" => statusHttpProvedor is 500 or 502 or 503 or 504
                    ? HttpStatusCode.ServiceUnavailable
                    : HttpStatusCode.BadGateway,
                _ => statusHttpProvedor.HasValue
                    ? HttpStatusCode.BadGateway
                    : HttpStatusCode.InternalServerError
            };
        }
    }
}
