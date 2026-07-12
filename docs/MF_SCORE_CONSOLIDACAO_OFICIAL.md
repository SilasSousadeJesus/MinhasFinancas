# MF Score — Consolidação Oficial de Direção

Este documento consolida a posição oficial do projeto após a leitura conjunta de:

- `docs/MF_Score_Framework_Definitivo.md`
- `docs/MF_SCORE_REVIEW.md`
- `docs/propostas matemáticas para indicadores e penalizações.md`
- `docs/MF_SCORE_CALIBRATION_CONTEXT.md`

O objetivo deste material é separar claramente:

1. o **framework conceitual** que passa a orientar o `MF Score`;
2. as **propostas matemáticas** que foram avaliadas;
3. o que **entra na próxima implementação**;
4. o que fica para **etapas futuras**.

Este documento não altera o código por si só. Ele define a direção oficial para a próxima evolução do motor.

---

## 1. Premissas oficiais do projeto

### 1.1 O que o MF Score mede

O `MF Score` mede:

> A saúde financeira do usuário considerando sua situação atual, seus riscos e sua capacidade de manter estabilidade financeira.

Isso significa que o score não deve medir apenas:

- riqueza acumulada;
- disciplina subjetiva;
- renda isolada;
- comportamento de crédito bancário tradicional.

Ele deve refletir principalmente:

- equilíbrio operacional do presente;
- vulnerabilidades estruturais;
- risco materializado;
- capacidade de continuidade financeira.

### 1.2 Apetite de risco oficial

O apetite de risco do produto passa a ser tratado explicitamente como:

`MODERADO`

Na prática, isso significa:

- o modelo não deve ser permissivo a ponto de maquiar fragilidades relevantes;
- o modelo não deve ser punitivo a ponto de derrubar drasticamente o score por qualquer oscilação pequena;
- fragilidade estrutural deve degradar pilares com clareza;
- ruptura materializada deve gerar penalidade relevante;
- persistência temporal deve agravar o risco, mas sem opacidade excessiva.

### 1.3 Princípio central

O `MF Score` continua sendo um **motor de risco financeiro pessoal explicável**.

Logo:

- fórmulas devem ser auditáveis;
- pesos devem ser compreensíveis;
- penalizações devem ser rastreáveis;
- o usuário e a equipe devem conseguir explicar por que o score subiu ou caiu.

---

## 2. Consolidação do framework conceitual

Esta seção define o que está **aceito oficialmente** como estrutura conceitual do motor.

## 2.1 Posicionamento conceitual oficial

Foi oficialmente aceito que o `MF Score` é um:

`Score de risco financeiro pessoal`

e não um:

- score de riqueza;
- score comportamental puro;
- score de felicidade financeira;
- score bancário tradicional.

## 2.2 Arquitetura em camadas

Permanece oficial a arquitetura em quatro camadas:

1. Indicadores
2. Pilares
3. Penalizações críticas
4. Persistência temporal e histórico

Essa estrutura continua correta e não será substituída na próxima implementação.

## 2.3 Regra de não dupla penalização

Este princípio continua oficial e inegociável:

- um problema estrutural deve, antes de tudo, degradar o pilar correspondente;
- a penalização crítica só deve existir quando houver gravidade adicional, materialização real ou persistência claramente justificável.

Exemplos:

- reserva baixa degrada Liquidez;
- comprometimento alto degrada Fluxo de Caixa e/ou Endividamento;
- inadimplência gera penalização crítica porque representa ruptura real;
- fluxo negativo recorrente gera penalização crítica porque representa persistência de deterioração.

## 2.4 Pilares continuam válidos

Os cinco pilares atuais permanecem:

1. Fluxo de Caixa
2. Liquidez e Reserva
3. Endividamento e Obrigações
4. Patrimônio
5. Planejamento e Disciplina

Na próxima implementação, eles serão preservados.

Não haverá mudança estrutural de pilares nesta etapa.

## 2.5 Escalas continuam válidas

Permanecem oficiais:

- pilares em `0 a 100`
- score final em `0 a 1000`

## 2.6 Horizontes temporais futuros continuam válidos

Permanecem oficiais nesta etapa os horizontes:

- 30 dias
- 90 dias
- 180 dias
- 365 dias

Ainda não será adotado um único indicador unificado de exaustão futura.

---

## 3. Separação oficial: framework conceitual x propostas matemáticas

