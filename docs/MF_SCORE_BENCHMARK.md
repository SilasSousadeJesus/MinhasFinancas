# MF Score Benchmark

## Objetivo

Este documento existe para validar o comportamento do Motor Financeiro diante dos cenários oficiais previamente auditados.

Ele não é:

- documentação matemática;
- documentação técnica do motor;
- substituto de `docs/MF_SCORE.md`.

Ele serve exclusivamente para auditoria, calibração e regressão do comportamento do `MF Score`.

A pergunta central deste benchmark é:

> A nota atribuída pelo motor faz sentido para este cenário?

Este benchmark faz parte da governança oficial do projeto.

## Como funciona o benchmark

Cada cenário representa um usuário sintético cuidadosamente construído.

Cada cenário possui:

- contexto financeiro coerente;
- comportamento financeiro consistente;
- objetivo específico de validação;
- resultado produzido pelo motor;
- avaliação humana esperada.

As notas humanas não representam uma verdade científica absoluta.

Elas representam o comportamento esperado oficialmente adotado pelo produto para fins de:

- calibração;
- comparação entre versões;
- identificação de regressões;
- auditoria do comportamento do MF Score.

A arquitetura da versão `mf-score-v2.4-1000` permanece congelada.

As diferenças registradas neste benchmark devem orientar ajustes futuros de:

- faixas;
- limites;
- pesos internos;
- curvas de pontuação;
- intensidade das penalizações.

Nenhuma regra específica deve ser criada para um cenário isolado.

## Regras de status

Os cenários usam os seguintes status:

- `Aprovado`: score atual dentro da faixa aceitável.
- `Aprovado com ressalvas`: score atual até `30` pontos fora da faixa.
- `Recalibrar`: score atual mais de `30` pontos fora da faixa.
- `Cenário inválido`: a massa sintética não representa corretamente o objetivo declarado.

Critério da diferença:

- diferença negativa: motor abaixo da referência humana;
- diferença positiva: motor acima da referência humana.

## Primeira Rodada de Referência Humana do Benchmark - v1

Esta é a primeira rodada oficial de avaliação humana dos 12 cenários sintéticos do Laboratório do MF Score.

As notas e faixas abaixo passam a ser referências provisórias oficiais para a próxima calibração numérica.

Elas só poderão ser revisadas mediante:

- nova auditoria documentada;
- mudança comprovada na massa do cenário;
- identificação de incoerência conceitual;
- decisão explícita registrada neste benchmark e em `docs/CHANGELOG.md`.

## Benchmark dos Cenários

### MF-CENARIO-01

#### Identificação

- **Código:** `MF-CENARIO-01`
- **Nome:** `Estudante Base`
- **Objetivo:** `Validar perfil inicial com baixa renda, baixo patrimônio e despesas controladas sem ruptura.`

#### Resultado Atual

- **Modelo do Motor:** `mf-score-v2.4-1000`
- **Score Base:** `720`
- **Score Final:** `720`
- **Classificação:** `Bom`
- **Risco:** `Risco Moderado`
- **Penalidade Total:** `0`

#### Avaliação Humana

- **Nota considerada justa:** `780`
- **Faixa aceitável:** `760-820`
- **Diferença:** `-60`
- **Status:** `Recalibrar`

#### Justificativa Humana

Usuário em início de vida financeira, sem dívidas ou inadimplência, com fluxo positivo, economia elevada para sua renda e reserva ainda em formação. A ausência de patrimônio consolidado decorre principalmente do estágio de vida, e não de deterioração financeira.

#### Indicadores Responsáveis

- `Fluxo de Caixa`
- `Liquidez e Reserva`
- `Patrimônio`
- `Planejamento e Disciplina`

#### Decisão da Auditoria

O motor reconhece corretamente que o cenário é saudável, mas ainda subavalia demais um perfil inicial sem rupturas relevantes.

### MF-CENARIO-02

#### Identificação

- **Código:** `MF-CENARIO-02`
- **Nome:** `Primeiro Emprego`
- **Objetivo:** `Validar transição entre vida financeira inicial e começo de estabilidade operacional.`

#### Resultado Atual

- **Modelo do Motor:** `mf-score-v2.4-1000`
- **Score Base:** `450`
- **Score Final:** `270`
- **Classificação:** `Muito Crítico`
- **Risco:** `Risco Muito Alto`
- **Penalidade Total:** `180`

