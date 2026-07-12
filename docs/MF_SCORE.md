# MF Score

O `MF Score` é o modelo oficial de avaliação da saúde financeira do sistema.

Ele responde à pergunta:

> Qual é o nível de risco financeiro pessoal do usuário se a trajetória atual continuar?

O modelo não mede apenas riqueza, disciplina subjetiva ou comportamento de crédito tradicional. Ele mede a combinação entre:

- capacidade operacional do mês;
- proteção financeira;
- pressão de dívidas e obrigações;
- situação patrimonial real;
- qualidade do planejamento e da execução.

Na versão atual do motor, `mf-score-v2.4-1000`, o objetivo principal passou a ser corrigir distorções conceituais identificadas na auditoria completa do laboratório, preservando a arquitetura oficial do Motor Financeiro.

## Filosofia oficial

- O `MF Score` mede saúde financeira com apetite de risco `moderado`.
- O modelo deve ser explicável, auditável e rastreável.
- Indicadores ruins reduzem pilares antes de qualquer penalização crítica.
- Penalizações críticas existem apenas para risco materializado, grave ou persistente.
- Um mesmo fato econômico não deve ser punido duas vezes.
- O motor deve corrigir distorções sem reinventar sua arquitetura central.

## Escalas oficiais

### Pilares

- cada pilar usa escala `0 a 100`

### MF Score final

- score base e score final usam escala `0 a 1000`

## Estrutura do modelo

O `MF Score` continua organizado em cinco pilares:

1. Fluxo de Caixa
2. Liquidez e Reserva
3. Endividamento e Obrigações
4. Patrimônio
5. Planejamento e Disciplina

## Pesos oficiais

- Fluxo de Caixa: `30%`
- Liquidez e Reserva: `25%`
- Endividamento e Obrigações: `20%`
- Patrimônio: `15%`
- Planejamento e Disciplina: `10%`

## O que mudou na versão `mf-score-v2.4-1000`

Esta rodada não alterou a arquitetura geral do motor. Ela corrigiu conceitos.

### 1. Fluxo de Caixa passou a medir capacidade operacional do mês

O pilar `Fluxo de Caixa` deixou de misturar em excesso eficiência, pressão futura e planejamento implícito.

Agora ele se concentra principalmente em:

- `Economia Mensal`
- `Percentual de Economia`
- `Comprometimento da Renda`

Objetivo:

- responder se o mês fecha positivo, negativo ou com folga real;
- reduzir redundância entre economia absoluta, economia percentual e comprometimento;
- evitar que a pressão futura de médio e longo prazo domine a leitura operacional do mês.

### 2. Endividamento e Obrigações passou a separar naturezas diferentes de dívida

O pilar `Endividamento e Obrigações` agora distingue explicitamente:

- dívidas de consumo;
- financiamentos patrimoniais;
- obrigações futuras recorrentes;
- inadimplência.

Decisão conceitual importante:

- financiamento patrimonial não recebe o mesmo tratamento de dívida de consumo;
- passivos patrimoniais continuam reduzindo a nota, mas com severidade menor do que dívidas de consumo de mesmo valor relativo;
- obrigações futuras continuam relevantes, mas entram como pressão estrutural e não como sinônimo automático de ruptura.

### 3. Patrimônio passou a priorizar a situação patrimonial real

O pilar `Patrimônio` agora usa como leitura principal:

- ativos;
- passivos;
- patrimônio líquido real;
- proporção do patrimônio líquido sobre a base de ativos.

O `Patrimônio-alvo` continua existindo, mas passou a ter papel secundário:

- ele mede evolução em relação à meta;
- ele não deve derrubar excessivamente a nota de quem já possui patrimônio líquido positivo e relevante;
- ele funciona como régua de progresso, e não como fotografia principal da situação patrimonial.

### 4. Planejamento passou a valorizar mais execução do que mera configuração

O pilar `Planejamento e Disciplina` continua usando a base mínima do `Perfil Financeiro`, mas perdeu dependência excessiva de configuração pura.

Agora a nota privilegia mais:

- execução observável;
- consistência financeira;
- cumprimento operacional;
- aderência real ao comportamento esperado.

Sinais usados nessa camada:

- `Percentual de Economia`
- `Capacidade de Formação de Reserva`
- fluxo negativo atual
- meses consecutivos negativos
- inadimplência
- cura recente da inadimplência
- plano estratégico vigente, quando existir
- compromissos financeiros, quando existirem

Regra permanente:

- plano estratégico e compromissos são sinais opcionais;
- se eles não existirem, não devem punir o usuário.

### 5. Penalizações temporais de fluxo negativo deixaram de se somar entre si

Antes, havia risco de somar penalização de:

- mês negativo atual;
- persistência de fluxo negativo.

Na versão atual, isso foi substituído por uma lógica progressiva única.

O motor aplica apenas o nível mais grave correspondente:

- `1 mês`
- `2 meses`
- `3+ meses`
- `6+ meses`
- `12+ meses`

Objetivo:

- eliminar dupla penalização temporal;
- preservar proporcionalidade;
- distinguir alerta pontual de deterioração persistente.

