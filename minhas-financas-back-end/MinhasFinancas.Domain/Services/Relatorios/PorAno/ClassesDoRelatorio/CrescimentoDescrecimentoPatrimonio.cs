namespace MinhasFinancas.Domain.Services.Relatorios.PorAno.ClassesDoRelatorio
{
    public class CrescimentoDescrecimentoPatrimonio
    {
        public string Ano { get; set; } = string.Empty;
        public decimal PorcentagemCrescimentoAtivo { get; set; } = decimal.Zero;
        public decimal PorcentagemCrescimentoPassivo { get; set; } = decimal.Zero;
    }
}
