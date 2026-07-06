using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.AssistenteFinanceiro;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssistenteFinanceiroController : ControllerBase
    {
        private readonly IAssistenteFinanceiroAppService _appService;

        public AssistenteFinanceiroController(IAssistenteFinanceiroAppService appService)
        {
            _appService = appService;
        }

        [Authorize]
        [HttpPost("GerarAnalise/{usuarioId}")]
        public async Task<IActionResult> GerarAnalise(
            [FromRoute] string usuarioId,
            [FromBody] GerarAnaliseAssistenteFinanceiroDTO? dto,
            CancellationToken cancellationToken)
        {
            var retorno = await _appService.GerarAnaliseAsync(
                usuarioId,
                dto?.PerguntaUsuario,
                cancellationToken);

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
