using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.Usuario;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioAppService _appService;
        public UsuarioController(IUsuarioAppService usuarioAppService)
        {
            _appService = usuarioAppService;
        }

        [HttpPost("Cadastrar")]
        public async Task<IActionResult> CadastrarUsuario([FromBody] CadastrarUsuarioDTO cadastroUsuarioDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.Cadastrar(cadastroUsuarioDTO);

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

        [HttpGet("BuscarUmUsuario/{usuarioId}")]
        public async Task<IActionResult> BuscarUmUsuario([FromRoute] string usuarioId)
        {

            var dados = await _appService.BuscarUmUsuario(usuarioId);

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

        [HttpGet("BuscarTodosOsUsuario")]
        public async Task<IActionResult> BuscarTodosOsUsuario()
        {

            var dados = await _appService.BuscarTodosOsUsuario();

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


        [HttpPatch("EditarUsuario/{usuarioId}")]
        public async Task<IActionResult> EditarUsuario([FromRoute] string usuarioId, [FromBody] EditarUsuarioDTO editarUsuarioDTO)
        {

            var dados = await _appService.EditarUsuario(usuarioId, editarUsuarioDTO);

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

        [HttpDelete("DeletarUsuario/{usuarioId}")]
        public async Task<IActionResult> DeletarUsuario([FromRoute] string usuarioId)
        {

            var dados = await _appService.DeletarUsuario(usuarioId);

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
