using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.MfScorePersona;
using MinhasFinancas.Application.Interfaces;

namespace minhas_financas_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MfScorePersonasController : ControllerBase
    {
        private readonly IMfScorePersonaAppService _appService;

        public MfScorePersonasController(IMfScorePersonaAppService appService)
        {
            _appService = appService;
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTodas()
        {
            var retorno = await _appService.BuscarTodasAsync();
            return StatusCode((int)retorno.HttpStatusCode, retorno);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> BuscarUma(Guid id)
        {
            var retorno = await _appService.BuscarUmaAsync(id);
            return StatusCode((int)retorno.HttpStatusCode, retorno);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] SalvarMfScorePersonaDTO dto)
        {
            var retorno = await _appService.CadastrarAsync(dto);
            return StatusCode((int)retorno.HttpStatusCode, retorno);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Editar(Guid id, [FromBody] SalvarMfScorePersonaDTO dto)
        {
            var retorno = await _appService.EditarAsync(id, dto);
            return StatusCode((int)retorno.HttpStatusCode, retorno);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Inativar(Guid id)
        {
            var retorno = await _appService.InativarAsync(id);
            return StatusCode((int)retorno.HttpStatusCode, retorno);
        }

        [HttpPost("{id:guid}/RodarScore")]
        public async Task<IActionResult> RodarScore(Guid id)
        {
            var retorno = await _appService.RodarScoreAsync(id);
            return StatusCode((int)retorno.HttpStatusCode, retorno);
        }

        [HttpPost("{id:guid}/MarcarAuditada")]
        public async Task<IActionResult> MarcarAuditada(Guid id)
        {
            var retorno = await _appService.MarcarAuditadaAsync(id);
            return StatusCode((int)retorno.HttpStatusCode, retorno);
        }

        [HttpPost("{id:guid}/MarcarCasoCanonico")]
        public async Task<IActionResult> MarcarCasoCanonico(Guid id)
        {
            var retorno = await _appService.MarcarCasoCanonicoAsync(id);
            return StatusCode((int)retorno.HttpStatusCode, retorno);
        }
    }
}
