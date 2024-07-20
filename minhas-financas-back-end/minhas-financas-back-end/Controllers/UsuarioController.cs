using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> CadastrarUsuario(CadastroUsuarioDTO cadastroUsuarioDTO)
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

        [HttpGet("BuscarUmUsuario")]
        public async Task<IActionResult> BuscarUmUsuario(string UsuarioId)
        {

            var dados = await _appService.BuscarUmUsuario(UsuarioId);

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


        [HttpPatch("EditarUsuario")]
        public async Task<IActionResult> EditarUsuario(string usuarioId, EditarUsuarioDTO editarUsuarioDTO)
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

        [HttpDelete("DeletarUsuario")]
        public async Task<IActionResult> DeletarUsuario(string UsuarioId)
        {

            var dados = await _appService.DeletarUsuario(UsuarioId);

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
