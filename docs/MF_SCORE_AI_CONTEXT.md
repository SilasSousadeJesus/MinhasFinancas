# MF Score AI Context

Este documento foi criado para servir como base de conhecimento enxuta e estruturada para IAs externas que vão ajudar na calibração e evolução do `MF Score`.

Ele não substitui a documentação oficial do projeto. Ele resume os pontos mais importantes para que uma IA consiga analisar o modelo com contexto suficiente, sem precisar ler toda a base.

## Objetivo deste documento

Fornecer a uma IA externa contexto suficiente para:

- entender o que é o `MF Score`
- entender o que ele mede
- entender o que ele não deve medir
- conhecer a arquitetura conceitual atual do modelo
- conhecer os indicadores já existentes
- conhecer as regras de penalização crítica
- conhecer os princípios oficiais de calibragem
- conhecer as limitações atuais do motor
- ajudar a sugerir pontuações, faixas e punições mais coerentes

## Contexto do produto

O sistema se chama `Minhas Finanças`.

Ele é um sistema de gestão financeira pessoal com múltiplos módulos, incluindo:

- lançamentos
- fluxo de caixa
- patrimônio
- perfil financeiro
- projeções
- simulações
- assistente financeiro
- saúde financeira

O modelo oficial de risco financeiro pessoal do sistema é o `MF Score`.

## O que o MF Score é

O `MF Score` é uma régua de risco financeiro pessoal.

Ele responde à pergunta:

> Qual é o risco financeiro pessoal do usuário se ele continuar seguindo a trajetória atual?

Ele não é:

- score de riqueza
- score de felicidade financeira
- score de crédito bancário tradicional

Ele é:

- score de risco financeiro pessoal
- score de proteção
- score de pressão estrutural
- score de deterioração operacional
- score de maturidade financeira

## Filosofia oficial do modelo

Princípios obrigatórios do sistema:

1. Indicadores ruins reduzem nota de pilar.
2. Penalizações críticas são reservadas para risco grave, materializado ou persistente.
3. Um mesmo fato econômico não deve ser punido duas vezes.
4. O score deve ser rastreável, explicável e calibrável.
5. A IA não cria o score; ela apenas ajuda a interpretá-lo ou calibrá-lo.

## Escalas oficiais

### MF Score

- escala final: `0 a 1000`

### Pilares

- escala: `0 a 100`

## Classificação oficial atual

- `900-1000` = Excelente = Risco Muito Baixo
- `800-899` = Muito Bom = Risco Baixo
- `700-799` = Bom = Risco Moderado
- `600-699` = Atenção = Risco Moderado-Alto
- `400-599` = Crítico = Risco Alto
- `0-399` = Muito Crítico = Risco Muito Alto

## Arquitetura conceitual atual do MF Score

O modelo hoje está organizado em quatro camadas:

1. Nota dos pilares
2. Penalizações críticas
3. Persistência temporal do risco
4. Histórico mensal do score

### 1. Nota dos pilares

Os pilares são a leitura estrutural do risco.

Eles devem absorver:

- fragilidades de liquidez
- pressão de caixa
- pressão futura
- endividamento
- patrimônio fraco
- planejamento insuficiente

### 2. Penalizações críticas

As penalizações críticas não existem para repetir a deterioração dos pilares.

Elas existem para capturar:

- risco já materializado
- evento grave
- deterioração operacional real
- persistência temporal negativa

### 3. Persistência temporal

O modelo já considera que risco recorrente é mais grave que risco pontual.

Hoje isso aparece principalmente em:

- meses consecutivos no vermelho

### 4. Histórico mensal

O sistema já possui persistência mensal do score via `HistoricoMfScore`.

Isso permite:

- evolução histórica
- tendência real
- comparação entre competências
- futura calibragem temporal mais sofisticada

## Pilares oficiais

1. Fluxo de Caixa
2. Liquidez
3. Endividamento
4. Patrimônio
5. Planejamento

## Pesos oficiais dos pilares

- Fluxo de Caixa: `30%`
- Liquidez: `25%`
- Endividamento: `20%`
- Patrimônio: `15%`
- Planejamento: `10%`

## Indicadores existentes no sistema

A IA deve trabalhar prioritariamente com esses indicadores já existentes:

- Economia Mensal
- Percentual de Economia
- Reserva de Emergência Atual
- Reserva de Emergência Ideal
- Comprometimento da Renda
- Comprometimento Financeiro Futuro (30 dias)
- Pressão Financeira Acumulada 90 dias
- Pressão Financeira Acumulada 180 dias
- Pressão Financeira Acumulada 365 dias
- Endividamento Patrimonial
- Patrimônio Líquido Atual
- Percentual do Patrimônio Alvo

## Pesos oficiais por indicador

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

## Escala de status dos indicadores

Cada indicador usa uma escala padronizada:

- `Excelente` = `100`
- `Bom` = `80`
- `Atenção` = `55`
- `Crítica` = `25`

## Regra central de não dupla penalização

Este é um princípio obrigatório.

Exemplos:

- reserva zero já reduz `Liquidez`
- comprometimento alto já reduz `Fluxo de Caixa`
- pressão futura já reduz `Fluxo de Caixa` e `Endividamento`

