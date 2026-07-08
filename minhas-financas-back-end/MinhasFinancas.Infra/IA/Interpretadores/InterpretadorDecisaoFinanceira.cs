using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using MinhasFinancas.Infra.IA.Enums;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Interpretadores
{
    public class InterpretadorDecisaoFinanceira
    {
        private static readonly CultureInfo Cultura = new("pt-BR");

        public DecisaoFinanceiraIA Interpretar(string? perguntaUsuario, string? contextoFinanceiro = null)
        {
            var textoOriginal = string.IsNullOrWhiteSpace(perguntaUsuario)
                ? string.Empty
                : perguntaUsuario.Trim();

            var textoNormalizado = Normalizar(textoOriginal);
            var contextoNormalizado = Normalizar(contextoFinanceiro);

            var tipo = DetectarTipo(textoNormalizado);
            var categoria = DetectarCategoria(tipo);
            var descricao = MontarDescricao(tipo);
            var valorEstimado = ExtrairValorEstimado(textoOriginal);
            var prazo = ExtrairPrazo(textoOriginal);
            var formaPagamento = DetectarFormaPagamento(textoNormalizado);
            var objetivoRelacionado = DetectarObjetivoRelacionado(textoNormalizado, contextoNormalizado, tipo);
            var confianca = CalcularConfianca(tipo, valorEstimado, prazo, formaPagamento, objetivoRelacionado);

            return new DecisaoFinanceiraIA
            {
                TipoDecisao = tipo,
                Categoria = categoria,
                Descricao = descricao,
                ValorEstimado = valorEstimado,
                Prazo = prazo,
                FormaPagamento = formaPagamento,
                ObjetivoRelacionado = objetivoRelacionado,
                OrigemDaDecisao = "Pergunta do usuario",
                TextoOriginalUsuario = textoOriginal,
                TextoInterpretado = MontarTextoInterpretado(descricao, categoria, valorEstimado, prazo, formaPagamento, objetivoRelacionado),
                GrauConfiancaInterpretacao = confianca
            };
        }

        private static TipoDecisaoFinanceira DetectarTipo(string texto)
        {
            if (TextoContem(texto, "comprar", "compra", "adquirir"))
            {
                return TipoDecisaoFinanceira.Compra;
            }

            if (TextoContem(texto, "financiar", "financiamento", "parcelar", "parcelado"))
            {
                return TipoDecisaoFinanceira.Financiamento;
            }

            if (TextoContem(texto, "investir", "investimento", "aplicar", "aplicacao"))
            {
                return TipoDecisaoFinanceira.Investimento;
            }

            if (TextoContem(texto, "vender", "venda", "desfazer"))
            {
                return TipoDecisaoFinanceira.Venda;
            }

            if (TextoContem(texto, "trocar", "substituir", "substituicao"))
            {
                return TipoDecisaoFinanceira.Substituicao;
            }

            if (TextoContem(texto, "emprestar", "emprestimo", "emprestimos", "pegar emprestado", "tomar emprestimo"))
            {
                return TipoDecisaoFinanceira.Emprestimo;
            }

            return TipoDecisaoFinanceira.Indefinida;
        }

        private static string DetectarCategoria(TipoDecisaoFinanceira tipo)
        {
            return tipo switch
            {
                TipoDecisaoFinanceira.Compra => "Aquisição",
                TipoDecisaoFinanceira.Financiamento => "Crédito",
                TipoDecisaoFinanceira.Investimento => "Aplicação",
                TipoDecisaoFinanceira.Venda => "Desmobilização",
                TipoDecisaoFinanceira.Substituicao => "Substituição",
                TipoDecisaoFinanceira.Emprestimo => "Endividamento",
                _ => "Indefinida"
            };
        }

        private static string MontarDescricao(TipoDecisaoFinanceira tipo)
        {
            return tipo switch
            {
                TipoDecisaoFinanceira.Compra => "Decisão relacionada a uma compra ou aquisição.",
                TipoDecisaoFinanceira.Financiamento => "Decisão relacionada a um financiamento ou compra parcelada.",
                TipoDecisaoFinanceira.Investimento => "Decisão relacionada a investimento ou aplicação de recursos.",
                TipoDecisaoFinanceira.Venda => "Decisão relacionada a venda ou redução de um bem ou posição.",
                TipoDecisaoFinanceira.Substituicao => "Decisão relacionada a troca ou substituição de uma opção financeira.",
                TipoDecisaoFinanceira.Emprestimo => "Decisão relacionada a empréstimo ou tomada de crédito.",
                _ => "Decisão financeira ainda não claramente identificada."
            };
        }

        private static decimal? ExtrairValorEstimado(string textoOriginal)
        {
            if (string.IsNullOrWhiteSpace(textoOriginal))
            {
                return null;
            }

            var valorComMoeda = Regex.Match(textoOriginal, @"R\$\s*([\d\.\,]+)");
            if (valorComMoeda.Success && decimal.TryParse(valorComMoeda.Groups[1].Value, NumberStyles.Any, Cultura, out var valor))
            {
                return valor;
            }

            var valorNumerico = Regex.Match(textoOriginal, @"(\d+[\.,]?\d*)\s*(mil|milhar|milhares)?", RegexOptions.IgnoreCase);
            if (valorNumerico.Success && decimal.TryParse(valorNumerico.Groups[1].Value, NumberStyles.Any, Cultura, out var numero))
            {
                var multiplicador = valorNumerico.Groups[2].Success ? 1000m : 1m;
                return numero * multiplicador;
            }

            return null;
        }

        private static string? ExtrairPrazo(string textoOriginal)
        {
            if (string.IsNullOrWhiteSpace(textoOriginal))
            {
                return null;
            }

            var prazo = Regex.Match(textoOriginal, @"(\d+\s*(?:x|vezes|meses|mes|anos|ano))", RegexOptions.IgnoreCase);
            return prazo.Success ? prazo.Value.Trim() : null;
        }

        private static string? DetectarFormaPagamento(string textoNormalizado)
        {
            if (TextoContem(textoNormalizado, "a vista", "avista"))
            {
                return "À vista";
            }

            if (TextoContem(textoNormalizado, "parcelado", "parcelas", "em x", "x sem juros"))
            {
                return "Parcelado";
            }

            if (TextoContem(textoNormalizado, "financiamento", "financiar"))
            {
                return "Financiado";
            }

            if (TextoContem(textoNormalizado, "cartao", "credito"))
            {
                return "Cartão de crédito";
            }

            if (TextoContem(textoNormalizado, "pix"))
            {
                return "PIX";
            }

            return null;
        }

        private static string? DetectarObjetivoRelacionado(string textoNormalizado, string contextoNormalizado, TipoDecisaoFinanceira tipo)
        {
            if (TextoContem(textoNormalizado, "reserva", "emergencia") || TextoContem(contextoNormalizado, "reserva", "emergencia"))
            {
                return "Reserva de emergência";
            }

            if (TextoContem(textoNormalizado, "carro", "veiculo", "automovel") || TextoContem(contextoNormalizado, "carro", "veiculo", "automovel"))
            {
                return "Mobilidade / veículo";
            }

            if (TextoContem(textoNormalizado, "casa", "imovel", "apartamento", "moradia") || TextoContem(contextoNormalizado, "casa", "imovel", "apartamento", "moradia"))
            {
                return "Moradia";
            }

            if (TextoContem(textoNormalizado, "invest", "aplicar", "aplicacao") || TextoContem(contextoNormalizado, "invest", "aplicar", "aplicacao"))
            {
                return "Investimentos";
            }

            if (TextoContem(textoNormalizado, "divida", "emprest", "credito", "juros") || TextoContem(contextoNormalizado, "divida", "emprest", "credito", "juros"))
            {
                return "Redução de endividamento";
            }

            return tipo switch
            {
                TipoDecisaoFinanceira.Compra when textoNormalizado.Contains("carro") || textoNormalizado.Contains("veiculo") => "Mobilidade / veículo",
                TipoDecisaoFinanceira.Compra when textoNormalizado.Contains("casa") || textoNormalizado.Contains("imovel") => "Moradia",
                _ => null
            };
        }

        private static int CalcularConfianca(
            TipoDecisaoFinanceira tipo,
            decimal? valorEstimado,
            string? prazo,
            string? formaPagamento,
            string? objetivoRelacionado)
        {
            var confianca = tipo == TipoDecisaoFinanceira.Indefinida ? 35 : 60;

            if (valorEstimado.HasValue)
            {
                confianca += 10;
            }

            if (!string.IsNullOrWhiteSpace(prazo))
            {
                confianca += 10;
            }

            if (!string.IsNullOrWhiteSpace(formaPagamento))
            {
                confianca += 10;
            }

            if (!string.IsNullOrWhiteSpace(objetivoRelacionado))
            {
                confianca += 10;
            }

            return Math.Clamp(confianca, 0, 100);
        }

        private static string MontarTextoInterpretado(
            string descricao,
            string categoria,
            decimal? valorEstimado,
            string? prazo,
            string? formaPagamento,
            string? objetivoRelacionado)
        {
            var builder = new StringBuilder();
            builder.Append($"O sistema interpretou esta solicitação como {descricao.ToLowerInvariant()}");
            builder.Append($" dentro da categoria {categoria.ToLowerInvariant()}");

            if (valorEstimado.HasValue)
            {
                builder.Append($", com valor estimado de {valorEstimado.Value.ToString("C", Cultura)}");
            }

            if (!string.IsNullOrWhiteSpace(prazo))
            {
                builder.Append($", prazo aproximado de {prazo}");
            }

            if (!string.IsNullOrWhiteSpace(formaPagamento))
            {
                builder.Append($", forma de pagamento sugerida: {formaPagamento}");
            }

            if (!string.IsNullOrWhiteSpace(objetivoRelacionado))
            {
                builder.Append($", possivelmente relacionada ao objetivo {objetivoRelacionado.ToLowerInvariant()}");
            }

            builder.Append('.');
            return builder.ToString();
        }

        private static bool TextoContem(string texto, params string[] termos)
        {
            return termos.Any(termo => texto.Contains(termo, StringComparison.Ordinal));
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
    }
}
