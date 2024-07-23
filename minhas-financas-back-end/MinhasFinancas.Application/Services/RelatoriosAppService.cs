using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Services.DashBoard;
using MinhasFinancas.Domain.Services.Relatorios;
using MinhasFinancas.Domain.Services.Relatorios.PorCategoria;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class RelatoriosAppService : IRelatoriosAppService
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly ILancamentoAppService _lancamentoAppService;
        private readonly ICategoriaAppService _categoriaAppService;


        public RelatoriosAppService(IUsuarioAppService usuarioAppService, ILancamentoAppService lancamentoAppService, ICategoriaAppService categoriaAppService)
        {
            _usuarioAppService = usuarioAppService;
            _lancamentoAppService = lancamentoAppService;
            _categoriaAppService = categoriaAppService;
        }

        public async Task<RetornoGenerico> RelatoriosPorCategoriaLancamento(string usuarioId)
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

                if (!listaLancamentos.Sucesso)
                {

                    retorno.Sucesso = listaLancamentos.Sucesso;
                    retorno.HttpStatusCode = listaLancamentos.HttpStatusCode;
                    retorno.MensagemSistema = listaLancamentos.MensagemSistema;
                    retorno.MensagemUsuario = listaLancamentos.MensagemUsuario;
                    retorno.Dados = null;
                }

                var listaCategorias= await _categoriaAppService.BuscarTodosOsElementosAsync(usuarioId);

                if (!listaCategorias.Sucesso)
                {

                    retorno.Sucesso = listaCategorias.Sucesso;
                    retorno.HttpStatusCode = listaCategorias.HttpStatusCode;
                    retorno.MensagemSistema = listaCategorias.MensagemSistema;
                    retorno.MensagemUsuario = listaCategorias.MensagemUsuario;
                    retorno.Dados = null;
                }

                var relatorio = new RelatorioPorCategoria(listaLancamentos.Dados, listaCategorias.Dados);

                retorno.Sucesso = relatorio != null ? true : false;
                retorno.HttpStatusCode = relatorio != null ? HttpStatusCode.OK : HttpStatusCode.NotFound;
                retorno.MensagemSistema = relatorio != null ? "Relatorio construido com sucesso" : "Informações não encontradas";
                retorno.MensagemUsuario = relatorio != null ? "Relatorio construido com sucesso" : "Informações não encontradas";
                retorno.Dados = relatorio;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel encontrar as informações";
                retorno.Dados = null;
                return retorno;
            }
        }
    }
}
