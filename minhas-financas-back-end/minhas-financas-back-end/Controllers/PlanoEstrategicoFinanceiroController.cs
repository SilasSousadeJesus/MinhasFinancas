using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.PlanoEstrategicoFinanceiro;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanoEstrategicoFinanceiroController : ControllerBase
    {
        private readonly IPlanoEstrategicoFinanceiroAppService _appService;

        public PlanoEstrategicoFinanceiroController(IPlanoEstrategicoFinanceiroAppService appService)
        {
            _appService = appService;
        }

        [Authorize]
        [HttpGet("BuscarTodos/{usuarioId}")]
        public async Task<IActionResult> BuscarTodos([FromRoute] string usuarioId)
        {
            var dados = await _appService.BuscarTodosAsync(usuarioId);
            return Responder(dados);
        }

        [Authorize]
        [HttpGet("BuscarVigente/{usuarioId}")]
        public async Task<IActionResult> BuscarVigente([FromRoute] string usuarioId)
        {
            var dados = await _appService.BuscarVigenteAsync(usuarioId);
            return Responder(dados);
        }

        [Authorize]
        [HttpGet("BuscarUm/{usuarioId}/{planoId}")]
        public async Task<IActionResult> BuscarUm([FromRoute] string usuarioId, [FromRoute] Guid planoId)
        {
            var dados = await _appService.BuscarUmAsync(usuarioId, planoId);
            return Responder(dados);
        }

        [Authorize]
        [HttpPost("Cadastrar/{usuarioId}")]
        public async Task<IActionResult> Cadastrar([FromRoute] string usuarioId, [FromBody] SalvarPlanoEstrategicoFinanceiroDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.CadastrarAsync(usuarioId, dto);
            return Responder(dados);
        }

        [Authorize]
        [HttpPut("AtualizarVersao/{usuarioId}/{planoId}")]
        public async Task<IActionResult> AtualizarVersao([FromRoute] string usuarioId, [FromRoute] Guid planoId, [FromBody] SalvarPlanoEstrategicoFinanceiroDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.AtualizarVersaoAsync(usuarioId, planoId, dto);
            return Responder(dados);
        }

        [Authorize]
        [HttpDelete("Inativar/{usuarioId}/{planoId}")]
        public async Task<IActionResult> Inativar([FromRoute] string usuarioId, [FromRoute] Guid planoId)
        {
            var dados = await _appService.InativarAsync(usuarioId, planoId);
            return Responder(dados);
        }

        private IActionResult Responder(RetornoGenerico dados)
        {
            if (dados.Sucesso)
            {
                return Ok(dados);
            }

            return dados.HttpStatusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized => Unauthorized(dados),
                System.Net.HttpStatusCode.NotFound => NotFound(dados),
                System.Net.HttpStatusCode.BadRequest => BadRequest(dados),
                System.Net.HttpStatusCode.InternalServerError => StatusCode(500, dados),
                _ => BadRequest(dados)
            };
        }
    }
}
