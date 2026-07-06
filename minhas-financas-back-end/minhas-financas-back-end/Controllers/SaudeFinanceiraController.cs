using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaudeFinanceiraController : ControllerBase
    {
        private readonly ISaudeFinanceiraAppService _appService;

        public SaudeFinanceiraController(ISaudeFinanceiraAppService appService)
        {
            _appService = appService;
        }

        [Authorize]
        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> BuscarSaudeFinanceira([FromRoute] string usuarioId)
        {
            var dados = await _appService.BuscarSaudeFinanceira(usuarioId);

            if (!dados.Sucesso)
            {
                return dados.HttpStatusCode switch
                {
                    System.Net.HttpStatusCode.Unauthorized => Unauthorized(dados),
                    System.Net.HttpStatusCode.NotFound => NotFound(dados),
                    System.Net.HttpStatusCode.BadRequest => BadRequest(dados),
                    System.Net.HttpStatusCode.InternalServerError => StatusCode(500, dados),
                    _ => BadRequest(dados)
                };
            }

            return Ok(dados);
        }
    }
}
