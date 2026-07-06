using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Services.DashBoard;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class DashboardAppService : IDashboardAppService
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly IAnaliseFinanceiraAppService _analiseFinanceiraAppService;

        public DashboardAppService(
            IUsuarioAppService usuarioAppService,
            ILancamentoRepository lancamentoRepository,
            IAnaliseFinanceiraAppService analiseFinanceiraAppService)
        {
            _usuarioAppService = usuarioAppService;
            _lancamentoRepository = lancamentoRepository;
            _analiseFinanceiraAppService = analiseFinanceiraAppService;
        }

        public async Task<RetornoGenerico> BuscarInformacoesDashboard(string usuarioId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorUsuario = await _usuarioAppService.BuscarUmUsuario(usuarioId);

                if (!buscaPorUsuario.Sucesso)
                {
                    retorno.Sucesso = buscaPorUsuario.Sucesso;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = buscaPorUsuario.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorUsuario.MensagemUsuario;
                    retorno.Dados = null;
                    return retorno;
                }

                var listaLancamentos = await _lancamentoRepository.BuscarTodosOsElementosAsync(usuarioId);
                var indicadoresFinanceiros = await _analiseFinanceiraAppService.BuscarPainelIndicadoresInternoAsync(usuarioId);

                var dashboard = new Dashboard(listaLancamentos, indicadoresFinanceiros);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Dashboard construido com sucesso";
                retorno.MensagemUsuario = "Dashboard construido com sucesso";
                retorno.Dados = dashboard;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel encontrar o dashboard";
                retorno.Dados = null;
                return retorno;
            }
        }
    }
}
