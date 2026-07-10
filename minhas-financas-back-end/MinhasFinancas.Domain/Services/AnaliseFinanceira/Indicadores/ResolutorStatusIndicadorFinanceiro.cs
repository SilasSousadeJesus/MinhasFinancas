using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    internal static class ResolutorStatusIndicadorFinanceiro
    {
        public static StatusIndicadorFinanceiro ResolverMetaMinima(decimal valorAtual, decimal valorMeta)
        {
            if (valorMeta <= 0)
            {
                return valorAtual > 0 ? StatusIndicadorFinanceiro.Bom : StatusIndicadorFinanceiro.Atencao;
            }

            if (valorAtual >= valorMeta)
            {
                return StatusIndicadorFinanceiro.Excelente;
            }

            if (valorAtual <= 0)
            {
                return StatusIndicadorFinanceiro.Critico;
            }

            return StatusIndicadorFinanceiro.Atencao;
        }

        public static StatusIndicadorFinanceiro ResolverMetaMaxima(decimal valorAtual, decimal valorMeta)
        {
            if (valorAtual <= 0)
            {
                return StatusIndicadorFinanceiro.Excelente;
            }

            if (valorMeta <= 0)
            {
                return StatusIndicadorFinanceiro.Atencao;
            }

            if (valorAtual <= valorMeta)
            {
                return StatusIndicadorFinanceiro.Excelente;
            }

            return StatusIndicadorFinanceiro.Atencao;
        }

        public static StatusIndicadorFinanceiro ResolverProgresso(decimal percentualAtual)
        {
            if (percentualAtual >= 100)
            {
                return StatusIndicadorFinanceiro.Excelente;
            }

            if (percentualAtual > 0)
            {
                return StatusIndicadorFinanceiro.Atencao;
            }

            return StatusIndicadorFinanceiro.Critico;
        }

        public static StatusIndicadorFinanceiro ResolverFaixaCrescente(
            decimal valorAtual,
            decimal limiteExcelente,
            decimal limiteBom,
            decimal limiteAtencao)
        {
            if (valorAtual >= limiteExcelente)
            {
                return StatusIndicadorFinanceiro.Excelente;
            }

            if (valorAtual >= limiteBom)
            {
                return StatusIndicadorFinanceiro.Bom;
            }

            if (valorAtual >= limiteAtencao)
            {
                return StatusIndicadorFinanceiro.Atencao;
            }

            return StatusIndicadorFinanceiro.Critico;
        }

        public static StatusIndicadorFinanceiro ResolverFaixaDecrescente(
            decimal valorAtual,
            decimal limiteExcelente,
            decimal limiteBom,
            decimal limiteAtencao)
        {
            if (valorAtual <= limiteExcelente)
            {
                return StatusIndicadorFinanceiro.Excelente;
            }

            if (valorAtual <= limiteBom)
            {
                return StatusIndicadorFinanceiro.Bom;
            }

            if (valorAtual <= limiteAtencao)
            {
                return StatusIndicadorFinanceiro.Atencao;
            }

            return StatusIndicadorFinanceiro.Critico;
        }
    }
}
