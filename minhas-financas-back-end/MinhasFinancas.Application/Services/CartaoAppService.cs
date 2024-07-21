using AutoMapper;
using MinhasFinancas.Application.DTOs.Cartao;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class CartaoAppService : ICartaoAppService
    {

        private readonly IMapper _mapper;
        private readonly ICartaoRepository _cartaoRepository;
        private readonly IUsuarioAppService _usuarioAppService;
        public CartaoAppService(IMapper mapper, ICartaoRepository cartaoRepository, IUsuarioAppService usuarioAppService)
        {
            _mapper = mapper;
            _cartaoRepository = cartaoRepository;
            _usuarioAppService = usuarioAppService;
        }
        public async Task<RetornoGenerico> BuscarTodosOsElementosAsync(string Id)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorusuario = await _usuarioAppService.BuscarUmUsuario(Id);

                if (!buscaPorusuario.Sucesso)
                {
                    retorno.Sucesso = buscaPorusuario.Sucesso;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = buscaPorusuario.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorusuario.MensagemUsuario;
                    retorno.Dados = null;
                    return retorno;
                }

                var lista = await _cartaoRepository.BuscarTodosOsElementosAsync(Id);

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

        public async Task<RetornoGenerico> BuscarUmElementoAsync(string usuarioId, Guid BancoId)
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

                var cartao = await _cartaoRepository.BuscarUmElementoAsync(usuarioId, BancoId);

                retorno.Sucesso = cartao != null ? true : false;
                retorno.HttpStatusCode = cartao != null ? HttpStatusCode.OK : HttpStatusCode.NotFound;
                retorno.MensagemSistema = cartao != null ? "Cartão Encontrado" : "Cartão não encontrado";
                retorno.MensagemUsuario = cartao != null ? "Cartão Encontrado" : "Cartão não encontrado";
                retorno.Dados = cartao;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel encontrar o Cartão";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> CadastrarElementoAsync(CadastrarCartaoDTO elementoDTO)
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

                var cartao = _mapper.Map<Cartao>(elementoDTO);

                await _cartaoRepository.CadastrarElementoAsync(cartao);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Cartao cadastrado com sucesso";
                retorno.MensagemUsuario = "Cartao cadastrado";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel criar o Cartao";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> DeletarElementoAsync(string idPatrono, Guid idElemento)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorCartao = await BuscarUmElementoAsync(idPatrono, idElemento);

                if (!buscaPorCartao.Sucesso)
                {
                    retorno.Sucesso = buscaPorCartao.Sucesso;
                    retorno.HttpStatusCode = buscaPorCartao.HttpStatusCode;
                    retorno.MensagemSistema = buscaPorCartao.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorCartao.MensagemUsuario;
                    retorno.Dados = null;

                    return retorno;
                }

                await _cartaoRepository.DeletarElementoAsync(buscaPorCartao.Dados);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "cartão Deletado com sucesso";
                retorno.MensagemUsuario = "cartão Deletado";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel Deletado o cartão";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> EditarElementoAsync(string idPatrono, Guid elementoId, EditarCartaoDTO elementoDTO)
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

                var cartao = _mapper.Map<Cartao>(elementoDTO);
                cartao.Id = elementoId;
                cartao.UsuarioId = idPatrono;

                await _cartaoRepository.EditarElementoAsync(cartao);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Cartao Editado com sucesso";
                retorno.MensagemUsuario = "Cartao Editado";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel Editar o Cartao";
                retorno.Dados = null;
                return retorno;
            }
        }
    }
}
