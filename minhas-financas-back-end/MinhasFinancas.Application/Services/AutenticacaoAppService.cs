using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MinhasFinancas.Application.DTOs;
using MinhasFinancas.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static MinhasFinancas.Application.Configurations.Configurations;
using MinhasFinancas.Application.ViewModel;

namespace MinhasFinancas.Application.Services
{
    public class AutenticacaoAppService : IAutenticacaoAppService
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser>   _userManager;
        private readonly JwtOptions _jwtOptions;

        public AutenticacaoAppService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IOptions<JwtOptions> jwtOptions) {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
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

            var credenciais = resultado.Succeeded ? await GerarCredenciais(loginDTO.Email) : null;

            return new RetornoGenerico
            {
                Sucesso = resultado.Succeeded,
                HttpStatusCode = resultado.Succeeded ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Unauthorized,
                MensagemSistema = mensagem,
                MensagemUsuario = mensagem,
                Dados = new TokenViewModel(credenciais.Item1, credenciais.Item2)
            };
        }

        private async Task<Tuple<string, string>> GerarCredenciais(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var accessTokenClaims = await ObterClaims(user, adicionarClaimsUsuario: true);
            var refreshTokenClaims = await ObterClaims(user, adicionarClaimsUsuario: false);

            var teste = _jwtOptions.AccessTokenExpiration;

            var dataExpiracaoAccessToken = DateTime.Now.AddSeconds(_jwtOptions.AccessTokenExpiration);
            var dataExpiracaoRefreshToken = DateTime.Now.AddSeconds(_jwtOptions.RefreshTokenExpiration);

            var accessToken = GerarToken(accessTokenClaims, dataExpiracaoAccessToken);
            var refreshToken = GerarToken(refreshTokenClaims, dataExpiracaoRefreshToken);

            return Tuple.Create(accessToken, refreshToken);
        }

        private string GerarToken(IEnumerable<Claim> claims, DateTime dataExpiracao)
        {
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: dataExpiracao,
                signingCredentials: _jwtOptions.SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private async Task<IList<Claim>> ObterClaims(IdentityUser user, bool adicionarClaimsUsuario)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()));

            if (adicionarClaimsUsuario)
            {
                var userClaims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);

                claims.AddRange(userClaims);

                foreach (var role in roles)
                    claims.Add(new Claim("role", role));
            }

            return claims;
        }

    }
}
