using MinhasFinancas.Infra.IA.Especialistas.Modelos;

namespace MinhasFinancas.Infra.IA.Especialistas
{
    public static class ResumoEspecialistasFinanceiros
    {
        public static IEnumerable<string> ParaTexto(IEnumerable<ParecerEspecialistaIA> pareceres)
        {
            foreach (var parecer in pareceres.Where(item => item is not null))
            {
                yield return $"### {parecer.NomeEspecialista}";
                yield return $"Situação: {parecer.SituacaoAtual}";
                yield return $"Conclusão: {parecer.Conclusao}";

                if (parecer.Riscos.Count > 0)
                {
                    yield return "Riscos:";
                    foreach (var risco in parecer.Riscos)
                    {
                        yield return $"- {risco}";
                    }
                }

                if (parecer.PontosPositivos.Count > 0)
                {
                    yield return "Pontos positivos:";
                    foreach (var ponto in parecer.PontosPositivos)
                    {
                        yield return $"- {ponto}";
                    }
                }

                if (parecer.Recomendacoes.Count > 0)
                {
                    yield return "Recomendações:";
                    foreach (var recomendacao in parecer.Recomendacoes)
                    {
                        yield return $"- {recomendacao}";
                    }
                }

                if (!string.IsNullOrWhiteSpace(parecer.Prioridade))
                {
                    yield return $"Prioridade: {parecer.Prioridade}";
                }

                if (parecer.Observacoes.Count > 0)
                {
                    yield return "Observações:";
                    foreach (var observacao in parecer.Observacoes)
                    {
                        yield return $"- {observacao}";
                    }
                }
            }
        }
    }
}
