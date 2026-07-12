namespace MinhasFinancas.Domain.Services.AnaliseFinanceira
{
    public static class CalibradorNumericoMfScore
    {
        public static int CalcularNotaLiquidez(
            decimal percentualReservaFormada,
            decimal mesesParaFormarReservaIdeal,
            bool possuiCapacidadeFormacaoReserva,
            bool possuiFluxoNegativoAtual,
            int mesesConsecutivosFluxoNegativo,
            bool possuiInadimplencia)
        {
            var scoreReservaFormada = InterpolarCrescente(
                percentualReservaFormada,
                (0m, 18m),
                (10m, 28m),
                (25m, 42m),
                (50m, 58m),
                (75m, 72m),
                (100m, 88m),
                (150m, 100m));

            var scoreCapacidade = !possuiCapacidadeFormacaoReserva
                ? 0m
                : mesesParaFormarReservaIdeal <= 0m
                    ? 100m
                    : InterpolarDecrescente(
                        mesesParaFormarReservaIdeal,
                        (0m, 100m),
                        (4m, 90m),
                        (8m, 75m),
                        (12m, 55m),
                        (24m, 30m),
                        (48m, 10m));

            var nota = (scoreReservaFormada * 0.70m) + (scoreCapacidade * 0.30m);

            if (percentualReservaFormada >= 100m)
            {
                nota = Math.Max(nota, 88m);
            }
            else if (percentualReservaFormada >= 75m)
            {
                nota = Math.Max(nota, 78m);
            }
            else if (percentualReservaFormada >= 50m && possuiCapacidadeFormacaoReserva)
            {
                nota = Math.Max(nota, 68m);
            }
            else if (percentualReservaFormada >= 25m && scoreCapacidade >= 75m)
            {
                nota = Math.Max(nota, 60m);
            }
            else if (percentualReservaFormada <= 0m && possuiCapacidadeFormacaoReserva && scoreCapacidade >= 75m)
            {
                nota = Math.Max(nota, 50m);
            }

            if (!possuiCapacidadeFormacaoReserva && percentualReservaFormada < 25m)
            {
                nota = Math.Min(nota, 25m);
            }

            if (possuiFluxoNegativoAtual)
            {
                nota = Math.Min(nota, percentualReservaFormada >= 50m ? 58m : 45m);
            }

            if (mesesConsecutivosFluxoNegativo >= 3)
            {
                nota = Math.Min(nota, percentualReservaFormada >= 75m ? 52m : 38m);
            }

            if (possuiInadimplencia)
            {
                nota = Math.Min(nota, percentualReservaFormada >= 100m ? 45m : 32m);
            }

            return NormalizarNota(nota);
        }

        public static int CalcularNotaFluxoDeCaixa(
            decimal percentualEconomiaAtual,
            decimal relacaoEconomiaMeta,
            decimal comprometimentoRendaAtual,
            bool possuiFluxoNegativoAtual,
            int mesesConsecutivosFluxoNegativo,
            bool possuiInadimplencia)
        {
            var scorePercentualEconomia = InterpolarCrescente(
                percentualEconomiaAtual,
                (-40m, 0m),
                (-20m, 10m),
                (0m, 40m),
                (5m, 55m),
                (10m, 68m),
                (20m, 82m),
                (30m, 92m),
                (40m, 100m));

            var scoreRelacaoMeta = relacaoEconomiaMeta <= 0m
                ? (percentualEconomiaAtual > 0m ? 70m : 30m)
                : percentualEconomiaAtual < 0m
                    ? 10m
                    : InterpolarCrescente(
                        relacaoEconomiaMeta,
                        (0m, 30m),
                        (0.50m, 55m),
                        (1.00m, 80m),
                        (1.50m, 92m),
                        (2.00m, 100m));

            var scoreComprometimento = InterpolarDecrescente(
                comprometimentoRendaAtual,
                (0m, 100m),
                (40m, 95m),
                (60m, 88m),
                (80m, 76m),
                (95m, 66m),
                (110m, 42m),
                (130m, 12m),
                (150m, 0m));

            var nota = (scorePercentualEconomia * 0.45m) + (scoreRelacaoMeta * 0.20m) + (scoreComprometimento * 0.35m);

            if (!possuiFluxoNegativoAtual &&
                !possuiInadimplencia &&
                mesesConsecutivosFluxoNegativo == 0 &&
                percentualEconomiaAtual >= 5m &&
                comprometimentoRendaAtual <= 95m)
            {
                nota = Math.Max(nota, 68m);
            }

            if (!possuiFluxoNegativoAtual &&
                !possuiInadimplencia &&
                mesesConsecutivosFluxoNegativo == 0 &&
                percentualEconomiaAtual >= 8m &&
                comprometimentoRendaAtual <= 95m)
            {
                nota = Math.Max(nota, 74m);
            }

            if (!possuiFluxoNegativoAtual &&
                !possuiInadimplencia &&
                mesesConsecutivosFluxoNegativo == 0 &&
                percentualEconomiaAtual >= 10m)
            {
                nota = Math.Max(nota, comprometimentoRendaAtual <= 80m ? 80m : 76m);
            }

            if (!possuiFluxoNegativoAtual &&
                !possuiInadimplencia &&
                percentualEconomiaAtual >= 20m &&
                comprometimentoRendaAtual <= 75m)
            {
                nota = Math.Max(nota, 88m);
            }

            if (possuiFluxoNegativoAtual || percentualEconomiaAtual < 0m)
            {
                nota = Math.Min(nota, comprometimentoRendaAtual > 115m ? 32m : 42m);
            }

            if (mesesConsecutivosFluxoNegativo >= 3)
            {
                nota = Math.Min(nota, 38m);
            }

            if (possuiInadimplencia)
            {
                nota = Math.Min(nota, 34m);
            }

            return NormalizarNota(nota);
        }

        public static int CalcularNotaEndividamento(
            decimal exposicaoPassivosAtual,
            decimal comprometimentoFuturo30Dias,
            decimal pressao90Dias,
            decimal pressao180Dias,
            decimal pressao365Dias,
            bool possuiInadimplencia,
            bool possuiFluxoNegativoAtual,
            decimal patrimonioLiquidoAtual)
        {
            var scoreExposicaoPassivos = InterpolarDecrescente(
                exposicaoPassivosAtual,
                (0m, 100m),
                (20m, 94m),
                (35m, 84m),
                (55m, 68m),
                (85m, 42m),
                (130m, 15m),
                (180m, 0m));

            var score30Dias = InterpolarDecrescente(
                comprometimentoFuturo30Dias,
                (0m, 100m),
                (50m, 94m),
                (70m, 84m),
                (90m, 72m),
                (110m, 52m),
                (130m, 25m),
                (160m, 5m));

            var score90Dias = InterpolarDecrescente(
                pressao90Dias,
                (0m, 100m),
                (60m, 95m),
                (80m, 86m),
                (100m, 74m),
                (120m, 56m),
                (150m, 28m),
                (180m, 10m));

            var score180Dias = InterpolarDecrescente(
                pressao180Dias,
                (0m, 100m),
                (70m, 96m),
                (90m, 88m),
                (110m, 78m),
                (130m, 60m),
                (160m, 35m),
                (190m, 12m));

            var score365Dias = InterpolarDecrescente(
                pressao365Dias,
                (0m, 100m),
                (75m, 97m),
                (95m, 90m),
                (115m, 80m),
                (140m, 64m),
                (170m, 40m),
                (200m, 15m));

            var nota = (scoreExposicaoPassivos * 0.45m)
                + (score30Dias * 0.30m)
                + (score90Dias * 0.15m)
                + (score180Dias * 0.06m)
                + (score365Dias * 0.04m);

            if (!possuiInadimplencia &&
                patrimonioLiquidoAtual > 0m &&
                exposicaoPassivosAtual <= 25m &&
                comprometimentoFuturo30Dias <= 95m)
            {
                nota = Math.Max(nota, 72m);
            }

            if (!possuiInadimplencia &&
                !possuiFluxoNegativoAtual &&
                exposicaoPassivosAtual <= 20m &&
                comprometimentoFuturo30Dias <= 90m &&
                pressao90Dias <= 95m)
            {
                nota = Math.Max(nota, 78m);
            }

            if (!possuiInadimplencia &&
                exposicaoPassivosAtual <= 150m &&
                comprometimentoFuturo30Dias <= 115m)
            {
                nota = Math.Max(nota, 48m);
            }

            if (!possuiInadimplencia &&
                exposicaoPassivosAtual <= 60m &&
                comprometimentoFuturo30Dias <= 105m)
            {
                nota = Math.Max(nota, 60m);
            }

            if (!possuiInadimplencia &&
                exposicaoPassivosAtual <= 5m &&
                comprometimentoFuturo30Dias >= 60m)
            {
                nota = Math.Min(nota, 82m);
            }

            if (!possuiInadimplencia &&
                exposicaoPassivosAtual <= 25m &&
                comprometimentoFuturo30Dias >= 80m)
            {
                nota = Math.Min(nota, 78m);
            }

            if (comprometimentoFuturo30Dias >= 120m && pressao90Dias >= 120m)
            {
                nota = Math.Min(nota, 32m);
            }

            if (possuiInadimplencia)
            {
                nota = Math.Min(nota, 30m);
            }

            return NormalizarNota(nota);
        }

        public static decimal ObterPenalidadeFluxoNegativo(int mesesConsecutivos)
        {
            return mesesConsecutivos switch
            {
                >= 12 => 15m,
                >= 6 => 11m,
                >= 3 => 7m,
                >= 2 => 4m,
                >= 1 => 2m,
                _ => 0m
            };
        }

        public static decimal InterpolarCrescente(decimal valorAtual, params (decimal Valor, decimal Nota)[] pontos)
        {
            return Interpolar(valorAtual, pontos);
        }

        public static decimal InterpolarDecrescente(decimal valorAtual, params (decimal Valor, decimal Nota)[] pontos)
        {
            return Interpolar(valorAtual, pontos);
        }

        private static decimal Interpolar(decimal valorAtual, params (decimal Valor, decimal Nota)[] pontos)
        {
            if (pontos.Length == 0)
            {
                return 0m;
            }

            var ordenados = pontos.OrderBy(p => p.Valor).ToArray();

            if (valorAtual <= ordenados[0].Valor)
            {
                return ordenados[0].Nota;
            }

            if (valorAtual >= ordenados[^1].Valor)
            {
                return ordenados[^1].Nota;
            }

            for (var indice = 1; indice < ordenados.Length; indice++)
            {
                var anterior = ordenados[indice - 1];
                var atual = ordenados[indice];

                if (valorAtual <= atual.Valor)
                {
                    var intervaloValor = atual.Valor - anterior.Valor;
                    if (intervaloValor == 0m)
                    {
                        return atual.Nota;
                    }

                    var progresso = (valorAtual - anterior.Valor) / intervaloValor;
                    return anterior.Nota + ((atual.Nota - anterior.Nota) * progresso);
                }
            }

            return ordenados[^1].Nota;
        }

        private static int NormalizarNota(decimal nota)
        {
            return (int)Math.Round(Math.Clamp(nota, 0m, 100m), MidpointRounding.AwayFromZero);
        }
    }
}
