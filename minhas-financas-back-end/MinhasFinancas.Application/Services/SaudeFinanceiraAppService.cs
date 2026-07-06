using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Services.AnaliseFinanceira;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class SaudeFinanceiraAppService : ISaudeFinanceiraAppService
    {
        private readonly IAnaliseFinanceiraAppService _analiseFinanceiraAppService;
        private readonly ISaudeFinanceiraService _saudeFinanceiraService;

        public SaudeFinanceiraAppService(
            IAnaliseFinanceiraAppService analiseFinanceiraAppService,
            ISaudeFinanceiraService saudeFinanceiraService)
        {
            _analiseFinanceiraAppService = analiseFinanceiraAppService;
            _saudeFinanceiraService = saudeFinanceiraService;
        }

        public async Task<RetornoGenerico> BuscarSaudeFinanceira(string usuarioId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var painelIndicadores = await _analiseFinanceiraAppService.BuscarPainelIndicadoresInternoAsync(usuarioId);

                if (painelIndicadores is null)
                {
                    retorno.Sucesso = false;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = "Usuário não encontrado para saúde financeira.";
                    retorno.MensagemUsuario = "Não foi possível carregar a saúde financeira.";
                    retorno.Dados = null;
                    return retorno;
                }

                var painelSaude = _saudeFinanceiraService.GerarPainel(painelIndicadores);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Saúde financeira carregada com sucesso.";
                retorno.MensagemUsuario = "Saúde financeira carregada com sucesso.";
                retorno.Dados = painelSaude;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possível carregar a saúde financeira.";
                retorno.Dados = null;
                return retorno;
            }
        }
    }
}
