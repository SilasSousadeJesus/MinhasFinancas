# MF Score Validation

Este documento registra como validamos se o `MF Score` continua coerente ao longo do tempo.

Enquanto `docs/MF_SCORE.md` explica o funcionamento oficial do motor, este arquivo explica como protegemos o modelo contra regressões conceituais e numéricas.

## Objetivo

Garantir que mudanças no Motor Financeiro:

- melhorem a leitura do risco real;
- não criem distorções entre cenários;
- não violem a regra de não dupla penalização;
- permaneçam coerentes com o laboratório oficial.

## Filosofia de validação

O `MF Score` é um modelo de saúde financeira com apetite de risco `moderado`.

Portanto, a validação não deve procurar:

- maximizar severidade a qualquer custo;
- suavizar artificialmente casos ruins;
- premiar configuração em vez de execução;
- tratar financiamentos patrimoniais como se fossem iguais a dívidas de consumo.

## Escalas oficiais

- `MF Score final`: `0 a 1000`
- `MF Score base`: `0 a 1000`
- `pilares`: `0 a 100`

## Princípio central

Um mesmo fato econômico não deve ser punido duas vezes.

Logo:

- reserva baixa reduz `Liquidez e Reserva`
- comprometimento alto reduz `Fluxo de Caixa`
- pressão futura reduz `Endividamento e Obrigações`
- persistência de fluxo negativo usa apenas o nível progressivo mais grave

Esses fatores só devem virar penalização crítica quando houver:

- inadimplência;
- patrimônio líquido negativo;
- persistência temporal relevante;
- risco materializado.

## Referência oficial de cenários

Os cenários do laboratório continuam sendo a referência oficial de validação do Motor Financeiro.

Nesta rodada, eles devem ser usados para verificar se a versão `mf-score-v2.4-1000` corrigiu os principais problemas conceituais identificados na auditoria:

- mistura entre dívida de consumo e financiamento patrimonial;
- peso excessivo do patrimônio-alvo na leitura patrimonial;
- redundância interna do pilar `Fluxo de Caixa`;
- peso exagerado da simples configuração no pilar `Planejamento e Disciplina`;
- soma indevida de penalizações temporais de fluxo negativo;
- subprojeção de receitas recorrentes em `180` e `365` dias;
- faixas qualitativas brandas demais em pressões acima de `100%`.

## Casos que a validação precisa proteger

- estudante com estrutura ainda inicial
- família financiada
- alta renda organizada
- patrimônio elevado com fluxo ruim
- autônomos com oscilação real
- inadimplência materializada
- forte renda sem reserva
- boa execução com pouco patrimônio

## Regras esperadas por tema

### Endividamento e Obrigações

- dívida de consumo deve pesar mais que financiamento patrimonial de mesmo valor relativo;
- obrigações futuras recorrentes devem reduzir a nota, mas não equivaler automaticamente a inadimplência;
- inadimplência continua sendo o sinal mais grave do pilar.

### Patrimônio

- patrimônio líquido positivo relevante deve sustentar uma boa leitura patrimonial;
- patrimônio-alvo deve funcionar como evolução, não como base principal da nota;
- patrimônio negativo continua sendo sinal grave.

### Fluxo de Caixa

- o pilar deve responder se o mês fecha bem ou mal;
- `Economia Mensal`, `Percentual de Economia` e `Comprometimento da Renda` não devem competir entre si de modo redundante;
- pressão futura não deve dominar o fluxo mensal.

### Planejamento e Disciplina

- configuração mínima do perfil continua importante;
- execução real deve pesar mais do que simples preenchimento de parâmetros;
- plano estratégico e compromissos só contam quando existirem;
- a ausência desses elementos não deve punir automaticamente.

### Persistência temporal

- a penalização do mês negativo não deve ser somada com a penalização de recorrência;
- deve prevalecer somente o nível mais grave entre:
  - `1 mês`
  - `2 meses`
  - `3+ meses`
  - `6+ meses`
  - `12+ meses`

### Projeção futura

- receitas recorrentes precisam aparecer corretamente em `180` e `365` dias;
- pressões futuras acima de `100%` não podem permanecer apenas como `Atenção`.

## Auditoria operacional

A validação conceitual precisa ser acompanhada de auditoria operacional.

Endpoint interno:

- `POST /api/MfScoreAuditoria/GerarPlanilha`

Uso obrigatório quando houver mudança em:

- indicadores
- pilares
- pesos
- penalizações críticas
- classificação
- tendência
- projeção futura

## Base oficial de simulação

Além das personas em memória, o projeto possui uma base oficial de simulação persistida para inspeção pelo laboratório.

Essa base deve ser usada para:

- comparar comportamento entre cenários;
- inspecionar a leitura do motor em usuários sintéticos completos;
- validar se a correção conceitual permaneceu coerente em casos mais realistas.

## Resultado esperado desta rodada

Após a refatoração conceitual da versão `mf-score-v2.4-1000`, a próxima execução da auditoria deve confirmar:

- melhor separação entre dívida de consumo e financiamento patrimonial;
- leitura patrimonial menos dependente da meta;
- fluxo mensal mais coerente com a operação do mês;
- planejamento menos dependente de configuração pura;
- melhor coerência dos horizontes futuros após correção da projeção de receitas;
- classificação mais severa quando a pressão acumulada ultrapassa `100%`.

## Consolidação oficial pós auditoria humana da `v2.4`

A primeira rodada completa de auditoria humana confirmou que a versão `mf-score-v2.4-1000` corrigiu praticamente todas as distorções conceituais relevantes da `v2.3`.

Com isso:

- a arquitetura da `v2.4` passa a ser considerada estável;
- o benchmark oficial dos 12 cenários passa a ser obrigatório em qualquer evolução futura;
- a próxima etapa de validação deixa de buscar redesenho estrutural e passa a focar calibração fina numérica.
