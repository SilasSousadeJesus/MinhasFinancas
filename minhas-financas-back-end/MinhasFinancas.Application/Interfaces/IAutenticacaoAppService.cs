using MinhasFinancas.Application.DTOs;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IAutenticacaoAppService
    {
        Task<RetornoGenerico> Login(LoginDTO loginDTO);
    }
}
