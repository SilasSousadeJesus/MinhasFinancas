using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.Interfaces;
using Newtonsoft.Json;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelatoriosController : ControllerBase
    {
        private readonly IRelatoriosAppService _appService;

        public RelatoriosController(IRelatoriosAppService relatoriosAppService)
        {
            _appService = relatoriosAppService;
        }
        [Authorize]
        [HttpGet("PorCategoria/{usuarioId}")]
        public async Task<IActionResult> RelatoriosPorCategoria([FromRoute] string usuarioId)
        {

            var dados = await _appService.RelatoriosPorCategoriaLancamento(usuarioId);

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
        [HttpGet("PorAno/{usuarioId}")]
        public async Task<IActionResult> RelatoriosPorAno([FromRoute] string usuarioId)
        {

            var dados = await _appService.RelatoriosValoresAnoPorAno(usuarioId);

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