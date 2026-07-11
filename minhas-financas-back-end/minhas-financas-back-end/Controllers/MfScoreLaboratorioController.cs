using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.Interfaces;

namespace minhas_financas_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class MfScoreLaboratorioController : ControllerBase
    {
        private readonly IMfScoreLaboratorioAppService _appService;

        public MfScoreLaboratorioController(IMfScoreLaboratorioAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("Usuarios")]
        public async Task<IActionResult> BuscarUsuarios()
        {
            var retorno = await _appService.BuscarUsuariosAsync();
            return StatusCode((int)retorno.HttpStatusCode, retorno);
        }

        [HttpGet("Usuarios/{usuarioId}/Score")]
        public async Task<IActionResult> BuscarScoreUsuario(string usuarioId)
        {
            var retorno = await _appService.BuscarScoreUsuarioAsync(usuarioId);
            return StatusCode((int)retorno.HttpStatusCode, retorno);
        }

        [HttpPost("GerarBaseSimulacao")]
        public async Task<IActionResult> GerarBaseSimulacao()
        {
            var retorno = await _appService.GerarBaseSimulacaoAsync();
            return StatusCode((int)retorno.HttpStatusCode, retorno);
        }

        [HttpDelete("LimparBaseSimulacao")]
        public async Task<IActionResult> LimparBaseSimulacao()
        {
            var retorno = await _appService.LimparBaseSimulacaoAsync();
            return StatusCode((int)retorno.HttpStatusCode, retorno);
        }
    }
}
