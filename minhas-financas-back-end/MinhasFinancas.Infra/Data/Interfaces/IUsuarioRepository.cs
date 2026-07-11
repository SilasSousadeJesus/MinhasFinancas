using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<bool> ExisteUsuarioAsync(string usuarioId);
        Task<List<string>> BuscarIdsUsuariosAtivosAsync();
        Task<List<Usuario>> BuscarUsuariosParaLaboratorioAsync();
        Task<List<Usuario>> BuscarUsuariosSinteticosAsync();
        Task<Usuario?> BuscarResumoUsuarioAsync(string usuarioId);
        Task<Usuario?> BuscarPorEmailAsync(string email);
        Task DeletarUsuarioESeusDados(Usuario elemento);
    }
}
