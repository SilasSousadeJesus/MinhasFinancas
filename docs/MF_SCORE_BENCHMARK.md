# MF Score Benchmark

## Objetivo

Este documento existe para validar o comportamento do Motor Financeiro diante de cenários oficiais previamente auditados.

Ele não é:

- documentação matemática;
- documentação técnica do motor;
- substituto de `docs/MF_SCORE.md`.

Ele serve exclusivamente para auditoria, calibração e regressão do comportamento do `MF Score`.

A pergunta central deste benchmark é:

> A nota atribuída pelo motor faz sentido para este cenário?

Este benchmark passa a fazer parte da governança oficial do projeto.

## Status atual do benchmark

A primeira rodada completa de auditoria humana da versão `mf-score-v2.4-1000` foi concluída.

Conclusão oficial desta rodada:

- a arquitetura da `v2.4` foi aprovada;
- as divergências remanescentes são predominantemente numéricas, e não conceituais;
- a próxima etapa oficial do MF Score passa a ser calibração fina de notas, faixas, pesos e curvas;
- o benchmark oficial dos 12 cenários passa a ser obrigatório em qualquer evolução futura do motor.

Nesta consolidação documental:

- os resultados atuais produzidos pelo motor já foram registrados abaixo;
- os campos de nota justa, faixa aceitável, justificativa humana detalhada, indicadores responsáveis e decisão final por cenário continuam reservados para transcrição fina da auditoria humana consolidada.

## Como funciona o benchmark

Cada cenário representa um usuário sintético cuidadosamente construído.

Cada cenário possui:

- contexto financeiro coerente;
- comportamento financeiro consistente;
- objetivo específico de validação;
- resultado produzido pelo motor;
- avaliação humana esperada.

Os cenários abaixo usam como referência oficial a Base Oficial de Simulação do MF Score e o Laboratório do MF Score.

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

- **Nota considerada justa:** `Pendente de auditoria`
- **Faixa aceitável:** `Pendente de auditoria`
- **Status:** `Pendente`

#### Justificativa Humana

Será preenchido durante a auditoria humana.

#### Indicadores Responsáveis

- `Pendente`

#### Decisão da Auditoria

Pendente.

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

- **Nota considerada justa:** `Pendente de auditoria`
- **Faixa aceitável:** `Pendente de auditoria`
- **Status:** `Pendente`

#### Justificativa Humana

Será preenchido durante a auditoria humana.

#### Indicadores Responsáveis

- `Pendente`

#### Decisão da Auditoria

Pendente.

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

- **Nota considerada justa:** `Pendente de auditoria`
- **Faixa aceitável:** `Pendente de auditoria`
- **Status:** `Pendente`

#### Justificativa Humana

Será preenchido durante a auditoria humana.

#### Indicadores Responsáveis

- `Pendente`

#### Decisão da Auditoria

Pendente.

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

- **Nota considerada justa:** `Pendente de auditoria`
- **Faixa aceitável:** `Pendente de auditoria`
- **Status:** `Pendente`

#### Justificativa Humana

Será preenchido durante a auditoria humana.

#### Indicadores Responsáveis

- `Pendente`

#### Decisão da Auditoria

Pendente.

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

- **Nota considerada justa:** `Pendente de auditoria`
- **Faixa aceitável:** `Pendente de auditoria`
- **Status:** `Pendente`

#### Justificativa Humana

Será preenchido durante a auditoria humana.

#### Indicadores Responsáveis

- `Pendente`

#### Decisão da Auditoria

Pendente.

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

- **Nota considerada justa:** `Pendente de auditoria`
- **Faixa aceitável:** `Pendente de auditoria`
- **Status:** `Pendente`

#### Justificativa Humana

Será preenchido durante a auditoria humana.

#### Indicadores Responsáveis

- `Pendente`

#### Decisão da Auditoria

Pendente.

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

- **Nota considerada justa:** `Pendente de auditoria`
- **Faixa aceitável:** `Pendente de auditoria`
- **Status:** `Pendente`

#### Justificativa Humana

Será preenchido durante a auditoria humana.

#### Indicadores Responsáveis

- `Pendente`

#### Decisão da Auditoria

Pendente.

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

- **Nota considerada justa:** `Pendente de auditoria`
- **Faixa aceitável:** `Pendente de auditoria`
- **Status:** `Pendente`

#### Justificativa Humana

Será preenchido durante a auditoria humana.

#### Indicadores Responsáveis

- `Pendente`

#### Decisão da Auditoria

Pendente.

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

- **Nota considerada justa:** `Pendente de auditoria`
- **Faixa aceitável:** `Pendente de auditoria`
- **Status:** `Pendente`

#### Justificativa Humana

Será preenchido durante a auditoria humana.

#### Indicadores Responsáveis

