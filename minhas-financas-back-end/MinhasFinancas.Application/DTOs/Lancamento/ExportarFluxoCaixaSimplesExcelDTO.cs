namespace MinhasFinancas.Application.DTOs.Lancamento
{
    public class ExportarFluxoCaixaSimplesExcelDTO
    {
        public string TipoPeriodo { get; set; } = "mes-atual";
        public int? Ano { get; set; }
        public int? Mes { get; set; }
        public int? AnoInicial { get; set; }
        public int? MesInicial { get; set; }
        public int? AnoFinal { get; set; }
        public int? MesFinal { get; set; }
    }
}
