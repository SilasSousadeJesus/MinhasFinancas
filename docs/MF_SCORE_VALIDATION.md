# MF Score Validation

Este documento complementa `docs/MF_SCORE.md`.

Enquanto o `MF_SCORE.md` explica como o modelo funciona, este documento explica como validamos se ele continua coerente ao longo do tempo.

## Objetivo

Criar uma base permanente de validaÃ§Ã£o para o MF Score.

A ideia Ã© proteger o Motor Financeiro contra mudanÃ§as que pareÃ§am corretas isoladamente, mas produzam resultados incoerentes no conjunto.

## Filosofia

O MF Score Ã© um modelo de risco financeiro pessoal.

Modelos de risco nÃ£o evoluem por opiniÃ£o.

Eles evoluem com validaÃ§Ã£o contÃ­nua, cenÃ¡rios oficiais e casos canÃ´nicos.

## Suite Oficial de ValidaÃ§Ã£o do MF Score

A suÃ­te oficial Ã© o conjunto de cenÃ¡rios que deve ser usado para verificar se o comportamento do MF Score continua coerente apÃ³s qualquer alteraÃ§Ã£o relevante.

Ela existe para responder:

- o score continua reagindo de forma esperada?
- o modelo ainda diferencia risco alto, mÃ©dio e baixo com clareza?
- a mudanÃ§a melhora a leitura do risco ou apenas ajusta nÃºmeros?

## CenÃ¡rios oficiais

### CenÃ¡rio 01 - Vida Financeira Excelente

**DescriÃ§Ã£o**

UsuÃ¡rio com alta renda, liquidez elevada, patrimÃ´nio elevado, fluxo saudÃ¡vel e sem dÃ­vidas.

**Contexto financeiro**

- renda forte
- liquidez robusta
- patrimÃ´nio alto
- endividamento praticamente nulo
- fluxo de caixa saudÃ¡vel

**Principais indicadores**

- economia mensal favorÃ¡vel
- percentual de economia alto
- reserva de emergÃªncia muito confortÃ¡vel
- endividamento baixo
- patrimÃ´nio lÃ­quido elevado

**Expectativa qualitativa**

MF Score muito alto.

**Faixa esperada**

`90-100`

**Justificativa**

Esse cenÃ¡rio combina proteÃ§Ã£o, disciplina e base patrimonial forte.

---

### CenÃ¡rio 02 - Boa renda, mas liquidez inexistente e cartÃ£o elevado

**DescriÃ§Ã£o**

UsuÃ¡rio com boa renda, mas sem reserva e com cartÃ£o muito pressionado.

**Contexto financeiro**

- renda boa
- liquidez inexistente
- pressÃ£o de cartÃ£o elevada
- renda jÃ¡ bastante comprometida

**Principais indicadores**

- comprometimento alto
- liquidez inexistente
- pressÃ£o financeira elevada
- endividamento em alerta

**Expectativa qualitativa**

MF Score moderado.

**Faixa esperada**

`60-74`

**Justificativa**

A renda ajuda, mas nÃ£o elimina o risco estrutural causado por falta de liquidez e pressÃ£o recorrente.

---

### CenÃ¡rio 03 - PatrimÃ´nio elevado com fluxo de caixa ruim

**DescriÃ§Ã£o**

UsuÃ¡rio com patrimÃ´nio alto, mas fluxo ruim e comprometimento elevado.

**Contexto financeiro**

- patrimÃ´nio relevante
- fluxo de caixa pressionado
- compromissos elevados

**Principais indicadores**

- patrimÃ´nio lÃ­quido forte
- economia mensal fraca
- comprometimento alto
- pressÃ£o futura relevante

**Expectativa qualitativa**

O patrimÃ´nio reduz o risco estrutural, mas o fluxo limita o score.

**Faixa esperada**

`55-75`

**Justificativa**

PatrimÃ´nio ajuda, mas nÃ£o compensa totalmente um fluxo de caixa instÃ¡vel.

---

### CenÃ¡rio 04 - Excelente fluxo com pouco patrimÃ´nio

**DescriÃ§Ã£o**

UsuÃ¡rio com Ã³timo fluxo de caixa, boa liquidez e pouca estrutura patrimonial.

**Contexto financeiro**

- fluxo forte
- liquidez boa
- patrimÃ´nio ainda pequeno
- sem dÃ­vidas relevantes

**Principais indicadores**

- economia mensal forte
- percentual de economia bom
- reserva atual saudÃ¡vel
- endividamento baixo

**Expectativa qualitativa**

Score elevado.

**Faixa esperada**

`75-90`

**Justificativa**

O fluxo saudÃ¡vel compensa parcialmente a base patrimonial ainda pequena.

---

### CenÃ¡rio 05 - InadimplÃªncia

**DescriÃ§Ã£o**

UsuÃ¡rio com compromissos jÃ¡ vencidos e situaÃ§Ã£o de inadimplÃªncia.

**Contexto financeiro**

- atraso em obrigaÃ§Ãµes
- pressÃ£o de caixa imediata
- risco de agravamento estrutural

**Principais indicadores**

- comprometimento crÃ­tico
- pressÃ£o de curto prazo
- risco operacional elevado

**Expectativa qualitativa**

PenalizaÃ§Ã£o relevante.

**Faixa esperada**

`0-49`

**Justificativa**

InadimplÃªncia Ã© sinal forte de risco imediato e nÃ£o deve ser suavizada pelo score.

---

### CenÃ¡rio 06 - Comprometimento extremo

**DescriÃ§Ã£o**

