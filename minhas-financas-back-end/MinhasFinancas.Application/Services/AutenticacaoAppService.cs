using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MinhasFinancas.Application.DTOs.Autenticacao;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Application.ViewModel;
using MinhasFinancas.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static MinhasFinancas.Application.Configurations.Configurations;

namespace MinhasFinancas.Application.Services
{
    public class AutenticacaoAppService : IAutenticacaoAppService
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly JwtOptions _jwtOptions;

        public AutenticacaoAppService(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager, IOptions<JwtOptions> jwtOptions)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<RetornoGenerico> Login(LoginDTO loginDTO)
        {
            var mensagem = string.Empty;

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user is null) {
                return new RetornoGenerico
                {
                    Sucesso = false,
                    HttpStatusCode = System.Net.HttpStatusCode.NotFound,
                    MensagemSistema = "Usuario não encontrado",
                    MensagemUsuario = "Usuario não encontrado",
                    Dados = null
                };
            }

            var resultado = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Senha, false, true);

            mensagem = resultado.Succeeded ? "Login efetuado com sucesso" :
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

        private async Task<IList<Claim>> ObterClaims(Usuario user, bool adicionarClaimsUsuario)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, user.Nome));
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
