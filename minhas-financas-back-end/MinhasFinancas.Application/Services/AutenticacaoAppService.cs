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

        public Task<RetornoGenerico> Cadastrar(CadastroUsuarioDTO cadastroUsuarioDTO)
        {
            var identityUser = new IdentityUser
            {
                UserName = cadastroUsuarioDTO.Email,
                Email = cadastroUsuarioDTO.Email,
                EmailConfirmed = true
            };
               

            throw new NotImplementedException();
        }

        public Task<RetornoGenerico> Login(LoginDTO loginDTO)
        {
            throw new NotImplementedException();
        }
    }
}
