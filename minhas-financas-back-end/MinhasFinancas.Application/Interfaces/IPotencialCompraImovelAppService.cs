using MinhasFinancas.Application.DTOs.PotencialCompra;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IPotencialCompraImovelAppService
    {
        Task<RetornoGenerico> CalcularPotencialCompraImovel(PotencialCompraDTO potencialCompraDTO);
    }
}
