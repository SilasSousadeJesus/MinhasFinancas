# Auditoria do MF Score

## Auditoria Comparativa da `mf-score-v2.5-1000`

Esta seção consolida a auditoria comparativa oficial da `v2.5`.

Escopo desta leitura:

- diagnóstico e documentação apenas;
- nenhum código alterado;
- nenhuma fórmula, faixa, peso, indicador, pilar ou penalização alterados;
- métricas oficiais calculadas somente sobre os 9 cenários válidos do benchmark:
  - `MF-CENARIO-01`
  - `MF-CENARIO-03`
  - `MF-CENARIO-04`
  - `MF-CENARIO-05`
  - `MF-CENARIO-06`
  - `MF-CENARIO-08`
  - `MF-CENARIO-10`
  - `MF-CENARIO-11`
  - `MF-CENARIO-12`

### Métricas oficiais da auditoria

- `v2.4`: `1/9` cenários válidos dentro da faixa.
- `v2.5`: `3/9` cenários válidos dentro da faixa.
- diferença absoluta média da `v2.4`: `178,89` pontos.
- diferença absoluta média da `v2.5`: `92,22` pontos.
- redução percentual do erro médio: `48,45%`.
- cenários que melhoraram: `7`.
- cenários que pioraram: `1`.
- cenários sem mudança relevante: `1`.

### Tabela comparativa dos 9 cenários válidos

| Cenário | Score v2.4 | Score v2.5 | Nota humana | Faixa aceitável | Dif. v2.4 | Dif. v2.5 | Variação do erro | Status v2.5 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| `MF-CENARIO-01` | 720 | 820 | 780 | 760-820 | -60 | +40 | +20 | Dentro da faixa |
| `MF-CENARIO-03` | 570 | 690 | 800 | 780-850 | -230 | -110 | +120 | Fora da faixa |
| `MF-CENARIO-04` | 740 | 870 | 930 | 900-960 | -190 | -60 | +130 | Fora da faixa |
| `MF-CENARIO-05` | 0 | 170 | 180 | 150-250 | -180 | -10 | +170 | Dentro da faixa |
| `MF-CENARIO-06` | 350 | 440 | 620 | 580-680 | -270 | -180 | +90 | Fora da faixa |
| `MF-CENARIO-08` | 0 | 0 | 40 | 0-80 | -40 | -40 | 0 | Dentro da faixa |
| `MF-CENARIO-10` | 90 | 130 | 220 | 180-280 | -130 | -90 | +40 | Fora da faixa |
| `MF-CENARIO-11` | 420 | 650 | 720 | 680-780 | -300 | -70 | +230 | Fora da faixa |
| `MF-CENARIO-12` | 290 | 270 | 500 | 450-560 | -210 | -230 | -20 | Fora da faixa |

Leitura consolidada:

- a maior melhoria individual foi `MF-CENARIO-11`, com ganho de `230` pontos na aproximação ao benchmark;
- a única regressão líquida foi `MF-CENARIO-12`, piorando `20` pontos;
- `MF-CENARIO-08` permaneceu estável e coerente no piso da escala;
- a `v2.5` foi especialmente eficaz na transição dos cenários médios e recuperáveis, mas ainda não resolveu a base residual dos casos com fluxo persistentemente ruim.

### Tabela separada dos 3 cenários inválidos

| Cenário | Score v2.5 | Nota humana atual | Faixa atual | Leitura técnica | Situação |
| --- | ---: | ---: | ---: | --- | --- |
| `MF-CENARIO-02` | 380 | 600 | 560-650 | Continua incompatível com a ideia de “primeiro emprego” porque a massa mantém deterioração prolongada demais. | Inválido para calibração |
| `MF-CENARIO-07` | 170 | 320 | 280-380 | Continua não representando atraso leve; o cenário ainda se comporta como deterioração estrutural mais grave do que o nome sugere. | Inválido para calibração |
| `MF-CENARIO-09` | 670 | 700 | 660-760 | O score ficou próximo da faixa, mas a massa ainda não representa proteção madura suficiente para o objetivo declarado. | Inválido para calibração |

