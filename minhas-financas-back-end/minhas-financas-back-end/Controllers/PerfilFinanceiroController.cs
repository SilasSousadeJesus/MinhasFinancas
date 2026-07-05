using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.PerfilFinanceiro;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilFinanceiroController : ControllerBase
    {
        private readonly IPerfilFinanceiroAppService _appService;

        public PerfilFinanceiroController(IPerfilFinanceiroAppService appService)
        {
            _appService = appService;
        }

        [Authorize]
        [HttpGet("BuscarPerfilFinanceiro/{usuarioId}")]
        public async Task<IActionResult> BuscarPerfilFinanceiro([FromRoute] string usuarioId)
        {
            var dados = await _appService.BuscarVisaoGeralAsync(usuarioId);
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

        [Authorize]
        [HttpPost("SalvarPerfilFinanceiro/{usuarioId}")]
        public async Task<IActionResult> SalvarPerfilFinanceiro([FromRoute] string usuarioId, [FromBody] SalvarPerfilFinanceiroDTO configuracaoDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.SalvarConfiguracaoAsync(usuarioId, configuracaoDTO);
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
