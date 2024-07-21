using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.Banco;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BancoController : ControllerBase
    {
        private readonly IBancoAppService _appService;

        public BancoController(IBancoAppService bancoAppService) 
        {
            _appService = bancoAppService;  
        }

        [HttpPost("Cadastrar")]
        public async Task<IActionResult> CadastrarUsuario(CadastrarBancoDTO cadastroBancoDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.CadastrarElementoAsync(cadastroBancoDTO);

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


        [HttpGet("BuscarTodosOsBancos/{usuarioId}")]
        public async Task<IActionResult> BuscarTodosOsBancos([FromRoute] string usuarioId)
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

        [HttpGet("BuscarUmBanco/{usuarioId}/{bancoId}")]
        public async Task<IActionResult> BuscarUmBanco([FromRoute] string usuarioId, [FromRoute] Guid bancoId)
        {

            var dados = await _appService.BuscarUmElementoAsync(usuarioId, bancoId);

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

        [HttpPut("EditarBanco/{usuarioId}/{bancoId}")]
        public async Task<IActionResult> EditarBanco([FromRoute] string usuarioId, [FromRoute] Guid bancoId, [FromBody] EditarBancoDTO editarBancoDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.EditarElementoAsync(usuarioId, bancoId, editarBancoDTO);

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

        [HttpDelete("DeletarBanco/{usuarioId}/{bancoId}")]
        public async Task<IActionResult> DeletarBanco(string usuarioId, Guid bancoId)
        {

            var dados = await _appService.DeletarElementoAsync(usuarioId, bancoId);

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
