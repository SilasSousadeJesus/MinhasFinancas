using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IPatrimonioRepository
    {
        Task<List<SnapshotPatrimonial>> BuscarSnapshotsAsync(string usuarioId);
        Task CadastrarSnapshotAsync(SnapshotPatrimonial snapshot);
    }
}
