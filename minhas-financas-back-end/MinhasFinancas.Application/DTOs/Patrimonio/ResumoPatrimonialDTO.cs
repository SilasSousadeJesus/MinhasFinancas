namespace MinhasFinancas.Application.DTOs.Patrimonio
{
    public class ResumoPatrimonialDTO
    {
        public decimal TotalAtivos { get; set; }
        public decimal TotalPassivos { get; set; }
        public decimal PatrimonioLiquido { get; set; }
        public int QuantidadeAtivos { get; set; }
        public int QuantidadePassivos { get; set; }
    }
}
