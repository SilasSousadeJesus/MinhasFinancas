using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.Projecao;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjecaoController : ControllerBase
    {
        private readonly IProjecaoAppService _appService;

        public ProjecaoController(IProjecaoAppService appService)
        {
            _appService = appService;
        }

        [Authorize]
        [HttpGet("BuscarTodas/{usuarioId}")]
        public async Task<IActionResult> BuscarTodas([FromRoute] string usuarioId)
        {
            var dados = await _appService.BuscarTodosAsync(usuarioId);

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
        [HttpGet("BuscarUma/{usuarioId}/{projecaoId}")]
        public async Task<IActionResult> BuscarUma([FromRoute] string usuarioId, [FromRoute] Guid projecaoId)
        {
            var dados = await _appService.BuscarUmAsync(usuarioId, projecaoId);

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
        [HttpPost("Cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] CadastrarProjecaoDTO projecaoDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.CadastrarAsync(projecaoDTO);

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
        [HttpPut("Editar/{usuarioId}/{projecaoId}")]
        public async Task<IActionResult> Editar([FromRoute] string usuarioId, [FromRoute] Guid projecaoId, [FromBody] EditarProjecaoDTO projecaoDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.EditarAsync(usuarioId, projecaoId, projecaoDTO);

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
        [HttpDelete("Deletar/{usuarioId}/{projecaoId}")]
        public async Task<IActionResult> Deletar([FromRoute] string usuarioId, [FromRoute] Guid projecaoId)
        {
            var dados = await _appService.DeletarAsync(usuarioId, projecaoId);

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
        [HttpPost("Calcular/{usuarioId}")]
        public async Task<IActionResult> Calcular([FromRoute] string usuarioId, [FromBody] CalcularProjecaoDTO calcularProjecaoDTO)
        {
            var dados = await _appService.CalcularAsync(usuarioId, calcularProjecaoDTO);

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
        [HttpPost("CalcularSalva/{usuarioId}/{projecaoId}")]
        public async Task<IActionResult> CalcularSalva([FromRoute] string usuarioId, [FromRoute] Guid projecaoId)
        {
            var dados = await _appService.CalcularAsync(usuarioId, projecaoId);

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
