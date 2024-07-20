using MinhasFinancas.Application.DTOs.Autenticacao;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IAutenticacaoAppService
    {
        Task<RetornoGenerico> Login(LoginDTO loginDTO);
    }
}
