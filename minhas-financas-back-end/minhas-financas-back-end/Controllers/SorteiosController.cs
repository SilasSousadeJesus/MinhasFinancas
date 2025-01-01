using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SorteiosController : ControllerBase
    {
        private readonly ISorteiosAppService _appService;

        public SorteiosController(ISorteiosAppService sorteiosAppService)
        {
            _appService = sorteiosAppService;
        }

        [HttpGet()]
        public async Task<IActionResult> BuscarInformacoesDashboard()
        {

            var dados = await _appService.MegaSena();

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
