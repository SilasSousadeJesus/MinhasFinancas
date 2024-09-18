using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.Cartao;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartaoController : ControllerBase
    {
        private readonly ICartaoAppService _appService;

        public CartaoController(ICartaoAppService cartaoAppService)
        {
            _appService = cartaoAppService;
        }
        [Authorize]
        [HttpPost("CadastrarCartao")]
        public async Task<IActionResult> CadastrarCartao([FromBody] CadastrarCartaoDTO cadastroCartaoDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.CadastrarElementoAsync(cadastroCartaoDTO);

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
        [HttpGet("BuscarTodosOsCartoes/{usuarioId}")]
        public async Task<IActionResult> BuscarTodosOsCartoes([FromRoute] string usuarioId)
        {

            var dados = await _appService.BuscarTodosOsElementosAsync(usuarioId);

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
        [HttpGet("BuscarUmCartao/{usuarioId}/{cartaoId}")]
        public async Task<IActionResult> BuscarUmCartao([FromRoute] string usuarioId, [FromRoute] Guid cartaoId)
        {

            var dados = await _appService.BuscarUmElementoAsync(usuarioId, cartaoId);

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
        [HttpPut("EditarCartao/{usuarioId}/{cartaoId}")]
        public async Task<IActionResult> EditarCartao([FromRoute] string usuarioId, [FromRoute] Guid cartaoId, [FromBody] EditarCartaoDTO editarCartaoDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.EditarElementoAsync(usuarioId, cartaoId, editarCartaoDTO);

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
        [HttpDelete("DeletarCartao/{usuarioId}/{cartaoId}")]
        public async Task<IActionResult> DeletarCartao([FromRoute] string usuarioId, [FromRoute] Guid cartaoId)
        {

            var dados = await _appService.DeletarElementoAsync(usuarioId, cartaoId);

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
