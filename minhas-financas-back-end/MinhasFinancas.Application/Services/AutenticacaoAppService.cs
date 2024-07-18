using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MinhasFinancas.Application.DTOs;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.Application.Services
{
    public class AutenticacaoAppService : IAutenticacaoAppService
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser>   _userManager;
        //private readonly JwtOptions _jwtOptions; 

        public AutenticacaoAppService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager) {
            _signInManager = signInManager;
            _userManager = userManager;    
        }

        public async Task<RetornoGenerico> Cadastrar(CadastroUsuarioDTO cadastroUsuarioDTO)
        {

            var identityUser = new IdentityUser
            {
                UserName = cadastroUsuarioDTO.Email,
                Email = cadastroUsuarioDTO.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, cadastroUsuarioDTO.Senha);
            if (result.Succeeded) await _userManager.SetLockoutEnabledAsync(identityUser, false);

            var mensagem = result.Succeeded ? "Usuario criado com sucesso" : "Usuario não pode ser criado";
            var statusCode = result.Succeeded ? System.Net.HttpStatusCode.Created : System.Net.HttpStatusCode.BadRequest;

            return new RetornoGenerico(result.Succeeded, mensagem, mensagem, statusCode);
        }

        public async Task<RetornoGenerico> Login(LoginDTO loginDTO)
        {
            var resultado = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Senha, false, true);

            var mensagem = resultado.Succeeded ? "Login efetuado com sucesso" :
                           resultado.IsLockedOut ? "Esta Conta está bloqueada" :
                           resultado.IsNotAllowed ? "Esta Conta não tem permissão para fazer login" :
                           resultado.RequiresTwoFactor ? "É necessário confirmar o login no seu email" :
                           "Erro ao tentar efetuar o login";

            //var token = resultado.Succeeded ? await GerarRoken(loginDTO.Email) : null;
            string token =  null;

            return new RetornoGenerico
            {
                Sucesso = resultado.Succeeded,
                HttpStatusCode = resultado.Succeeded ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Unauthorized,
                MensagemSistema = mensagem,
                MensagemUsuario = mensagem,
                Dados = token
            };
        }

    }
}
