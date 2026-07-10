# MF Score Validation

Este documento complementa `docs/MF_SCORE.md`.

Enquanto o `MF_SCORE.md` explica como o modelo funciona, este documento registra como validamos se ele continua coerente ao longo do tempo.

## Objetivo

Criar uma base permanente de validação para o `MF Score`.

O objetivo é proteger o Motor Financeiro contra mudanças que pareçam corretas isoladamente, mas produzam resultados incoerentes no conjunto.

## Filosofia

O `MF Score` é um modelo de risco financeiro pessoal.

Modelos de risco não evoluem por opinião. Eles evoluem com:

- cenários oficiais
- validação contínua
- auditoria operacional
- auditoria humana
- rastreabilidade das mudanças

## Escalas oficiais

- `MF Score final`: `0 a 1000`
- `MF Score base`: `0 a 1000`
- `pilares`: `0 a 100`

## O que esta suíte protege

Esta suíte existe para responder:

- o score continua reagindo de forma esperada?
- o modelo ainda diferencia risco alto, médio e baixo com clareza?
- a mudança melhorou a leitura do risco ou apenas alterou números?
- a regra de não dupla penalização continua sendo respeitada?

## Princípio central de validação

Um mesmo fato econômico não deve ser punido duas vezes.

Portanto:

- `reserva zero` deve reduzir prioritariamente `Liquidez`
- `comprometimento alto` deve reduzir prioritariamente `Fluxo de Caixa`
- `pressão futura` deve reduzir prioritariamente `Fluxo de Caixa` e `Endividamento`

Esses fatores só devem virar penalização crítica quando houver:

- fluxo negativo
- inadimplência
- persistência temporal negativa
- risco efetivamente materializado

## Cenários oficiais

### Cenário 01 - Vida Financeira Excelente

**Expectativa qualitativa**

MF Score muito alto.

**Faixa esperada**

`900-1000`

---

### Cenário 02 - Boa renda, mas liquidez inexistente e cartão elevado

**Expectativa qualitativa**

MF Score moderado, sem colapso artificial do score apenas por falta de reserva e pressão alta.

**Faixa esperada**

`600-740`

---

### Cenário 03 - Patrimônio elevado com fluxo de caixa ruim

**Expectativa qualitativa**

O patrimônio reduz risco estrutural, mas o fluxo limita o score.

**Faixa esperada**

`550-750`

---

### Cenário 04 - Excelente fluxo com pouco patrimônio

**Expectativa qualitativa**

Score elevado, sem depender de patrimônio alto.

**Faixa esperada**

`750-900`

---

### Cenário 05 - Inadimplência

**Expectativa qualitativa**

Penalização forte por risco materializado.

**Faixa esperada**

`0-490`

---

### Cenário 06 - Comprometimento extremo

**Expectativa qualitativa**

Score baixo ou crítico, principalmente se houver pouca folga operacional.

**Faixa esperada**

`0-590`

---

### Cenário 07 - Reserva inexistente sem dívidas

**Expectativa qualitativa**

Fragilidade estrutural importante, mas sem colapso automático do score só por reserva zero.

**Faixa esperada**

`500-790`

---

### Cenário 08 - Planejamento financeiro excelente

**Expectativa qualitativa**

Score alto, com ganho estrutural sem mascarar riscos reais.

**Faixa esperada**

`780-920`

## Casos canônicos

Casos canônicos são cenários que nunca devem produzir resultado incoerente.

Exemplos:

- reserva zero e comprometimento alto não podem gerar score excelente
- inadimplência nunca pode ser classificada como situação saudável
- fluxo mensal negativo recorrente deve agravar o score
- patrimônio alto não pode mascarar ruptura operacional persistente
- boa renda sem reserva não deve parecer ausência total de risco, mas também não deve despencar artificialmente como se já houvesse inadimplência

## Matriz de sensibilidade

### Comprometimento

Faixas observadas:

- `30%`
- `40%`
- `50%`
- `60%`
- `70%`
- `80%`
- `90%`

Comportamento esperado:

- queda progressiva do pilar de fluxo
- sem penalização crítica automática enquanto não houver materialização de risco

### Liquidez

Faixas observadas:

- `12 meses`
- `6 meses`
- `3 meses`
- `1 mês`
- `0 meses`

Comportamento esperado:

- melhora progressiva da proteção
- queda estrutural do pilar de liquidez
- ausência total de reserva não deve, sozinha, simular inadimplência

### Persistência temporal

Faixas observadas:

- `1 mês negativo`
- `2 meses negativos consecutivos`
- `3 ou mais meses negativos consecutivos`

Comportamento esperado:

- `1 mês`: alerta ou penalização leve
- `2 meses`: agravamento moderado
- `3+ meses`: agravamento forte

## Auditoria operacional

Esta validação conceitual é complementada por auditoria operacional real.

Endpoint interno de desenvolvimento:

- `POST /api/MfScoreAuditoria/GerarPlanilha`

Essa auditoria:

- monta personas sintéticas
- executa o motor oficial
- compara score obtido com faixa esperada
- gera planilha `.xlsx`

O uso dessa auditoria é obrigatório sempre que houver mudança em:

- indicadores
- pesos
- pilares
- penalizações críticas
- classificações
- tendência
- histórico do score

## Auditoria humana

Além da validação automática, existe auditoria humana cega:

- `POST /api/MfScoreAuditoria/GerarPlanilhaAuditoriaHumana`

Ela serve para:

- avaliar se o motor foi severo demais
- avaliar se o motor foi permissivo demais
- amadurecer faixas esperadas e casos canônicos

## Personas persistidas de calibração

O projeto também possui um CRUD persistido de `Personas de Calibração do MF Score`.

Essa ferramenta serve para:

- ampliar cenários sintéticos sem editar código
- registrar score humano sugerido
- registrar faixa esperada
- rodar o motor oficial
- promover personas maduras a casos canônicos

## Critérios de evolução

Nenhuma alteração no Motor Financeiro deve ser considerada concluída sem:

1. revisar esta suíte
2. rodar a auditoria operacional
3. avaliar impacto nas personas persistidas
4. registrar mudanças no changelog
5. sincronizar `docs/MF_SCORE.md`, `docs/INDICADORES_FINANCEIROS.md` e `docs/MF_SCORE_AUDIT.md`
