using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Application.DTOs.PotencialCompra
{
    public class PotencialCompraDTO
    {
        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        public decimal RendaMensal { get; set; } = 0;
        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        public decimal EntradaFGTS { get; set; } = 0;
    }
}
