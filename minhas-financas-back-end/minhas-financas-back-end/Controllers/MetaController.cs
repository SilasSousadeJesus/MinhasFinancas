using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.Banco;
using MinhasFinancas.Application.DTOs.Meta;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetaController : ControllerBase
    {
        private readonly IMetaAppService _appService;

        public MetaController(IMetaAppService appService)
        {
            _appService = appService;
        }


        [HttpPost("Cadastrar")]
        public async Task<IActionResult> Cadastrar(CadastrarMetaDTO cadastrarMetaDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.CadastrarElementoAsync(cadastrarMetaDTO);

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

        [HttpGet("BuscarTodosAsMetas/{usuarioId}")]
        public async Task<IActionResult> BuscarTodosAsMetas([FromRoute] string usuarioId)
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

        [HttpGet("BuscarUmaMeta/{usuarioId}/{metaId}")]
        public async Task<IActionResult> BuscarUmaConta([FromRoute] string usuarioId, [FromRoute] Guid metaId)
        {

            var dados = await _appService.BuscarUmElementoAsync(usuarioId, metaId);

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

        [HttpPut("EditarMeta/{usuarioId}/{metaId}")]
        public async Task<IActionResult> EditarMeta([FromRoute] string usuarioId, [FromRoute] Guid metaId, [FromBody] EditarMetalDTO editarMetalDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.EditarElementoAsync(usuarioId, metaId, editarMetalDTO);

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

        [HttpDelete("DeletarMeta/{usuarioId}/{metaId}")]
        public async Task<IActionResult> DeletarConta(string usuarioId, Guid metaId)
        {

            var dados = await _appService.DeletarElementoAsync(usuarioId, metaId);

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

        [HttpPost("AtualizarAndamentoMeta/{idPatrono}/{elementoId}/{valor}")]
        public async Task<IActionResult> AtualizarAndamentoMeta([FromRoute]  string idPatrono, [FromRoute] Guid elementoId, [FromRoute] decimal valor)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.AtualizarAndamentoMetaAsync(idPatrono, elementoId, valor);

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
