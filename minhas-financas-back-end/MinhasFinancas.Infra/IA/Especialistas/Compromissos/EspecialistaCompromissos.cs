using MinhasFinancas.Infra.IA.Especialistas.Modelos;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Especialistas.Compromissos
{
    public class EspecialistaCompromissos : EspecialistaFinanceiroBase
    {
        public override string Nome => "Especialista em Compromissos";

        public override ParecerEspecialistaIA Avaliar(ContextoAssistenteFinanceiro contexto)
        {
            var possuiCompromissos = contexto.CompromissosFinanceiros.Count > 0;

            var situacao = possuiCompromissos
                ? $"Existem {contexto.CompromissosFinanceiros.Count} compromisso(s) ativo(s) acompanhando a execução financeira do usuário."
                : "Ainda não há compromissos ativos acompanhando as decisões financeiras assumidas.";

            var conclusao = possuiCompromissos
                ? "Os compromissos ajudam a transformar intenção em execução, mas precisam continuar sendo acompanhados com disciplina."
                : "Sem compromissos ativos, a execução das decisões ainda depende apenas da memória do usuário.";

            var riscos = possuiCompromissos
                ? new[] { "Compromissos sem acompanhamento podem perder força e virar apenas intenção registrada." }
                : new[] { "A ausência de compromissos reduz a rastreabilidade da execução das decisões assumidas." };

            var pontosPositivos = possuiCompromissos
                ? new[] { "O sistema já possui uma base de execução para acompanhar decisões assumidas." }
                : new[] { "Ainda existe espaço para transformar mais recomendações em compromissos rastreáveis." };

            var recomendacoes = possuiCompromissos
                ? new[]
                {
                    "Revisar periodicamente os compromissos ativos.",
                    "Evitar manter compromissos sem prioridade clara."
                }
                : new[]
                {
                    "Converter recomendações relevantes em compromissos quando fizer sentido.",
                    "Acompanhar a execução das intenções importantes."
                };

            return CriarParecer(Nome, situacao, conclusao, riscos, pontosPositivos, recomendacoes, possuiCompromissos ? "Estratégica" : "Longo prazo", []);
        }
    }
}
