using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.Lancamento;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Reports;
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
            filtro.DataInicialVencimento ??= ParseQueryDate(Request.Query["DataInicialVencimento"].FirstOrDefault())
                ?? ParseQueryDate(Request.Query["dataInicialVencimento"].FirstOrDefault());
            filtro.DataFinalVencimento ??= ParseQueryDate(Request.Query["DataFinalVencimento"].FirstOrDefault())
                ?? ParseQueryDate(Request.Query["dataFinalVencimento"].FirstOrDefault());
            filtro.DataInicialEfetivacao ??= ParseQueryDate(Request.Query["DataInicialEfetivacao"].FirstOrDefault())
                ?? ParseQueryDate(Request.Query["dataInicialEfetivacao"].FirstOrDefault());
            filtro.DataFinalEfetivacao ??= ParseQueryDate(Request.Query["DataFinalEfetivacao"].FirstOrDefault())
                ?? ParseQueryDate(Request.Query["dataFinalEfetivacao"].FirstOrDefault());

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
        [HttpGet("FluxoCaixaSimples/{usuarioId}")]
        public async Task<IActionResult> BuscarFluxoCaixaSimples([FromRoute] string usuarioId, [FromQuery] int ano, [FromQuery] int mes)
        {
            var dados = await _appService.BuscarFluxoCaixaSimplesAsync(usuarioId, ano, mes);

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
        [HttpGet("ExportarExcel/{usuarioId}")]
        public async Task<IActionResult> ExportarExcel([FromRoute] string usuarioId, [FromQuery] FiltroListagemLancamentoDTO filtro)
        {
            filtro.DataInicialLancamento ??= ParseQueryDate(Request.Query["DataInicialLancamento"].FirstOrDefault())
                ?? ParseQueryDate(Request.Query["dataInicialLancamento"].FirstOrDefault());
            filtro.DataFinalLancamento ??= ParseQueryDate(Request.Query["DataFinalLancamento"].FirstOrDefault())
                ?? ParseQueryDate(Request.Query["dataFinalLancamento"].FirstOrDefault());
            filtro.DataInicialVencimento ??= ParseQueryDate(Request.Query["DataInicialVencimento"].FirstOrDefault())
                ?? ParseQueryDate(Request.Query["dataInicialVencimento"].FirstOrDefault());
            filtro.DataFinalVencimento ??= ParseQueryDate(Request.Query["DataFinalVencimento"].FirstOrDefault())
                ?? ParseQueryDate(Request.Query["dataFinalVencimento"].FirstOrDefault());
            filtro.DataInicialEfetivacao ??= ParseQueryDate(Request.Query["DataInicialEfetivacao"].FirstOrDefault())
                ?? ParseQueryDate(Request.Query["dataInicialEfetivacao"].FirstOrDefault());
            filtro.DataFinalEfetivacao ??= ParseQueryDate(Request.Query["DataFinalEfetivacao"].FirstOrDefault())
                ?? ParseQueryDate(Request.Query["dataFinalEfetivacao"].FirstOrDefault());

            var dados = await _appService.ExportarLancamentosExcelAsync(usuarioId, filtro);

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

            if (dados.Dados is not ArquivoRelatorioDTO arquivo)
            {
                return StatusCode(500, "Arquivo de relatório não encontrado.");
            }

            return File(arquivo.Conteudo, arquivo.ContentType, arquivo.NomeArquivo);
        }

        [Authorize]
        [HttpGet("BaixarModeloImportacaoExcel/{usuarioId}")]
        public async Task<IActionResult> BaixarModeloImportacaoExcel([FromRoute] string usuarioId)
        {
            var dados = await _appService.BaixarModeloImportacaoLancamentosExcelAsync(usuarioId);

            if (!dados.Sucesso)
            {
                return BadRequest(dados);
            }

            if (dados.Dados is not ArquivoRelatorioDTO arquivo)
            {
                return StatusCode(500, "Arquivo de modelo não encontrado.");
            }

            return File(arquivo.Conteudo, arquivo.ContentType, arquivo.NomeArquivo);
        }

        [Authorize]
        [HttpPost("ImportarExcel/{usuarioId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportarExcel([FromRoute] string usuarioId, [FromForm] IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Selecione uma planilha preenchida para importar.");
            }

            if (!string.Equals(Path.GetExtension(arquivo.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("A importação aceita apenas arquivos .xlsx.");
            }

            await using var stream = arquivo.OpenReadStream();
            var dados = await _appService.ImportarLancamentosExcelAsync(usuarioId, stream);

            if (!dados.Sucesso)
            {
                return dados.HttpStatusCode == System.Net.HttpStatusCode.NotFound ? NotFound(dados) : BadRequest(dados);
            }

            return Ok(dados);
        }

        [Authorize]
        [HttpGet("ExportarFluxoCaixaSimplesExcel/{usuarioId}")]
        public async Task<IActionResult> ExportarFluxoCaixaSimplesExcel([FromRoute] string usuarioId, [FromQuery] ExportarFluxoCaixaSimplesExcelDTO filtro)
        {
            var dados = await _appService.ExportarFluxoCaixaSimplesExcelAsync(usuarioId, filtro);

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

            if (dados.Dados is not ArquivoRelatorioDTO arquivo)
            {
                return StatusCode(500, "Arquivo de relatório não encontrado.");
            }

            return File(arquivo.Conteudo, arquivo.ContentType, arquivo.NomeArquivo);
        }

        [Authorize]
        [HttpGet("Parcelamentos/{usuarioId}/{grupoParcelamentoId}")]
        public async Task<IActionResult> BuscarParcelamento([FromRoute] string usuarioId, [FromRoute] Guid grupoParcelamentoId)
        {
            var dados = await _appService.BuscarParcelamentoAsync(usuarioId, grupoParcelamentoId);

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
        [HttpPut("Parcelamentos/{usuarioId}/{grupoParcelamentoId}/EditarEmLote")]
        public async Task<IActionResult> EditarParcelamentoEmLote(
            [FromRoute] string usuarioId,
            [FromRoute] Guid grupoParcelamentoId,
            [FromBody] EditarParcelamentoEmLoteDTO editarParcelamento)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.EditarParcelamentoEmLoteAsync(usuarioId, grupoParcelamentoId, editarParcelamento);

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
        [HttpPost("EfetivarLancamento/{usuarioId}/{faturamentoId}")]
        public async Task<IActionResult> EfetivarLancamento([FromRoute] string usuarioId, [FromRoute] Guid faturamentoId)
        {
            var dados = await _appService.EfetivarLancamentoAsync(usuarioId, faturamentoId);

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
