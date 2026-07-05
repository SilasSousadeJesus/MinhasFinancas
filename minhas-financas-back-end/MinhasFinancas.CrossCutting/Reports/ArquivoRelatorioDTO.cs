namespace MinhasFinancas.CrossCutting.Reports
{
    public class ArquivoRelatorioDTO
    {
        public string NomeArquivo { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/octet-stream";
        public byte[] Conteudo { get; set; } = [];
    }
}
