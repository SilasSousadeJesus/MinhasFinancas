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

        private static string NormalizarNome(string nome)
        {
            return nome.Trim();
        }


        // categoria
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

        public async Task<RetornoGenerico> CadastrarElementoAsync(CadastrarCategoriaDTO elementoDTO)
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

                elementoDTO.NomeCategoria = NormalizarNome(elementoDTO.NomeCategoria);

                var categoriaDuplicada = await _categoriaRepository.ExisteCategoriaComNomeAsync(
                    elementoDTO.UsuarioId!,
                    elementoDTO.NomeCategoria
                );

                if (categoriaDuplicada)
                {
                    retorno.Sucesso = false;
                    retorno.HttpStatusCode = HttpStatusCode.BadRequest;
                    retorno.MensagemSistema = "Ja existe uma categoria com esse nome para este usuario";
                    retorno.MensagemUsuario = "Voce ja possui uma categoria com esse nome";
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
                retorno.MensagemUsuario = "Não foi possivel deletar a categoria";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> EditarElementoAsync(string idPatrono, Guid elementoId, EditarCategoriaDTO elementoDTO)
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

                elementoDTO.NomeCategoria = NormalizarNome(elementoDTO.NomeCategoria);

                var categoriaDuplicada = await _categoriaRepository.ExisteCategoriaComNomeAsync(
                    idPatrono,
                    elementoDTO.NomeCategoria,
                    elementoId
                );

                if (categoriaDuplicada)
                {
                    retorno.Sucesso = false;
                    retorno.HttpStatusCode = HttpStatusCode.BadRequest;
                    retorno.MensagemSistema = "Ja existe uma categoria com esse nome para este usuario";
                    retorno.MensagemUsuario = "Voce ja possui uma categoria com esse nome";
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

        // subcategoria
        public async Task<RetornoGenerico> BuscarTodosAsSubCategoriaAsync(string usuarioId, Guid categoriaId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var categoria = await BuscarUmElementoAsync(usuarioId, categoriaId);

                if (!categoria.Sucesso)
                {
                    retorno.Sucesso = categoria.Sucesso;
                    retorno.HttpStatusCode = categoria.HttpStatusCode;
                    retorno.MensagemSistema = categoria.MensagemSistema;
                    retorno.MensagemUsuario = categoria.MensagemUsuario;
                    retorno.Dados = null;
                    return retorno;
                }

                var lista = await _categoriaRepository.BuscarTodosAsSubCategoriasAsync(usuarioId, categoriaId);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = $"{lista.Count} subcategoria(s) encontrada(s)";
                retorno.MensagemUsuario = $"{lista.Count} subcategoria(s) encontrada(s)";
                retorno.Dados = lista;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possível buscar a lista de subcategorias";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> EditarSubCategoriaAsync(string usuarioId, Guid categoriaId, Guid subCategoriaId, EditarSubCategoriaDTO editarSubCategoriaDTO)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var categoria = await BuscarUmElementoAsync(usuarioId, categoriaId);
                if (!categoria.Sucesso)
                {
                    retorno.Sucesso = categoria.Sucesso;
                    retorno.HttpStatusCode = categoria.HttpStatusCode;
                    retorno.MensagemSistema = categoria.MensagemSistema;
                    retorno.MensagemUsuario = categoria.MensagemUsuario;
                    retorno.Dados = null;
                    return retorno;
                }

                var subCategoriaExistente = await _categoriaRepository.BuscarUmaSubCategoriaAsync(categoriaId, subCategoriaId);
                if (subCategoriaExistente == null)
                {
                    return new RetornoGenerico(false, "Subcategoria não encontrada", "Subcategoria não encontrada", HttpStatusCode.NotFound);
                }

                editarSubCategoriaDTO.NomeSubCategoria = NormalizarNome(editarSubCategoriaDTO.NomeSubCategoria);

                var subCategoriaDuplicada = await _categoriaRepository.ExisteSubCategoriaComNomeAsync(
                    categoriaId,
                    editarSubCategoriaDTO.NomeSubCategoria,
                    subCategoriaId
                );

                if (subCategoriaDuplicada)
                {
                    return new RetornoGenerico(
                        false,
                        "Ja existe uma subcategoria com esse nome nesta categoria",
                        "Essa categoria ja possui uma subcategoria com esse nome",
                        HttpStatusCode.BadRequest
                    );
                }

                var subCategoria = _mapper.Map<SubCategoria>(editarSubCategoriaDTO);
                subCategoria.Id = subCategoriaId;
                subCategoria.CategoriaId = categoriaId;

                await _categoriaRepository.EditarSubCategoriaAsync(subCategoria);

                return new RetornoGenerico(true, "Subcategoria editada com sucesso", "Subcategoria editada", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possível editar a subcategoria";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> DeletarSubCategoriaAsync(string usuarioId, Guid categoriaId, Guid subCategoriaId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var categoria = await BuscarUmElementoAsync(usuarioId, categoriaId);
                if (!categoria.Sucesso)
                {
                    retorno.Sucesso = categoria.Sucesso;
                    retorno.HttpStatusCode = categoria.HttpStatusCode;
                    retorno.MensagemSistema = categoria.MensagemSistema;
                    retorno.MensagemUsuario = categoria.MensagemUsuario;
                    retorno.Dados = null;
                    return retorno;
                }

                var subCategoria = await _categoriaRepository.BuscarUmaSubCategoriaAsync(categoriaId, subCategoriaId);
                if (subCategoria == null)
                {
                    return new RetornoGenerico(false, "Subcategoria não encontrada", "Subcategoria não encontrada", HttpStatusCode.NotFound);
                }

                await _categoriaRepository.DeletarSubCategoriaAsync(subCategoria);

                return new RetornoGenerico(true, "Subcategoria deletada com sucesso", "Subcategoria deletada", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possível deletar a subcategoria";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> CadastrarSubCategoriaAsync(string usuarioId, Guid categoriaId, CadastrarSubCategoriaDTO cadastrarSubCategoriaDTO)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var categoria = await BuscarUmElementoAsync(usuarioId, categoriaId);
                if (!categoria.Sucesso)
                {
                    retorno.Sucesso = categoria.Sucesso;
                    retorno.HttpStatusCode = categoria.HttpStatusCode;
                    retorno.MensagemSistema = categoria.MensagemSistema;
                    retorno.MensagemUsuario = categoria.MensagemUsuario;
                    retorno.Dados = null;
                    return retorno;
                }

                cadastrarSubCategoriaDTO.NomeSubCategoria = NormalizarNome(cadastrarSubCategoriaDTO.NomeSubCategoria);

                var subCategoriaDuplicada = await _categoriaRepository.ExisteSubCategoriaComNomeAsync(
                    categoriaId,
                    cadastrarSubCategoriaDTO.NomeSubCategoria
                );

                if (subCategoriaDuplicada)
                {
                    return new RetornoGenerico(
                        false,
                        "Ja existe uma subcategoria com esse nome nesta categoria",
                        "Essa categoria ja possui uma subcategoria com esse nome",
                        HttpStatusCode.BadRequest
                    );
                }

                var subCategoria = _mapper.Map<SubCategoria>(cadastrarSubCategoriaDTO);
                subCategoria.Id = Guid.NewGuid();
                subCategoria.CategoriaId = categoriaId;

                await _categoriaRepository.CadastrarSubCategoriaAsync(subCategoria);

                return new RetornoGenerico(true, "Subcategoria cadastrada com sucesso", "Subcategoria cadastrada", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possível criar a subcategoria";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> BuscarUmaSubCategoriaAsync(Guid categoriaId, Guid subCategoriaId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var subCategoria = await _categoriaRepository.BuscarUmaSubCategoriaAsync(categoriaId, subCategoriaId);

                retorno.Sucesso = subCategoria != null;
                retorno.HttpStatusCode = subCategoria != null ? HttpStatusCode.OK : HttpStatusCode.NotFound;
                retorno.MensagemSistema = subCategoria != null ? "Subcategoria encontrada" : "Subcategoria não encontrada";
                retorno.MensagemUsuario = subCategoria != null ? "Subcategoria encontrada" : "Subcategoria não encontrada";
                retorno.Dados = subCategoria;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possível encontrar a subcategoria";
                retorno.Dados = null;
                return retorno;
            }
        }


    }
}
