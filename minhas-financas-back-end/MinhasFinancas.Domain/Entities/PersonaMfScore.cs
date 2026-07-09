using MinhasFinancas.CrossCutting.Util.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class PersonaMfScore
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Descricao { get; set; } = string.Empty;

        [MaxLength(500)]
        public string ObjetivoDaPersona { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal RendaMensal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ReceitasPrevistas30Dias { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ReceitasPrevistas90Dias { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ReceitasPrevistas180Dias { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ReceitasPrevistas12Meses { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DespesasMensais { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Obrigacoes30Dias { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Obrigacoes90Dias { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Obrigacoes180Dias { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Obrigacoes12Meses { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ReservaEmergencia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PatrimonioBruto { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Passivos { get; set; }

        [Column(TypeName = "decimal(18,2)")]
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

        [MaxLength(2000)]
        public string? JustificativaNotaHumana { get; set; }

        public EnumStatusPersonaMfScore Status { get; set; } = EnumStatusPersonaMfScore.Rascunho;
        public bool EhCasoCanonico { get; set; }

        [MaxLength(2000)]
        public string? Observacoes { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    }
}
