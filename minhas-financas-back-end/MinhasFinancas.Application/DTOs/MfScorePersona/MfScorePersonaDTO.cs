using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Application.DTOs.MfScorePersona
{
    public class MfScorePersonaDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string ObjetivoDaPersona { get; set; } = string.Empty;
        public decimal RendaMensal { get; set; }
        public decimal ReceitasPrevistas30Dias { get; set; }
        public decimal ReceitasPrevistas90Dias { get; set; }
        public decimal ReceitasPrevistas180Dias { get; set; }
        public decimal ReceitasPrevistas12Meses { get; set; }
        public decimal DespesasMensais { get; set; }
        public decimal Obrigacoes30Dias { get; set; }
        public decimal Obrigacoes90Dias { get; set; }
        public decimal Obrigacoes180Dias { get; set; }
        public decimal Obrigacoes12Meses { get; set; }
        public decimal ReservaEmergencia { get; set; }
        public decimal PatrimonioBruto { get; set; }
        public decimal Passivos { get; set; }
        public decimal PatrimonioLiquido { get; set; }
        public bool PossuiPerfilFinanceiroConfigurado { get; set; }
        public bool PossuiPlanoEstrategico { get; set; }
        public bool PossuiMetas { get; set; }
        public bool PossuiCompromissos { get; set; }
        public int CompromissosCumpridos { get; set; }
        public bool PossuiInadimplencia { get; set; }
        public int? ScoreHumanoSugerido { get; set; }
        public int? FaixaEsperadaMin { get; set; }
        public int? FaixaEsperadaMax { get; set; }
        public string? JustificativaNotaHumana { get; set; }
        public EnumStatusPersonaMfScore Status { get; set; }
        public bool EhCasoCanonico { get; set; }
        public string? Observacoes { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
