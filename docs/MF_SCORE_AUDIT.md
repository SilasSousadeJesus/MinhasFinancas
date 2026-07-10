# Auditoria do MF Score

Este documento é a leitura única e consolidada do estado atual do `MF Score`.

Ele existe para responder:

1. como o Motor Financeiro funciona hoje
2. como ele está sendo governado
3. quais limitações conhecidas ainda existem

## Resumo executivo

O `MF Score` já é o modelo oficial de risco financeiro pessoal do sistema.

Na versão atual, o modelo foi reformulado para:

- usar escala final de `0 a 1000`
- preservar pilares em `0 a 100`
- separar nota dos pilares de penalizações críticas
- tratar persistência temporal como agravante real
- registrar histórico mensal persistido

O motor está funcional, auditável e já integrado às principais telas, mas continua em fase de calibração contínua.

Na revisão mais recente, o motor recebeu três avanços estruturais importantes:

- regularização semântica da meta de `Economia Mensal`
- maior granularidade de status em indicadores centrais
- amadurecimento da inadimplência de modelo binário para matriz gradual
- recalibragem proporcional das penalizações temporais de fluxo negativo
- reforço explícito do pilar `Planejamento` com configuração mínima do perfil financeiro
- redução deliberada da influência dos horizontes 90/180/365 sobre o risco corrente

## Arquitetura atual do score

O cálculo oficial possui quatro camadas:

1. indicadores
2. pilares
3. penalizações críticas
4. histórico e persistência temporal

### Indicadores

Medem fatos financeiros específicos.

### Pilares

Organizam os indicadores em cinco grupos:

1. Fluxo de Caixa
2. Liquidez
3. Endividamento
4. Patrimônio
5. Planejamento

### Penalizações críticas

Aplicadas apenas para:

- risco grave
- risco já materializado
- persistência temporal negativa

### Histórico

Registrado em `HistoricoMfScore` por competência mensal.

## Governança oficial

### Regra de não dupla penalização

Este é um princípio oficial do Motor Financeiro.

Exemplos:

- reserva zero reduz `Liquidez`, mas não deve gerar penalização crítica automática só por existir
- comprometimento alto reduz `Fluxo de Caixa`, mas não deve gerar penalização crítica automática sem ruptura
- pressão futura reduz pilares estruturais, mas não deve sozinha simular inadimplência

### Penalizações críticas aceitas na versão atual

- inadimplência
- fluxo mensal negativo
- dois meses consecutivos negativos
- três ou mais meses consecutivos negativos
- patrimônio líquido negativo
- dados essenciais ausentes

### Penalizações críticas removidas ou reclassificadas

Os seguintes itens deixaram de ser tratados como penalização crítica automática e passaram a agir prioritariamente nos pilares:

- reserva de emergência inexistente
- comprometimento elevado da renda
- pressão futura de 30 dias
- pressão financeira acumulada
- endividamento patrimonial alto sem evidência de ruptura materializada

## Histórico mensal oficial

O projeto agora possui persistência mensal do score por meio de `HistoricoMfScore`.

### Objetivo

- preservar a fotografia mensal do score
- sustentar tendência com base real
- permitir auditoria histórica
- preparar análises comparativas futuras

### Campos principais

- `CompetenciaAno`
- `CompetenciaMes`
- `MfScoreBase`
- `MfScoreFinal`
- `Classificacao`
- `Risco`
- `PenalidadeTotal`
- `VersaoModelo`
- `JsonPilares`
- `JsonIndicadoresCriticos`
- `JsonResumo`

## Hangfire

O projeto agora possui job recorrente mensal para registrar o histórico do `MF Score`.

### Configuração atual

- cron: `0 2 1 * *`
- execução: dia `01` de cada mês
- competência calculada: `mês anterior`

### Responsabilidades

- buscar usuários ativos
- calcular o score oficial
- persistir o histórico
- registrar logs
- isolar falhas por usuário

## Cobertura atual dos pilares

| Pilar | Cobertura atual | Avaliação técnica |
| --- | --- | --- |
| Fluxo de Caixa | Excelente | Mede economia, percentual de economia, comprometimento da renda e curto prazo com boa clareza. |
| Liquidez | Excelente | Reserva atual e meta ideal formam leitura robusta de proteção imediata. |
| Endividamento | Boa | Cobre endividamento patrimonial e pressão futura, mas ainda pode amadurecer leitura comportamental de dívida. |
| Patrimônio | Boa | Cobre patrimônio líquido atual e patrimônio-alvo, mas ainda pode evoluir em qualidade patrimonial e liquidez dos ativos. |
| Planejamento | Boa | Agora exige configuração mínima explícita do perfil financeiro, mas ainda não mede toda a disciplina comportamental do usuário. |

## Cobertura do domínio

| Conceito | Cobertura atual | Observação |
| --- | --- | --- |
| Fluxo de Caixa | Completo | Bem representado no mês corrente e curto prazo. |
| Liquidez | Completo | Reserva atual e meta ideal estão bem representadas. |
| Endividamento | Parcial | Há boa leitura patrimonial e futura, mas a semântica de dívida ainda pode amadurecer. |
| Patrimônio | Parcial | Boa fotografia atual, mas pouca profundidade qualitativa no cálculo. |
| Planejamento | Boa | Já combina configuração explícita do perfil financeiro com sinais de execução, mas ainda não cobre todo o comportamento real. |
| Persistência temporal | Inicial | Já existe penalização por recorrência negativa e histórico persistido. |
| Tendência histórica | Inicial | Já pode usar histórico real, mas ainda pode amadurecer. |

