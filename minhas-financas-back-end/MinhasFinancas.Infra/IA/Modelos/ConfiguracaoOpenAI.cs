namespace MinhasFinancas.Infra.IA.Modelos
{
    public class ConfiguracaoOpenAI
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gpt-5.4-mini";
        public int TimeoutSeconds { get; set; } = 60;
        public int MaxTokens { get; set; } = 1500;
        public int MaxInputCharacters { get; set; } = 24000;
        public int RetryCount { get; set; } = 2;
        public string BaseUrl { get; set; } = "https://api.openai.com/v1/";
        public decimal InputPricePerMillionTokens { get; set; } = 0.75m;
        public decimal OutputPricePerMillionTokens { get; set; } = 4.50m;
    }
}
