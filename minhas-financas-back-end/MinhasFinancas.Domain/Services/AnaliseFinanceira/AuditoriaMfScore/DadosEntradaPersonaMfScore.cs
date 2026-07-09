namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.AuditoriaMfScore
{
    public class DadosEntradaPersonaMfScore
    {
        public decimal Renda { get; set; }
        public decimal Despesas { get; set; }
        public decimal Reserva { get; set; }
        public decimal Patrimonio { get; set; }
        public decimal Passivos { get; set; }
        public decimal ObrigacoesFuturas30Dias { get; set; }
        public decimal ObrigacoesFuturas90Dias { get; set; }
        public decimal ObrigacoesFuturas180Dias { get; set; }
        public decimal ObrigacoesFuturas12Meses { get; set; }
    }
}
