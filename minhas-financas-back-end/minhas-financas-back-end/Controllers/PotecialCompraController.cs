using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.PotencialCompra;
using MinhasFinancas.Application.DTOs.Usuario;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PotecialCompraController : ControllerBase
    {
        private readonly IPotencialCompraImovelAppService _appService;
        public PotecialCompraController(IPotencialCompraImovelAppService potencialCompraImovelAppService)
        {
            _appService = potencialCompraImovelAppService;
        }

        [HttpPost()]
        public async Task<IActionResult> PotecialCompra([FromBody] PotencialCompraDTO potencialCompraDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.CalcularPotencialCompraImovel(potencialCompraDTO);

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