#### Avaliação Humana

- **Nota considerada justa:** `600`
- **Faixa aceitável:** `560-650`
- **Diferença:** `-330`
- **Status:** `Cenário inválido`

#### Justificativa Humana

A massa apresenta fluxo negativo persistente por 12 meses, o que é incompatível com uma simples transição saudável para o primeiro emprego. A nota humana pressupõe correção ou revisão da coerência do cenário antes de promovê-lo como referência definitiva.

#### Indicadores Responsáveis

- `Fluxo de Caixa`
- `Persistência temporal`
- `Liquidez e Reserva`
- `Planejamento e Disciplina`

#### Decisão da Auditoria

O cenário precisa ser reconstruído antes de servir como caso definitivo de benchmark.

### MF-CENARIO-03

#### Identificação

- **Código:** `MF-CENARIO-03`
- **Nome:** `CLT Organizado`
- **Objetivo:** `Validar perfil saudável de média renda com disciplina consistente.`

#### Resultado Atual

- **Modelo do Motor:** `mf-score-v2.4-1000`
- **Score Base:** `570`
- **Score Final:** `570`
- **Classificação:** `Crítico`
- **Risco:** `Risco Alto`
- **Penalidade Total:** `0`

#### Avaliação Humana

- **Nota considerada justa:** `800`
- **Faixa aceitável:** `780-850`
- **Diferença:** `-230`
- **Status:** `Recalibrar`

#### Justificativa Humana

O cenário apresenta renda estável, ausência de passivos e inadimplência e algum patrimônio, mas a margem mensal é pequena e a reserva ainda incompleta. A nota humana reconhece estabilidade, sem tratá-lo como perfil de excelência plena.

#### Indicadores Responsáveis

- `Fluxo de Caixa`
- `Liquidez e Reserva`
- `Patrimônio`
- `Planejamento e Disciplina`

#### Decisão da Auditoria

O motor continua severo demais com perfis organizados e recuperáveis que ainda não chegaram à maturidade máxima.

### MF-CENARIO-04

#### Identificação

- **Código:** `MF-CENARIO-04`
- **Nome:** `Alta Renda Forte`
- **Objetivo:** `Validar cenário de baixo risco e alta maturidade financeira.`

#### Resultado Atual

- **Modelo do Motor:** `mf-score-v2.4-1000`
- **Score Base:** `740`
- **Score Final:** `740`
- **Classificação:** `Bom`
- **Risco:** `Risco Moderado`
- **Penalidade Total:** `0`

#### Avaliação Humana

- **Nota considerada justa:** `930`
- **Faixa aceitável:** `900-960`
- **Diferença:** `-190`
- **Status:** `Recalibrar`

#### Justificativa Humana

Usuário com grande capacidade de economia, patrimônio superior à meta, ausência de passivos e inadimplência e boa capacidade de completar a reserva. Deve permanecer entre os melhores cenários da base.

#### Indicadores Responsáveis

- `Fluxo de Caixa`
- `Liquidez e Reserva`
- `Patrimônio`
- `Endividamento e Obrigações`

#### Decisão da Auditoria

O motor já reconhece que é um cenário forte, mas ainda não entrega diferenciação suficiente para colocá-lo no topo da escala.

### MF-CENARIO-05

#### Identificação

- **Código:** `MF-CENARIO-05`
- **Nome:** `Alta Renda Caos`
- **Objetivo:** `Validar risco moderado-alto em perfil de renda alta com disciplina fraca.`

#### Resultado Atual

- **Modelo do Motor:** `mf-score-v2.4-1000`
- **Score Base:** `280`
- **Score Final:** `0`
- **Classificação:** `Muito Crítico`
- **Risco:** `Risco Muito Alto`
- **Penalidade Total:** `280`

#### Avaliação Humana

- **Nota considerada justa:** `180`
- **Faixa aceitável:** `150-250`
- **Diferença:** `-180`
- **Status:** `Recalibrar`

#### Justificativa Humana

Alta renda não compensa fluxo negativo persistente, patrimônio líquido negativo, endividamento de consumo elevado e ausência de proteção. O cenário é muito crítico, mas deve preservar alguma granularidade em relação a uma situação de inadimplência grave.

