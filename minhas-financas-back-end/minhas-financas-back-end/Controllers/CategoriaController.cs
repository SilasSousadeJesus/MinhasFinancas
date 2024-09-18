using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Application.DTOs.Categoria;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaAppService _appService;

        public CategoriaController(ICategoriaAppService categoriaAppService)
        {
            _appService = categoriaAppService;
        }

        // CATEGORIA
        [Authorize]
        [HttpPost("CadastrarCategoria")]
        public async Task<IActionResult> CadastrarCategoria([FromBody] CadastrarCategoriaDTO cadastrarCategoria)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.CadastrarElementoAsync(cadastrarCategoria);

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
        [HttpGet("BuscarTodosAsCategorias/{usuarioId}")]
        public async Task<IActionResult> BuscarTodosAsCategorias([FromRoute] string usuarioId)
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
        [HttpGet("BuscarUmaCategoria/{usuarioId}/{categoriaId}")]
        public async Task<IActionResult> BuscarUmaCategoria([FromRoute] string usuarioId, [FromRoute] Guid categoriaId)
        {

            var dados = await _appService.BuscarUmElementoAsync(usuarioId, categoriaId);

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
        [HttpPut("EditarCategoria/{usuarioId}/{categoriaId}")]
        public async Task<IActionResult> EditarCategoria([FromRoute] string usuarioId, [FromRoute] Guid categoriaId, [FromBody] EditarCategoriaDTO editarCategoria)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.EditarElementoAsync(usuarioId, categoriaId, editarCategoria);

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
        [HttpDelete("DeletarCategoria/{usuarioId}/{categoriaId}")]
        public async Task<IActionResult> DeletarCategoria([FromRoute] string usuarioId, [FromRoute] Guid categoriaId)
        {

            var dados = await _appService.DeletarElementoAsync(usuarioId, categoriaId);

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


        // SUBCATEGORIA
        [Authorize]
        [HttpPost("CadastrarSubCategoria/{usuarioId}/{categoriaId}")]
        public async Task<IActionResult> CadastrarSubCategoria([FromRoute] string usuarioId,[FromRoute] Guid categoriaId, [FromBody] CadastrarSubCategoriaDTO cadastrarCategoria)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var dados = await _appService.CadastrarSubCategoriaAsync(usuarioId, categoriaId, cadastrarCategoria);

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
        [HttpGet("BuscarTodosAsSubCategorias/{usuarioId}/{categoriaId}")]
        public async Task<IActionResult> BuscarTodosAsSubCategorias(string usuarioId, Guid categoriaId)
        {

            var dados = await _appService.BuscarTodosAsSubCategoriaAsync(usuarioId, categoriaId);

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
        [HttpGet("BuscarUmaSubCategoria/{categoriaId}/{subCategoriaId}")]
        public async Task<IActionResult> BuscarUmaSubCategoria([FromRoute] Guid categoriaId, [FromRoute] Guid subCategoriaId)
        {

            var dados = await _appService.BuscarUmaSubCategoriaAsync(categoriaId, subCategoriaId);

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
        [HttpPut("EditarSubCategoria/{usuarioId}/{categoriaId}/{subCategoriaId}")]
        public async Task<IActionResult> EditarSubCategoria([FromRoute] string usuarioId, [FromRoute] Guid categoriaId, [FromRoute] Guid subCategoriaId, [FromBody] EditarSubCategoriaDTO  editarSubCategoriaDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dados = await _appService.EditarSubCategoriaAsync(usuarioId, categoriaId, subCategoriaId, editarSubCategoriaDTO);

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
        [HttpDelete("DeletarSubCategoria/{usuarioId}/{categoriaId}/{subCategoriaId}")]
        public async Task<IActionResult> DeletarCategoria([FromRoute] string UsuarioId, [FromRoute] Guid categoriaId, [FromRoute] Guid subCategoriaId)
        {

            var dados = await _appService.DeletarSubCategoriaAsync(UsuarioId, categoriaId, subCategoriaId);

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