UsuÃ¡rio com renda fortemente comprometida, mesmo podendo ter algum patrimÃ´nio.

**Contexto financeiro**

- renda pressionada
- obrigaÃ§Ãµes altas
- pouca folga operacional

**Principais indicadores**

- comprometimento muito elevado
- pressÃ£o futura muito alta
- liquidez insuficiente

**Expectativa qualitativa**

Score limitado mesmo havendo patrimÃ´nio.

**Faixa esperada**

`0-59`

**Justificativa**

PatrimÃ´nio nÃ£o deve mascarar falta de folga de caixa.

---

### CenÃ¡rio 07 - liquidez inexistente

**DescriÃ§Ã£o**

UsuÃ¡rio sem reserva de emergÃªncia configurada ou efetiva.

**Contexto financeiro**

- ausÃªncia de proteÃ§Ã£o
- vulnerabilidade alta a imprevistos

**Principais indicadores**

- liquidez atual zero
- cobertura nula
- leitura estrutural fraca

**Expectativa qualitativa**

PenalizaÃ§Ã£o estrutural.

**Faixa esperada**

`0-69`

**Justificativa**

Sem reserva, o sistema deve refletir fragilidade real de proteÃ§Ã£o financeira.

---

### CenÃ¡rio 08 - Planejamento financeiro excelente

**DescriÃ§Ã£o**

UsuÃ¡rio com plano estratÃ©gico, metas, compromissos e consistÃªncia muito bons.

**Contexto financeiro**

- plano estratÃ©gico claro
- metas bem definidas
- compromissos alinhados
- alta consistÃªncia

**Principais indicadores**

- planejamento configurado
- disciplina forte
- boa coerÃªncia estratÃ©gica

**Expectativa qualitativa**

Pequeno ganho de score, nunca dominante.

**Faixa esperada**

`+3 a +10 pontos` sobre um cenÃ¡rio semelhante sem estrutura estratÃ©gica.

**Justificativa**

Planejamento melhora o score, mas nÃ£o deve sobrepor riscos financeiros reais de caixa, reserva ou endividamento.

## Casos canÃ´nicos

Casos canÃ´nicos sÃ£o cenÃ¡rios que nunca devem produzir resultados incoerentes.

Exemplos:

- reserva zero + comprometimento 90% + pressÃ£o financeira elevada nunca pode gerar score excelente
- inadimplÃªncia nunca pode ser classificada como situaÃ§Ã£o saudÃ¡vel
- endividamento extremo nunca deve ser compensado apenas por patrimÃ´nio isolado
- boa renda sem reserva nÃ£o deve parecer ausÃªncia total de risco

Esses casos protegem o Motor Financeiro contra regressÃµes.

## Matriz de sensibilidade

A matriz de sensibilidade serve para entender como o score reage quando apenas uma variÃ¡vel muda.

### Comprometimento

Faixas de observaÃ§Ã£o:

- 30%
- 40%
- 50%
- 60%
- 70%
- 80%
- 90%

Comportamento esperado:

- o score deve cair progressivamente
- a pressÃ£o sobre o fluxo de caixa deve ficar mais evidente
- o impacto nÃ£o deve ser linear cego se outros pilares estiverem fortes

### Liquidez

Faixas de observaÃ§Ã£o:

- 12 meses
- 6 meses
- 3 meses
- 1 mÃªs
- 0 meses

Comportamento esperado:

- o score deve melhorar com maior proteÃ§Ã£o
- a queda da liquidez deve reduzir a nota do pilar correspondente
- a ausÃªncia total de reserva deve acionar leitura estrutural de risco

### PressÃ£o financeira

Horizontes observados:

- 30 dias
- 90 dias
- 180 dias
- 12 meses

Comportamento esperado:

- pressÃµes mais curtas devem impactar mais o curto prazo
- horizontes maiores devem mostrar risco acumulado
- o score nÃ£o deve ignorar pressÃ£o futura relevante

## Validador do MF Score

Na arquitetura atual, a validaÃ§Ã£o pode comeÃ§ar como documentaÃ§Ã£o e testes automatizados dentro da prÃ³pria base.

Se no futuro fizer sentido criar uma ferramenta dedicada, o nome sugerido Ã©:

- `MfScoreValidator`
- ou `MotorFinanceiroValidator`

### Responsabilidades esperadas

- executar cenÃ¡rios oficiais automaticamente
- comparar score esperado com score obtido
- destacar regressÃµes
- registrar diferenÃ§as de comportamento

## CritÃ©rios de evoluÃ§Ã£o

Nenhuma alteraÃ§Ã£o no Motor Financeiro deve ser considerada concluÃ­da sem passar pela Suite Oficial de ValidaÃ§Ã£o.

Isso vale para:

- novos indicadores
- mudanÃ§as de pesos
- mudanÃ§as de pilares
- mudanÃ§as de penalizaÃ§Ã£o
- mudanÃ§as de classificaÃ§Ã£o
- mudanÃ§as nas fÃ³rmulas

## RecomendaÃ§Ã£o para a primeira calibraÃ§Ã£o prÃ¡tica

A primeira calibraÃ§Ã£o prÃ¡tica deve comeÃ§ar pelos cenÃ¡rios mais extremos:

1. reserva zero
2. inadimplÃªncia
3. comprometimento extremo
4. patrimÃ´nio alto com fluxo ruim
5. alta renda com proteÃ§Ã£o baixa

Esses casos mostram rapidamente se o modelo estÃ¡ subestimando risco ou protegendo demais o usuÃ¡rio.


