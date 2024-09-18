using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.BemPatrimonial;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BemMaterialController : ControllerBase
    {
        private readonly IBemPatrimonialAppService _appService;

        public BemMaterialController(IBemPatrimonialAppService bemPatrimonialAppService)
        {
            _appService = bemPatrimonialAppService;
        }

        [Authorize]
        [HttpPost("CadastrarBemMaterial")]
        public async Task<IActionResult> CadastrarBemMaterial([FromBody] CadastrarBemPatrimonialDTO cadastrarBemPatrimonialDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.CadastrarElementoAsync(cadastrarBemPatrimonialDTO);

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
        [HttpGet("BuscarTodosOsBemMateriais/{usuarioId}")]
        public async Task<IActionResult> BuscarTodosOsBemMateriais([FromRoute] string usuarioId)
        {

            var dados = await _appService.BuscarTodosOsElementosAsync(usuarioId);

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
        [HttpGet("BuscarUmBemMaterial/{usuarioId}/{bemMaterialId}")]
        public async Task<IActionResult> BuscarUmBemMaterial([FromRoute] string usuarioId, [FromRoute] Guid bemMaterialId)
        {

            var dados = await _appService.BuscarUmElementoAsync(usuarioId, bemMaterialId);

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
        [HttpPut("EditarBemMaterial/{usuarioId}/{bemMaterialId}")]
        public async Task<IActionResult> EditarBemMaterial([FromRoute] string usuarioId, [FromRoute] Guid bemMaterialId, [FromBody] EditarBemPatrimonialDTO  editarCartaoDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.EditarElementoAsync(usuarioId, bemMaterialId, editarCartaoDTO);

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
        [HttpDelete("DeletarBemMaterial/{usuarioId}/{bemMaterialId}")]
        public async Task<IActionResult> DeletarBemMaterial([FromRoute] string usuarioId, [FromRoute] Guid bemMaterialId)
        {

            var dados = await _appService.DeletarElementoAsync(usuarioId, bemMaterialId);

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
