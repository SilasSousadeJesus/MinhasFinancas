using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacaoController : ControllerBase
    {
        private readonly IAutenticacaoAppService _appService;
        public AutenticacaoController(IAutenticacaoAppService autenticacaoAppService)
        {
            _appService = autenticacaoAppService;
        }

        //[HttpPost("Cadastrar")]
        //public async Task<IActionResult> CadastrarUsuario(CadastroUsuarioDTO cadastroUsuarioDTO)
        //{
        //    if(!ModelState.IsValid) return BadRequest(ModelState);

        //    var dados = await _appService.Cadastrar(cadastroUsuarioDTO);

        //    if (!dados.Sucesso)
        //    {
        //        return dados.HttpStatusCode switch
        //        {
        //            System.Net.HttpStatusCode.Unauthorized => Unauthorized(dados),
        //            System.Net.HttpStatusCode.NotFound => NotFound(dados),
        //            System.Net.HttpStatusCode.BadRequest => BadRequest(dados),
        //            System.Net.HttpStatusCode.InternalServerError => StatusCode(500, dados),
        //            _ => BadRequest(dados)
        //        };
        //    }

        //    return Ok(dados);
        //}

        [HttpPost("Login")]
        public async Task<IActionResult> LoginUsuario(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.Login(loginDTO);

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