### 3.1 O que aceitamos do framework conceitual

Foi aceito como direção oficial:

- o `MF Score` deve medir risco financeiro pessoal;
- o modelo deve distinguir risco estrutural, operacional, materializado e persistente;
- a camada de penalizações críticas deve continuar separada dos pilares;
- a inadimplência precisa amadurecer;
- o pilar `Planejamento` ainda é uma lacuna real do modelo;
- a calibragem deve respeitar explicabilidade e rastreabilidade.

### 3.2 O que não aceitamos automaticamente do framework

Não foi aceito automaticamente:

- adotar todos os números propostos como oficiais;
- mover imediatamente indicadores entre pilares sem nova validação;
- zerar pilares por combinação indireta de outros pilares;
- introduzir complexidade temporal alta antes de estabilizar a base.

### 3.3 O que aceitamos das propostas matemáticas

As propostas matemáticas foram consideradas úteis como fonte de evolução, especialmente em:

- granularidade de status;
- amadurecimento da inadimplência;
- correção de inconsistências semânticas;
- melhoria da proporcionalidade das penalizações.

### 3.4 O que não aceitamos automaticamente das propostas matemáticas

Não foi aceito para a próxima implementação:

- reescrever todo o motor com funções sigmoides;
- trocar imediatamente o modelo atual por um motor totalmente contínuo e não linear;
- aplicar personas dinâmicas no cálculo do score;
- colapsar os quatro horizontes futuros em um único índice matemático;
- introduzir já agora curva de cura, reincidência e amortização residual complexa;
- adotar Machine Learning.

---

## 4. Decisão oficial sobre o caminho da próxima implementação

### 4.1 Estratégia escolhida

A próxima implementação do `MF Score` seguirá um caminho:

`incremental, explicável, matematicamente mais coerente e compatível com apetite de risco moderado`

Isso significa:

- não faremos uma reescrita completa do motor;
- corrigiremos primeiro as distorções mais importantes;
- introduziremos mais nuance onde hoje o modelo está binário ou polarizado;
- manteremos a arquitetura atual.

### 4.2 Objetivo da próxima implementação

A próxima implementação terá foco em três problemas centrais:

1. corrigir incoerências de cálculo;
2. reduzir binarismo excessivo;
3. tornar as penalizações mais proporcionais ao risco real.

---

## 5. O que entra na próxima implementação do MF Score

Esta seção define o escopo oficial da próxima rodada.

## 5.1 Entra: correção semântica do indicador Economia Mensal

### Problema atual

Hoje `Economia Mensal` compara valor em moeda com meta percentual.

Isso é conceitualmente incorreto.

### Decisão oficial

Na próxima implementação, a meta monetária da economia mensal será derivada da renda do mês.

### Nova referência oficial

`MetaEconomiaMensal = ReceitaMensalAtual * (PercentualEconomiaMensalDesejado / 100)`

### Consequência

O indicador continua existindo, mas passa a comparar:

- valor atual economizado em moeda;
- contra uma meta monetária coerente com o percentual desejado do perfil.

Essa mudança entra na próxima implementação.

## 5.2 Entra: maior granularidade de status para indicadores centrais

### Problema atual

O modelo usa pouco o status `Bom` e fica excessivamente polarizado entre:

- Excelente
- Atenção
- Crítico

### Decisão oficial

Na próxima implementação, alguns indicadores centrais deixarão de depender apenas de `ResolverMetaMinima` e `ResolverMetaMaxima` binários e passarão a trabalhar com faixas explícitas.

### Indicadores priorizados nesta etapa

- Percentual de Economia
- Reserva de Emergência Atual
- Comprometimento da Renda
- Comprometimento Financeiro Futuro 30 dias
- Endividamento Patrimonial

### Faixas oficiais aprovadas para a próxima implementação

#### Percentual de Economia

- `Excelente` → `>= 20%`
- `Bom` → `>= 10% e < 20%`
- `Atenção` → `>= 0% e < 10%`
- `Crítico` → `< 0%`

#### Reserva de Emergência Atual

Baseada em `CoberturaReservaEmMeses`:

- `Excelente` → `>= 6 meses`
- `Bom` → `>= 4 e < 6 meses`
- `Atenção` → `>= 2 e < 4 meses`
- `Crítico` → `< 2 meses`

#### Comprometimento da Renda

- `Excelente` → `<= 20%`
- `Bom` → `> 20% e <= 35%`
- `Atenção` → `> 35% e <= 50%`
- `Crítico` → `> 50%`

