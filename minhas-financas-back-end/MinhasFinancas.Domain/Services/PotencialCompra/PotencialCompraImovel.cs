namespace MinhasFinancas.Domain.Services.PotencialCompra
{
    public class PotencialCompraImovel
    {
        public decimal RendaMensal { get; private set; }
        public decimal EntradaFGTS { get; private set; }
        public decimal ValorImovel { get; private set; }
        public decimal ValorFinanciado { get; private set; }
        public decimal ITBIOutrasTaxas { get; private set; }
        public decimal ValorTotalComTaxas { get; private set; }


        public PotencialCompraImovel()
        {

        }

        public PotencialCompraImovel(decimal rendaMensal, decimal entradaFGTS)
        {
            RendaMensal = rendaMensal;
            EntradaFGTS = entradaFGTS;
            CalcularPotencialDeCompra();
        }

        private void CalcularPotencialDeCompra()
        {
            // 1. Calcula o valor máximo da parcela (30% da renda mensal)
            decimal valorMaximoParcela = RendaMensal * 0.30m; // 30% da renda mensal

            // 2. Estima o valor financiável com base na parcela máxima
            int prazoMeses = 360; 

            decimal valorFinanciadoMaximo = Math.Round(valorMaximoParcela * prazoMeses);

            // 3. Calcula o valor do imóvel 
 
            ValorImovel = valorFinanciadoMaximo;

            // valor financiado para ser a diferença entre o valor do imóvel e a entrada
            ValorFinanciado = valorFinanciadoMaximo - EntradaFGTS;

            // 4. Calcula ITBI e outras taxas (5% do valor do imóvel)
            decimal porcentagemTaxas = 0.05m;
            ITBIOutrasTaxas = ValorImovel * porcentagemTaxas;

            // 5. Valor total com taxas
            ValorTotalComTaxas = ValorImovel + ITBIOutrasTaxas;
        }

        public void ExibirResultado()
        {
            Console.WriteLine("Entrada: R$ " + EntradaFGTS.ToString("N2"));
            Console.WriteLine("Valor a financiar: R$ " + ValorFinanciado.ToString("N2"));
            Console.WriteLine("ITBI e outras taxas: R$ " + ITBIOutrasTaxas.ToString("N2"));
            Console.WriteLine("Valor total do imóvel com taxas: R$ " + ValorTotalComTaxas.ToString("N2"));
        }
    }
}
