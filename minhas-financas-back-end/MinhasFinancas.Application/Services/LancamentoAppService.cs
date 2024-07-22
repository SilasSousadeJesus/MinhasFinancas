using AutoMapper;
using MinhasFinancas.Application.DTOs.Lancamento;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Entities;
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
                retorno.MensagemUsuario = "Não foi possivel buscar a lista de lançamentos";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> BuscarUmElementoAsync(string usuarioId, Guid lancamentoId)
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

                var lancamento = await _lancamentoRepository.BuscarUmElementoAsync(usuarioId, lancamentoId);

                retorno.Sucesso = lancamento != null ? true : false;
                retorno.HttpStatusCode = lancamento != null ? HttpStatusCode.OK : HttpStatusCode.NotFound;
                retorno.MensagemSistema = lancamento != null ? "lancamento Encontrado" : "lancamento não Encontrado";
                retorno.MensagemUsuario = lancamento != null ? "lancamento Encontrado" : "lancamento não Encontrado";
                retorno.Dados = lancamento;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel encontrar a lancamento";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> CadastrarElementoAsync(CadastrarLancamento elementoDTO)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorUsuario = await _usuarioAppService.BuscarUmUsuario(elementoDTO.UsuarioId);

                if (!buscaPorUsuario.Sucesso)
                {
                    retorno.Sucesso = buscaPorUsuario.Sucesso;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = buscaPorUsuario.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorUsuario.MensagemUsuario;
                    retorno.Dados = null;

                    return retorno;
                }

                var categoria = _mapper.Map<Lancamento>(elementoDTO);

                await _lancamentoRepository.CadastrarElementoAsync(categoria);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Lancamento cadastrada com sucesso";
                retorno.MensagemUsuario = "Lancamento cadastrada";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel criar o Lancamento";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> DeletarElementoAsync(string idPatrono, Guid idElemento)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorLancamento = await BuscarUmElementoAsync(idPatrono, idElemento);

                if (!buscaPorLancamento.Sucesso)
                {
                    retorno.Sucesso = buscaPorLancamento.Sucesso;
                    retorno.HttpStatusCode = buscaPorLancamento.HttpStatusCode;
                    retorno.MensagemSistema = buscaPorLancamento.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorLancamento.MensagemUsuario;
                    retorno.Dados = null;

                    return retorno;
                }

                await _lancamentoRepository.DeletarElementoAsync(buscaPorLancamento.Dados);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Lancamento deletada com sucesso";
                retorno.MensagemUsuario = "Lancamento Deletada";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel deletar o Lancamento";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> EditarElementoAsync(string idPatrono, Guid elementoId, EditarLancamento elementoDTO)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorBanco = await BuscarUmElementoAsync(idPatrono, elementoId);

                if (!buscaPorBanco.Sucesso)
                {
                    retorno.Sucesso = buscaPorBanco.Sucesso;
                    retorno.HttpStatusCode = buscaPorBanco.HttpStatusCode;
                    retorno.MensagemSistema = buscaPorBanco.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorBanco.MensagemUsuario;
                    retorno.Dados = null;

                    return retorno;
                }

                var categoria = _mapper.Map<Lancamento>(elementoDTO);
                categoria.Id = elementoId;
                categoria.UsuarioId = idPatrono;

                await _lancamentoRepository.EditarElementoAsync(categoria);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Lançamento Editado com sucesso";
                retorno.MensagemUsuario = "Lançamento Editado";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel editar o Lançamento";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> BuscarLancamentosPorCategoriaAsync(string usuarioId)
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

                var lista = await _lancamentoRepository.BuscarLancamentosPorCategoriaAsync(usuarioId);

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
                retorno.MensagemUsuario = "Não foi possivel buscar a lista de elementos";
                retorno.Dados = null;
                return retorno;
            }
        }

    }
}
