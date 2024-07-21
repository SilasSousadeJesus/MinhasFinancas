using MinhasFinancas.Application.DTOs.Usuario;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IUsuarioAppService
    {
        Task<RetornoGenerico> Cadastrar(CadastrarUsuarioDTO loginDTO);
        Task<RetornoGenerico> BuscarUmUsuario(string UsuarioId);
        Task<RetornoGenerico> BuscarTodosOsUsuario();
        Task<RetornoGenerico> EditarUsuario(string usuarioId, EditarUsuarioDTO editarUsuarioDTO);
        Task<RetornoGenerico> DeletarUsuario(string UsuarioId);
    }
}
