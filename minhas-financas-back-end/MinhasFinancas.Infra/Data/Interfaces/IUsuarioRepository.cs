using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<bool> ExisteUsuarioAsync(string usuarioId);
        Task<List<string>> BuscarIdsUsuariosAtivosAsync();
        Task DeletarUsuarioESeusDados(Usuario elemento);
    }
}
