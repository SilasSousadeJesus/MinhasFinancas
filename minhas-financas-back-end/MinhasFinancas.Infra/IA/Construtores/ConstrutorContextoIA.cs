using System.Globalization;
using System.Text;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;
using MinhasFinancas.Infra.IA.Enums;
using MinhasFinancas.Infra.IA.Interpretadores;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Construtores
{
    public class ConstrutorContextoIA
    {
        private readonly InterpretadorMemoriaFinanceira _interpretadorMemoriaFinanceira;
        private readonly InterpretadorDecisaoFinanceira _interpretadorDecisaoFinanceira;
        private readonly InterpretadorEstrategico _interpretadorEstrategico;

        public ConstrutorContextoIA(
            InterpretadorMemoriaFinanceira interpretadorMemoriaFinanceira,
            InterpretadorDecisaoFinanceira interpretadorDecisaoFinanceira,
            InterpretadorEstrategico interpretadorEstrategico)
        {
            _interpretadorMemoriaFinanceira = interpretadorMemoriaFinanceira;
            _interpretadorDecisaoFinanceira = interpretadorDecisaoFinanceira;
            _interpretadorEstrategico = interpretadorEstrategico;
        }

        public ContextoAssistenteFinanceiro Construir(
            ResumoFinanceiroIA resumoFinanceiroIA,
            string? perguntaUsuario = null,
            IEnumerable<MemoriaFinanceiraResumidaIA>? memoriaFinanceira = null,
            DecisaoFinanceiraIA? decisaoFinanceira = null,
            InterpretacaoPlanoEstrategicoIA? interpretacaoPlanoEstrategico = null,
            ConsistenciaEstrategicaIA? consistenciaEstrategica = null,
            IEnumerable<CompromissoFinanceiro>? compromissosFinanceiros = null)
        {
            var cultura = new CultureInfo("pt-BR");

            var prioridades = resumoFinanceiroIA.PrioridadesImediatas
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToList();

            var destaques = resumoFinanceiroIA.DestaquesPositivos
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToList();

            var insightsPrioritarios = resumoFinanceiroIA.Insights.Prioritarios
                .Select(FormatarInsight)
                .ToList();

            var insightsPositivos = resumoFinanceiroIA.Insights.DestaquesPositivos
                .Select(FormatarInsight)
                .ToList();

            var pontosAtencao = resumoFinanceiroIA.SaudeFinanceira.PontosAtencao
                .Select(FormatarPontoAtencao)
                .ToList();

            var indicadoresEmAtencao = resumoFinanceiroIA.Indicadores.Todos
                .Where(indicador => indicador.Status is StatusIndicadorFinanceiro.Atencao or StatusIndicadorFinanceiro.Critico)
                .Select(indicador => FormatarIndicador(indicador, cultura))
                .ToList();

            var indicadoresPositivos = resumoFinanceiroIA.Indicadores.Todos
                .Where(indicador => indicador.Status is StatusIndicadorFinanceiro.Bom or StatusIndicadorFinanceiro.Excelente)
                .Select(indicador => FormatarIndicador(indicador, cultura))
                .ToList();

            var todosIndicadores = resumoFinanceiroIA.Indicadores.Todos
                .Select(indicador => FormatarIndicador(indicador, cultura))
                .ToList();

            var interpretacaoMemoria = _interpretadorMemoriaFinanceira.Interpretar(memoriaFinanceira);
            var memoriaFinanceiraResumida = interpretacaoMemoria.MemoriaFinanceiraCompacta;
            var decisaoInterpretada = decisaoFinanceira;

            if (decisaoInterpretada is null && !string.IsNullOrWhiteSpace(perguntaUsuario))
            {
                decisaoInterpretada = _interpretadorDecisaoFinanceira.Interpretar(perguntaUsuario);
            }

            var interpretacaoPlano = interpretacaoPlanoEstrategico ?? new InterpretacaoPlanoEstrategicoIA
            {
                PossuiPlanoVigente = false,
                ResumoEstrategico = "Nao ha Plano Estrategico Financeiro vigente cadastrado.",
                TextoParaIA = "Nao ha Plano Estrategico Financeiro vigente cadastrado.",
                AlertasEstrategicos = ["Nao ha Plano Estrategico Financeiro vigente cadastrado."]
            };
            var narrativaPlanoEstrategico = _interpretadorEstrategico.InterpretarPlanoParaContexto(interpretacaoPlano);
            var compromissosAtivos = compromissosFinanceiros?
                .Where(item => item is not null)
                .Select(item => MontarLinhaCompromissoFinanceiro(item!))
                .ToList() ?? [];
            var consistencia = consistenciaEstrategica ?? new ConsistenciaEstrategicaIA
            {
                PossuiPlano = false,
                Resumo = "Nao foi possivel calcular consistencia estrategica.",
                TextoParaIA = "Nao foi possivel calcular consistencia estrategica."
            };

            var secoes = new List<string>
            {
                MontarSecao(
                    "Resumo Executivo",
                    [
                        $"Data de referencia: {resumoFinanceiroIA.DataReferencia:dd/MM/yyyy}",
                        $"Resumo do sistema: {resumoFinanceiroIA.ResumoExecutivo}"
                    ]),
                MontarSecao(
                    "Saude Financeira",
                    [
                        $"Pontuacao geral: {resumoFinanceiroIA.SaudeFinanceira.PontuacaoGeral}/100",
                        $"Classificacao atual: {resumoFinanceiroIA.SaudeFinanceira.Classificacao}"
                    ]),
                MontarSecao(
                    "Pontos de Atencao Tecnicos",
                    pontosAtencao,
                    "- Nenhum ponto de atencao tecnico relevante foi identificado pelo sistema."),
                MontarSecao(
                    "Prioridades Imediatas",
                    prioridades.Select(item => $"- {item}"),
                    "- Nenhuma prioridade imediata foi registrada."),
                MontarSecao(
                    "Destaques Positivos",
                    destaques.Select(item => $"- {item}"),
                    "- Nenhum destaque positivo foi registrado."),
                MontarSecao(
                    "Indicadores em Atencao",
                    indicadoresEmAtencao,
                    "- Nenhum indicador esta em faixa de atencao ou critica."),
                MontarSecao(
                    "Indicadores Positivos",
                    indicadoresPositivos,
                    "- Nenhum indicador positivo foi destacado nesta leitura."),
                MontarSecao(
                    "Indicadores Financeiros Consolidados",
                    todosIndicadores,
                    "- Nao ha indicadores consolidados disponiveis."),
                MontarSecao(
                    "Insights Financeiros Prioritarios",
                    insightsPrioritarios,
                    "- Nenhum insight prioritario esta disponivel."),
                MontarSecao(
                    "Insights Positivos",
                    insightsPositivos,
                    "- Nenhum insight positivo esta disponivel."),
                MontarSecao(
                    "Evolucao Financeira",
                    MontarLinhasEvolucaoFinanceira(interpretacaoMemoria),
                    "- Ainda nao existem analises suficientes para avaliar evolucao."),
                MontarSecao(
                    "Memoria Financeira",
                    memoriaFinanceiraResumida,
                    "Nao existem analises anteriores."),
                MontarSecao(
                    "Plano Estrategico Financeiro",
                    narrativaPlanoEstrategico,
                    "- Nao ha Plano Estrategico Financeiro vigente cadastrado."),
                MontarSecao(
                    "Compromissos Financeiros",
                    compromissosAtivos,
                    "- Nenhum compromisso financeiro ativo foi encontrado."),
                MontarSecao(
                    "Consistencia Estrategica",
                    MontarLinhasConsistenciaEstrategica(consistencia),
                    "- Nao foi possivel calcular consistencia estrategica."),
                MontarSecao(
                    "Decisao Financeira Interpretada",
                    MontarLinhasDecisaoFinanceira(decisaoInterpretada),
                    "- Nenhuma decisao financeira foi interpretada nesta leitura."),
                MontarSecao(
                    "Cobertura Atual do Contexto",
                    [
                        "- Perfil financeiro: ja refletido de forma indireta nos indicadores, na saude financeira e nas prioridades.",
                        "- Patrimonio: ja refletido pelos indicadores de patrimonio liquido atual e percentual do patrimonio-alvo.",
                        "- Fluxo de caixa: ja refletido pelos indicadores de economia mensal, percentual de economia e comprometimento da renda.",
                        "- Tendencias, radar financeiro operacional, projecoes, simulacoes e detalhamento patrimonial ainda nao sao enviados como blocos proprios nesta fase.",
                        "- Quando algum dado nao estiver explicitamente descrito no contexto, a analise deve se limitar as informacoes estruturadas recebidas."
                    ])
            };

            if (!string.IsNullOrWhiteSpace(perguntaUsuario))
            {
                secoes.Add(MontarSecao("Pergunta do Usuario", [$"- {perguntaUsuario}"]));
            }

            return new ContextoAssistenteFinanceiro
            {
                DataReferencia = resumoFinanceiroIA.DataReferencia,
                PontuacaoSaudeFinanceira = resumoFinanceiroIA.SaudeFinanceira.PontuacaoGeral,
                ClassificacaoSaudeFinanceira = resumoFinanceiroIA.SaudeFinanceira.Classificacao,
                PrioridadesImediatas = prioridades,
                DestaquesPositivos = destaques,
                InsightsPrioritarios = insightsPrioritarios,
                MemoriaFinanceiraResumida = memoriaFinanceiraResumida,
                ResumoEvolucaoFinanceira = interpretacaoMemoria.ResumoEvolucao,
                EvolucaoFinanceira = interpretacaoMemoria.Narrativas,
                CompromissosFinanceiros = compromissosAtivos,
                ResumoExecutivo = resumoFinanceiroIA.ResumoExecutivo,
                DecisaoFinanceira = decisaoInterpretada,
                InterpretacaoPlanoEstrategico = interpretacaoPlano,
                ConsistenciaEstrategica = consistencia,
                ContextoTextual = string.Join(Environment.NewLine + Environment.NewLine, secoes),
                PerguntaUsuario = perguntaUsuario ?? string.Empty
            };
        }

        private static string MontarSecao(string titulo, IEnumerable<string> linhas, string? vazio = null)
        {
            var conteudo = linhas
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToList();

            if (conteudo.Count == 0 && !string.IsNullOrWhiteSpace(vazio))
            {
                conteudo.Add(vazio);
            }

            var builder = new StringBuilder();
            builder.AppendLine($"## {titulo}");

            foreach (var linha in conteudo)
            {
                builder.AppendLine(linha);
            }

            return builder.ToString().TrimEnd();
        }

        private static string FormatarInsight(InsightFinanceiro insight)
        {
            return $"- [{insight.Tipo}] {insight.Titulo} | Descricao: {insight.Descricao} | Acao sugerida: {insight.AcaoSugerida}";
        }

        private static string FormatarPontoAtencao(PontoAtencaoSaudeFinanceira pontoAtencao)
        {
            return $"- {pontoAtencao.Nome} | Status: {FormatarStatus(pontoAtencao.Status)} | Descricao: {pontoAtencao.Descricao} | Observacao: {pontoAtencao.Observacao}";
        }

        private static string FormatarIndicador(IndicadorFinanceiro indicador, CultureInfo cultura)
        {
            return $"- {indicador.Nome} | Atual: {FormatarValor(indicador.ValorAtual, indicador.Formato, cultura)} | Ideal: {FormatarValor(indicador.ValorIdeal, indicador.Formato, cultura)} | Percentual: {indicador.Percentual:N2}% | Status: {FormatarStatus(indicador.Status)} | Descricao: {indicador.Descricao} | Observacao: {indicador.Observacao}";
        }

        private static IEnumerable<string> MontarLinhasEvolucaoFinanceira(InterpretacaoMemoriaFinanceiraIA interpretacao)
        {
            var linhas = new List<string>();

            if (!string.IsNullOrWhiteSpace(interpretacao.ResumoEvolucao))
            {
                linhas.Add($"- Resumo da evolucao: {interpretacao.ResumoEvolucao}");
            }

            linhas.AddRange(interpretacao.Narrativas.Where(item => !string.IsNullOrWhiteSpace(item)));
            return linhas;
        }

        private static IEnumerable<string> MontarLinhasConsistenciaEstrategica(ConsistenciaEstrategicaIA consistencia)
        {
            yield return $"- Possui plano vigente: {(consistencia.PossuiPlano ? "Sim" : "Nao")}";
            yield return $"- Nivel de consistencia: {FormatarNivelConsistencia(consistencia.NivelConsistencia)}";

            if (!string.IsNullOrWhiteSpace(consistencia.Resumo))
            {
                yield return $"- Resumo: {consistencia.Resumo}";
            }

            if (consistencia.MotivosFavoraveis.Count > 0)
            {
                yield return $"- Motivos favoraveis: {string.Join("; ", consistencia.MotivosFavoraveis)}";
            }

            if (consistencia.MotivosDesfavoraveis.Count > 0)
            {
                yield return $"- Motivos desfavoraveis: {string.Join("; ", consistencia.MotivosDesfavoraveis)}";
            }

            if (consistencia.ObjetivosImpactados.Count > 0)
            {
                yield return $"- Objetivos impactados: {string.Join("; ", consistencia.ObjetivosImpactados)}";
            }
        }

        private static IEnumerable<string> MontarLinhasDecisaoFinanceira(DecisaoFinanceiraIA? decisaoFinanceira)
        {
            if (decisaoFinanceira is null)
            {
                yield break;
            }

            yield return $"- Tipo de decisão: {decisaoFinanceira.TipoDecisao}";
            yield return $"- Categoria interpretada: {decisaoFinanceira.Categoria}";
            yield return $"- Descrição: {decisaoFinanceira.Descricao}";

            if (!string.IsNullOrWhiteSpace(decisaoFinanceira.TextoOriginalUsuario))
            {
                yield return $"- Pergunta original: {decisaoFinanceira.TextoOriginalUsuario}";
            }

            if (!string.IsNullOrWhiteSpace(decisaoFinanceira.TextoInterpretado))
            {
                yield return $"- Interpretação: {decisaoFinanceira.TextoInterpretado}";
            }

            if (decisaoFinanceira.ValorEstimado.HasValue)
            {
                yield return $"- Valor estimado: {decisaoFinanceira.ValorEstimado.Value.ToString("C", new CultureInfo("pt-BR"))}";
            }

            if (!string.IsNullOrWhiteSpace(decisaoFinanceira.Prazo))
            {
                yield return $"- Prazo identificado: {decisaoFinanceira.Prazo}";
            }

            if (!string.IsNullOrWhiteSpace(decisaoFinanceira.FormaPagamento))
            {
                yield return $"- Forma de pagamento: {decisaoFinanceira.FormaPagamento}";
            }

            if (!string.IsNullOrWhiteSpace(decisaoFinanceira.ObjetivoRelacionado))
            {
                yield return $"- Objetivo relacionado: {decisaoFinanceira.ObjetivoRelacionado}";
            }

            yield return $"- Grau de confiança: {decisaoFinanceira.GrauConfiancaInterpretacao}/100";
        }

        private static string MontarLinhaCompromissoFinanceiro(CompromissoFinanceiro compromisso)
        {
            var origem = compromisso.Origem switch
            {
                EnumOrigemCompromissoFinanceiro.Manual => "Manual",
                EnumOrigemCompromissoFinanceiro.IA => "IA",
                _ => "Indefinida"
            };

            var status = compromisso.Status switch
            {
                EnumStatusCompromissoFinanceiro.EmAndamento => "Em andamento",
                EnumStatusCompromissoFinanceiro.Concluido => "Concluído",
                EnumStatusCompromissoFinanceiro.Cancelado => "Cancelado",
                _ => "Indefinido"
            };

            return $"- {compromisso.Descricao} (Origem: {origem}; Status: {status})";
        }

        private static string FormatarNivelConsistencia(NivelConsistenciaEstrategica nivel)
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

        private static string FormatarStatus(StatusIndicadorFinanceiro status)
        {
            return status switch
            {
                StatusIndicadorFinanceiro.Excelente => "Excelente",
                StatusIndicadorFinanceiro.Bom => "Bom",
                StatusIndicadorFinanceiro.Atencao => "Atencao",
                _ => "Critico"
            };
        }

        private static string FormatarValor(decimal valor, FormatoValorIndicadorFinanceiro formato, CultureInfo cultura)
        {
            return formato switch
            {
                FormatoValorIndicadorFinanceiro.Percentual => $"{valor:N2}%",
                FormatoValorIndicadorFinanceiro.Meses => $"{valor:N2} mes(es)",
                _ => valor.ToString("C", cultura)
            };
        }
    }
}
