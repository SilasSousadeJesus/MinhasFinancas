using AutoMapper;
using MinhasFinancas.Application.DTOs.BemPatrimonial;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class BemPatrimonialAppService : IBemPatrimonialAppService
    {

        private readonly IMapper _mapper;
        private readonly IBemMaterialRepository _bemMaterialRepository;
        private readonly IUsuarioAppService _usuarioAppService;
        public BemPatrimonialAppService(IMapper mapper, IBemMaterialRepository bemMaterialRepository, IUsuarioAppService usuarioAppService)
        {
            _mapper = mapper;
            _bemMaterialRepository = bemMaterialRepository;
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

                var lista = await _bemMaterialRepository.BuscarTodosOsElementosAsync(Id);

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
                retorno.MensagemUsuario = "Não foi possivel buscar a lista de bens patrimoniais";
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

                var bemPatrimonial = await _bemMaterialRepository.BuscarUmElementoAsync(usuarioId, BancoId);

                retorno.Sucesso = bemPatrimonial != null ? true : false;
                retorno.HttpStatusCode = bemPatrimonial != null ? HttpStatusCode.OK : HttpStatusCode.NotFound;
                retorno.MensagemSistema = bemPatrimonial != null ? "Bem Patrimonial Encontrado" : "Bem Patrimonial não encontrado";
                retorno.MensagemUsuario = bemPatrimonial != null ? "Bem Patrimonial Encontrado" : "Bem Patrimonial não encontrado";
                retorno.Dados = bemPatrimonial;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel encontrar o Bem Patrimonial";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> CadastrarElementoAsync(CadastrarBemPatrimonialDTO elementoDTO)
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

                var dataReferencia = elementoDTO.DataAquisicao ?? DateTime.Now;
                var bemPatrimonial = new BemPatrimonial
                {
                    Id = Guid.NewGuid(),
                    NomeBemPatrimonial = elementoDTO.NomeBemPatrimonial,
                    Descricao = elementoDTO.Descricao,
                    Tipo = elementoDTO.Tipo,
                    UsuarioId = elementoDTO.UsuarioId,
                    DataCadastro = DateTime.Now,
                    DataAquisicao = elementoDTO.DataAquisicao,
                    Permanencia = true,
                    Ativo = true,
                    DataPermanencia = new List<PermanenciaBemMaterial>
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            BemPatrimonialId = Guid.Empty,
                            DataPermanencia = dataReferencia,
                            Valor = elementoDTO.ValorAtual
                        }
                    }
                };

                foreach (var permanencia in bemPatrimonial.DataPermanencia)
                {
                    permanencia.BemPatrimonialId = bemPatrimonial.Id;
                }

                await _bemMaterialRepository.CadastrarElementoAsync(bemPatrimonial);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Bem patrimonial cadastrado com sucesso";
                retorno.MensagemUsuario = "Bem patrimonial cadastrado";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel criar o Bem patrimonial";
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

                BemPatrimonial bemPatrimonial = buscaPorCartao.Dados;
                bemPatrimonial.Ativo = false;

                await _bemMaterialRepository.EditarElementoAsync(bemPatrimonial);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "bem patrimonial Deletado com sucesso";
                retorno.MensagemUsuario = "bem patrimonial Deletado com sucesso";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel Deletado o bem patrimonial";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> EditarElementoAsync(string idPatrono, Guid elementoId, EditarBemPatrimonialDTO elementoDTO)
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

                BemPatrimonial bemAtual = buscaPorBanco.Dados;
                bemAtual.NomeBemPatrimonial = elementoDTO.NomeBemPatrimonial;
                bemAtual.Descricao = elementoDTO.Descricao;
                bemAtual.Tipo = elementoDTO.Tipo;
                bemAtual.UsuarioId = idPatrono;
                bemAtual.DataAquisicao = elementoDTO.DataAquisicao;
                bemAtual.Ativo = true;

                await _bemMaterialRepository.EditarElementoAsync(bemAtual);

                var ultimaPermanencia = bemAtual.DataPermanencia?
                    .OrderByDescending(x => x.DataPermanencia)
                    .FirstOrDefault();

                if (ultimaPermanencia == null || ultimaPermanencia.Valor != elementoDTO.ValorAtual)
                {
                    await _bemMaterialRepository.CadastrarPermanenciaAsync(new PermanenciaBemMaterial
                    {
                        Id = Guid.NewGuid(),
                        BemPatrimonialId = elementoId,
                        DataPermanencia = DateTime.Now,
                        Valor = elementoDTO.ValorAtual,
                    });
                }

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "bem Editado com sucesso";
                retorno.MensagemUsuario = "bem Editado";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel Editar o bem";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> BuscarUltimaDataPermanencia(Guid ultimaDataPermanente)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var ultimaDataPermanencia = await _bemMaterialRepository.BuscarUltimaDataPermanencia(ultimaDataPermanente);

                retorno.Sucesso = ultimaDataPermanencia != null ? true : false;
                retorno.HttpStatusCode = ultimaDataPermanencia != null ? HttpStatusCode.OK : HttpStatusCode.NotFound;
                retorno.MensagemSistema = ultimaDataPermanencia != null ? "ultima data do bem patrimonial encontrado" : "ultima data não encontrada";
                retorno.MensagemUsuario = ultimaDataPermanencia != null ? "ultima data do bem patrimonial encontrado" : "ultima data não encontrada";
                retorno.Dados = ultimaDataPermanencia;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel encontrar a ultima data";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task EditarUltimaDataPermanencia(PermanenciaBemMaterial permanencia)
        {
            var retorno = new RetornoGenerico();

            try
            {
                await _bemMaterialRepository.EditarUltimaDataPermanencia(permanencia);

                Task.CompletedTask.Wait();
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel encontrar a ultima data";
                retorno.Dados = null;
            }
        }
    }
}
