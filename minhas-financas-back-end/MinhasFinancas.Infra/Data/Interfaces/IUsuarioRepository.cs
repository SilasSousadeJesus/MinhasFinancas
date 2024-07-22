using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IUsuarioRepository
    {
        Task DeletarUsuarioESeusDados(Usuario elemento);
    }
}
