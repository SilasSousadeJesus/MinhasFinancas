using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardAppService _appService;

        public DashboardController(IDashboardAppService  dashboardAppService)
        {
            _appService = dashboardAppService;
        }

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> BuscarInformacoesDashboard([FromRoute] string usuarioId)
        {

            var dados = await _appService.BuscarInformacoesDashboard(usuarioId);

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
