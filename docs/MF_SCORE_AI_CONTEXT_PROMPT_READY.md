# MF Score AI Context - Prompt Ready

Este documento é uma versão otimizada para uso com outras IAs.

Ele foi organizado para funcionar como contexto pronto de calibragem do `MF Score`, com foco em:

- clareza
- regras fixas
- estrutura de análise
- menor ambiguidade

## 1. Contexto

Você está analisando o `MF Score`, que é o modelo oficial de risco financeiro pessoal do sistema `Minhas Finanças`.

O sistema é um gerenciador financeiro pessoal com módulos como:

- lançamentos
- fluxo de caixa
- patrimônio
- perfil financeiro
- projeções
- simulações
- saúde financeira
- assistente financeiro

O `MF Score` é usado como a régua oficial para interpretar risco financeiro pessoal do usuário.

Ele não é um score de riqueza, nem um score de crédito bancário tradicional.

Ele é um score de:

- risco financeiro pessoal
- proteção financeira
- pressão estrutural
- deterioração operacional
- persistência de fragilidade financeira

Pergunta central do modelo:

> Qual é o risco financeiro pessoal do usuário se ele continuar seguindo a trajetória atual?

## 2. Regras fixas do sistema

Estas regras são obrigatórias e não devem ser ignoradas:

### 2.1 Escalas

- `MF Score final`: `0 a 1000`
- `MF Score base`: `0 a 1000`
- `pilares`: `0 a 100`

### 2.2 Classificação oficial

- `900-1000` = Excelente = Risco Muito Baixo
- `800-899` = Muito Bom = Risco Baixo
- `700-799` = Bom = Risco Moderado
- `600-699` = Atenção = Risco Moderado-Alto
- `400-599` = Crítico = Risco Alto
- `0-399` = Muito Crítico = Risco Muito Alto

### 2.3 Regra de não dupla penalização

Um mesmo fato econômico não deve ser punido duas vezes.

Exemplos:

- reserva zero reduz `Liquidez`, mas não deve automaticamente gerar penalização crítica só por existir
- comprometimento alto reduz `Fluxo de Caixa`, mas não deve automaticamente gerar penalização crítica se ainda não houve ruptura real
- pressão futura reduz pilares estruturais, mas não deve automaticamente ser tratada como inadimplência

### 2.4 Separação obrigatória entre camadas

O modelo separa:

1. nota dos pilares
2. penalizações críticas
3. persistência temporal
4. histórico mensal

### 2.5 O que é penalização crítica

Penalização crítica só deve existir para:

- risco grave
- risco materializado
- risco persistente
- deterioração operacional real

### 2.6 O que não deve ser penalização crítica automática

Os itens abaixo devem afetar prioritariamente os pilares:

- reserva baixa
- reserva inexistente
- comprometimento alto
- pressão futura
- pressão financeira acumulada
- endividamento patrimonial alto sem ruptura materializada

## 3. Estrutura do modelo

### 3.1 Pilares oficiais

1. Fluxo de Caixa
2. Liquidez
3. Endividamento
4. Patrimônio
5. Planejamento

### 3.2 Pesos dos pilares

- Fluxo de Caixa: `30%`
- Liquidez: `25%`
- Endividamento: `20%`
- Patrimônio: `15%`
- Planejamento: `10%`

### 3.3 Indicadores existentes

Os indicadores atualmente disponíveis no sistema são:

- Economia Mensal
- Percentual de Economia
- Reserva de Emergência Atual
- Reserva de Emergência Ideal
- Comprometimento da Renda
- Comprometimento Financeiro Futuro 30 dias
- Pressão Financeira Acumulada 90 dias
- Pressão Financeira Acumulada 180 dias
- Pressão Financeira Acumulada 365 dias
- Endividamento Patrimonial
- Patrimônio Líquido Atual
- Percentual do Patrimônio Alvo

### 3.4 Pesos dos indicadores

- `EconomiaMensal` = `1.0`
- `PercentualEconomia` = `1.0`
- `ReservaEmergenciaAtual` = `1.5`
- `ReservaEmergenciaIdeal` = `0.5`
- `ComprometimentoRenda` = `1.5`
- `ComprometimentoFinanceiroFuturo` = `1.5`
- `ComprometimentoFinanceiroFuturo90Dias` = `1.0`
- `ComprometimentoFinanceiroFuturo180Dias` = `0.75`
- `ComprometimentoFinanceiroFuturo365Dias` = `0.5`
- `EndividamentoPatrimonial` = `1.5`
- `PatrimonioLiquidoAtual` = `1.25`
- `PercentualPatrimonioAlvo` = `0.75`

### 3.5 Escala de status dos indicadores

- `Excelente` = `100`
- `Bom` = `80`
- `Atenção` = `55`
- `Crítica` = `25`

