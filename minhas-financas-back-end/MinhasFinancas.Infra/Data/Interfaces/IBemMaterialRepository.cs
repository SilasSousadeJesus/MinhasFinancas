using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IBemMaterialRepository : IRepository<BemPatrimonial>
    {
        Task CadastrarElementoAsync(List<BemPatrimonial> listaElemento);
    }
}
