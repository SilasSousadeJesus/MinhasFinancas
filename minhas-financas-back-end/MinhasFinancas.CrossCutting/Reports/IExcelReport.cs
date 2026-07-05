namespace MinhasFinancas.CrossCutting.Reports
{
    public interface IExcelReport<in TModel>
    {
        ArquivoRelatorioDTO Gerar(TModel model);
    }
}
