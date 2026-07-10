# MF Score AI Question Flow

Este documento organiza a sequência ideal de perguntas para conversar com outra IA sobre a evolução do `MF Score`.

O objetivo é evitar que a calibragem comece pelos números antes de validar a arquitetura conceitual do modelo.

## Ordem recomendada

1. enquadramento conceitual
2. revisão da arquitetura do score
3. avaliação por indicador
4. proposta de faixas por indicador
5. avaliação por pilar
6. revisão das penalizações críticas
7. persistência temporal
8. revisão da inadimplência
9. avaliação das personas
10. revisão das faixas esperadas das personas
11. proposta prática de calibragem
12. roadmap de evolução

## Pergunta 01 - Enquadramento conceitual

```text
Com base no contexto fornecido, quero que você faça primeiro uma leitura conceitual do nosso MF Score.

Responda em profundidade:

1. O MF Score, do jeito que está estruturado hoje, está mais próximo de:
   - um score de riqueza,
   - um score de saúde financeira,
   - ou um score de risco financeiro pessoal?

2. Essa arquitetura atual está conceitualmente correta?
   - nota dos pilares
   - penalizações críticas
   - persistência temporal
   - histórico mensal

3. A separação entre:
   - fragilidade estrutural
   - evento crítico
   - risco materializado
   - risco persistente
está bem desenhada?

4. A regra de não dupla penalização está conceitualmente bem aplicada?

5. Antes de falar em pesos ou números, quais são os principais acertos e os principais erros conceituais do modelo atual?

Não quero ainda proposta de pesos finais.
Quero primeiro validar a lógica do modelo.
```

## Pergunta 02 - Revisão da arquitetura do score

```text
Agora quero que você avalie a arquitetura do MF Score como se estivesse revisando um motor de risco.

Analise:

1. O que deveria pertencer obrigatoriamente à camada de pilares?
2. O que deveria pertencer obrigatoriamente à camada de penalizações críticas?
3. O que deveria pertencer obrigatoriamente à camada temporal?
4. O que nunca deveria estar em penalização crítica porque já foi absorvido pelos pilares?
5. Quais fronteiras conceituais você sugere entre:
   - deterioração estrutural
   - ruptura operacional
   - evento crítico
   - reincidência

Quero que você proponha uma arquitetura conceitual limpa e explique o racional.
```

## Pergunta 03 - Avaliação por indicador

```text
Agora quero que você analise os indicadores já existentes no sistema, um por um.

Para cada indicador, explique:

1. O que ele mede em termos de risco financeiro real.
2. Se ele deve afetar:
   - pilar
   - penalização crítica
   - persistência temporal
   - ou combinação dessas camadas
3. Qual a severidade ideal de impacto.
4. Quais erros de calibragem devo evitar.
5. Como esse indicador se relaciona com uma lógica inspirada em análise de crédito, sem virar score bancário.

Indicadores:
- Economia Mensal
- Percentual de Economia
- Reserva de Emergência Atual
- Reserva de Emergência Ideal
- Comprometimento da Renda
- Comprometimento Financeiro Futuro 30 dias
- Pressão Financeira Acumulada 90 dias
- Pressão Financeira Acumulada 180 dias
- Pressão Financeira Acumulada 365 dias
- Endividamento Patrimonial
- Patrimônio Líquido Atual
- Percentual do Patrimônio Alvo
```

## Pergunta 04 - Proposta de faixas por indicador

```text
Agora quero que você proponha faixas ideais por indicador.

Para cada indicador, defina faixas como:
- saudável
- atenção
- risco moderado
- risco alto
- risco grave

Explique:
1. por que essas faixas fazem sentido;
2. como elas refletem risco real;
3. como evitar dupla penalização;
4. quando a faixa deve afetar apenas o pilar;
5. quando a faixa pode participar de uma regra crítica.

Quero resposta em formato de tabela + comentários.
```

## Pergunta 05 - Avaliação por pilar

```text
Agora quero que você avalie os cinco pilares do MF Score.

Para cada pilar:
1. explique o que ele deveria representar conceitualmente;
2. diga quais indicadores devem pesar mais;
3. diga quais devem pesar menos;
4. diga quais combinações são mais perigosas;
5. diga quando um pilar fraco deve apenas baixar nota;
6. diga quando ele deveria acionar uma penalização crítica adicional.

Pilares:
- Fluxo de Caixa
- Liquidez
- Endividamento
- Patrimônio
- Planejamento
```

## Pergunta 06 - Penalizações críticas

