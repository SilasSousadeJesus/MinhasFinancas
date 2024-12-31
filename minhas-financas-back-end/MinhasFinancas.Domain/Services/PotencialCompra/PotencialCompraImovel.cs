namespace MinhasFinancas.Domain.Services.PotencialCompra
{
    public class PotencialCompraImovel
    {
        public decimal RendaMensal { get; set; }
        public decimal Entrada { get; set; }
        public int Prazo { get; set; } // em anos
        public decimal ValorFinanciado { get; set; } // em anos
        public decimal PotencialCompra { get; set; } // em anos

        public PotencialCompraImovel(decimal rendaMensal, decimal entrada, int prazo = 30)
        {
            RendaMensal = rendaMensal;
            Entrada = entrada;
            Prazo = prazo;

            ValorFinanciado = CalcularValorFinanciado();
            PotencialCompra = CalcularPotencialCompra();
        }

        public decimal CalcularValorFinanciado()
        {
            // Supondo que a parcela não pode comprometer mais que 30% da renda mensal
            decimal parcelaMaxima = RendaMensal * 0.30m;
            int numeroParcelas = Prazo * 12;
            decimal valorFinanciado = parcelaMaxima * numeroParcelas;
            return valorFinanciado;
        }

        public decimal CalcularPotencialCompra()
        {
            decimal valorFinanciado = CalcularValorFinanciado();
            return Entrada + valorFinanciado;
        }
    }
}
