using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IMetaRepository : IRepository<Meta>
    {
        Task AtualizarAndamentoMetaAsync(Meta elemento);
        Task CadastrarNovoAporteAsync(AporteMeta elemento);
    }
}
