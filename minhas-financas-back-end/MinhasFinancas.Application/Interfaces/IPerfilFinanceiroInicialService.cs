using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IPerfilFinanceiroInicialService
    {
        Task<PerfilFinanceiro> GarantirPerfilFinanceiroValidoAsync(string usuarioId);
    }
}
