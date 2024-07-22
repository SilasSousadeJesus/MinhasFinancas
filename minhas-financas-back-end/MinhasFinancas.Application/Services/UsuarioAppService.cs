using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Application.DTOs.Categoria;
using MinhasFinancas.Application.DTOs.Usuario;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Application.Resources;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Application.Services
{
    public class UsuarioAppService : IUsuarioAppService
    {

        private string mensagemSistema = string.Empty;
        private string mensagemUsuario = string.Empty;

        private readonly IMapper _mapper;
        private readonly UserManager<Usuario> _userManager;
        private readonly ICategoriaRepository _categoriaRepository;


        public UsuarioAppService(UserManager<Usuario> userManager, ICategoriaRepository categoriaRepository, IMapper mapper)
        {
            _userManager = userManager;
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;   
        }


        public async Task<RetornoGenerico> Cadastrar(CadastrarUsuarioDTO cadastroUsuarioDTO)
        {

            var identityUser = new Usuario
            {
                Nome = cadastroUsuarioDTO.Nome,
                UserName = cadastroUsuarioDTO.Email,
                Email = cadastroUsuarioDTO.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, cadastroUsuarioDTO.Senha);
            if (result.Succeeded) {
                await _userManager.SetLockoutEnabledAsync(identityUser, false);
                await InformacoesComplementares(identityUser.Id);
            }

            string mensagem = result.Succeeded ? "Usuario criado com sucesso" : "Usuario não pode ser criado";
            mensagemSistema = mensagem;
            mensagemUsuario = mensagem;

            var statusCode = result.Succeeded ? System.Net.HttpStatusCode.Created : System.Net.HttpStatusCode.BadRequest;

            return new RetornoGenerico(result.Succeeded, mensagemSistema, mensagemUsuario, statusCode);
        }

        public async Task<RetornoGenerico> BuscarUmUsuario(string UsuarioId)
        {
            var user = await _userManager.FindByIdAsync(UsuarioId);

            mensagemSistema = user == null ? "Usuario não encontrado" : "Usuario Encontrado";
            mensagemUsuario = user == null ? "Usuario não encontrado" : "Usuario Encontrado";

            var resultado = user == null ? false : true;

            return new RetornoGenerico
            {
                Sucesso = resultado,
                HttpStatusCode = resultado ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                MensagemSistema = mensagemSistema,
                MensagemUsuario = mensagemUsuario,
                Dados = user
            };
        }
        public async Task<RetornoGenerico> BuscarTodosOsUsuario()
        {
            var users = await _userManager.Users.ToListAsync();

            var mensagem = users.Count > 0 ? $"{users.Count} usuários cadastrados" : "Não há usuários cadastrados";
            var sucesso = users.Count > 0;

            return new RetornoGenerico
            {
                Sucesso = sucesso,
                HttpStatusCode = sucesso ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                MensagemSistema = mensagem,
                MensagemUsuario = mensagem,
                Dados = users
            };
        }

        public async Task<RetornoGenerico> DeletarUsuario(string usuarioId)
        {
            var user = await _userManager.FindByIdAsync(usuarioId);
            var mensagem = user == null ? "Usuario não encontrado" : "Usuario deletado";
            var sucesso = user != null;

            if (sucesso)
            {
                await _userManager.DeleteAsync(user);
            }

            return new RetornoGenerico
            {
                Sucesso = sucesso,
                HttpStatusCode = sucesso ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                MensagemSistema = mensagem,
                MensagemUsuario = mensagem,
                Dados = null
            };
        }

        public async Task<RetornoGenerico> EditarUsuario(string usuarioId, EditarUsuarioDTO editarUsuarioDTO)
        {
            var user = await _userManager.FindByIdAsync(usuarioId);
            if (user == null)
            {
                return new RetornoGenerico
                {
                    Sucesso = false,
                    HttpStatusCode = System.Net.HttpStatusCode.NotFound,
                    MensagemSistema = "Usuário não encontrado",
                    MensagemUsuario = "Usuário não encontrado",
                    Dados = null
                };
            }

            user.Nome = editarUsuarioDTO.Nome != user.Nome ? editarUsuarioDTO.Nome : user.Nome;
            user.Email = editarUsuarioDTO.Email != user.Email ? editarUsuarioDTO.Email : user.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return new RetornoGenerico
                {
                    Sucesso = false,
                    HttpStatusCode = System.Net.HttpStatusCode.BadRequest,
                    MensagemSistema = "Erro ao atualizar usuário",
                    MensagemUsuario = "Erro ao atualizar usuário",
                    Dados = null
                };
            }

            return new RetornoGenerico
            {
                Sucesso = true,
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                MensagemSistema = "Usuário atualizado com sucesso",
                MensagemUsuario = "Usuário atualizado com sucesso",
                Dados = user
            };
        }

        private async Task InformacoesComplementares(string UsuarioId) 
        {
            var listaCategorias = CategoriasSubCategorias.ConstrutorCategoriasSubCategorias(UsuarioId);

            await _categoriaRepository.CadastrarListaDeCategoriasAsync(listaCategorias);
        }


    }
}
