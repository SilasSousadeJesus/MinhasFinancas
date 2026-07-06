using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Services.AnaliseFinanceira;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class InteligenciaFinanceiraAppService : IInteligenciaFinanceiraAppService
    {
        private readonly IAnaliseFinanceiraAppService _analiseFinanceiraAppService;
        private readonly ISaudeFinanceiraService _saudeFinanceiraService;
        private readonly IInsightsFinanceirosService _insightsFinanceirosService;
        private readonly IResumoFinanceiroIAService _resumoFinanceiroIAService;

        public InteligenciaFinanceiraAppService(
            IAnaliseFinanceiraAppService analiseFinanceiraAppService,
            ISaudeFinanceiraService saudeFinanceiraService,
            IInsightsFinanceirosService insightsFinanceirosService,
            IResumoFinanceiroIAService resumoFinanceiroIAService)
        {
            _analiseFinanceiraAppService = analiseFinanceiraAppService;
            _saudeFinanceiraService = saudeFinanceiraService;
            _insightsFinanceirosService = insightsFinanceirosService;
            _resumoFinanceiroIAService = resumoFinanceiroIAService;
        }

        public async Task<RetornoGenerico> BuscarInsightsFinanceiros(string usuarioId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var inteligencia = await MontarInteligenciaAsync(usuarioId);

                if (inteligencia is null)
                {
                    retorno.Sucesso = false;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = "Usuário não encontrado para insights financeiros.";
                    retorno.MensagemUsuario = "Não foi possível carregar os insights financeiros.";
                    retorno.Dados = null;
                    return retorno;
                }

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Insights financeiros carregados com sucesso.";
                retorno.MensagemUsuario = "Insights financeiros carregados com sucesso.";
                retorno.Dados = inteligencia.Value.Insights;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possível carregar os insights financeiros.";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> BuscarResumoFinanceiroIA(string usuarioId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var inteligencia = await MontarInteligenciaAsync(usuarioId);

                if (inteligencia is null)
                {
                    retorno.Sucesso = false;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = "Usuário não encontrado para resumo financeiro IA.";
                    retorno.MensagemUsuario = "Não foi possível carregar o resumo financeiro IA.";
                    retorno.Dados = null;
                    return retorno;
                }

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Resumo financeiro IA carregado com sucesso.";
                retorno.MensagemUsuario = "Resumo financeiro IA carregado com sucesso.";
                retorno.Dados = inteligencia.Value.Resumo;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possível carregar o resumo financeiro IA.";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<ResumoFinanceiroIA?> BuscarResumoFinanceiroIAInternoAsync(string usuarioId)
        {
            var inteligencia = await MontarInteligenciaAsync(usuarioId);
            return inteligencia?.Resumo;
        }

        private async Task<(PainelInsightsFinanceiros Insights, ResumoFinanceiroIA Resumo)?> MontarInteligenciaAsync(string usuarioId)
        {
            var painelIndicadores = await _analiseFinanceiraAppService.BuscarPainelIndicadoresInternoAsync(usuarioId);

            if (painelIndicadores is null)
            {
                return null;
            }

            var painelSaude = _saudeFinanceiraService.GerarPainel(painelIndicadores);
            var painelInsights = _insightsFinanceirosService.GerarPainel(painelSaude);
            var resumo = _resumoFinanceiroIAService.GerarResumo(DateTime.Today, painelSaude, painelInsights);

            return (painelInsights, resumo);
        }
    }
}
