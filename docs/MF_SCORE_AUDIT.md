# Auditoria do MF Score

Este documento consolida a visão oficial de auditoria do `MF Score`.

Ele responde:

1. como o motor está desenhado hoje;
2. quais problemas conceituais já foram corrigidos;
3. quais limitações continuam abertas;
4. qual deve ser a próxima rodada de calibração.

## Resumo executivo

O `MF Score` continua sendo o modelo oficial de saúde financeira do sistema.

Na versão atual, `mf-score-v2.4-1000`, o motor preserva:

- cinco pilares;
- escala final de `0 a 1000`;
- histórico mensal;
- tendência;
- laboratório;
- personas e base oficial de simulação.

Esta rodada foi uma refatoração conceitual, não uma reinvenção arquitetural.

As principais correções incorporadas foram:

- separação semântica entre dívida de consumo, financiamento patrimonial, obrigações recorrentes e inadimplência;
- reposicionamento do pilar `Patrimônio` para priorizar a situação patrimonial real;
- foco operacional mais claro no pilar `Fluxo de Caixa`;
- redução do peso de configuração pura no pilar `Planejamento e Disciplina`;
- substituição da penalização temporal somada por um único nível progressivo de persistência de fluxo negativo;
- correção da projeção de receitas recorrentes nos horizontes de `180` e `365` dias;
- endurecimento qualitativo dos indicadores de pressão acumulada acima de `100%`;
- melhoria da linguagem de apresentação dos indicadores.

Após a primeira rodada completa de auditoria humana pós benchmark, a conclusão oficial passou a ser:

- a arquitetura da versão `mf-score-v2.4-1000` está aprovada;
- a próxima etapa do projeto deve focar calibração fina de notas e curvas;
- mudanças estruturais só devem voltar a ser discutidas se uma auditoria futura demonstrar falha conceitual relevante.
- o detalhamento oficial por cenário agora está consolidado em `docs/MF_SCORE_BENCHMARK.md`, incluindo notas humanas, faixas aceitáveis, diferenças e a invalidação formal de `MF-CENARIO-02`, `MF-CENARIO-07` e `MF-CENARIO-09`.

## Arquitetura atual do score

O cálculo oficial continua em quatro camadas:

1. indicadores
2. pilares
3. penalizações críticas
4. histórico e persistência temporal

### Pilares oficiais

1. Fluxo de Caixa
2. Liquidez e Reserva
3. Endividamento e Obrigações
4. Patrimônio
5. Planejamento e Disciplina

## Governança oficial

### Regra de não dupla penalização

Permanece oficial:

- reserva baixa reduz `Liquidez e Reserva`, mas não gera crítica automática;
- comprometimento alto reduz `Fluxo de Caixa`, mas não simula ruptura sozinho;
- pressão futura reduz `Endividamento e Obrigações`, mas não equivale automaticamente a inadimplência;
- persistência de fluxo negativo usa apenas o nível progressivo mais grave, sem empilhar penalizações temporais sobre o mesmo fato.

### Penalizações críticas aceitas na versão atual

- inadimplência atual
- reincidência ou cura recente da inadimplência
- persistência de fluxo negativo
- patrimônio líquido negativo
- dados essenciais insuficientes

## Cobertura atual dos pilares

| Pilar | Cobertura atual | Avaliação técnica |
| --- | --- | --- |
| Fluxo de Caixa | Excelente | Passou a medir melhor a capacidade operacional do mês, com menos redundância conceitual. |
| Liquidez e Reserva | Excelente | Continua robusto e agora mantém leitura mais humana na formação de reserva. |
| Endividamento e Obrigações | Boa | Evoluiu bastante ao separar naturezas diferentes de dívida, mas ainda precisa calibração numérica fina. |
| Patrimônio | Boa | Agora reflete melhor a situação patrimonial real, sem depender em excesso do patrimônio-alvo. |
| Planejamento e Disciplina | Boa | Ficou conceitualmente mais correto, mas ainda exige amadurecimento de execução histórica e estratégica. |

## Cobertura do domínio

| Conceito | Cobertura atual | Observação |
| --- | --- | --- |
| Fluxo de Caixa | Forte | Mais aderente à pergunta operacional: o mês fecha bem ou mal? |
| Liquidez | Forte | Boa combinação entre reserva atual, reserva ideal e capacidade de formação. |
| Endividamento | Melhorada | Já diferencia dívida de consumo, passivo patrimonial e obrigações futuras. |
| Patrimônio | Melhorada | O foco principal passou a ser a situação patrimonial real. |
| Planejamento | Parcial | Ainda há espaço para aprofundar aderência, disciplina histórica e execução estratégica. |
| Persistência temporal | Parcial | A lógica ficou mais correta, mas ainda precisa validação quantitativa completa no laboratório. |

## Limitações conhecidas

- o pilar `Planejamento e Disciplina` ainda não captura toda a execução comportamental ao longo do tempo;
- a reincidência e a cura da inadimplência ainda podem amadurecer em granularidade histórica;
- os horizontes `30/90/180/365` continuam válidos, mas ainda precisam nova rodada de confirmação quantitativa após a correção da projeção de receitas;
- a auditoria completa do laboratório deve ser rerrodada formalmente com a versão `mf-score-v2.4-1000` para consolidar as novas faixas e notas;
- a calibração fina entre cenários de alta renda, autônomos, patrimônio elevado com fluxo ruim e famílias financiadas ainda depende da próxima rodada operacional.

