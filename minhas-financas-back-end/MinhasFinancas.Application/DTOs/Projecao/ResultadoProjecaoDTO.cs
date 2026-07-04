namespace MinhasFinancas.Application.DTOs.Projecao
{
    public class ResultadoProjecaoDTO
    {
        public decimal RendaManualTotal { get; set; } = decimal.Zero;
        public decimal ValorAcumuladoInicial { get; set; } = decimal.Zero;
        public decimal ValorObjetivo { get; set; } = decimal.Zero;
        public decimal ValorRestanteParaObjetivo { get; set; } = decimal.Zero;
        public decimal PercentualConcluido { get; set; } = decimal.Zero;
        public string? MesObjetivo { get; set; }
        public int? QuantidadeMesesParaObjetivo { get; set; }
        public bool ObjetivoAlcancado { get; set; }
        public List<LinhaResultadoProjecaoDTO> Linhas { get; set; } = new();
    }
}
