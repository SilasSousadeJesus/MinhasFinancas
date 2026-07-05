namespace MinhasFinancas.Application.DTOs.Patrimonio
{
    public class LinhaEvolucaoPatrimonialDTO
    {
        public DateTime DataReferencia { get; set; }
        public decimal TotalAtivos { get; set; }
        public decimal TotalPassivos { get; set; }
        public decimal PatrimonioLiquido { get; set; }
    }
}
