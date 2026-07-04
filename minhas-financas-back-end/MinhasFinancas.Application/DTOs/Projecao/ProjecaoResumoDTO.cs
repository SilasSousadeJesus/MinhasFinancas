namespace MinhasFinancas.Application.DTOs.Projecao
{
    public class ProjecaoResumoDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataInicial { get; set; }
        public decimal ValorAcumuladoInicial { get; set; }
        public decimal ValorObjetivo { get; set; }
        public int MesesLimite { get; set; }
        public int QuantidadeRendas { get; set; }
        public decimal RendaManualTotal { get; set; }
        public ResultadoProjecaoDTO? ResultadoAtual { get; set; }
    }
}
