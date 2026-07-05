using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.SimulacaoFinanceira;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimulacaoFinanceiraController : ControllerBase
    {
        private readonly ISimulacaoFinanceiraAppService _appService;

        public SimulacaoFinanceiraController(ISimulacaoFinanceiraAppService appService)
        {
            _appService = appService;
        }

        [Authorize]
        [HttpGet("BuscarTodas/{usuarioId}")]
        public async Task<IActionResult> BuscarTodas([FromRoute] string usuarioId)
        {
            var dados = await _appService.BuscarTodasAsync(usuarioId);
            return Responder(dados);
        }

        [Authorize]
        [HttpGet("BuscarUma/{usuarioId}/{simulacaoId}")]
        public async Task<IActionResult> BuscarUma([FromRoute] string usuarioId, [FromRoute] Guid simulacaoId)
        {
            var dados = await _appService.BuscarUmaAsync(usuarioId, simulacaoId);
            return Responder(dados);
        }

        [Authorize]
        [HttpPost("Cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] CadastrarSimulacaoFinanceiraDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var dados = await _appService.CadastrarAsync(dto);
            return Responder(dados);
        }

        [Authorize]
        [HttpPut("Editar/{usuarioId}/{simulacaoId}")]
        public async Task<IActionResult> Editar([FromRoute] string usuarioId, [FromRoute] Guid simulacaoId, [FromBody] EditarSimulacaoFinanceiraDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var dados = await _appService.EditarAsync(usuarioId, simulacaoId, dto);
            return Responder(dados);
        }

        [Authorize]
        [HttpDelete("Inativar/{usuarioId}/{simulacaoId}")]
        public async Task<IActionResult> Inativar([FromRoute] string usuarioId, [FromRoute] Guid simulacaoId)
        {
            var dados = await _appService.InativarAsync(usuarioId, simulacaoId);
            return Responder(dados);
        }

        [Authorize]
        [HttpGet("Calcular/{usuarioId}/{simulacaoId}")]
        public async Task<IActionResult> Calcular([FromRoute] string usuarioId, [FromRoute] Guid simulacaoId)
        {
            var dados = await _appService.CalcularAsync(usuarioId, simulacaoId);
            return Responder(dados);
        }

        private IActionResult Responder(RetornoGenerico dados)
        {
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
