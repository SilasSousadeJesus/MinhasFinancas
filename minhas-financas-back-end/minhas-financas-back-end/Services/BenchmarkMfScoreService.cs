using Microsoft.AspNetCore.Hosting;
using MinhasFinancas.Application.DTOs.MfScoreLaboratorio;
using MinhasFinancas.Application.Interfaces;
using System.Text;
using System.Text.RegularExpressions;

namespace minhas_financas_back_end.Services
{
    public class BenchmarkMfScoreService : IBenchmarkMfScoreService
    {
        private readonly IWebHostEnvironment _environment;

        public BenchmarkMfScoreService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<BenchmarkCenarioMfScoreLaboratorioDTO?> BuscarCenarioAsync(string codigoCenario)
        {
            if (string.IsNullOrWhiteSpace(codigoCenario))
            {
                return null;
            }

            var caminho = LocalizarArquivoBenchmark();
            if (string.IsNullOrWhiteSpace(caminho) || !File.Exists(caminho))
            {
                return null;
            }

            var markdown = await File.ReadAllTextAsync(caminho, Encoding.UTF8);
            var secao = ExtrairSecaoCenario(markdown, codigoCenario);
            if (string.IsNullOrWhiteSpace(secao))
            {
                return null;
            }

            return new BenchmarkCenarioMfScoreLaboratorioDTO
            {
                CodigoCenario = codigoCenario,
                NomeCenario = ExtrairValorEntreCrases(secao, "Nome"),
                NotaHumanaReferencia = ExtrairInteiro(secao, @"-\s+\*\*Nota considerada justa:\*\*\s+`(?<valor>-?\d+)`"),
                FaixaAceitavelMinima = ExtrairInteiro(secao, @"-\s+\*\*Faixa aceit[aá]vel:\*\*\s+`(?<min>\d+)-(?<max>\d+)`", "min"),
                FaixaAceitavelMaxima = ExtrairInteiro(secao, @"-\s+\*\*Faixa aceit[aá]vel:\*\*\s+`(?<min>\d+)-(?<max>\d+)`", "max"),
                FaixaAceitavelTexto = ExtrairTexto(secao, @"-\s+\*\*Faixa aceit[aá]vel:\*\*\s+`(?<valor>[^`]+)`"),
                DiferencaRegistrada = ExtrairInteiro(secao, @"-\s+\*\*Diferença:\*\*\s+`(?<valor>-?\d+)`"),
                Status = ExtrairTexto(secao, @"-\s+\*\*Status:\*\*\s+`(?<valor>[^`]+)`"),
                JustificativaHumana = ExtrairBloco(secao, "Justificativa Humana", "Indicadores Responsáveis"),
                IndicadoresResponsaveis = ExtrairLista(secao, "Indicadores Responsáveis", "Decisão da Auditoria"),
                DecisaoAuditoria = ExtrairBloco(secao, "Decisão da Auditoria", null)
            };
        }

        private string? LocalizarArquivoBenchmark()
        {
            var diretorio = new DirectoryInfo(_environment.ContentRootPath);

            while (diretorio != null)
            {
                var candidato = Path.Combine(diretorio.FullName, "docs", "MF_SCORE_BENCHMARK.md");
                if (File.Exists(candidato))
                {
                    return candidato;
                }

                diretorio = diretorio.Parent;
            }

            return null;
        }

        private static string ExtrairSecaoCenario(string markdown, string codigoCenario)
        {
            var match = Regex.Match(
                markdown,
                $@"###\s+{Regex.Escape(codigoCenario)}(?<conteudo>.*?)(?=\r?\n###\s+MF-CENARIO-\d{{2}}|\r?\n##\s+Resumo Geral|\z)",
                RegexOptions.Singleline);

            return match.Success ? match.Value : string.Empty;
        }

        private static string ExtrairValorEntreCrases(string texto, string campo)
        {
            var pattern = $@"-\s+\*\*{Regex.Escape(campo)}:\*\*\s+`(?<valor>[^`]+)`";
            return ExtrairTexto(texto, pattern);
        }

        private static int ExtrairInteiro(string texto, string pattern, string grupo = "valor")
        {
            var match = Regex.Match(texto, pattern, RegexOptions.Singleline);
            if (!match.Success)
            {
                return 0;
            }

            return int.TryParse(match.Groups[grupo].Value.Trim(), out var valor) ? valor : 0;
        }

        private static string ExtrairTexto(string texto, string pattern)
        {
            var match = Regex.Match(texto, pattern, RegexOptions.Singleline);
            return match.Success ? match.Groups["valor"].Value.Trim() : string.Empty;
        }

        private static string ExtrairBloco(string texto, string inicio, string? fim)
        {
            var pattern = fim == null
                ? $@"####\s+{Regex.Escape(inicio)}\s*(?<valor>.*)$"
                : $@"####\s+{Regex.Escape(inicio)}\s*(?<valor>.*?)(?=\r?\n####\s+{Regex.Escape(fim)})";

            var match = Regex.Match(texto, pattern, RegexOptions.Singleline);
            return match.Success ? match.Groups["valor"].Value.Trim() : string.Empty;
        }

        private static List<string> ExtrairLista(string texto, string inicio, string fim)
        {
            var bloco = ExtrairBloco(texto, inicio, fim);
            if (string.IsNullOrWhiteSpace(bloco))
            {
                return [];
            }

            return bloco
                .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => item.StartsWith("- "))
                .Select(item => item[2..].Trim().Trim('`'))
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToList();
        }
    }
}
