using MinhasFinancas.Infra.IA.Especialistas.Modelos;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Especialistas
{
    public abstract class EspecialistaFinanceiroBase : Interfaces.IEspecialistaFinanceiro
    {
        public abstract string Nome { get; }
        public abstract ParecerEspecialistaIA Avaliar(ContextoAssistenteFinanceiro contexto);

        protected static bool ContemTexto(string? texto, params string[] termos)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return false;
            }

            return termos.Any(termo => texto.Contains(termo, StringComparison.OrdinalIgnoreCase));
        }

        protected static bool ContemEmLista(IEnumerable<string> textos, params string[] termos)
        {
            return textos.Any(texto => ContemTexto(texto, termos));
        }

        protected static string PrimeiroTextoOuPadrao(IEnumerable<string> textos, string padrao)
        {
            var primeiro = textos.FirstOrDefault(item => !string.IsNullOrWhiteSpace(item));
            return string.IsNullOrWhiteSpace(primeiro) ? padrao : primeiro.Trim();
        }

        protected static List<string> FiltrarTextos(IEnumerable<string> textos, params string[] termos)
        {
            return textos
                .Where(texto => ContemTexto(texto, termos))
                .Select(texto => texto.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        protected static ParecerEspecialistaIA CriarParecer(
            string nomeEspecialista,
            string situacaoAtual,
            string conclusao,
            IEnumerable<string> riscos,
            IEnumerable<string> pontosPositivos,
            IEnumerable<string> recomendacoes,
            string prioridade,
            IEnumerable<string> observacoes)
        {
            return new ParecerEspecialistaIA
            {
                NomeEspecialista = nomeEspecialista,
                SituacaoAtual = situacaoAtual,
                Conclusao = conclusao,
                Riscos = riscos.Where(item => !string.IsNullOrWhiteSpace(item)).Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
                PontosPositivos = pontosPositivos.Where(item => !string.IsNullOrWhiteSpace(item)).Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
                Recomendacoes = recomendacoes.Where(item => !string.IsNullOrWhiteSpace(item)).Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
                Prioridade = prioridade,
                Observacoes = observacoes.Where(item => !string.IsNullOrWhiteSpace(item)).Distinct(StringComparer.OrdinalIgnoreCase).ToList()
            };
        }
    }
}
