using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalisesFinanceirasHistoricasController : ControllerBase
    {
        private readonly IAnaliseFinanceiraHistoricaAppService _appService;

        public AnalisesFinanceirasHistoricasController(IAnaliseFinanceiraHistoricaAppService appService)
        {
            _appService = appService;
        }

        [Authorize]
        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> BuscarTodas([FromRoute] string usuarioId)
        {
            var retorno = await _appService.BuscarTodasAsync(usuarioId);

            if (!retorno.Sucesso)
            {
                return retorno.HttpStatusCode switch
                {
                    System.Net.HttpStatusCode.Unauthorized => Unauthorized(retorno),
                    System.Net.HttpStatusCode.NotFound => NotFound(retorno),
                    System.Net.HttpStatusCode.BadRequest => BadRequest(retorno),
                    _ => StatusCode((int)retorno.HttpStatusCode, retorno)
                };
            }

            return Ok(retorno);
        }

        [Authorize]
        [HttpGet("{usuarioId}/{analiseId:guid}")]
        public async Task<IActionResult> BuscarDetalhe([FromRoute] string usuarioId, [FromRoute] Guid analiseId)
        {
            var retorno = await _appService.BuscarDetalheAsync(usuarioId, analiseId);

            if (!retorno.Sucesso)
            {
                return retorno.HttpStatusCode switch
                {
                    System.Net.HttpStatusCode.Unauthorized => Unauthorized(retorno),
                    System.Net.HttpStatusCode.NotFound => NotFound(retorno),
                    System.Net.HttpStatusCode.BadRequest => BadRequest(retorno),
                    _ => StatusCode((int)retorno.HttpStatusCode, retorno)
                };
            }

            return Ok(retorno);
        }
    }
}
