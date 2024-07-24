namespace MinhasFinancas.Domain.Services.Relatorios.PorAno.ClassesDoRelatorio
{
    public class ValorPatrimonio
    {
        public string Ano { get; set; } = string.Empty;
        public decimal ValorAtivo { get; set; } = decimal.Zero;
        public decimal ValorPassivo { get; set; } = decimal.Zero;
    }
}