```text
Agora quero que você proponha uma revisão da camada de penalizações críticas.

Quero que você responda:

1. Quais penalizações críticas atuais fazem sentido manter?
2. Quais estão fracas demais?
3. Quais estão fortes demais?
4. Quais poderiam ser progressivas?
5. Quais deveriam depender de recorrência?
6. Quais nunca deveriam existir por duplicarem efeito de pilar?

Quero uma proposta inspirada em modelos de deterioração de risco real.

Explique cada penalização com:
- gatilho objetivo
- motivo
- severidade sugerida
- se deve ser fixa, progressiva ou temporal
- risco de exagero
```

## Pergunta 07 - Persistência temporal

```text
Agora quero que você modele a camada temporal do MF Score.

Analise:
1. como o score deveria reagir a 1 mês negativo;
2. como deveria reagir a 2 meses consecutivos negativos;
3. como deveria reagir a 3 ou mais meses;
4. como a recorrência de inadimplência deveria pesar;
5. como a melhora consistente ao longo dos meses deveria aliviar o risco.

Quero que você proponha:
- regras temporais
- gravidade progressiva
- possíveis bônus de recuperação
- cuidados para não distorcer a fotografia atual
```

## Pergunta 08 - Revisão da inadimplência

```text
Agora quero focar especificamente na mecânica de inadimplência.

Hoje o sistema trata inadimplência de forma binária:
- existe despesa vencida e pendente = há inadimplência

Quero que você avalie:
1. essa lógica já é suficiente para uma V1?
2. o que falta para aproximá-la de um modelo de risco mais realista?
3. faz sentido graduar por:
   - valor vencido
   - quantidade de títulos vencidos
   - dias de atraso
   - recorrência histórica?
4. como você desenharia uma evolução segura dessa regra sem complicar demais o modelo?

Quero resposta prática e arquitetural.
```

## Pergunta 09 - Avaliação das personas

```text
Agora quero que você avalie nossas personas sintéticas de calibração.

Para cada persona, devolva:
1. leitura executiva do perfil;
2. principais forças;
3. principais fragilidades;
4. risco esperado;
5. faixa ideal de MF Score em 0 a 1000;
6. justificativa detalhada;
7. pilares que puxam para cima;
8. pilares que puxam para baixo;
9. penalizações críticas que devem entrar;
10. penalizações que não devem entrar;
11. erro de calibragem mais provável neste caso.

Use as personas atuais do sistema como referência.
```

## Pergunta 10 - Revisão das faixas esperadas das personas

```text
Agora quero que você revise nossas faixas esperadas atuais das personas.

Quero que você diga, para cada persona:
1. se a faixa atual faz sentido;
2. se está larga demais;
3. se está estreita demais;
4. se está severa demais;
5. se está permissiva demais;
6. qual faixa você recomendaria.

Quero uma tabela com:
- persona
- faixa atual
- faixa sugerida
- motivo da mudança
```

## Pergunta 11 - Proposta de calibragem prática

```text
Agora quero que você transforme toda a análise anterior em uma proposta prática de calibragem.

Quero que você organize a resposta em:

1. mudanças conceituais prioritárias
2. mudanças de pesos por pilar
3. mudanças de pesos por indicador
4. mudanças nas penalizações críticas
5. mudanças na camada temporal
6. mudanças nas faixas esperadas das personas
7. ordem recomendada de implementação
8. riscos de implementação
9. como validar sem quebrar o modelo

Quero uma proposta aplicável, não apenas teórica.
```

## Pergunta 12 - Plano de evolução do MF Score

```text
Por fim, quero que você monte um roadmap técnico de evolução do MF Score.

Separe em:
- curto prazo
- médio prazo
- longo prazo

Inclua:
1. o que deve ser corrigido primeiro;
2. o que deve esperar mais maturidade de dados;
3. o que depende de histórico real;
4. o que depende de evolução do domínio;
5. o que depende de decisão conceitual e não de código.

Quero um roadmap pragmático e priorizado.
```

## Sequência curta recomendada

Se quiser fazer em blocos maiores, esta é a ordem recomendada:

### Bloco 1

- Pergunta 01
- Pergunta 02

### Bloco 2

- Pergunta 03
- Pergunta 04
- Pergunta 05

### Bloco 3

- Pergunta 06
- Pergunta 07
- Pergunta 08

### Bloco 4

- Pergunta 09
- Pergunta 10
- Pergunta 11
- Pergunta 12

## Uso recomendado com os outros arquivos

Sugestão de pacote:

1. `docs/MF_SCORE_AI_CONTEXT_PROMPT_READY.md`
2. `docs/MF_SCORE_AI_QUESTION_FLOW.md`
3. prompt principal de calibragem
4. personas ou cenários reais
