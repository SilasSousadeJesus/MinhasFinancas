using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<bool> ExisteUsuarioAsync(string usuarioId);
        Task<List<string>> BuscarIdsUsuariosAtivosAsync();
        Task<List<Usuario>> BuscarUsuariosParaLaboratorioAsync();
        Task<Usuario?> BuscarResumoUsuarioAsync(string usuarioId);
        Task DeletarUsuarioESeusDados(Usuario elemento);
    }
}
