using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores
{
    public class ReservaEmergenciaIdealIndicador : ICalculadorIndicadorFinanceiro
    {
        public CodigoIndicadorFinanceiro Codigo => CodigoIndicadorFinanceiro.ReservaEmergenciaIdeal;

        public IndicadorFinanceiro Calcular(ContextoAnaliseFinanceira contexto, DadosReferenciaAnaliseFinanceira dadosReferencia)
        {
            var percentualDesejado = contexto.ConfiguracaoPerfilFinanceiro?.PercentualReservaEmergenciaDesejado ?? 0m;
            var mesesDesejados = contexto.ConfiguracaoPerfilFinanceiro?.MesesReservaEmergenciaDesejados ?? 0;
            var configurado = percentualDesejado > 0 && mesesDesejados > 0;

            return new IndicadorFinanceiro
            {
                Codigo = Codigo,
                Nome = "Reserva de emergência ideal",
                ValorAtual = dadosReferencia.ReservaEmergenciaIdealConfigurada,
                ValorIdeal = dadosReferencia.BaseReservaEmergenciaIntegral,
                Percentual = percentualDesejado,
                Status = configurado ? StatusIndicadorFinanceiro.Bom : StatusIndicadorFinanceiro.Atencao,
                Descricao = "Meta de reserva calculada a partir das despesas mensais, dos meses desejados e do percentual definido no perfil financeiro.",
                Observacao = $"Perfil atual: {mesesDesejados} mês(es) de reserva com fator de {percentualDesejado:N2}%.",
                Formato = FormatoValorIndicadorFinanceiro.Moeda
            };
        }
    }
}
