using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IBemMaterialRepository : IRepository<BemPatrimonial>
    {
        Task CadastrarElementoAsync(List<BemPatrimonial> listaElemento);

        Task<PermanenciaBemMaterial> BuscarUltimaDataPermanencia(Guid bemMaterialId);

        Task EditarUltimaDataPermanencia(PermanenciaBemMaterial ultimaDataPermanencia);
        Task CadastrarPermanenciaAsync(PermanenciaBemMaterial permanencia);
    }
}
