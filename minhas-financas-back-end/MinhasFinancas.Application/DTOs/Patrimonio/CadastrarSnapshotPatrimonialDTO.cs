namespace MinhasFinancas.Application.DTOs.Patrimonio
{
    public class CadastrarSnapshotPatrimonialDTO
    {
        public DateTime DataReferencia { get; set; }
        public string Observacao { get; set; } = string.Empty;
    }
}