- `Pendente`

#### Decisão da Auditoria

Pendente.

### MF-CENARIO-10

#### Identificação

- **Código:** `MF-CENARIO-10`
- **Nome:** `Autonomo Sem Res`
- **Objetivo:** `Validar risco elevado em perfil volátil sem colchão financeiro.`

#### Resultado Atual

- **Modelo do Motor:** `mf-score-v2.4-1000`
- **Score Base:** `280`
- **Score Final:** `0`
- **Classificação:** `Muito Crítico`
- **Risco:** `Risco Muito Alto`
- **Penalidade Total:** `280`

#### Avaliação Humana

- **Nota considerada justa:** `Pendente de auditoria`
- **Faixa aceitável:** `Pendente de auditoria`
- **Status:** `Pendente`

#### Justificativa Humana

Será preenchido durante a auditoria humana.

#### Indicadores Responsáveis

- `Pendente`

#### Decisão da Auditoria

Pendente.

### MF-CENARIO-11

#### Identificação

- **Código:** `MF-CENARIO-11`
- **Nome:** `Familia Financia`
- **Objetivo:** `Validar cenário familiar com obrigações elevadas, mas sob controle.`

#### Resultado Atual

- **Modelo do Motor:** `mf-score-v2.4-1000`
- **Score Base:** `500`
- **Score Final:** `500`
- **Classificação:** `Crítico`
- **Risco:** `Risco Alto`
- **Penalidade Total:** `0`

#### Avaliação Humana

- **Nota considerada justa:** `Pendente de auditoria`
- **Faixa aceitável:** `Pendente de auditoria`
- **Status:** `Pendente`

#### Justificativa Humana

Será preenchido durante a auditoria humana.

#### Indicadores Responsáveis

- `Pendente`

#### Decisão da Auditoria

Pendente.

### MF-CENARIO-12

#### Identificação

- **Código:** `MF-CENARIO-12`
- **Nome:** `Patrimonio Fluxo`
- **Objetivo:** `Validar cenário em que riqueza acumulada não mascara deterioração operacional.`

#### Resultado Atual

- **Modelo do Motor:** `mf-score-v2.4-1000`
- **Score Base:** `460`
- **Score Final:** `280`
- **Classificação:** `Muito Crítico`
- **Risco:** `Risco Muito Alto`
- **Penalidade Total:** `180`

#### Avaliação Humana

- **Nota considerada justa:** `Pendente de auditoria`
- **Faixa aceitável:** `Pendente de auditoria`
- **Status:** `Pendente`

#### Justificativa Humana

Será preenchido durante a auditoria humana.

#### Indicadores Responsáveis

- `Pendente`

#### Decisão da Auditoria

Pendente.

## Resumo Geral

| Cenário | Score Atual | Score Esperado | Diferença | Status |
| --- | --- | --- | --- | --- |
| `MF-CENARIO-01` | 720 | Pendente | — | Pendente |
| `MF-CENARIO-02` | 270 | Pendente | — | Pendente |
| `MF-CENARIO-03` | 570 | Pendente | — | Pendente |
| `MF-CENARIO-04` | 740 | Pendente | — | Pendente |
| `MF-CENARIO-05` | 0 | Pendente | — | Pendente |
| `MF-CENARIO-06` | 350 | Pendente | — | Pendente |
| `MF-CENARIO-07` | 0 | Pendente | — | Pendente |
| `MF-CENARIO-08` | 0 | Pendente | — | Pendente |
| `MF-CENARIO-09` | 520 | Pendente | — | Pendente |
| `MF-CENARIO-10` | 0 | Pendente | — | Pendente |
| `MF-CENARIO-11` | 500 | Pendente | — | Pendente |
| `MF-CENARIO-12` | 280 | Pendente | — | Pendente |

## Consolidação pós benchmark da versão `mf-score-v2.4-1000`

A primeira rodada completa de auditoria humana concluiu que a versão `mf-score-v2.4-1000` representa a primeira versão arquiteturalmente madura do `MF Score`.

Conclusões oficiais desta rodada:

- a arquitetura da `v2.4` foi aprovada;
- as distorções remanescentes são principalmente numéricas;
- não há necessidade, neste momento, de redesenhar a arquitetura do motor;
- a próxima etapa oficial passa a ser calibração fina de notas, faixas, pesos e curvas;
- o Benchmark Oficial dos 12 cenários passa a ser obrigatório para qualquer evolução futura.

Principais observações consolidadas:

- perfis saudáveis ainda ficaram abaixo do esperado em alguns casos;
- `Liquidez e Reserva` continua dominante demais em determinados cenários;
- os horizontes futuros ainda pesam bastante;
- as penalizações ficaram muito mais coerentes;
- os perfis extremos ficaram muito mais próximos da percepção humana.

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
