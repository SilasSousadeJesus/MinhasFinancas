using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.CompromissoFinanceiro;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompromissosFinanceirosController : ControllerBase
    {
        private readonly ICompromissoFinanceiroAppService _appService;

        public CompromissosFinanceirosController(ICompromissoFinanceiroAppService appService)
        {
            _appService = appService;
        }

        [Authorize]
        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> BuscarTodos([FromRoute] string usuarioId)
        {
            var retorno = await _appService.BuscarTodosOsElementosAsync(usuarioId);
            return Responder(retorno);
        }

        [Authorize]
        [HttpGet("{usuarioId}/{compromissoId:guid}")]
        public async Task<IActionResult> BuscarUm([FromRoute] string usuarioId, [FromRoute] Guid compromissoId)
        {
            var retorno = await _appService.BuscarUmElementoAsync(usuarioId, compromissoId);
            return Responder(retorno);
        }

        [Authorize]
        [HttpPost("Cadastrar/{usuarioId}")]
        public async Task<IActionResult> Cadastrar([FromRoute] string usuarioId, [FromBody] SalvarCompromissoFinanceiroDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            dto ??= new SalvarCompromissoFinanceiroDTO();
            dto.UsuarioId = usuarioId;
            var retorno = await _appService.CadastrarElementoAsync(dto);

            return Responder(retorno);
        }

        [Authorize]
        [HttpPut("Editar/{usuarioId}/{compromissoId:guid}")]
        public async Task<IActionResult> Editar(
            [FromRoute] string usuarioId,
            [FromRoute] Guid compromissoId,
            [FromBody] SalvarCompromissoFinanceiroDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var retorno = await _appService.EditarElementoAsync(usuarioId, compromissoId, dto);
            return Responder(retorno);
        }

        [Authorize]
        [HttpPut("Concluir/{usuarioId}/{compromissoId:guid}")]
        public async Task<IActionResult> Concluir([FromRoute] string usuarioId, [FromRoute] Guid compromissoId)
        {
            var retorno = await _appService.ConcluirAsync(usuarioId, compromissoId);
            return Responder(retorno);
        }

        [Authorize]
        [HttpPut("Cancelar/{usuarioId}/{compromissoId:guid}")]
        public async Task<IActionResult> Cancelar([FromRoute] string usuarioId, [FromRoute] Guid compromissoId)
        {
            var retorno = await _appService.CancelarAsync(usuarioId, compromissoId);
            return Responder(retorno);
        }

        [Authorize]
        [HttpDelete("Excluir/{usuarioId}/{compromissoId:guid}")]
        public async Task<IActionResult> Excluir([FromRoute] string usuarioId, [FromRoute] Guid compromissoId)
        {
            var retorno = await _appService.ExcluirAsync(usuarioId, compromissoId);
            return Responder(retorno);
        }

        private IActionResult Responder(RetornoGenerico retorno)
        {
            if (retorno.Sucesso)
            {
                return Ok(retorno);
            }

            return retorno.HttpStatusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized => Unauthorized(retorno),
                System.Net.HttpStatusCode.NotFound => NotFound(retorno),
                System.Net.HttpStatusCode.BadRequest => BadRequest(retorno),
                System.Net.HttpStatusCode.InternalServerError => StatusCode(500, retorno),
                _ => StatusCode((int)retorno.HttpStatusCode, retorno)
            };
        }
    }
}