#### Comprometimento Financeiro Futuro 30 dias

- `Excelente` → `<= 25%`
- `Bom` → `> 25% e <= 40%`
- `Atenção` → `> 40% e <= 55%`
- `Crítico` → `> 55%`

#### Endividamento Patrimonial

- `Excelente` → `<= 15%`
- `Bom` → `> 15% e <= 30%`
- `Atenção` → `> 30% e <= 50%`
- `Crítico` → `> 50%`

### Observação importante

Essas faixas entram na próxima implementação como primeira régua oficial de calibragem moderada.

Elas poderão ser refinadas futuramente com base em:

- auditoria humana;
- personas;
- testes comparativos;
- histórico real do motor.

## 5.3 Entra: inadimplência gradual

### Problema atual

Hoje a inadimplência é binária:

- existe atraso → penaliza cheio
- não existe atraso → não penaliza

Isso é inadequado.

### Decisão oficial

A próxima implementação passa a usar uma matriz gradual de inadimplência baseada em:

- dias de atraso;
- materialidade do valor vencido em relação à renda mensal.

### Regra oficial aprovada para a próxima implementação

O cálculo deve considerar:

- `DiasAtraso = DataReferencia - DataVencimento`
- `PercentualValorEmAtraso = ValorTotalEmAtraso / ReceitaMensalAtual`

### Níveis aprovados

#### Nível 1 — Atraso técnico

Critério:

- até `7 dias` de atraso
- e valor total em atraso `< 10%` da renda mensal

Penalidade:

`-30 pontos`

#### Nível 2 — Estresse moderado

Critério:

- `8 a 30 dias` de atraso
  **ou**
- valor em atraso entre `10% e 25%` da renda mensal

Penalidade:

`-90 pontos`

#### Nível 3 — Inadimplência relevante

Critério:

- `31 a 60 dias` de atraso
  **ou**
- valor em atraso entre `25% e 50%` da renda mensal

Penalidade:

`-170 pontos`

#### Nível 4 — Inadimplência grave

Critério:

- mais de `60 dias` de atraso
  **ou**
- valor em atraso `> 50%` da renda mensal

Penalidade:

`-250 pontos`

### Regra de escolha

Se tempo e materialidade caírem em níveis diferentes, prevalece o **nível mais grave**.

### Justificativa

Essa abordagem é compatível com apetite de risco moderado porque:

- pune pouco o atraso técnico;
- pune fortemente a inadimplência estrutural;
- preserva explicabilidade;
- evita introduzir uma fórmula contínua opaca logo nesta etapa.

## 5.4 Entra: manutenção da estrutura atual de pilares e pesos

Na próxima implementação:

- os cinco pilares continuam os mesmos;
- os pesos dos pilares continuam os mesmos;
- os pesos dos indicadores continuam os mesmos.

### Justificativa

Primeiro vamos corrigir:

- semântica;
- granularidade;
- proporcionalidade da inadimplência.

Só depois reabriremos discussão de pesos.

## 5.5 Entra: manutenção temporária das penalizações estruturais já existentes

Permanecem na próxima implementação, sem reescrita completa:

- fluxo mensal negativo;
- dois meses consecutivos negativos;
- três ou mais meses consecutivos negativos;
- patrimônio líquido negativo;
- dados essenciais insuficientes.

### Decisão oficial

Essas penalizações permanecem por enquanto, mas serão reavaliadas após a entrada da inadimplência gradual e da nova régua de status.

### Justificativa

Mudar ao mesmo tempo:

- status;
- inadimplência;
- temporalidade;
- pesos;
- pilares

geraria complexidade demais para uma única rodada.

## 5.6 Entra: revalidação das personas após a implementação

Após implementar as mudanças acima, o projeto deverá reavaliar oficialmente as personas sintéticas e casos canônicos.

### Importante

Essa revalidação entra como etapa obrigatória do ciclo, mas não como mudança estrutural do cálculo em si.

---

## 6. O que não entra na próxima implementação

## 6.1 Não entra: sigmoides e funções contínuas complexas

Embora matematicamente interessantes, essas propostas ainda aumentam:

- opacidade;
- dificuldade de auditoria;
- dificuldade de explicação ao usuário;
- risco de calibração prematura.

## 6.2 Não entra: personas dinâmicas no cálculo oficial

Ainda não entra:

