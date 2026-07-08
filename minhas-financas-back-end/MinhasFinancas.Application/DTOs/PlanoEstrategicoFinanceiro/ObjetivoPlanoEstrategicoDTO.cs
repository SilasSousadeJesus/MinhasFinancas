using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Application.DTOs.PlanoEstrategicoFinanceiro
{
    public class ObjetivoPlanoEstrategicoDTO
    {
        public Guid? Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public EnumPrioridadeObjetivoPlanoEstrategico Prioridade { get; set; } = EnumPrioridadeObjetivoPlanoEstrategico.Media;
        public EnumStatusObjetivoPlanoEstrategico Status { get; set; } = EnumStatusObjetivoPlanoEstrategico.Planejado;
        public int Ordem { get; set; }
        public DateTime? DataAlvo { get; set; }
        public decimal? ValorAlvo { get; set; }
        public decimal? ValorAtual { get; set; }
        public string? Observacao { get; set; }
    }
}