## 4. Penalizações críticas atuais

As penalizações críticas atuais do sistema se concentram em:

1. inadimplência
2. fluxo de caixa mensal negativo
3. dois meses consecutivos no vermelho
4. três ou mais meses consecutivos no vermelho
5. patrimônio líquido negativo
6. ausência de dados essenciais

## 5. Mecânicas atuais importantes

### 5.1 Inadimplência

Hoje a inadimplência é identificada quando existe pelo menos uma despesa:

- com `Status = Pendente`
- com `DataVencimento < DataReferencia`

Isto significa:

- a conta venceu
- o pagamento ainda não foi efetivado

Hoje essa leitura ainda é binária:

- existe inadimplência
- ou não existe inadimplência

Ainda não existe gradação por:

- valor vencido
- quantidade de contas vencidas
- dias de atraso

### 5.2 Persistência temporal

Hoje o modelo já considera:

- `1 mês negativo`
- `2 meses negativos consecutivos`
- `3 ou mais meses negativos consecutivos`

A lógica atual é:

- quanto mais persistente o problema, mais forte a penalização

### 5.3 Histórico mensal

Existe histórico mensal persistido em `HistoricoMfScore`.

Esse histórico guarda:

- competência
- score base
- score final
- classificação
- risco
- penalidade total
- pilares
- indicadores críticos
- resumo serializado
- versão do modelo

### 5.4 Job mensal

Existe um job recorrente via Hangfire:

- cron: `0 2 1 * *`
- roda no dia `01`
- calcula a competência anterior

## 6. Personas e validação

O sistema já possui personas sintéticas para calibração.

Faixas esperadas atuais:

- Vida Financeira Excelente: `900-1000`
- Boa renda, reserva zero e cartão alto: `600-740`
- Patrimônio alto com fluxo ruim: `550-750`
- Excelente fluxo com pouco patrimônio: `750-900`
- Inadimplência: `0-490`
- Comprometimento extremo: `0-590`
- Reserva inexistente sem dívidas: `500-790`
- Planejamento excelente: `780-920`

## 7. Limitações atuais conhecidas

A análise deve considerar que o modelo ainda possui limitações em aberto:

- pilar `Planejamento` ainda usa proxies
- plano estratégico ainda não pesa de forma madura no score
- compromissos financeiros ainda não pesam diretamente no pilar de planejamento
- inadimplência ainda é simples e binária
- qualidade patrimonial ainda não é tratada com profundidade
- tendência histórica ainda está em estágio inicial

## 8. O que a IA deve fazer

Ao analisar este modelo, a IA deve:

1. interpretar o score como score de risco financeiro pessoal
2. sugerir faixas e pontuações coerentes por indicador
3. sugerir ajustes de severidade
4. sugerir ajustes de pesos, se necessário
5. distinguir claramente:
   - risco estrutural
   - risco operacional
   - risco materializado
   - risco persistente
6. avaliar se a calibragem se aproxima de uma lógica inspirada em análise de crédito, mas adaptada a finanças pessoais

## 9. O que a IA não deve fazer

A IA não deve:

- inventar score de riqueza
- transformar reserva zero em inadimplência automática
- transformar comprometimento alto em calote consumado
- duplicar punição entre pilar e penalização crítica
- ignorar a camada temporal
- sugerir mudanças genéricas sem racional técnico

## 10. Formato obrigatório de resposta esperado da IA

Peça que a IA responda sempre com esta estrutura:

1. Leitura do modelo atual
2. Avaliação conceitual por indicador
3. Avaliação por pilar
4. Penalizações críticas sugeridas
5. Persistência temporal sugerida
6. Avaliação das personas
7. Recomendações de calibragem
8. Tabela-resumo final

## 11. Tabela-resumo esperada

A resposta ideal da IA deve terminar com uma tabela contendo:

- indicador
- o que ele mede
- tipo de impacto
- severidade sugerida
- se afeta pilar, penalização crítica ou persistência
- risco de dupla penalização
- observações de calibragem

## 12. Uso recomendado

Para obter o melhor resultado com outra IA:

1. envie este documento primeiro
2. envie depois o prompt principal de calibragem
3. em seguida envie:
   - as personas do sistema
   - ou cenários específicos
   - ou trechos do cálculo atual

## 13. Frase-resumo para orientar a IA

Se precisar condensar tudo em uma única diretriz:

> Você está ajudando a calibrar um score de risco financeiro pessoal. O modelo deve se inspirar em lógica de análise de crédito para severidade e deterioração, mas sem virar um score bancário tradicional. Ele deve ser explicável, evitar dupla penalização, separar fragilidade estrutural de evento crítico e refletir risco financeiro real do usuário.
