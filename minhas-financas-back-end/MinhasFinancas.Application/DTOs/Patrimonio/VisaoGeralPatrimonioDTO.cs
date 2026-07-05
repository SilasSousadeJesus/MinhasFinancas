namespace MinhasFinancas.Application.DTOs.Patrimonio
{
    public class VisaoGeralPatrimonioDTO
    {
        public ResumoPatrimonialDTO Resumo { get; set; } = new();
        public List<ItemAtivoPatrimonialDTO> Ativos { get; set; } = new();
        public List<ItemPassivoPatrimonialDTO> Passivos { get; set; } = new();
        public List<SnapshotPatrimonialDTO> Snapshots { get; set; } = new();
        public List<LinhaEvolucaoPatrimonialDTO> Evolucao { get; set; } = new();
    }
}
