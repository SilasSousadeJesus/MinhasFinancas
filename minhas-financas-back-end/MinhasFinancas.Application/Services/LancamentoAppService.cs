using AutoMapper;
using MinhasFinancas.Application.DTOs.Lancamento;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Services.DashBoard;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class LancamentoAppService : ILancamentoAppService
    {

        private readonly IMapper _mapper;
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly IUsuarioAppService _usuarioAppService;
        public LancamentoAppService(IMapper mapper, ILancamentoRepository lancamentoRepository, IUsuarioAppService usuarioAppService)
        {
            _mapper = mapper;
            _lancamentoRepository = lancamentoRepository;
            _usuarioAppService = usuarioAppService;
        }

        public async Task<RetornoGenerico> BuscarTodosOsElementosAsync(string id)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorusuario = await _usuarioAppService.BuscarUmUsuario(id);

                if (!buscaPorusuario.Sucesso)
                {
                    retorno.Sucesso = buscaPorusuario.Sucesso;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = buscaPorusuario.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorusuario.MensagemUsuario;
                    retorno.Dados = null;
                    return retorno;
                }

                var lista = await _lancamentoRepository.BuscarTodosOsElementosAsync(id);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = $"{lista.Count} elemento(s) encontrado(s)";
                retorno.MensagemUsuario = $"{lista.Count} elemento(s)  encontrado(s)";
                retorno.Dados = lista;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel buscar a lista de cartões";
                retorno.Dados = null;
                return retorno;
            }
        }

        public Task<RetornoGenerico> BuscarUmElementoAsync(string usuarioId, Guid BancoId)
        {
            throw new NotImplementedException();
        }

        public Task<RetornoGenerico> CadastrarElementoAsync(CadastrarLancamento elementoDTO)
        {
            throw new NotImplementedException();
        }

        public Task<RetornoGenerico> DeletarElementoAsync(string idPatrono, Guid idElemento)
        {
            throw new NotImplementedException();
        }

        public Task<RetornoGenerico> EditarElementoAsync(string idPatrono, Guid elementoId, EditarLancamento elementoDTO)
        {
            throw new NotImplementedException();
        }
    }
}
