using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Domain.Services.Relatorios.PorAno.ClassesDoRelatorio
{
    public class ValorPatrimonio
    {
        public int Ano { get; set; }
        public string TipoPatrimonio { get; set; } = string.Empty;
        public decimal ValorAtivo { get; set; } = decimal.Zero;
        public decimal ValorPassivo { get; set; } = decimal.Zero;
    }
}
