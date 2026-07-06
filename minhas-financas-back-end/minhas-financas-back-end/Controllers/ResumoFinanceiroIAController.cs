using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumoFinanceiroIAController : ControllerBase
    {
        private readonly IInteligenciaFinanceiraAppService _appService;

        public ResumoFinanceiroIAController(IInteligenciaFinanceiraAppService appService)
        {
            _appService = appService;
        }

        [Authorize]
        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> BuscarResumoFinanceiroIA([FromRoute] string usuarioId)
        {
            var dados = await _appService.BuscarResumoFinanceiroIA(usuarioId);

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
