namespace MinhasFinancas.Domain.Services.DashBoard.ClassesDoDashboard
{
    public class ProximoVencimentoDashboard
    {
        public string Descricao { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
        public DateTime DataVencimento { get; set; }
        public string Situacao { get; set; } = string.Empty;
    }
}