#### Indicadores Responsáveis

- `Fluxo de Caixa`
- `Endividamento e Obrigações`
- `Patrimônio`
- `Persistência temporal`

#### Decisão da Auditoria

O motor acerta a gravidade, mas comprime demais o resultado no piso e perde diferenciação na base da escala.

### MF-CENARIO-06

#### Identificação

- **Código:** `MF-CENARIO-06`
- **Nome:** `Divida Organizada`
- **Objetivo:** `Validar cenário de dívida relevante sem inadimplência ativa.`

#### Resultado Atual

- **Modelo do Motor:** `mf-score-v2.4-1000`
- **Score Base:** `450`
- **Score Final:** `350`
- **Classificação:** `Muito Crítico`
- **Risco:** `Risco Muito Alto`
- **Penalidade Total:** `100`

#### Avaliação Humana

- **Nota considerada justa:** `620`
- **Faixa aceitável:** `580-680`
- **Diferença:** `-270`
- **Status:** `Recalibrar`

#### Justificativa Humana

Usuário adimplente, com fluxo positivo e plano de quitação, mas possui patrimônio líquido negativo, baixa liquidez e dívida relevante. Deve ser tratado como risco real e controlável, não como equivalente a inadimplência materializada.

#### Indicadores Responsáveis

- `Endividamento e Obrigações`
- `Liquidez e Reserva`
- `Patrimônio`
- `Planejamento e Disciplina`

#### Decisão da Auditoria

O motor ainda aproxima demais dívida organizada de situações muito mais graves do que ela deveria representar.

### MF-CENARIO-07

#### Identificação

- **Código:** `MF-CENARIO-07`
- **Nome:** `Atraso Leve`
- **Objetivo:** `Validar inadimplência leve sem colapso completo do score.`

#### Resultado Atual

- **Modelo do Motor:** `mf-score-v2.4-1000`
- **Score Base:** `280`
- **Score Final:** `0`
- **Classificação:** `Muito Crítico`
- **Risco:** `Risco Muito Alto`
- **Penalidade Total:** `280`

#### Avaliação Humana

- **Nota considerada justa:** `320`
- **Faixa aceitável:** `280-380`
- **Diferença:** `-320`
- **Status:** `Cenário inválido`

#### Justificativa Humana

A massa atual não representa corretamente atraso leve, pois apresenta 12 meses no vermelho, patrimônio negativo e nenhuma inadimplência reconhecida. O cenário deve ser tratado como inválido ou pendente de reconstrução antes de utilizar a faixa como referência definitiva.

#### Indicadores Responsáveis

- `Fluxo de Caixa`
- `Persistência temporal`
- `Patrimônio`
- `Inadimplência`

#### Decisão da Auditoria

O cenário precisa ser reconstruído antes de ser usado como caso oficial de atraso leve.

### MF-CENARIO-08

#### Identificação

- **Código:** `MF-CENARIO-08`
- **Nome:** `Atraso Grave`
- **Objetivo:** `Validar cenário de risco alto por inadimplência grave e pouca proteção.`

#### Resultado Atual

- **Modelo do Motor:** `mf-score-v2.4-1000`
- **Score Base:** `270`
- **Score Final:** `0`
- **Classificação:** `Muito Crítico`
- **Risco:** `Risco Muito Alto`
- **Penalidade Total:** `430`

#### Avaliação Humana

- **Nota considerada justa:** `40`
- **Faixa aceitável:** `0-80`
- **Diferença:** `-40`
- **Status:** `Aprovado`

#### Justificativa Humana

Cenário extremo e coerente, com inadimplência grave, atraso longo, materialidade superior à renda, patrimônio negativo e fluxo deficitário. Score próximo ao piso é aceitável.

#### Indicadores Responsáveis

- `Inadimplência`
- `Endividamento e Obrigações`
- `Patrimônio`
- `Persistência temporal`

#### Decisão da Auditoria

O motor reconhece corretamente um caso extremo e o posiciona de forma coerente no piso da escala.

### MF-CENARIO-09

#### Identificação

- **Código:** `MF-CENARIO-09`
- **Nome:** `Autonomo Reserva`
- **Objetivo:** `Validar perfil volátil com proteção financeira madura.`

#### Resultado Atual