## Achados formais

### MF-001 — Planejamento ainda não mede toda a execução real

- **Impacto:** o pilar já evoluiu, mas ainda não captura integralmente disciplina histórica e aderência de longo prazo.
- **Prioridade:** Alta
- **Status:** Aberto

### MF-002 — Endividamento precisava separar naturezas diferentes de obrigação

- **Impacto:** financiamento patrimonial estava conceitualmente próximo demais de dívida de consumo.
- **Prioridade:** Alta
- **Status:** Mitigado nesta rodada conceitual

### MF-003 — Patrimônio-alvo não podia dominar a leitura patrimonial

- **Impacto:** usuários com patrimônio líquido positivo relevante poderiam ser subavaliados por estarem distantes da meta.
- **Prioridade:** Alta
- **Status:** Mitigado nesta rodada conceitual

### MF-004 — Projeção futura de receitas estava subestimando cenários recorrentes

- **Impacto:** a pressão futura ficava artificialmente inflada, especialmente em `180` e `365` dias.
- **Prioridade:** Alta
- **Status:** Mitigado em código; pendente rerrodada formal da auditoria do laboratório

## Dívida técnica do Motor Financeiro

- rerrodar a auditoria operacional completa dos cenários oficiais na versão `mf-score-v2.4-1000`
- consolidar a auditoria humana sobre os 12 cenários do laboratório
- revisar pesos finos dos horizontes futuros após observar a nova projeção de receitas
- amadurecer reincidência e cura da inadimplência com base histórica mais longa
- aprofundar o pilar `Planejamento e Disciplina` com sinais determinísticos de execução real

## Laboratório do MF Score

O laboratório continua sendo a referência oficial de validação prática do motor.

Ele deve ser usado para:

- inspecionar usuários reais;
- inspecionar a base oficial de simulação;
- comparar o comportamento do score entre cenários;
- validar se a refatoração conceitual manteve coerência entre casos como:
  - estudante
  - família financiada
  - alta renda organizada
  - patrimônio elevado com fluxo ruim
  - autônomos
- inadimplência materializada

## Benchmark oficial

O projeto passa a manter também `docs/MF_SCORE_BENCHMARK.md` como referência permanente de comportamento esperado.

Esse benchmark não substitui a auditoria técnica. Ele registra a expectativa humana oficial sobre os 12 cenários da Base Oficial de Simulação e deve ser atualizado em toda rodada relevante do motor.

## Situação atual

O `MF Score` hoje está:

- implementado;
- integrado à Saúde Financeira;
- integrado ao Assistente Financeiro;
- historizado;
- governado por documentação própria;
- pronto para a próxima rodada operacional de calibração da versão `mf-score-v2.4-1000`.

## Próxima rodada oficial

Depois desta refatoração conceitual, a próxima rodada deve:

1. recalibrar notas, faixas, pesos finos e curvas com base no benchmark oficial;
2. revisar a dominância relativa de `Liquidez e Reserva`;
3. revisar a influência residual dos horizontes `30/90/180/365`;
4. preservar a arquitetura da `v2.4`, salvo evidência futura de falha conceitual relevante.

## Direção oficial da auditoria para a `v2.5`

A auditoria consolidada da `v2.4` passa a reconhecer o seguinte padrão:

- `Fluxo de Caixa` continua excessivamente severo em cenários saudáveis ou recuperáveis;
- `Liquidez e Reserva` é hoje a maior fonte de divergência entre motor e benchmark humano;
- os extremos da escala ainda estão comprimidos;
- a arquitetura do motor continua correta, mas a sensibilidade numérica ainda não está madura.

### Leitura oficial da divergência atual

1. `Liquidez e Reserva` tende a exigir uma reserva muito próxima da ideal para conceder notas realmente boas.
2. `Fluxo de Caixa` ainda derruba demais cenários com organização razoável, pouca folga e ausência de ruptura.
3. `Endividamento e Obrigações` ainda precisa distinguir melhor dívida organizada de risco equivalente à inadimplência.
4. as penalizações críticas existentes ainda achatam demais a base da escala em alguns cenários ruins, porém não terminais.

### Diretriz de execução da próxima sprint

A próxima sprint do MF Score deverá ser tratada como **calibração fina numérica**.

Ela não deverá:

- criar novos indicadores;
- criar novos pilares;
- alterar a arquitetura do motor;
- criar novas penalizações.

Ela deverá atuar apenas em:

- curvas;
- pesos finos;
- faixas qualitativas;
- severidade relativa das penalizações já existentes.

### Ordem prioritária de atuação

Com base na evidência dos cenários auditados, a ordem oficial de impacto passa a ser:

1. `Liquidez e Reserva`
2. `Fluxo de Caixa`
3. `Endividamento e Obrigações`
4. compressão das penalizações

### Critério de fechamento da `v2.5`

A `v2.5` só deverá ser considerada concluída quando:

1. os 12 cenários oficiais forem rerrodados;
2. o benchmark for comparado automaticamente;
3. for medido quantos cenários entraram na faixa aceitável;
4. a documentação registrar claramente quais divergências restantes ainda exigem nova calibração.
