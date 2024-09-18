using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.Lancamento;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LancamentoController : ControllerBase
    {
        private readonly ILancamentoAppService _appService;

        public LancamentoController(ILancamentoAppService lancamentoAppService)
        {
            _appService = lancamentoAppService;
        }
        [Authorize]
        [HttpPost("CadastrarLancamento")]
        public async Task<IActionResult> CadastrarCartao([FromBody] CadastrarLancamentoDTO cadastrarLancamento)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.CadastrarElementoAsync(cadastrarLancamento);

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
        [HttpGet("BuscarTodosOsLancamento/{usuarioId}")]
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
        [HttpGet("BuscarUmLancamento/{usuarioId}/{faturamentoId}")]
        public async Task<IActionResult> BuscarUmCartao([FromRoute] string usuarioId, [FromRoute] Guid faturamentoId)
        {

            var dados = await _appService.BuscarUmElementoAsync(usuarioId, faturamentoId);

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
        [HttpPut("EditarLancamento/{usuarioId}/{faturamentoId}")]
        public async Task<IActionResult> EditarCartao([FromRoute] string usuarioId, [FromRoute] Guid faturamentoId, [FromBody] EditarLancamentoDTO editarLancamento)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.EditarElementoAsync(usuarioId, faturamentoId, editarLancamento);

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
        [HttpDelete("DeletarLancamento/{usuarioId}/{faturamentoId}")]
        public async Task<IActionResult> DeletarCartao([FromRoute] string usuarioId, [FromRoute] Guid faturamentoId)
        {

            var dados = await _appService.DeletarElementoAsync(usuarioId, faturamentoId);

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
        [HttpGet("BuscarLancamentosPorCategoria/{usuarioId}")]
        public async Task<IActionResult> BuscarLancamentosPorCategoria([FromRoute] string usuarioId)
        {

            var dados = await _appService.BuscarLancamentosPorCategoriaAsync(usuarioId);

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
