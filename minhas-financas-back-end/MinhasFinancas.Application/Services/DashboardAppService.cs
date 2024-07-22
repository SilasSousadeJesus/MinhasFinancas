using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Services.DashBoard;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class DashboardAppService : IDashboardAppService
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly ILancamentoAppService _lancamentoAppService;


        public DashboardAppService(IUsuarioAppService usuarioAppService, ILancamentoAppService lancamentoAppService)
        {
            _usuarioAppService = usuarioAppService;
            _lancamentoAppService = lancamentoAppService;   
        }

        public async Task<RetornoGenerico> BuscarInformacoesDashboard(string usuarioId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorusuario = await _usuarioAppService.BuscarUmUsuario(usuarioId);

                if (!buscaPorusuario.Sucesso)
                {
                    retorno.Sucesso = buscaPorusuario.Sucesso;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = buscaPorusuario.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorusuario.MensagemUsuario;
                    retorno.Dados = null;
                    return retorno;
                }

                var listaLancamentos = await _lancamentoAppService.BuscarTodosOsElementosAsync(usuarioId);

                if (!listaLancamentos.Sucesso) {

                    retorno.Sucesso = listaLancamentos.Sucesso;
                    retorno.HttpStatusCode = listaLancamentos.HttpStatusCode;
                    retorno.MensagemSistema = listaLancamentos.MensagemSistema;
                    retorno.MensagemUsuario = listaLancamentos.MensagemUsuario;
                    retorno.Dados = null;
                }

                var dashboard = new Dashboard(listaLancamentos.Dados);

                retorno.Sucesso = dashboard != null ? true : false;
                retorno.HttpStatusCode = dashboard != null ? HttpStatusCode.OK : HttpStatusCode.NotFound;
                retorno.MensagemSistema = dashboard != null ? "Dashboard construido com sucesso" : "Informações não encontradas";
                retorno.MensagemUsuario = dashboard != null ? "Dashboard construido com sucesso" : "Informações não encontradas";
                retorno.Dados = dashboard;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel encontrar o banco";
                retorno.Dados = null;
                return retorno;
            }
        }
   
    }
}
