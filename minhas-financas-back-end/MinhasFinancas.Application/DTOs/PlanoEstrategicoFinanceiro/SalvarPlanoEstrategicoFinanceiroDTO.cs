namespace MinhasFinancas.Application.DTOs.PlanoEstrategicoFinanceiro
{
    public class SalvarPlanoEstrategicoFinanceiroDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? Observacao { get; set; }
        public DateTime? DataInicioVigencia { get; set; }
        public List<ObjetivoPlanoEstrategicoDTO> Objetivos { get; set; } = [];
    }
}
