using AutoMapper;
using MinhasFinancas.Application.DTOs.Categoria;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class CategoriaAppService : ICategoriaAppService
    {
        private readonly IMapper _mapper;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUsuarioAppService _usuarioAppService;

        public CategoriaAppService(IMapper mapper, ICategoriaRepository categoriaRepository, IUsuarioAppService usuarioAppService)
        {
            _mapper = mapper;
            _categoriaRepository = categoriaRepository;
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

                var lista = await _categoriaRepository.BuscarTodosOsElementosAsync(id);

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

                var categoria = await _categoriaRepository.BuscarUmElementoAsync(usuarioId, BancoId);

                retorno.Sucesso = categoria != null ? true : false;
                retorno.HttpStatusCode = categoria != null ? HttpStatusCode.OK : HttpStatusCode.NotFound;
                retorno.MensagemSistema = categoria != null ? "Categoria Encontrada" : "Categoria não Encontrada";
                retorno.MensagemUsuario = categoria != null ? "Categoria Encontrada" : "Categoria não Encontrada";
                retorno.Dados = categoria;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel encontrar a categoria";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> CadastrarElementoAsync(CadastrarCategoria elementoDTO)
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

                var categoria = _mapper.Map<Categoria>(elementoDTO);

                await _categoriaRepository.CadastrarElementoAsync(categoria);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Categoria cadastrada com sucesso";
                retorno.MensagemUsuario = "Categoria cadastrada";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel criar a Categoria";
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

                await _categoriaRepository.DeletarElementoAsync(buscaPorCartao.Dados);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "categoria deletada com sucesso";
                retorno.MensagemUsuario = "categoria Deletada";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel Deletada a categoria";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> EditarElementoAsync(string idPatrono, Guid elementoId, EditarCategoria elementoDTO)
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

                var categoria = _mapper.Map<Categoria>(elementoDTO);
                categoria.Id = elementoId;
                categoria.UsuarioId = idPatrono;

                await _categoriaRepository.EditarElementoAsync(categoria);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "categoria Editada com sucesso";
                retorno.MensagemUsuario = "categoria Editada";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel Editar a categoria";
                retorno.Dados = null;
                return retorno;
            }
        }
    }
}