Esses três casos continuam úteis para regressão técnica, mas não podem orientar a calibragem residual da próxima versão enquanto a massa sintética não for reconstruída.

### Grupo A - Dentro da faixa

#### `MF-CENARIO-01`

- a calibração corrigiu bem o falso negativo do perfil inicial disciplinado;
- `Liquidez e Reserva` ainda não chega a nota excelente, mas já não derruba o caso;
- principal risco de regressão futura: aliviar demais perfis iniciais e perder separação em relação a perfis medianos mais maduros.

#### `MF-CENARIO-05`

- a `v2.5` descomprimiu a base e devolveu granularidade ao cenário “alta renda em caos”;
- o caso segue muito crítico, mas deixa de colapsar artificialmente para zero;
- principal risco de regressão futura: suavizar demais a combinação de patrimônio negativo, fluxo ruim e endividamento relevante.

#### `MF-CENARIO-08`

- o cenário extremo permaneceu corretamente no piso;
- a inadimplência grave continua funcionando como sinal materializado mais severo do motor;
- principal risco de regressão futura: reduzir a pena da inadimplência a ponto de aproximar atraso grave de cenários apenas vulneráveis.

### Grupo B - Próximos da faixa

#### `MF-CENARIO-03`

- ainda está `110` pontos abaixo da referência humana;
- o principal freio residual é `Liquidez e Reserva`, com secundário em `Fluxo de Caixa`;
- o caso já saiu da zona crítica excessiva, mas ainda não recebe crédito suficiente por disciplina consistente e ausência de ruptura.

#### `MF-CENARIO-04`

- ainda está `60` pontos abaixo da faixa, já em zona de calibração fina;
- a diferença residual parece vir mais de compressão no topo de `Liquidez e Reserva` e do uso conservador da escala alta do que de erro estrutural;
- o motor já identifica corretamente o perfil como forte, mas ainda não o leva com folga para a zona `900+`.

#### `MF-CENARIO-11`

- foi o maior avanço da sprint e ficou apenas `30` pontos abaixo da faixa mínima;
- a diferença restante parece fina e concentrada em `Liquidez e Reserva`, com influência secundária de `Endividamento e Obrigações`;
- não há sinal de leitura estrutural errada; o caso já parece mais próximo de severidade residual do que de falha conceitual.

### Grupo C - Distantes da faixa

#### `MF-CENARIO-06`

- continua `180` pontos abaixo da referência humana;
- principal responsável: `Endividamento e Obrigações`, agravado por `Liquidez e Reserva`;
- o motor ainda enxerga a dívida organizada com severidade alta demais quando combinada com patrimônio líquido negativo e baixa proteção.

#### `MF-CENARIO-10`

- continua `90` pontos abaixo da referência humana;
- o principal problema é a soma de `Fluxo de Caixa` muito baixo com `Liquidez e Reserva` quase colapsada;
- a pontuação final já fica acima de `MF-CENARIO-08`, o que preserva a lógica entre vulnerabilidade severa sem atraso e inadimplência grave, mas a distância ainda parece curta demais.

#### `MF-CENARIO-12`

- continua como principal caso residual da base ruim com patrimônio alto;
- o cenário combina:
  - `Fluxo de Caixa` muito baixo;
  - `Liquidez e Reserva` muito baixa;
  - `Endividamento e Obrigações` ainda fraco;
  - `Patrimônio` forte, mas insuficiente para compensar o risco operacional;
  - penalização ativa por persistência de fluxo negativo.
- o motor acerta a tese de que patrimônio não mascara deterioração operacional, mas ainda subestima demais a proteção patrimonial em relação à nota humana.

### Grupo D - Cenários inválidos

#### `MF-CENARIO-02`

- precisa voltar a representar transição saudável de início de vida financeira;
- a nova massa precisa remover a persistência longa de fluxo negativo;
- a validação futura deve provar estabilidade operacional crescente, e não deterioração crônica.

#### `MF-CENARIO-07`