- Entrante / baixa renda
- CLT alta renda
- Autônomo / PJ volátil

como mecanismo automático de mudança de pesos e inflexões.

Essa ideia continua promissora, mas é etapa posterior.

## 6.3 Não entra: unificação dos horizontes futuros em um único índice

O projeto continuará com:

- 30 dias
- 90 dias
- 180 dias
- 365 dias

por serem mais auditáveis e mais fáceis de explicar neste momento.

## 6.4 Não entra: curva de cura e reincidência avançada

Ficam adiados:

- multiplicador por reincidência em 90 dias;
- amortização residual da penalidade;
- limpeza progressiva do risco temporal;
- EMA.

## 6.5 Não entra: mudança radical do pilar Planejamento

Reconhecemos oficialmente que o pilar ainda é proxy.

Mas a próxima implementação não vai resolver esse problema por completo.

Essa evolução fica para uma etapa própria.

## 6.6 Não entra: Machine Learning

Continua rejeitado no estado atual do projeto.

---

## 7. Sequência oficial da próxima implementação

Ordem recomendada:

1. Corrigir `Economia Mensal`.
2. Introduzir as novas faixas de status dos indicadores priorizados.
3. Implementar inadimplência gradual.
4. Rodar auditoria operacional.
5. Rodar auditoria humana.
6. Revisar personas sintéticas e casos canônicos.
7. Só então decidir se haverá recalibração de pesos e temporalidade.

---

## 8. Próxima pauta após essa implementação

Depois dessa rodada, a próxima discussão oficial do `MF Score` deverá se concentrar em:

1. recalibração das penalizações temporais;
2. evolução real do pilar `Planejamento e Disciplina`;
3. possível revisão da posição do indicador `Comprometimento da Renda`;
4. revisão de pesos dos horizontes futuros;
5. possível entrada futura de personas dinâmicas.

---

## 9. Decisão oficial final

Fica oficialmente definido que:

- o **framework conceitual** proposto pela IA foi amplamente aceito;
- as **propostas matemáticas** serão adotadas apenas de forma incremental;
- o projeto seguirá uma evolução `moderada`, `explicável` e `auditável`;
- a próxima implementação do `MF Score` terá foco em:
  - correção semântica da economia mensal;
  - granularidade de status;
  - inadimplência gradual;
  - revalidação das personas após a mudança.

Em resumo:

> O projeto não vai reescrever o `MF Score` inteiro agora.  
> O projeto vai fortalecer o motor atual, removendo incoerências, reduzindo binarismos e tornando a penalização mais proporcional ao risco real.

---

## 10. Status após implementação da versão `mf-score-v2.4-1000`

O pacote conceitual aprovado neste documento foi implementado de forma incremental, preservando a arquitetura oficial do Motor Financeiro.

### Implementado nesta rodada

- correção do papel operacional do pilar `Fluxo de Caixa`
- separação conceitual entre dívida de consumo, financiamento patrimonial, obrigações recorrentes e inadimplência
- priorização da situação patrimonial real no pilar `Patrimônio`
- redução do peso de configuração pura e aumento de execução real no pilar `Planejamento e Disciplina`
- substituição da soma de penalizações temporais por um único nível progressivo de persistência de fluxo negativo
- correção da projeção de receitas recorrentes em `180` e `365` dias
- endurecimento qualitativo das leituras de pressão acumulada acima de `100%`
- melhoria da apresentação humana de indicadores analíticos

### O que continua para a próxima rodada

- rerrodar a auditoria operacional completa do laboratório já com `mf-score-v2.4-1000`
- consolidar a auditoria humana dos cenários oficiais
- recalibrar numericamente notas, faixas e pesos finos com base nessa nova evidência
- decidir se os horizontes `30/90/180/365` permanecem exatamente como estão ou se ainda precisam de nova redução de influência

---

## Complemento oficial - calibragem de falso positivo em perfis iniciantes

Fica oficialmente consolidado que:

- ausencia de patrimonio e passivos, por si so, nao caracteriza insolvencia;
- `patrimonio zerado sem passivos` deve ser lido como `ponto de partida patrimonial neutro`;
- `reserva zerada` continua sendo fragilidade estrutural, mas pode ser parcialmente atenuada quando a economia mensal atual permite formar a reserva ideal em prazo curto;
- essa atenuacao deve ocorrer na camada dos pilares, sem criar nova penalizacao critica.