- **Modelo do Motor:** `mf-score-v2.4-1000`
- **Score Base:** `520`
- **Score Final:** `520`
- **Classificação:** `Crítico`
- **Risco:** `Risco Alto`
- **Penalidade Total:** `0`

#### Avaliação Humana

- **Nota considerada justa:** `700`
- **Faixa aceitável:** `660-760`
- **Diferença:** `-180`
- **Status:** `Cenário inválido`

#### Justificativa Humana

A massa atual possui apenas cerca de `1,6` mês de despesas em reserva, o que não representa proteção madura para renda variável. O cenário precisa ser ajustado ou renomeado antes de consolidar definitivamente sua faixa.

#### Indicadores Responsáveis

- `Liquidez e Reserva`
- `Fluxo de Caixa`
- `Planejamento e Disciplina`

#### Decisão da Auditoria

O cenário não representa corretamente o objetivo declarado e deve ser revisto antes de calibrar o motor a partir dele.

### MF-CENARIO-10

#### Identificação

- **Código:** `MF-CENARIO-10`
- **Nome:** `Autonomo Sem Res`
- **Objetivo:** `Validar risco elevado em perfil volátil sem colchão financeiro.`

#### Resultado Atual

- **Modelo do Motor:** `mf-score-v2.4-1000`
- **Score Base:** `90`
- **Score Final:** `90`
- **Classificação:** `Muito Crítico`
- **Risco:** `Risco Muito Alto`
- **Penalidade Total:** `280`

#### Avaliação Humana

- **Nota considerada justa:** `220`
- **Faixa aceitável:** `180-280`
- **Diferença:** `-130`
- **Status:** `Recalibrar`

#### Justificativa Humana

Cenário coerente de vulnerabilidade elevada: fluxo negativo persistente, baixa reserva, passivos superiores aos ativos e patrimônio negativo, ainda sem inadimplência materializada. Deve ser muito crítico, mas superior ao cenário de atraso grave.

#### Indicadores Responsáveis

- `Fluxo de Caixa`
- `Liquidez e Reserva`
- `Patrimônio`
- `Persistência temporal`

#### Decisão da Auditoria

O motor acerta a direção, porém continua severo demais e aproxima excessivamente o caso de um colapso absoluto.

### MF-CENARIO-11

#### Identificação

- **Código:** `MF-CENARIO-11`
- **Nome:** `Familia Financiada`
- **Objetivo:** `Validar cenário familiar com obrigações elevadas, mas sob controle.`

#### Resultado Atual

- **Modelo do Motor:** `mf-score-v2.4-1000`
- **Score Base:** `420`
- **Score Final:** `420`
- **Classificação:** `Crítico`
- **Risco:** `Risco Alto`
- **Penalidade Total:** `0`

#### Avaliação Humana

- **Nota considerada justa:** `720`
- **Faixa aceitável:** `680-780`
- **Diferença:** `-300`
- **Status:** `Recalibrar`

#### Justificativa Humana

Família solvente, com patrimônio líquido positivo, financiamento lastreado, fluxo positivo e ausência de inadimplência, porém com orçamento rígido e liquidez insuficiente. Não deve ser equiparada a endividamento descontrolado.

#### Indicadores Responsáveis

- `Endividamento e Obrigações`
- `Patrimônio`
- `Liquidez e Reserva`
- `Fluxo de Caixa`

#### Decisão da Auditoria

O motor ainda pesa demais a rigidez financeira e não diferencia suficientemente uma família solvente de um quadro de endividamento problemático.

### MF-CENARIO-12

#### Identificação

- **Código:** `MF-CENARIO-12`
- **Nome:** `Patrimonio Fluxo`
- **Objetivo:** `Validar cenário em que riqueza acumulada não mascara deterioração operacional.`

#### Resultado Atual

- **Modelo do Motor:** `mf-score-v2.4-1000`
- **Score Base:** `290`
- **Score Final:** `290`
- **Classificação:** `Muito Crítico`
- **Risco:** `Risco Muito Alto`
- **Penalidade Total:** `180`

#### Avaliação Humana

- **Nota considerada justa:** `500`
- **Faixa aceitável:** `450-560`
- **Diferença:** `-210`
- **Status:** `Recalibrar`

#### Justificativa Humana

