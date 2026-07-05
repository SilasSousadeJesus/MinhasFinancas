using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IPassivoRepository : IRepository<Passivo>
    {
        Task CadastrarPermanenciaAsync(PermanenciaPassivo permanencia);
    }
}
