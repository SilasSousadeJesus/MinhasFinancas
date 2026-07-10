using MinhasFinancas.Application.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class SaudeFinanceiraAppService : ISaudeFinanceiraAppService
    {
        private readonly IMfScoreCalculoAppService _mfScoreCalculoAppService;

        public SaudeFinanceiraAppService(IMfScoreCalculoAppService mfScoreCalculoAppService)
        {
            _mfScoreCalculoAppService = mfScoreCalculoAppService;
        }

        public async Task<RetornoGenerico> BuscarSaudeFinanceira(string usuarioId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var calculo = await _mfScoreCalculoAppService.CalcularAsync(usuarioId);

                if (calculo is null)
                {
                    retorno.Sucesso = false;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = "Usuário não encontrado para saúde financeira.";
                    retorno.MensagemUsuario = "Não foi possível carregar a saúde financeira.";
                    retorno.Dados = null;
                    return retorno;
                }

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Saúde financeira carregada com sucesso.";
                retorno.MensagemUsuario = "Saúde financeira carregada com sucesso.";
                retorno.Dados = calculo.PainelSaude;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = ex.ToString();
                retorno.MensagemUsuario = "Não foi possível carregar a saúde financeira.";
                retorno.Dados = null;
                return retorno;
            }
        }
    }
}
