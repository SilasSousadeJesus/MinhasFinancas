using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Jobs
{
    public class MfScoreJobs : IMfScoreJobs
    {
        private readonly IMfScoreCalculoAppService _mfScoreCalculoAppService;
        private readonly ILogger<MfScoreJobs> _logger;

        public MfScoreJobs(IMfScoreCalculoAppService mfScoreCalculoAppService, ILogger<MfScoreJobs> logger)
        {
            _mfScoreCalculoAppService = mfScoreCalculoAppService;
            _logger = logger;
        }

        public async Task GerarHistoricoMensalAsync()
        {
            _logger.LogInformation("Iniciando geração mensal do histórico do MF Score.");

            var retorno = await _mfScoreCalculoAppService.GerarHistoricoMensalAsync();

            if (!retorno.Sucesso)
            {
                _logger.LogError("Falha ao gerar histórico mensal do MF Score: {Mensagem}", retorno.MensagemSistema);
                return;
            }

            _logger.LogInformation("Histórico mensal do MF Score concluído: {Mensagem}", retorno.MensagemSistema);
        }
    }
}