Patrimônio elevado reduz o risco de insolvência imediata, mas não elimina o fluxo negativo persistente e a falta de liquidez. O cenário deve permanecer crítico, porém claramente acima de usuários insolventes e inadimplentes.

#### Indicadores Responsáveis

- `Patrimônio`
- `Fluxo de Caixa`
- `Liquidez e Reserva`
- `Persistência temporal`

#### Decisão da Auditoria

O motor reconhece o problema operacional, mas continua achatando demais a proteção que o patrimônio ainda fornece.

## Resumo Geral

| Cenário | Score v2.4 | Nota humana | Faixa aceitável | Diferença | Status |
| --- | ---: | ---: | ---: | ---: | --- |
| `MF-CENARIO-01` | 720 | 780 | 760-820 | -60 | Recalibrar |
| `MF-CENARIO-02` | 270 | 600 | 560-650 | -330 | Cenário inválido |
| `MF-CENARIO-03` | 570 | 800 | 780-850 | -230 | Recalibrar |
| `MF-CENARIO-04` | 740 | 930 | 900-960 | -190 | Recalibrar |
| `MF-CENARIO-05` | 0 | 180 | 150-250 | -180 | Recalibrar |
| `MF-CENARIO-06` | 350 | 620 | 580-680 | -270 | Recalibrar |
| `MF-CENARIO-07` | 0 | 320 | 280-380 | -320 | Cenário inválido |
| `MF-CENARIO-08` | 0 | 40 | 0-80 | -40 | Aprovado |
| `MF-CENARIO-09` | 520 | 700 | 660-760 | -180 | Cenário inválido |
| `MF-CENARIO-10` | 90 | 220 | 180-280 | -130 | Recalibrar |
| `MF-CENARIO-11` | 420 | 720 | 680-780 | -300 | Recalibrar |
| `MF-CENARIO-12` | 290 | 500 | 450-560 | -210 | Recalibrar |

## Achados da Auditoria Humana da v2.4

1. A `v2.4` corrigiu as principais distorções conceituais da `v2.3`.
2. Os cenários extremos passaram a ser reconhecidos corretamente, mas existe perda de granularidade na parte inferior da escala, com vários cenários terminando em zero ou próximos do piso.
3. Perfis saudáveis ou recuperáveis continuam sendo avaliados com severidade excessiva.
4. Liquidez incompleta ainda reduz fortemente usuários com boa capacidade de recuperação.
5. Dívida organizada e financiamento patrimonial ainda precisam ser melhor diferenciados, numericamente, de dívida de consumo problemática.
6. O pilar `Planejamento` melhorou, mas ainda deve ser observado para não premiar excessivamente configuração sem execução.
7. Alguns cenários da Base de Simulação não representam corretamente o objetivo declarado e devem ser corrigidos antes de se tornarem casos definitivos:
   - `MF-CENARIO-02`
   - `MF-CENARIO-07`
   - `MF-CENARIO-09`
8. Nenhuma conclusão desta auditoria autoriza criação de exceções específicas por cenário.

## Governança do Benchmark

O benchmark passa a ser a principal referência de calibração do `MF Score`.

Toda alteração de:

- indicadores;
- pilares;
- pesos;
- penalizações;
- classificações;
- textos;
- regras críticas;
- fórmulas;

deverá obrigatoriamente ser validada contra todos os cenários oficiais.

O objetivo da calibração não é reproduzir exatamente um número, mas manter o resultado dentro da faixa considerada aceitável pela auditoria humana.

Toda futura versão do `MF Score` deverá informar:

- score por cenário;
- diferença em relação à referência;
- quantidade de cenários dentro da faixa;
- regressões;
- melhorias;
- cenários ainda inválidos.

Sempre que houver mudança no motor:

- todos os cenários deverão ser reexecutados;
- o benchmark deverá ser atualizado;
- qualquer regressão deverá ser registrada.

Nenhuma alteração do motor poderá ser considerada concluída sem atualização deste documento.

## Relação com a documentação oficial

Este benchmark complementa:

- `docs/MF_SCORE.md`
- `docs/MF_SCORE_AUDIT.md`
- `docs/MF_SCORE_VALIDATION.md`

Ele não substitui esses documentos.

Seu papel é registrar a expectativa humana oficial sobre o comportamento do motor diante dos cenários de referência do projeto.
