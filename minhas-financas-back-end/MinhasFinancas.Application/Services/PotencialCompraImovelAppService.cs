using AutoMapper;
using MinhasFinancas.Application.DTOs.PotencialCompra;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Services.PotencialCompra;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class PotencialCompraImovelAppService : IPotencialCompraImovelAppService
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioAppService _usuarioAppService;
        public PotencialCompraImovelAppService(IMapper mapper, IUsuarioAppService usuarioAppService)
        {
            _mapper = mapper;
            _usuarioAppService = usuarioAppService;
        }

        public async Task<RetornoGenerico> CalcularPotencialCompraImovel(PotencialCompraDTO potencialCompraDTO)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var potencialCompra = new PotencialCompraImovel(potencialCompraDTO.RendaMensal, potencialCompraDTO.EntradaFGTS);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Potencial de compra Calculado";
                retorno.MensagemUsuario = "Potencial de compra Calculado";
                retorno.Dados = potencialCompra;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel calcular o potencial de compra";
                return retorno;
            }
        }
    }
}
