using System.Globalization;
using System.Text;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;
using MinhasFinancas.Infra.IA.Enums;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Avaliadores
{
    public class AvaliadorConsistenciaEstrategica
    {
        private static readonly string[] PalavrasPositivas =
        [
            "pagar",
            "quitar",
            "reduzir",
            "diminuir",
            "guardar",
            "reservar",
            "investir",
            "aportar",
            "economizar",
            "fortalecer",
            "manter",
            "construir",
            "priorizar"
        ];

        private static readonly string[] PalavrasNegativas =
        [
            "comprar",
            "gastar",
            "parcelar",
            "financiar",
            "assumir",
            "aumentar",
            "contrair",
            "consumir",
            "trocar",
            "viajar",
            "renovar",
            "adquirir"
        ];

        private static readonly string[] PalavrasReserva =
        [
            "reserva",
            "emergencia",
            "emergência",
            "fundo de emergencia",
            "fundo de emergência"
        ];

        private static readonly string[] PalavrasDivida =
        [
            "divida",
            "dívida",
            "emprestimo",
            "empréstimo",
            "financiamento",
            "cartao",
            "cartão",
            "juros",
            "parcelamento"
        ];

        private static readonly string[] PalavrasPatrimonio =
        [
            "patrimonio",
            "patrimônio",
            "investimento",
            "investir",
            "aportar",
            "ativo",
            "bens"
        ];

        private static readonly string[] PalavrasMoradia =
        [
            "casa",
            "imovel",
            "imóvel",
            "apartamento",
            "moradia"
        ];

        private static readonly string[] PalavrasVeiculo =
        [
            "carro",
            "veiculo",
            "veículo",
            "automovel",
            "automóvel"
        ];

        public ConsistenciaEstrategicaIA Avaliar(
            ResumoFinanceiroIA resumoFinanceiroIA,
            PlanoEstrategicoFinanceiro? planoEstrategicoFinanceiro,
            InterpretacaoPlanoEstrategicoIA? interpretacaoPlanoEstrategico,
            string? perguntaUsuario = null,
            DecisaoFinanceiraIA? decisaoFinanceira = null)
        {
            if (planoEstrategicoFinanceiro is null || !planoEstrategicoFinanceiro.Ativo || interpretacaoPlanoEstrategico is null || !interpretacaoPlanoEstrategico.PossuiPlanoVigente)
            {
                return CriarSemPlano();
            }

            var objetivos = planoEstrategicoFinanceiro.Objetivos
                .Where(objetivo => !string.IsNullOrWhiteSpace(objetivo.Titulo))
                .OrderBy(objetivo => objetivo.Ordem)
                .ThenByDescending(objetivo => objetivo.Prioridade)
                .ToList();

            if (objetivos.Count == 0)
            {
                return new ConsistenciaEstrategicaIA
                {
                    PossuiPlano = true,
                    NivelConsistencia = NivelConsistenciaEstrategica.Indeterminada,
                    Resumo = "O plano estrategico vigente nao possui objetivos suficientes para avaliar consistencia.",
                    TextoParaIA = MontarTextoParaIA(
                        true,
                        NivelConsistenciaEstrategica.Indeterminada,
                        "O plano estrategico vigente nao possui objetivos suficientes para avaliar consistencia.",
                        [],
                        [],
                        [])
                };
            }

            var textoBasePergunta = string.IsNullOrWhiteSpace(perguntaUsuario) && decisaoFinanceira is not null
                ? $"{decisaoFinanceira.TextoOriginalUsuario} {decisaoFinanceira.TextoInterpretado} {decisaoFinanceira.Categoria} {decisaoFinanceira.Descricao}"
                : perguntaUsuario;

            var perguntaNormalizada = Normalizar(textoBasePergunta);
            var orientacaoPergunta = DetectarOrientacaoPergunta(perguntaNormalizada);
            var temaPergunta = DetectarTemaDecisao(perguntaNormalizada);

            var motivosFavoraveis = new List<string>();
            var motivosDesfavoraveis = new List<string>();
            var objetivosImpactados = new List<string>();
            var pontuacao = 50;

            foreach (var objetivo in objetivos)
            {
                var textoObjetivo = Normalizar($"{objetivo.Titulo} {objetivo.Descricao}");
                var temaObjetivo = DetectarTemaDecisao(textoObjetivo);

                if (temaObjetivo == TemaDecisao.Indefinido)
                {
                    continue;
                }

                if (!ObjetivosRelacionados(temaPergunta, temaObjetivo, perguntaNormalizada))
                {
                    if (orientacaoPergunta == OrientacaoPergunta.Negativa && TemaDeAltaPrioridade(temaObjetivo, objetivo.Prioridade))
                    {
                        var motivo = GerarMotivoDesfavoravel(temaObjetivo, objetivo.Titulo, false);
                        motivosDesfavoraveis.Add(motivo);
                        objetivosImpactados.Add(objetivo.Titulo.Trim());
                        pontuacao -= AjustarPeso(objetivo.Prioridade, 4);
                    }

                    continue;
                }

                objetivosImpactados.Add(objetivo.Titulo.Trim());

                if (orientacaoPergunta == OrientacaoPergunta.Indeterminada)
                {
                    var motivoNeutro = $"O objetivo \"{objetivo.Titulo.Trim()}\" foi relacionado ao tema da pergunta, mas sem sinal claro de alinhamento ou desalinhamento.";
                    motivosFavoraveis.Add(motivoNeutro);
                    pontuacao += AjustarPeso(objetivo.Prioridade, 1);
                    continue;
                }

                var alinhado = EstaAlinhado(temaObjetivo, orientacaoPergunta, perguntaNormalizada);
                if (alinhado)
                {
                    var motivo = GerarMotivoFavoravel(temaObjetivo, objetivo.Titulo);
                    motivosFavoraveis.Add(motivo);
                    pontuacao += AjustarPeso(objetivo.Prioridade, 2);
                }
                else
                {
                    var motivo = GerarMotivoDesfavoravel(temaObjetivo, objetivo.Titulo, true);
                    motivosDesfavoraveis.Add(motivo);
                    pontuacao -= AjustarPeso(objetivo.Prioridade, 2);
                }
            }

            if (objetivosImpactados.Count == 0 && orientacaoPergunta == OrientacaoPergunta.Indeterminada)
            {
                return new ConsistenciaEstrategicaIA
                {
                    PossuiPlano = true,
                    NivelConsistencia = NivelConsistenciaEstrategica.Indeterminada,
                    Resumo = "Nao foi possivel relacionar a pergunta aos objetivos estrategicos vigentes.",
                    TextoParaIA = MontarTextoParaIA(
                        true,
                        NivelConsistenciaEstrategica.Indeterminada,
                        "Nao foi possivel relacionar a pergunta aos objetivos estrategicos vigentes.",
                        motivosFavoraveis,
                        motivosDesfavoraveis,
                        objetivosImpactados)
                };
            }

            if (resumoFinanceiroIA.SaudeFinanceira.PontuacaoGeral < 50 && orientacaoPergunta == OrientacaoPergunta.Negativa)
            {
                pontuacao -= 5;
                motivosDesfavoraveis.Add("O momento financeiro atual exige cautela adicional antes de assumir uma decisão de maior impacto.");
            }

            pontuacao = Math.Clamp(pontuacao, 0, 100);
            var nivel = Classificar(pontuacao);
            var resumo = MontarResumo(nivel, pontuacao, objetivosImpactados, motivosFavoraveis, motivosDesfavoraveis);

            return new ConsistenciaEstrategicaIA
            {
                PossuiPlano = true,
                NivelConsistencia = nivel,
                Resumo = resumo,
                MotivosFavoraveis = motivosFavoraveis.Distinct().ToList(),
                MotivosDesfavoraveis = motivosDesfavoraveis.Distinct().ToList(),
                ObjetivosImpactados = objetivosImpactados.Distinct().ToList(),
                TextoParaIA = MontarTextoParaIA(
                    true,
                    nivel,
                    resumo,
                    motivosFavoraveis.Distinct().ToList(),
                    motivosDesfavoraveis.Distinct().ToList(),
                    objetivosImpactados.Distinct().ToList())
            };
        }

        private static ConsistenciaEstrategicaIA CriarSemPlano()
        {
            return new ConsistenciaEstrategicaIA
            {
                PossuiPlano = false,
                NivelConsistencia = NivelConsistenciaEstrategica.Indeterminada,
                Resumo = "Nao existe Plano Estrategico Financeiro vigente para avaliar a consistencia.",
                TextoParaIA = MontarTextoParaIA(
                    false,
                    NivelConsistenciaEstrategica.Indeterminada,
                    "Nao existe Plano Estrategico Financeiro vigente para avaliar a consistencia.",
                    [],
                    [],
                    [])
            };
        }

        private static NivelConsistenciaEstrategica Classificar(int pontuacao)
        {
            if (pontuacao >= 85)
            {
                return NivelConsistenciaEstrategica.MuitoAlta;
            }

            if (pontuacao >= 70)
            {
                return NivelConsistenciaEstrategica.Alta;
            }

            if (pontuacao >= 50)
            {
                return NivelConsistenciaEstrategica.Media;
            }

            if (pontuacao >= 30)
            {
                return NivelConsistenciaEstrategica.Baixa;
            }

            return NivelConsistenciaEstrategica.MuitoBaixa;
        }

        private static int AjustarPeso(EnumPrioridadeObjetivoPlanoEstrategico prioridade, int multiplicador)
        {
            var basePeso = prioridade switch
            {
                EnumPrioridadeObjetivoPlanoEstrategico.Critica => 6,
                EnumPrioridadeObjetivoPlanoEstrategico.Alta => 5,
                EnumPrioridadeObjetivoPlanoEstrategico.Media => 4,
                _ => 3
            };

            return basePeso * multiplicador;
        }

        private static bool EstaAlinhado(TemaDecisao temaObjetivo, OrientacaoPergunta orientacaoPergunta, string perguntaNormalizada)
        {
            return temaObjetivo switch
            {
                TemaDecisao.Reserva => orientacaoPergunta == OrientacaoPergunta.Positiva,
                TemaDecisao.Divida => orientacaoPergunta == OrientacaoPergunta.Positiva,
                TemaDecisao.Patrimonio => orientacaoPergunta == OrientacaoPergunta.Positiva,
                TemaDecisao.Moradia => perguntaNormalizada.Contains("casa") || perguntaNormalizada.Contains("imovel") || perguntaNormalizada.Contains("imóvel") || perguntaNormalizada.Contains("apartamento"),
                TemaDecisao.Veiculo => perguntaNormalizada.Contains("carro") || perguntaNormalizada.Contains("veiculo") || perguntaNormalizada.Contains("veículo"),
                _ => false
            };
        }

        private static bool ObjetivosRelacionados(TemaDecisao temaPergunta, TemaDecisao temaObjetivo, string perguntaNormalizada)
        {
            if (temaPergunta == TemaDecisao.Indefinido)
            {
                return temaObjetivo is TemaDecisao.Reserva or TemaDecisao.Divida or TemaDecisao.Patrimonio
                    ? perguntaNormalizada.Contains("objetivo")
                    : false;
            }

            return temaPergunta == temaObjetivo
                || (temaPergunta == TemaDecisao.Patrimonio && temaObjetivo == TemaDecisao.Reserva)
                || (temaPergunta == TemaDecisao.Reserva && temaObjetivo == TemaDecisao.Patrimonio);
        }

        private static bool TemaDeAltaPrioridade(TemaDecisao tema, EnumPrioridadeObjetivoPlanoEstrategico prioridade)
        {
            return prioridade is EnumPrioridadeObjetivoPlanoEstrategico.Alta or EnumPrioridadeObjetivoPlanoEstrategico.Critica
                && tema is TemaDecisao.Reserva or TemaDecisao.Divida or TemaDecisao.Patrimonio;
        }

        private static OrientacaoPergunta DetectarOrientacaoPergunta(string perguntaNormalizada)
        {
            var temPositiva = PalavrasPositivas.Any(palavra => perguntaNormalizada.Contains(palavra));
            var temNegativa = PalavrasNegativas.Any(palavra => perguntaNormalizada.Contains(palavra));

            if (temPositiva && !temNegativa)
            {
                return OrientacaoPergunta.Positiva;
            }

            if (temNegativa && !temPositiva)
            {
                return OrientacaoPergunta.Negativa;
            }

            if (temPositiva && temNegativa)
            {
                return OrientacaoPergunta.Negativa;
            }

            return OrientacaoPergunta.Indeterminada;
        }

        private static TemaDecisao DetectarTemaDecisao(string textoNormalizado)
        {
            if (PalavrasReserva.Any(textoNormalizado.Contains))
            {
                return TemaDecisao.Reserva;
            }

            if (PalavrasDivida.Any(textoNormalizado.Contains))
            {
                return TemaDecisao.Divida;
            }

            if (PalavrasPatrimonio.Any(textoNormalizado.Contains))
            {
                return TemaDecisao.Patrimonio;
            }

            if (PalavrasMoradia.Any(textoNormalizado.Contains))
            {
                return TemaDecisao.Moradia;
            }

            if (PalavrasVeiculo.Any(textoNormalizado.Contains))
            {
                return TemaDecisao.Veiculo;
            }

            return TemaDecisao.Indefinido;
        }

        private static string GerarMotivoFavoravel(TemaDecisao tema, string objetivo)
        {
            return tema switch
            {
                TemaDecisao.Reserva => $"A decisao favorece o objetivo \"{objetivo.Trim()}\" porque ajuda a fortalecer a reserva de emergencia.",
                TemaDecisao.Divida => $"A decisao favorece o objetivo \"{objetivo.Trim()}\" porque reduz a pressao do endividamento.",
                TemaDecisao.Patrimonio => $"A decisao favorece o objetivo \"{objetivo.Trim()}\" porque contribui para o crescimento patrimonial.",
                TemaDecisao.Moradia => $"A decisao favorece o objetivo \"{objetivo.Trim()}\" porque esta alinhada a uma meta de moradia de longo prazo.",
                TemaDecisao.Veiculo => $"A decisao favorece o objetivo \"{objetivo.Trim()}\" porque esta alinhada a uma meta de mobilidade planejada.",
                _ => $"A decisao favorece o objetivo \"{objetivo.Trim()}\"."
            };
        }

        private static string GerarMotivoDesfavoravel(TemaDecisao tema, string objetivo, bool relacionado)
        {
            return tema switch
            {
                TemaDecisao.Reserva => $"A decisao pode enfraquecer o objetivo \"{objetivo.Trim()}\" ao reduzir a capacidade de formar reserva de emergencia.",
                TemaDecisao.Divida => $"A decisao pode enfraquecer o objetivo \"{objetivo.Trim()}\" ao aumentar o comprometimento com dividas.",
                TemaDecisao.Patrimonio => $"A decisao pode atrasar o objetivo \"{objetivo.Trim()}\" ao diminuir a capacidade de acumular patrimonio.",
                TemaDecisao.Moradia => relacionado
                    ? $"A decisao entra em conflito com o objetivo \"{objetivo.Trim()}\" porque muda a direcao planejada para moradia."
                    : $"A decisao pode competir com o objetivo \"{objetivo.Trim()}\" e atrasar a meta planejada.",
                TemaDecisao.Veiculo => relacionado
                    ? $"A decisao entra em conflito com o objetivo \"{objetivo.Trim()}\" porque altera a estrategia planejada para mobilidade."
                    : $"A decisao pode competir com o objetivo \"{objetivo.Trim()}\" e atrasar a meta planejada.",
                _ => $"A decisao pode enfraquecer o objetivo \"{objetivo.Trim()}\"."
            };
        }

        private static string MontarResumo(
            NivelConsistenciaEstrategica nivel,
            int pontuacao,
            IReadOnlyCollection<string> objetivosImpactados,
            IReadOnlyCollection<string> motivosFavoraveis,
            IReadOnlyCollection<string> motivosDesfavoraveis)
        {
            var descricaoNivel = nivel switch
            {
                NivelConsistenciaEstrategica.MuitoAlta => "muito alta aderencia",
                NivelConsistenciaEstrategica.Alta => "boa aderencia",
                NivelConsistenciaEstrategica.Media => "aderencia parcial",
                NivelConsistenciaEstrategica.Baixa => "baixa aderencia",
                NivelConsistenciaEstrategica.MuitoBaixa => "muito baixa aderencia",
                _ => "aderencia indeterminada"
            };

            var impacto = objetivosImpactados.Count > 0
                ? $" Os objetivos impactados incluem {string.Join(", ", objetivosImpactados.Take(3))}."
                : string.Empty;

            return $"A decisao analisada apresenta {descricaoNivel} ao plano estrategico vigente, com nivel {nivel} e pontuacao interna de {pontuacao}/100.{impacto}".Trim();
        }

        private static string MontarTextoParaIA(
            bool possuiPlano,
            NivelConsistenciaEstrategica nivel,
            string resumo,
            IReadOnlyCollection<string> motivosFavoraveis,
            IReadOnlyCollection<string> motivosDesfavoraveis,
            IReadOnlyCollection<string> objetivosImpactados)
        {
            var builder = new StringBuilder();
            builder.AppendLine("## Consistencia Estrategica");
            builder.AppendLine($"Possui plano vigente: {(possuiPlano ? "Sim" : "Nao")}");
            builder.AppendLine($"Nivel de consistencia: {FormatarNivel(nivel)}");
            builder.AppendLine($"Resumo: {resumo}");
            builder.AppendLine();
            builder.AppendLine("Motivos favoraveis:");
            builder.AppendLine(motivosFavoraveis.Count == 0
                ? "- Nenhum motivo favoravel identificado."
                : string.Join(Environment.NewLine, motivosFavoraveis.Select(item => item.StartsWith("-") ? item : $"- {item}")));
            builder.AppendLine();
            builder.AppendLine("Motivos desfavoraveis:");
            builder.AppendLine(motivosDesfavoraveis.Count == 0
                ? "- Nenhum motivo desfavoravel identificado."
                : string.Join(Environment.NewLine, motivosDesfavoraveis.Select(item => item.StartsWith("-") ? item : $"- {item}")));
            builder.AppendLine();
            builder.AppendLine("Objetivos impactados:");
            builder.AppendLine(objetivosImpactados.Count == 0
                ? "- Nenhum objetivo foi impactado de forma clara."
                : string.Join(Environment.NewLine, objetivosImpactados.Distinct().Select(item => $"- {item}")));

            return builder.ToString().TrimEnd();
        }

        private static string FormatarNivel(NivelConsistenciaEstrategica nivel)
        {
            return nivel switch
            {
                NivelConsistenciaEstrategica.MuitoAlta => "Muito alta",
                NivelConsistenciaEstrategica.Alta => "Alta",
                NivelConsistenciaEstrategica.Media => "Média",
                NivelConsistenciaEstrategica.Baixa => "Baixa",
                NivelConsistenciaEstrategica.MuitoBaixa => "Muito baixa",
                _ => "Indeterminada"
            };
        }

        private static string Normalizar(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            var textoNormalizado = texto.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var caractere in textoNormalizado)
            {
                var categoria = CharUnicodeInfo.GetUnicodeCategory(caractere);
                if (categoria != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(caractere);
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

        private enum OrientacaoPergunta
        {
            Positiva,
            Negativa,
            Indeterminada
        }

        private enum TemaDecisao
        {
            Indefinido,
            Reserva,
            Divida,
            Patrimonio,
            Moradia,
            Veiculo
        }
    }
}
