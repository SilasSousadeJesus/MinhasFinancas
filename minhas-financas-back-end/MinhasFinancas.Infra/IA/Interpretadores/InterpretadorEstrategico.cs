using System.Globalization;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Interpretadores
{
    public class InterpretadorEstrategico
    {
        private static readonly CultureInfo Cultura = new("pt-BR");

        public InterpretacaoPlanoEstrategicoIA Interpretar(PlanoEstrategicoFinanceiro? plano)
        {
            if (plano is null || !plano.Ativo)
            {
                return new InterpretacaoPlanoEstrategicoIA
                {
                    PossuiPlanoVigente = false,
                    TextoParaIA = "Nao ha Plano Estrategico Financeiro vigente cadastrado.",
                    ResumoEstrategico = "Nao ha Plano Estrategico Financeiro vigente cadastrado.",
                    AlertasEstrategicos = ["Nao ha Plano Estrategico Financeiro vigente cadastrado."]
                };
            }

            var objetivosOrdenados = plano.Objetivos
                .Where(objetivo => !string.IsNullOrWhiteSpace(objetivo.Titulo))
                .OrderBy(objetivo => objetivo.Ordem)
                .ThenBy(objetivo => objetivo.Prioridade)
                .ToList();

            var prioridades = objetivosOrdenados
                .Where(objetivo => objetivo.Prioridade is EnumPrioridadeObjetivoPlanoEstrategico.Alta or EnumPrioridadeObjetivoPlanoEstrategico.Critica)
                .Select(FormatarObjetivo)
                .ToList();

            var emAndamento = objetivosOrdenados
                .Where(objetivo => objetivo.Status == EnumStatusObjetivoPlanoEstrategico.EmAndamento)
                .Select(FormatarObjetivo)
                .ToList();

            var concluidos = objetivosOrdenados
                .Where(objetivo => objetivo.Status == EnumStatusObjetivoPlanoEstrategico.Concluido)
                .Select(FormatarObjetivo)
                .ToList();

            var criticos = objetivosOrdenados
                .Where(objetivo =>
                    objetivo.Prioridade == EnumPrioridadeObjetivoPlanoEstrategico.Critica ||
                    (objetivo.Prioridade == EnumPrioridadeObjetivoPlanoEstrategico.Alta &&
                     objetivo.Status is not EnumStatusObjetivoPlanoEstrategico.Concluido and not EnumStatusObjetivoPlanoEstrategico.Cancelado))
                .Select(FormatarObjetivo)
                .ToList();

            var observacoes = MontarObservacoes(plano).ToList();
            var alertas = MontarAlertas(plano, objetivosOrdenados, prioridades, emAndamento, concluidos, criticos, observacoes).ToList();

            var resumo = MontarResumo(plano, prioridades, emAndamento, concluidos);
            var textoParaIA = MontarTextoParaIA(plano, resumo, prioridades, emAndamento, concluidos, criticos, alertas, observacoes);

            return new InterpretacaoPlanoEstrategicoIA
            {
                PossuiPlanoVigente = true,
                NumeroVersaoPlanoVigente = plano.NumeroVersao,
                NomePlano = plano.Nome,
                ResumoEstrategico = resumo,
                PrioridadesEstrategicas = prioridades,
                ObjetivosEmAndamento = emAndamento,
                ObjetivosConcluidos = concluidos,
                ObjetivosCriticosOuAltaPrioridade = criticos,
                AlertasEstrategicos = alertas,
                ObservacoesRelevantes = observacoes,
                TextoParaIA = textoParaIA
            };
        }

        public IEnumerable<string> InterpretarPlanoParaContexto(InterpretacaoPlanoEstrategicoIA interpretacao)
        {
            if (interpretacao is null)
            {
                yield return "- Nao ha Plano Estrategico Financeiro vigente cadastrado.";
                yield break;
            }

            if (!interpretacao.PossuiPlanoVigente)
            {
                yield return "- Nao ha Plano Estrategico Financeiro vigente cadastrado.";
                yield break;
            }

            if (!string.IsNullOrWhiteSpace(interpretacao.ResumoEstrategico))
            {
                yield return $"- Resumo estrategico: {interpretacao.ResumoEstrategico}";
            }

            if (interpretacao.PrioridadesEstrategicas.Count > 0)
            {
                yield return $"- Prioridades estrategicas: {string.Join("; ", interpretacao.PrioridadesEstrategicas)}";
            }

            if (interpretacao.ObjetivosEmAndamento.Count > 0)
            {
                yield return $"- Objetivos em andamento: {string.Join("; ", interpretacao.ObjetivosEmAndamento)}";
            }

            if (interpretacao.ObjetivosConcluidos.Count > 0)
            {
                yield return $"- Objetivos concluidos: {string.Join("; ", interpretacao.ObjetivosConcluidos)}";
            }

            if (interpretacao.ObjetivosCriticosOuAltaPrioridade.Count > 0)
            {
                yield return $"- Objetivos criticos ou de alta prioridade: {string.Join("; ", interpretacao.ObjetivosCriticosOuAltaPrioridade)}";
            }

            if (interpretacao.AlertasEstrategicos.Count > 0)
            {
                yield return $"- Alertas estrategicos: {string.Join("; ", interpretacao.AlertasEstrategicos)}";
            }

            if (interpretacao.ObservacoesRelevantes.Count > 0)
            {
                yield return $"- Observacoes relevantes: {string.Join("; ", interpretacao.ObservacoesRelevantes)}";
            }
        }

        private static string MontarResumo(
            PlanoEstrategicoFinanceiro plano,
            IReadOnlyCollection<string> prioridades,
            IReadOnlyCollection<string> emAndamento,
            IReadOnlyCollection<string> concluidos)
        {
            var partePrincipal = prioridades.Count > 0
                ? $"A estrategia vigente prioriza {FormatarLista(prioridades.Take(3).ToList())}."
                : "A estrategia vigente ainda nao possui prioridades destacadas.";

            var parteStatus = emAndamento.Count > 0 || concluidos.Count > 0
                ? $" O plano conta com {emAndamento.Count} objetivo(s) em andamento e {concluidos.Count} concluido(s)."
                : " O plano ainda nao apresenta objetivos em andamento ou concluidos.";

            var parteDescricao = string.IsNullOrWhiteSpace(plano.Descricao)
                ? string.Empty
                : $" Direcao declarada: {plano.Descricao.Trim()}";

            return $"{partePrincipal}{parteStatus}{parteDescricao}".Trim();
        }

        private static IEnumerable<string> MontarObservacoes(PlanoEstrategicoFinanceiro plano)
        {
            if (!string.IsNullOrWhiteSpace(plano.Observacao))
            {
                yield return $"- Observacao do plano: {plano.Observacao.Trim()}";
            }

            yield return $"- Versao vigente: {plano.NumeroVersao}";
            yield return $"- Vigente desde: {plano.DataInicioVigencia.ToString("dd/MM/yyyy", Cultura)}";
        }

        private static IEnumerable<string> MontarAlertas(
            PlanoEstrategicoFinanceiro plano,
            IReadOnlyCollection<ObjetivoPlanoEstrategico> objetivos,
            IReadOnlyCollection<string> prioridades,
            IReadOnlyCollection<string> emAndamento,
            IReadOnlyCollection<string> concluidos,
            IReadOnlyCollection<string> criticos,
            IReadOnlyCollection<string> observacoes)
        {
            if (objetivos.Count == 0)
            {
                yield return "Nenhum objetivo estrategico foi cadastrado nesta versao.";
                yield break;
            }

            if (criticos.Count > 0)
            {
                yield return "Existem objetivos de alta prioridade ainda nao concluido(s).";
            }

            if (emAndamento.Count == 0 && prioridades.Count > 0)
            {
                yield return "A estrategia possui prioridades definidas, mas nenhum objetivo aparece em andamento.";
            }

            if (concluidos.Count == 0)
            {
                yield return "Ainda nao ha objetivos concluidos nesta versao estrategica.";
            }

            if (string.IsNullOrWhiteSpace(plano.Descricao) && string.IsNullOrWhiteSpace(plano.Observacao))
            {
                yield return "O plano vigente poderia detalhar melhor a motivacao estrategica para dar mais contexto a analise.";
            }

            if (observacoes.Count == 0)
            {
                yield return "Nao ha observacoes relevantes registradas para a versao vigente.";
            }
        }

        private static string MontarTextoParaIA(
            PlanoEstrategicoFinanceiro plano,
            string resumo,
            IReadOnlyCollection<string> prioridades,
            IReadOnlyCollection<string> emAndamento,
            IReadOnlyCollection<string> concluidos,
            IReadOnlyCollection<string> criticos,
            IReadOnlyCollection<string> alertas,
            IReadOnlyCollection<string> observacoes)
        {
            var linhas = new List<string>
            {
                "## Plano Estrategico Financeiro",
                $"Plano vigente: versao {plano.NumeroVersao}",
                $"Nome do plano: {plano.Nome}",
                string.Empty,
                "Resumo estrategico:",
                resumo,
                string.Empty,
                "Objetivos prioritarios:",
                prioridades.Count == 0 ? "- Nenhum objetivo prioritario foi destacado." : MontarListaComPrefixo(prioridades),
                string.Empty,
                "Objetivos em andamento:",
                emAndamento.Count == 0 ? "- Nenhum objetivo em andamento." : MontarListaComPrefixo(emAndamento),
                string.Empty,
                "Objetivos concluidos:",
                concluidos.Count == 0 ? "- Nenhum objetivo concluido ainda." : MontarListaComPrefixo(concluidos),
                string.Empty,
                "Objetivos criticos ou de alta prioridade:",
                criticos.Count == 0 ? "- Nenhum objetivo critico foi identificado." : MontarListaComPrefixo(criticos),
                string.Empty,
                "Alertas estrategicos:",
                alertas.Count == 0 ? "- Nenhum alerta estrategico foi gerado." : MontarListaComPrefixo(alertas),
                string.Empty,
                "Observacoes relevantes:",
                observacoes.Count == 0 ? "- Nenhuma observacao relevante registrada." : string.Join(Environment.NewLine, observacoes)
            };

            return string.Join(Environment.NewLine, linhas.Where(linha => linha is not null));
        }

        private static string MontarListaComPrefixo(IEnumerable<string> itens)
        {
            return string.Join(Environment.NewLine, itens.Select(item => item.StartsWith("-") ? item : $"- {item}"));
        }

        private static string FormatarObjetivo(ObjetivoPlanoEstrategico objetivo)
        {
            var complemento = new List<string>();

            if (objetivo.DataAlvo.HasValue)
            {
                complemento.Add($"alvo {objetivo.DataAlvo.Value:dd/MM/yyyy}");
            }

            if (objetivo.ValorAlvo.HasValue)
            {
                complemento.Add($"valor alvo {objetivo.ValorAlvo.Value:C}");
            }

            if (complemento.Count == 0)
            {
                return objetivo.Titulo.Trim();
            }

            return $"{objetivo.Titulo.Trim()} ({string.Join(", ", complemento)})";
        }

        private static string FormatarLista(IReadOnlyCollection<string> itens)
        {
            return string.Join(", ", itens);
        }
    }
}
