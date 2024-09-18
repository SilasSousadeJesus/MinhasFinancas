using AutoMapper;
using MinhasFinancas.Application.DTOs.Meta;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class MetaAppService : IMetaAppService
    {
        private readonly IMapper _mapper;
        private readonly IMetaRepository _metaRepository;
        private readonly IUsuarioAppService _usuarioAppService;
        public MetaAppService(IMapper mapper, IMetaRepository metaRepository, IUsuarioAppService usuarioAppService)
        {
            _mapper = mapper;
            _metaRepository = metaRepository;
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

                var lista = await _metaRepository.BuscarTodosOsElementosAsync(id);

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
                retorno.MensagemUsuario = "Não foi possivel buscar as metas do usuario";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> BuscarUmElementoAsync(string usuarioId, Guid elementoId)
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

                var meta = await _metaRepository.BuscarUmElementoAsync(usuarioId, elementoId);

                retorno.Sucesso = meta != null ? true : false;
                retorno.HttpStatusCode = meta != null ? HttpStatusCode.OK : HttpStatusCode.NotFound;
                retorno.MensagemSistema = meta != null ? "meta encontrada" : "meta não encontrada";
                retorno.MensagemUsuario = meta != null ? "meta encontrada" : "meta não encontrada";
                retorno.Dados = meta;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel encontrar a meta";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> CadastrarElementoAsync(CadastrarMetaDTO elementoDTO)
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

                var meta = _mapper.Map<Meta>(elementoDTO);

                meta.AportesMeta.Add(new AporteMeta()
                {
                    DataAporte = DateTime.Now,
                     Id = Guid.NewGuid(),
                     MetaId = meta.Id,
                     Valor = elementoDTO.ValorAtual
                });
                
                meta.CalcularDiferenca();

                await _metaRepository.CadastrarElementoAsync(meta);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Meta cadastrada com sucesso";
                retorno.MensagemUsuario = "Meta cadastrada com sucesso";
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

        public async Task<RetornoGenerico> DeletarElementoAsync(string idPatrono, Guid elementoId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorCartao = await BuscarUmElementoAsync(idPatrono, elementoId);

                if (!buscaPorCartao.Sucesso)
                {
                    retorno.Sucesso = buscaPorCartao.Sucesso;
                    retorno.HttpStatusCode = buscaPorCartao.HttpStatusCode;
                    retorno.MensagemSistema = buscaPorCartao.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorCartao.MensagemUsuario;
                    retorno.Dados = null;

                    return retorno;
                }

                await _metaRepository.DeletarElementoAsync(buscaPorCartao.Dados);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "meta deletada com sucesso";
                retorno.MensagemUsuario = "meta deletada com sucesso";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel deletar a meta";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> EditarElementoAsync(string idPatrono, Guid elementoId, EditarMetalDTO elementoDTO)
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

                var meta = _mapper.Map<Meta>(elementoDTO);
                meta.Id = elementoId;
                meta.UsuarioId = idPatrono;

                if (elementoDTO.ValorFinal != decimal.Zero || elementoDTO.ValorAtual != decimal.Zero) {
                    if (meta.ValorFinal != elementoDTO.ValorFinal || meta.ValorAtual != elementoDTO.ValorAtual)
                    {
                        meta.AportesMeta.Add(new AporteMeta()
                        {
                            DataAporte = DateTime.Now,
                            Id = Guid.NewGuid(),
                            MetaId = meta.Id,
                            Valor = elementoDTO.ValorAtual
                        });
                        meta.CalcularDiferenca();
                    }
                }


                await _metaRepository.EditarElementoAsync(meta);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "meta Editada com sucesso";
                retorno.MensagemUsuario = "meta Editada com sucesso";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel Editar a meta";
                retorno.Dados = null;
                return retorno;
            }
        }


        public async Task<RetornoGenerico> AtualizarAndamentoMetaAsync(string idPatrono, Guid elementoId, decimal valor)
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

                var buscaPorMeta = await BuscarUmElementoAsync(idPatrono, elementoId);

                Meta meta = buscaPorMeta.Dados;

                meta.ValorAtual += valor;

                var aporte = new AporteMeta()
                {
                    DataAporte = DateTime.Now,
                    Id = Guid.NewGuid(),
                    MetaId = meta.Id,
                    Valor = valor
                };

                meta.CalcularDiferenca();

                await _metaRepository.EditarElementoAsync(meta);

                await _metaRepository.CadastrarNovoAporteAsync(aporte);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "meta Editada com sucesso";
                retorno.MensagemUsuario = "meta Editada com sucesso";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel Editar a meta";
                retorno.Dados = null;
                return retorno;
            }
        }

    }
}