## Limitações conhecidas

- o pilar `Planejamento` ainda não mede todo o comportamento financeiro real do usuário
- o plano estratégico vigente e os compromissos financeiros já podem influenciar o pilar `Planejamento`, mas ainda de forma inicial e determinística
- a ausência de plano estratégico ou compromissos não deve gerar penalização automática; esses sinais só contam quando existem
- a leitura de inadimplência já evoluiu para matriz gradual e passou a reconhecer reincidência/cura, mas ainda pode amadurecer em histórico mais rico de atraso
- a qualidade patrimonial ainda não é diferenciada com profundidade
- a tendência histórica ainda está em fase inicial de amadurecimento
- os horizontes 90/180/365 já estão mais leves, mas ainda exigem validação contínua para confirmar se a influência residual ficou adequada

## Achados formais

### MF-001 - Pilar Planejamento utiliza proxies

- **ID:** `MF-001`
- **Impacto:** o pilar evoluiu, mas ainda não captura integralmente a disciplina e a execução comportamental do usuário
- **Prioridade:** Alta
- **Status:** Aberto

### MF-002 - Faixas antigas de personas estavam otimistas demais

- **ID:** `MF-002`
- **Impacto:** três casos oficiais poderiam aparentar falha do motor quando o problema real era desalinhamento da faixa esperada
- **Prioridade:** Alta
- **Status:** Mitigado

Evidência da mitigação:

- a auditoria operacional mais recente registrou `8 de 8` cenários dentro da faixa esperada após revisão das personas:
  - `Boa renda, reserva zero e cartão alto`
  - `Excelente fluxo com pouco patrimônio`
  - `Planejamento excelente`

## Dívida técnica do Motor Financeiro

- amadurecer o pilar `Planejamento` com sinais estratégicos mais profundos e históricos
- consolidar a calibração de compromissos financeiros dentro do pilar `Planejamento`
- fortalecer a semântica de cura e reincidência da inadimplência
- revisar impacto da nova matriz gradual de inadimplência sobre personas e casos canônicos
- evoluir leitura histórica de tendência e persistência
- enriquecer o pilar `Patrimônio` com qualidade e liquidez patrimonial
- revisar se a nota opcional de plano e compromissos deve ganhar peso maior após validação humana suficiente

## Relação com validação e auditoria

Este documento governa lacunas, limitações e regras permanentes do Motor Financeiro.

Ele trabalha em conjunto com:

- `docs/MF_SCORE.md` - funcionamento oficial do modelo
- `docs/MF_SCORE_VALIDATION.md` - cenários, faixas esperadas e validação
- auditoria operacional - conferência prática do motor
- auditoria humana - calibração cega das personas
- laboratório do MF Score - inspeção visual e somente leitura do score em usuários reais

## Laboratório do MF Score

O projeto agora possui uma tela interna chamada `Laboratório do MF Score`.

### Objetivo

- inspecionar usuários reais do sistema
- entender como o motor oficial construiu o score daquele usuário
- facilitar auditoria visual sem criar dados artificiais

### Escopo

O laboratório exibe:

- score base
- score final
- classificação
- risco
- penalidade total
- pilares
- indicadores
- indicadores críticos
- penalizações
- regras críticas aplicadas
- dados resumidos de entrada
- observações de limitação e cobertura

### Regras

- a tela é somente leitura
- não cria, edita, exclui nem promove personas
- não altera fórmulas do `MF Score`
- não grava snapshots específicos do laboratório
- reutiliza o mesmo motor oficial já consumido por Saúde Financeira e Assistente Financeiro

## Situação atual

O `MF Score` hoje está:

- implementado
- integrado à Saúde Financeira
- integrado ao Assistente Financeiro
- calibrado por personas
- auditado automaticamente
- auditado humanamente
- persistido por competência mensal

## Decisões oficiais da rodada atual

- `Comprometimento da Renda` permanece conceitualmente em `Fluxo de Caixa`, e não em `Planejamento`
- os horizontes `30/90/180/365` continuam válidos, mas com peso decrescente à medida que o prazo aumenta
- o pilar `Planejamento e Disciplina` só pode atingir zona saudável com os cinco parâmetros básicos do `Perfil Financeiro` configurados
- a penalização temporal de fluxo negativo foi suavizada no primeiro mês e mantida forte quando a deterioração vira recorrência

O próximo passo não é reinventar o score. É continuar calibrando-o com rigor, usando histórico real, personas e auditoria contínua.

## Atualizacao oficial v2.3

- `Reserva zero` continua sendo fragilidade estrutural, mas agora a auditoria tambem observa a `Capacidade de Formacao de Reserva` para evitar falso positivo de risco em perfis iniciantes com alta sobra mensal.
- `Patrimonio zerado sem passivos` passa a ser interpretado como `ponto de partida patrimonial neutro`.
- O Laboratorio do MF Score deve exibir com clareza:
  - a observacao de neutralidade patrimonial quando aplicavel
  - o indicador `Capacidade de Formacao de Reserva`
  - os meses estimados para completar a reserva ideal
  - o efeito desse indicador dentro do pilar `Liquidez e Reserva`