Esses fatos não devem gerar, automaticamente, nova punição crítica equivalente, a menos que exista evidência de materialização do risco.

## O que deve afetar prioritariamente os pilares

Hoje o sistema entende que estes fatores devem agir primeiro na camada estrutural:

- reserva baixa ou inexistente
- comprometimento alto da renda
- pressão futura de 30 dias
- pressão financeira acumulada
- endividamento patrimonial alto

## Penalizações críticas oficiais atuais

As penalizações críticas hoje se concentram em:

1. inadimplência
2. fluxo de caixa mensal negativo
3. dois meses consecutivos no vermelho
4. três ou mais meses consecutivos no vermelho
5. patrimônio líquido negativo
6. ausência de dados essenciais

## Como a inadimplência é identificada hoje

Na implementação atual, a inadimplência é marcada quando existe pelo menos uma despesa:

- com `Status = Pendente`
- com `DataVencimento < DataReferencia`

Ou seja:

- é uma despesa vencida
- ainda não foi paga

Hoje essa regra é binária:

- existe inadimplência
- ou não existe inadimplência

Ainda não há gradação por:

- quantidade de contas vencidas
- valor total vencido
- dias de atraso

## Persistência temporal atual

O modelo hoje já considera:

- `1 mês negativo`: alerta ou penalização leve
- `2 meses consecutivos negativos`: agravamento moderado
- `3 ou mais meses consecutivos negativos`: agravamento forte

## Histórico mensal do score

Existe uma entidade persistida chamada `HistoricoMfScore`.

Ela registra por competência:

- score base
- score final
- classificação
- risco
- penalidade total
- pilares
- indicadores críticos
- resumo serializado
- versão do modelo

## Job mensal

O projeto possui um job Hangfire recorrente para gerar histórico mensal do `MF Score`.

Configuração atual:

- cron: `0 2 1 * *`
- roda no dia `01`
- calcula a competência anterior

Exemplo:

- no dia `01/08/2026`, gera o score da competência `07/2026`

## Personas de calibração

O projeto possui personas sintéticas persistidas para calibração.

Essas personas:

- não representam usuários reais
- servem para validar cenários
- possuem score humano sugerido
- possuem faixa esperada
- permitem rodar o motor oficial

## Faixas esperadas oficiais atuais das personas

- Vida Financeira Excelente: `900-1000`
- Boa renda, reserva zero e cartão alto: `600-740`
- Patrimônio alto com fluxo ruim: `550-750`
- Excelente fluxo com pouco patrimônio: `750-900`
- Inadimplência: `0-490`
- Comprometimento extremo: `0-590`
- Reserva inexistente sem dívidas: `500-790`
- Planejamento excelente: `780-920`

## O que a IA deve evitar ao sugerir mudanças

A IA não deve:

- tratar reserva baixa como inadimplência
- tratar comprometimento alto como evento crítico automático
- tratar pressão futura como calote consumado
- duplicar o mesmo efeito nos pilares e nas penalizações
- sugerir um score de riqueza em vez de um score de risco
- ignorar a camada temporal

## O que a IA deve priorizar ao analisar o modelo

A IA deve ajudar a responder:

- quais faixas fazem sentido para cada indicador?
- quais pesos estão severos demais ou brandos demais?
- quais eventos merecem penalização crítica de verdade?
- como inspirar o modelo em análise de crédito sem transformá-lo em score bancário?
- como diferenciar risco estrutural, risco operacional, risco materializado e risco persistente?

## Limitações atuais conhecidas

A IA também deve saber que o motor ainda tem limitações em aberto:

- o pilar `Planejamento` ainda usa proxies
- o plano estratégico ainda não entra de forma madura no score
- compromissos financeiros ainda não pesam diretamente no pilar de planejamento
- a leitura de inadimplência ainda é simples
- a qualidade patrimonial ainda não é tratada com profundidade
- a tendência histórica ainda está em estágio inicial

## Como usar este documento com um prompt

Recomendação prática:

1. envie este documento como base de conhecimento
2. envie o prompt de calibragem
3. depois envie as personas ou cenários específicos
4. peça que a IA:
   - analise cada perfil
   - proponha faixa de score
   - explique os pilares
   - diga o que deve ser pilar e o que deve ser penalização crítica

## Documentos oficiais relacionados

Se a IA precisar de mais profundidade, os documentos oficiais do projeto são:

- `docs/MF_SCORE.md`
- `docs/INDICADORES_FINANCEIROS.md`
- `docs/MF_SCORE_VALIDATION.md`
- `docs/MF_SCORE_AUDIT.md`
- `AI_CONTEXT.md`

## Resumo final para a IA

Se precisar resumir tudo em poucas linhas:

- o sistema possui um score oficial de risco financeiro pessoal chamado `MF Score`
- a escala final é `0 a 1000`
- os pilares ficam em `0 a 100`
- o modelo precisa separar deterioração estrutural de evento crítico
- o sistema proíbe dupla penalização
- a IA deve ajudar a calibrar pesos, faixas e punições com base em lógica de risco semelhante à análise de crédito, mas adaptada a finanças pessoais
- o objetivo não é medir riqueza; é medir risco financeiro pessoal com coerência, explicabilidade e utilidade prática