### 6. Projeção de receitas futuras foi corrigida

A projeção de receitas futuras passou a considerar corretamente receitas recorrentes nos horizontes de:

- `180 dias`
- `365 dias`

Isso corrige uma distorção importante observada em praticamente todos os cenários auditados, onde a pressão futura ficava artificialmente inflada porque a receita recorrente não era projetada com consistência suficiente.

### 7. Faixas qualitativas foram endurecidas onde havia subestimação de risco

Os indicadores de pressão acumulada foram recalibrados para evitar classificações brandas demais.

Regra conceitual oficial:

- percentuais acima de `100%` nunca devem ser classificados apenas como `Atenção`

Isso vale especialmente para:

- pressão financeira acumulada de `180 dias`
- pressão financeira acumulada de `365 dias`

### 8. Indicadores passaram a ter apresentação mais humana

Valores técnicos que atrapalhavam leitura executiva, como `999 meses`, deixaram de aparecer como valor cru na interface.

Exemplo oficial:

- em vez de `999 meses`
- a apresentação passa a indicar:
  - `Não projetável no ritmo atual`
  - ou observações equivalentes, como impossibilidade de formar a reserva com o fluxo atual

## Horizontes futuros

O motor continua trabalhando com quatro horizontes:

- `30 dias`
- `90 dias`
- `180 dias`
- `365 dias`

Decisão oficial desta etapa:

- manter os quatro horizontes;
- preservar o curto prazo como principal leitura operacional;
- usar os horizontes mais longos como sinais estruturais com peso decrescente;
- evitar colapsar tudo em um único índice antes da próxima rodada de validação.

## Como o score é calculado

O cálculo oficial possui quatro camadas:

1. indicadores
2. pilares
3. score base
4. penalizações críticas e persistência temporal

### 1. Indicadores

Cada indicador mede um aspecto específico da situação financeira.

### 2. Pilares

Os pilares consolidam esses indicadores por contexto financeiro.

### 3. Score base

O `MF Score Base` é a média ponderada dos cinco pilares, convertida para `0 a 1000`.

### 4. Penalizações críticas e persistência temporal

Depois do score base, o motor aplica apenas penalizações que representam:

- risco materializado;
- gravidade estrutural;
- persistência temporal relevante.

## Regra oficial de não dupla penalização

Um mesmo fato econômico não deve ser punido duas vezes.

Exemplos:

- reserva baixa reduz `Liquidez e Reserva`, mas não vira penalização crítica automática;
- comprometimento alto reduz `Fluxo de Caixa`, mas não vira crítica automática sem ruptura;
- pressão futura reduz `Endividamento e Obrigações`, mas não simula inadimplência sozinha;
- fluxo negativo persistente gera uma penalização temporal progressiva única, e não soma cega de níveis.

## Penalizações críticas oficiais

As penalizações críticas da versão atual se concentram em:

1. inadimplência atual
2. reincidência ou cura recente da inadimplência
3. persistência de fluxo negativo
4. patrimônio líquido negativo
5. dados essenciais insuficientes

### Inadimplência

A inadimplência continua gradual e considera:

- dias máximos de atraso;
- materialidade do valor vencido sobre a renda.

Se tempo e materialidade caírem em faixas diferentes, prevalece o nível mais grave.

### Fluxo negativo persistente

O modelo deixou de somar penalizações temporais de forma redundante.

Agora, utiliza apenas o nível progressivo mais severo aplicável ao histórico recente.

## Classificação oficial do MF Score

- `900-1000` - Excelente - Risco Muito Baixo
- `800-899` - Muito Bom - Risco Baixo
- `700-799` - Bom - Risco Moderado
- `600-699` - Atenção - Risco Moderado-Alto
- `400-599` - Crítico - Risco Alto
- `0-399` - Muito Crítico - Risco Muito Alto

## Tendência e histórico

O projeto continua com:

- histórico mensal persistido em `HistoricoMfScore`
- leitura de tendência
- laboratório
- personas
- base oficial de simulação

Esses elementos não foram removidos nesta rodada. Eles continuam sendo parte da governança oficial do Motor Financeiro.

## Papel do laboratório na versão atual

Os cenários oficiais do laboratório continuam sendo a referência principal para auditoria e calibração do modelo.

Nesta rodada, a auditoria completa dos cenários foi usada como base conceitual para corrigir:

- semântica de endividamento;
- leitura patrimonial;
- foco operacional do fluxo;
- peso excessivo de configuração em planejamento;
- projeção de receitas futuras;
- gradação das penalizações temporais;
- faixas qualitativas subdimensionadas.

## Próxima etapa oficial

Depois desta refatoração conceitual, a próxima rodada deve se concentrar em:

1. rerrodar a auditoria operacional completa do laboratório já com a versão `mf-score-v2.4-1000`
2. consolidar a auditoria humana dos cenários oficiais
3. recalibrar numericamante os pesos e faixas que ainda precisarem de ajuste fino
4. decidir se os horizontes `30/90/180/365` permanecem exatamente como estão ou se precisam de nova redução de influência

