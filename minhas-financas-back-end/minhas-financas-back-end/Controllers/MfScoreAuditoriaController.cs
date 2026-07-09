using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Reports;
using System.Net;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MfScoreAuditoriaController : ControllerBase
    {
        private readonly IMfScoreAuditoriaAppService _appService;
        private readonly IWebHostEnvironment _environment;

        public MfScoreAuditoriaController(IMfScoreAuditoriaAppService appService, IWebHostEnvironment environment)
        {
            _appService = appService;
            _environment = environment;
        }

        //[Authorize]
        [HttpPost("GerarPlanilha")]
        public async Task<IActionResult> GerarPlanilha()
        {
            if (!_environment.IsDevelopment())
            {
                return NotFound(new RetornoGenerico(
                    false,
                    "Endpoint interno de auditoria indisponivel fora do ambiente de desenvolvimento.",
                    "Ferramenta interna indisponivel neste ambiente.",
                    HttpStatusCode.NotFound,
                    null));
            }

            var dados = await _appService.GerarPlanilhaAsync();

            if (!dados.Sucesso)
            {
                return dados.HttpStatusCode switch
                {
                    HttpStatusCode.Unauthorized => Unauthorized(dados),
                    HttpStatusCode.NotFound => NotFound(dados),
                    HttpStatusCode.BadRequest => BadRequest(dados),
                    HttpStatusCode.InternalServerError => StatusCode(500, dados),
                    _ => BadRequest(dados)
                };
            }

            if (dados.Dados is not ArquivoRelatorioDTO arquivo)
            {
                return StatusCode(500, "Arquivo de auditoria nao encontrado.");
            }

            return File(arquivo.Conteudo, arquivo.ContentType, arquivo.NomeArquivo);
        }
    }
}
