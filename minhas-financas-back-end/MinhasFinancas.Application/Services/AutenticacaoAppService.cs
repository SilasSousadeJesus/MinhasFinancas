using MinhasFinancas.Application.DTOs;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.Application.Services
{
    internal class AutenticacaoAppService : IAutenticacaoAppService
    {
        public Task<RetornoGenerico> Cadastrar(CadastroUsuarioDTO loginDTO)
        {
            throw new NotImplementedException();
        }

        public Task<RetornoGenerico> Login(LoginDTO loginDTO)
        {
            throw new NotImplementedException();
        }
    }
}