- precisa representar atraso leve real;
- a nova massa precisa introduzir inadimplência leve explícita sem colapsar o resto da estrutura financeira;
- a validação futura deve comprovar atraso leve distinguível de atraso grave e de caos estrutural.

#### `MF-CENARIO-09`

- precisa representar proteção madura de autônomo;
- a nova massa deve elevar claramente a reserva e a resiliência de curto prazo;
- a validação futura deve provar que volatilidade com boa proteção não se confunde com vulnerabilidade sem colchão.

### Investigação obrigatória - `MF-CENARIO-12`

A regressão de `MF-CENARIO-12` de aproximadamente `290` para `270` não veio do pilar `Patrimônio`.

Leitura residual consolidada:

- o pilar `Patrimônio` continua sendo o melhor componente do cenário e preserva a ideia de proteção patrimonial;
- a queda final aparece porque o caso continua muito pressionado por:
  - `Fluxo de Caixa` extremamente fraco;
  - `Liquidez e Reserva` ainda muito baixa;
  - `Endividamento e Obrigações` ainda conservador demais para um perfil com riqueza acumulada;
  - penalização por persistência de fluxo negativo.

Em outras palavras:

- a `v2.5` melhorou o tratamento de vários cenários médios;
- mas o ganho nesses blocos não se converteu neste caso porque a deterioração operacional segue dominando a leitura;
- a regressão parece evitável em calibragem futura, não um efeito colateral arquitetural obrigatório.

### Investigação obrigatória - `MF-CENARIO-06`

O caso de dívida organizada permanece baixo porque três sinais continuam se reforçando:

- baixa liquidez;
- patrimônio líquido negativo;
- exposição relevante a obrigações e passivos, mesmo sem inadimplência.

Diagnóstico:

- o motor já separa conceitualmente dívida organizada de inadimplência;
- a distância residual é mais numérica do que conceitual;
- o tratamento de patrimônio negativo e de pressão de obrigações ainda empurra o caso para baixo demais.

### Investigação obrigatória - `MF-CENARIO-10`

O score `130` parece baixo principalmente por correlação de fragilidades:

- fluxo persistentemente ruim;
- ausência de reserva;
- patrimônio líquido negativo;
- baixa capacidade de absorção de volatilidade.

Diagnóstico:

- a `v2.5` preserva corretamente o fato de `MF-CENARIO-10` ficar acima de `MF-CENARIO-08`;
- ainda assim, a distância entre vulnerabilidade severa sem inadimplência e atraso grave continua curta;
- o problema residual parece ser mais de severidade combinada do que de erro conceitual isolado.

### Investigação obrigatória - `MF-CENARIO-11`

O score `650` já parece entrar em zona de diferença fina.

Leitura residual:

- o motor já reconhece solvência, patrimônio líquido positivo e financiamento sob controle;
- o principal freio ainda é `Liquidez e Reserva`;
- `Endividamento e Obrigações` ainda contribui, mas já não parece equiparar o caso a endividamento problemático.

### Investigação obrigatória - `MF-CENARIO-03` e `MF-CENARIO-04`

Nos dois casos, a diferença residual aponta mais para:

- conservadorismo remanescente de `Liquidez e Reserva`;
- compressão da parte alta da escala;
- e, em menor grau, folga operacional ainda não plenamente premiada.

Não há evidência, nesta auditoria, de necessidade de rever o benchmark humano desses dois cenários.

### Ranking residual dos pilares

Entre os 9 cenários válidos, a ordem residual mais consistente ficou:

1. `Liquidez e Reserva`
2. `Fluxo de Caixa`
3. `Endividamento e Obrigações`
4. `Patrimônio`
5. `Planejamento e Disciplina`

Justificativa resumida:

- `Liquidez e Reserva` ainda aparece em praticamente todos os casos próximos ou distantes da faixa;
- `Fluxo de Caixa` continua dominando a base residual dos cenários `10` e `12`;
- `Endividamento e Obrigações` segue relevante principalmente em `06`, `11` e parte de `12`;
- `Patrimônio` hoje pesa mais como agravante em cenários com patrimônio líquido negativo do que como erro autônomo de leitura nos casos fortes;
- `Planejamento e Disciplina` deixou de ser o gargalo principal desta rodada.

