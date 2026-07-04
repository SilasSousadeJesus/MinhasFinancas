using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.Lancamento;
using MinhasFinancas.Application.Interfaces;
using System.Globalization;

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
        public async Task<IActionResult> BuscarTodosOsCartoes([FromRoute] string usuarioId, [FromQuery] FiltroListagemLancamentoDTO filtro)
        {
            filtro.DataInicialLancamento ??= ParseQueryDate(Request.Query["DataInicialLancamento"].FirstOrDefault())
                ?? ParseQueryDate(Request.Query["dataInicialLancamento"].FirstOrDefault());
            filtro.DataFinalLancamento ??= ParseQueryDate(Request.Query["DataFinalLancamento"].FirstOrDefault())
                ?? ParseQueryDate(Request.Query["dataFinalLancamento"].FirstOrDefault());
            filtro.DataInicialPagamento ??= ParseQueryDate(Request.Query["DataInicialPagamento"].FirstOrDefault())
                ?? ParseQueryDate(Request.Query["dataInicialPagamento"].FirstOrDefault());
            filtro.DataFinalPagamento ??= ParseQueryDate(Request.Query["DataFinalPagamento"].FirstOrDefault())
                ?? ParseQueryDate(Request.Query["dataFinalPagamento"].FirstOrDefault());

            var dados = await _appService.BuscarTodosOsElementosAsync(usuarioId, filtro);

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

        private static DateTime? ParseQueryDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var formats = new[]
            {
                "yyyy-MM-dd",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm:ss.fff",
                "yyyy-MM-ddTHH:mm:ssZ",
                "O",
            };

            if (DateTime.TryParseExact(
                value,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces,
                out var parsedExact))
            {
                return parsedExact;
            }

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsed))
            {
                return parsed;
            }

            return null;
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
