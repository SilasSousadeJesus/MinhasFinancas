namespace MinhasFinancas.Application.ViewModel
{
    public class TokenViewModel
    {
        public TokenViewModel(string token, string refreshToken) {
            Token = token;
            RefrenshToken = refreshToken;
        }

        public string Token { get; set; } = string.Empty;
        public string RefrenshToken { get; set; } = string.Empty;
    }
}
