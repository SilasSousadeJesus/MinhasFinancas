using AutoMapper;
using MinhasFinancas.Application.DTOs.Passivo;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class PassivoAppService : IPassivoAppService
    {
        private readonly IMapper _mapper;
        private readonly IPassivoRepository _passivoRepository;
        private readonly IUsuarioAppService _usuarioAppService;
        public PassivoAppService(IMapper mapper, IPassivoRepository passivoRepository, IUsuarioAppService usuarioAppService)
        {
            _mapper = mapper;
            _passivoRepository = passivoRepository;
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

                var lista = await _passivoRepository.BuscarTodosOsElementosAsync(id);

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

                var passivo = await _passivoRepository.BuscarUmElementoAsync(usuarioId, BancoId);

                retorno.Sucesso = passivo != null ? true : false;
                retorno.HttpStatusCode = passivo != null ? HttpStatusCode.OK : HttpStatusCode.NotFound;
                retorno.MensagemSistema = passivo != null ? "passivo Encontrado" : "passivo não encontrado";
                retorno.MensagemUsuario = passivo != null ? "passivo Encontrado" : "passivo não encontrado";
                retorno.Dados = passivo;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel encontrar o passivo";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> CadastrarElementoAsync(CadastrarPassivoDTO elementoDTO)
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

                var passivo = new Passivo
                {
                    Id = Guid.NewGuid(),
                    NomePassivo = elementoDTO.NomeBemPatrimonial,
                    Descricao = elementoDTO.Descricao,
                    Tipo = elementoDTO.Tipo,
                    UsuarioId = elementoDTO.UsuarioId,
                    DataCadastro = DateTime.Now,
                    DataInicio = elementoDTO.DataInicio,
                    DataFim = elementoDTO.DataFim,
                    Permanencia = true,
                    Ativo = true,
                    DataPermanencia = new List<PermanenciaPassivo>
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            PassivoId = Guid.Empty,
                            DataPermanencia = elementoDTO.DataInicio ?? DateTime.Now,
                            Valor = elementoDTO.ValorAtual
                        }
                    }
                };

                foreach (var permanencia in passivo.DataPermanencia ?? new List<PermanenciaPassivo>())
                {
                    permanencia.PassivoId = passivo.Id;
                }

                await _passivoRepository.CadastrarElementoAsync(passivo);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "passivo cadastrado com sucesso";
                retorno.MensagemUsuario = "passivo cadastrado";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel criar o passivo";
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

                Passivo passivo = buscaPorCartao.Dados;
                passivo.Ativo = false;

                await _passivoRepository.EditarElementoAsync(passivo);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "passivo Deletado com sucesso";
                retorno.MensagemUsuario = "passivo Deletado com sucesso";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel Deletado o passivo";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> EditarElementoAsync(string idPatrono, Guid elementoId, EditarPassivoDTO elementoDTO)
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

                Passivo passivoAtual = buscaPorBanco.Dados;
                passivoAtual.NomePassivo = elementoDTO.NomeBemPatrimonial;
                passivoAtual.Descricao = elementoDTO.Descricao;
                passivoAtual.Tipo = elementoDTO.Tipo;
                passivoAtual.UsuarioId = idPatrono;
                passivoAtual.DataInicio = elementoDTO.DataInicio;
                passivoAtual.DataFim = elementoDTO.DataFim;
                passivoAtual.Ativo = true;

                await _passivoRepository.EditarElementoAsync(passivoAtual);

                var ultimaPermanencia = passivoAtual.DataPermanencia?
                    .OrderByDescending(x => x.DataPermanencia)
                    .FirstOrDefault();

                if (ultimaPermanencia == null || ultimaPermanencia.Valor != elementoDTO.ValorAtual)
                {
                    await _passivoRepository.CadastrarPermanenciaAsync(new PermanenciaPassivo
                    {
                        Id = Guid.NewGuid(),
                        PassivoId = elementoId,
                        DataPermanencia = DateTime.Now,
                        Valor = elementoDTO.ValorAtual,
                    });
                }

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Passivo Editado com sucesso";
                retorno.MensagemUsuario = "Passivo Editado";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel Editar o Passivo";
                retorno.Dados = null;
                return retorno;
            }
        }
    }
}
