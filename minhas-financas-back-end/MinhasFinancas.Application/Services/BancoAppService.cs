using AutoMapper;
using MinhasFinancas.Application.DTOs.Banco;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;
using MinhasFinancas.Infra.Data.Repositories;
using System.Net;

namespace MinhasFinancas.Application.Services
{


    public class BancoAppService : IBancoAppService
    {
        private readonly IMapper _mapper;
        private readonly IBancoRepository _bancoRepository;
        private readonly IUsuarioAppService _usuarioAppService;
        public BancoAppService(IMapper mapper, IBancoRepository bancoRepository, IUsuarioAppService usuarioAppService)
        {
            _mapper = mapper;
            _bancoRepository = bancoRepository;
            _usuarioAppService = usuarioAppService;
        }

        public async Task<RetornoGenerico> CadastrarBanco(CadastroBancoDTO cadastroBancoDTO)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorusuario = await _usuarioAppService.BuscarUmUsuario(cadastroBancoDTO.UsuarioId);

                if (!buscaPorusuario.Sucesso)
                {
                    retorno.Sucesso = buscaPorusuario.Sucesso;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = buscaPorusuario.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorusuario.MensagemUsuario;
                    retorno.Dados = null;

                    return retorno;
                }

                var banco = _mapper.Map<Banco>(cadastroBancoDTO);

                await _bancoRepository.CadastrarElementoAsync(banco);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Banco cadastrado com sucesso";
                retorno.MensagemUsuario = "Banco cadastrado";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel criar o banco";
                retorno.Dados = null;
                return retorno;
            }
        }
    }
}
