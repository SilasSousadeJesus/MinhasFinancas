using System.Net;
using MinhasFinancas.Application.DTOs.AnaliseFinanceiraHistorica;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.IA;
using MinhasFinancas.Infra.Data.Interfaces;
using MinhasFinancas.Infra.IA.Interpretadores;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Application.Services
{
    public class AssistenteFinanceiroAppService : IAssistenteFinanceiroAppService
    {
        private readonly IInteligenciaFinanceiraAppService _inteligenciaFinanceiraAppService;
        private readonly AssistenteFinanceiroService _assistenteFinanceiroService;
        private readonly IAnaliseFinanceiraHistoricaAppService _analiseFinanceiraHistoricaAppService;
        private readonly IPlanoEstrategicoFinanceiroRepository _planoEstrategicoFinanceiroRepository;
        private readonly InterpretadorDecisaoFinanceira _interpretadorDecisaoFinanceira;
        private readonly InterpretadorEstrategico _interpretadorEstrategico;

        public AssistenteFinanceiroAppService(
            IInteligenciaFinanceiraAppService inteligenciaFinanceiraAppService,
            AssistenteFinanceiroService assistenteFinanceiroService,
            IAnaliseFinanceiraHistoricaAppService analiseFinanceiraHistoricaAppService,
            IPlanoEstrategicoFinanceiroRepository planoEstrategicoFinanceiroRepository,
            InterpretadorDecisaoFinanceira interpretadorDecisaoFinanceira,
            InterpretadorEstrategico interpretadorEstrategico)
        {
            _inteligenciaFinanceiraAppService = inteligenciaFinanceiraAppService;
            _assistenteFinanceiroService = assistenteFinanceiroService;
            _analiseFinanceiraHistoricaAppService = analiseFinanceiraHistoricaAppService;
            _planoEstrategicoFinanceiroRepository = planoEstrategicoFinanceiroRepository;
            _interpretadorDecisaoFinanceira = interpretadorDecisaoFinanceira;
            _interpretadorEstrategico = interpretadorEstrategico;
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
                        "ResumoFinanceiroIA não encontrado para o usuário informado.",
                        "Não foi possível preparar a análise financeira.",
                        HttpStatusCode.NotFound,
                        null);
                }

                var memoriaFinanceira = (await _analiseFinanceiraHistoricaAppService.BuscarUltimasAnalisesResumidasAsync(usuarioId, 4))
                    .Select(item => new MemoriaFinanceiraResumidaIA
                    {
                        DataGeracao = item.DataGeracao,
                        PeriodoReferencia = item.PeriodoReferencia,
                        PontuacaoSaudeFinanceira = item.PontuacaoSaudeFinanceira,
                        ClassificacaoSaudeFinanceira = item.ClassificacaoSaudeFinanceira,
                        ResumoExecutivoSistema = item.ResumoExecutivoSistema,
                        PrincipaisRiscos = item.PrincipaisRiscos,
                        PrincipaisPontosPositivos = item.PrincipaisPontosPositivos,
                        PrincipaisRecomendacoes = item.PrincipaisRecomendacoes,
                        Prioridades = item.Prioridades
                    })
                    .ToList();

                var planoEstrategicoFinanceiro = await _planoEstrategicoFinanceiroRepository.BuscarVigenteAsync(usuarioId);
                var interpretacaoPlanoEstrategico = _interpretadorEstrategico.Interpretar(planoEstrategicoFinanceiro);
                var decisaoFinanceira = _interpretadorDecisaoFinanceira.Interpretar(perguntaUsuario);

                var contexto = _assistenteFinanceiroService.PrepararContexto(
                    resumo,
                    perguntaUsuario,
                    memoriaFinanceira,
                    decisaoFinanceira,
                    planoEstrategicoFinanceiro,
                    interpretacaoPlanoEstrategico);

                var requisicao = _assistenteFinanceiroService.PrepararRequisicao(
                    resumo,
                    perguntaUsuario,
                    memoriaFinanceira,
                    decisaoFinanceira,
                    planoEstrategicoFinanceiro,
                    interpretacaoPlanoEstrategico);
                var resposta = await _assistenteFinanceiroService.GerarRespostaAsync(requisicao, cancellationToken);

                try
                {
                    var analiseHistoricaId = await _analiseFinanceiraHistoricaAppService.RegistrarAsync(
                        new RegistrarAnaliseFinanceiraHistoricaDTO
                        {
                            UsuarioId = usuarioId,
                            ResumoFinanceiroIA = resumo,
                            ContextoAssistenteFinanceiro = contexto,
                            RequisicaoIA = requisicao,
                            RespostaIA = resposta
                        });

                    resposta.AnaliseFinanceiraHistoricaId = analiseHistoricaId;
                }
                catch
                {
                    resposta.AnaliseFinanceiraHistoricaId = null;
                }

                if (!resposta.Sucesso)
                {
                    return new RetornoGenerico(
                        false,
                        resposta.MensagemTecnica,
                        resposta.MensagemAmigavel,
                        MapearStatusHttp(resposta.CategoriaErro, resposta.StatusHttpProvedor),
                        resposta);
                }

                return new RetornoGenerico(
                    true,
                    "Análise técnica de IA gerada com sucesso.",
                    "Análise técnica gerada com sucesso.",
                    HttpStatusCode.OK,
                    resposta);
            }
            catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                return new RetornoGenerico(
                    false,
                    $"Tempo limite excedido ao gerar análise com IA: {ex.Message}",
                    "A IA demorou mais do que o esperado para responder.",
                    HttpStatusCode.ServiceUnavailable,
                    null);
            }
            catch (Exception ex)
            {
                return new RetornoGenerico(
                    false,
                    ex.ToString(),
                    "Não foi possível gerar a análise com IA.",
                    HttpStatusCode.InternalServerError,
                    null);
            }
        }

        private static HttpStatusCode MapearStatusHttp(CategoriaErroIA categoriaErro, int? statusHttpProvedor)
        {
            return categoriaErro switch
            {
                CategoriaErroIA.Configuracao => HttpStatusCode.InternalServerError,
                CategoriaErroIA.Autenticacao => HttpStatusCode.BadGateway,
                CategoriaErroIA.Permissao => HttpStatusCode.BadGateway,
                CategoriaErroIA.Timeout => HttpStatusCode.ServiceUnavailable,
                CategoriaErroIA.Limite => HttpStatusCode.ServiceUnavailable,
                CategoriaErroIA.Transiente => statusHttpProvedor is 500 or 502 or 503 or 504
                    ? HttpStatusCode.ServiceUnavailable
                    : HttpStatusCode.BadGateway,
                CategoriaErroIA.RespostaInvalida => HttpStatusCode.BadGateway,
                _ => statusHttpProvedor.HasValue
                    ? HttpStatusCode.BadGateway
                    : HttpStatusCode.InternalServerError
            };
        }
    }
}