### Avaliação das penalizações

#### Persistência de fluxo negativo

- continua sendo a penalização crítica mais sensível da base residual;
- afeta de forma importante `06`, `10` e `12`;
- hoje ela confirma fragilidade real, mas ainda comprime a recuperação em casos não terminais.

#### Patrimônio líquido negativo

- segue coerente em `05`, `08` e `10`;
- em `06`, o efeito combinado com baixa liquidez e obrigações ainda parece severo demais para um caso adimplente e organizado.

#### Inadimplência gradual

- continua coerente no extremo grave de `08`;
- ajuda a manter `05` e `10` acima de `08`, preservando a hierarquia entre caos sem atraso grave e inadimplência materializada.

Síntese:

- o score-base já é baixo antes das penalizações nos cenários mais problemáticos;
- as penalizações não são a única causa do resultado ruim;
- mas ainda reduzem demais a granularidade entre perfis muito frágeis e perfis praticamente terminais.

### Ordenação relativa dos cenários válidos

Pontos coerentes:

- `MF-CENARIO-04` continua acima dos demais cenários fortes;
- `MF-CENARIO-11` continua acima de `MF-CENARIO-06`;
- `MF-CENARIO-06` continua acima de `MF-CENARIO-10`;
- `MF-CENARIO-10` continua acima de `MF-CENARIO-08`;
- `MF-CENARIO-12` continua acima de perfis insolventes sem patrimônio;
- `MF-CENARIO-05` continua acima de `MF-CENARIO-08`, preservando a diferença entre caos grave sem inadimplência extrema e atraso grave.

Incoerências residuais:

- `MF-CENARIO-01` ficou acima de `MF-CENARIO-03`, enquanto o benchmark humano espera `CLT Organizado` acima de `Estudante Base`;
- a distância entre `MF-CENARIO-04` e `MF-CENARIO-01` ainda é pequena demais para o topo da escala, indicando compressão residual da zona alta.

### Recomendação documentada para o próximo passo

Sem iniciar a `v2.6`, a recomendação oficial desta auditoria é:

1. manter a arquitetura da `v2.5` congelada;
2. reconstruir primeiro os cenários inválidos `02`, `07` e `09`;
3. definir a próxima rodada de calibração fina com foco em:
   - `Liquidez e Reserva`;
   - `Fluxo de Caixa`;
   - severidade combinada entre patrimônio negativo, pressão operacional e persistência temporal;
4. revalidar explicitamente `MF-CENARIO-06`, `MF-CENARIO-10` e `MF-CENARIO-12` antes de qualquer nova discussão estrutural.

Este documento consolida a visão oficial de auditoria do `MF Score`.

Ele responde:

1. como o motor está desenhado hoje;
2. quais problemas conceituais já foram corrigidos;
3. quais limitações continuam abertas;
4. qual deve ser a próxima rodada de calibração.

## Resumo executivo

O `MF Score` continua sendo o modelo oficial de saúde financeira do sistema.

Na versão atual, `mf-score-v2.5-1000`, o motor preserva:

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

## Resultado oficial da sprint `mf-score-v2.5`

A sprint `v2.5` confirmou a leitura da auditoria da `v2.4`: a arquitetura estava correta, mas a sensibilidade numérica ainda precisava amadurecer.

Resultado consolidado:

- saída de `1/12` para `4/12` cenários dentro da faixa aceitável;
- redução da diferença média absoluta de `205` para `102,5` pontos;
- melhora relevante em `MF-CENARIO-01`, `MF-CENARIO-05`, `MF-CENARIO-09` e nos cenários intermediários de renda organizada.

As divergências remanescentes ficaram concentradas em:

- cenários com fluxo persistentemente negativo por longos períodos;
- base inferior da escala ainda comprimida em alguns casos recuperáveis;
- necessidade de ajuste fino adicional em `Liquidez e Reserva` e na tradução final dos cenários de meia nota.

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
