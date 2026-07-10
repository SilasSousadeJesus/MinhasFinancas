# MF Score — Contexto de Calibração para IA

Este documento foi criado para servir como base de conhecimento de outra IA que irá nos ajudar a revisar, calibrar e evoluir o `MF Score`.

O foco aqui não é propor um novo modelo, mas explicar com precisão como o motor funciona hoje:

- como os indicadores são calculados;
- como os pilares são montados;
- como as penalizações críticas são aplicadas;
- quais pesos e escalas são usados;
- quais limitações já identificamos no modelo atual.

Use este material como retrato fiel do estado atual do projeto antes de sugerir recalibrações.

---

## 1. Objetivo do MF Score

O `MF Score` é um score de risco financeiro pessoal.

Ele não foi concebido como:

- score de riqueza;
- score de disciplina isolada;
- score de crédito bancário tradicional;
- score de comportamento externo de mercado.

Ele existe para resumir, em uma escala de `0 a 1000`, o nível atual de saúde e risco financeiro do usuário a partir dos dados já registrados no sistema.

Hoje o motor combina:

1. indicadores financeiros;
2. pilares temáticos;
3. penalizações críticas;
4. histórico recente para tendência.

---

## 2. Escalas oficiais

### Escala dos pilares

- Cada pilar recebe nota de `0 a 100`.

### Escala final do MF Score

- O score final é convertido para `0 a 1000`.
- Conversão atual:
  - `nota normalizada * 10`

Exemplo:

- Pilar com nota `82` permanece `82`.
- MF Score final com nota normalizada `82` vira `820`.

---

## 3. Classificação final do MF Score

Faixas atuais:

- `900 a 1000` → `Excelente`
- `800 a 899` → `Muito Bom`
- `700 a 799` → `Bom`
- `600 a 699` → `Atenção`
- `400 a 599` → `Crítico`
- `0 a 399` → `Muito Crítico`

Mapeamento atual de risco:

- `Excelente` → `Risco Muito Baixo`
- `Muito Bom` → `Risco Baixo`
- `Bom` → `Risco Moderado`
- `Atenção` → `Risco Moderado-Alto`
- `Crítico` → `Risco Alto`
- `Muito Crítico` → `Risco Muito Alto`

---

## 4. Fonte de dados usada hoje

O motor consome dados já existentes no sistema, sem duplicação:

- `Lançamentos`
- `Patrimônio`
- `Perfil Financeiro`
- `Histórico de MF Score`

### Regras importantes de leitura

- Lançamentos cancelados são ignorados.
- O mês atual é definido por `DataReferencia`.
- Para fluxo e economia do mês atual, o motor usa `DataVencimento`.
- Para obrigações e receitas futuras, o motor usa apenas lançamentos `Pendente`.
- Ativos e passivos usam o valor mais recente de permanência patrimonial.

---

## 5. Dados-base calculados antes dos indicadores

Antes de montar os indicadores, o sistema consolida um objeto intermediário chamado `DadosReferenciaAnaliseFinanceira`.

### 5.1 Fluxo mensal atual

#### Receita mensal atual

Soma das receitas do mês atual:

`ReceitaMensalAtual = soma dos lançamentos do tipo Receita no mês da DataReferencia`

#### Despesa mensal atual

Soma das despesas do mês atual:

`DespesaMensalAtual = soma dos lançamentos do tipo Despesa no mês da DataReferencia`

#### Economia mensal atual

`EconomiaMensalAtual = ReceitaMensalAtual - DespesaMensalAtual`

#### Percentual de economia atual

`PercentualEconomiaAtual = (EconomiaMensalAtual / ReceitaMensalAtual) * 100`

Se `ReceitaMensalAtual <= 0`, o percentual atual fica `0`.

### 5.2 Patrimônio

#### Total de ativos

Soma do valor mais recente de cada ativo patrimonial.

#### Total de passivos

Soma do valor mais recente de cada passivo patrimonial.

#### Patrimônio líquido atual

`PatrimonioLiquidoAtual = TotalAtivos - TotalPassivos`

### 5.3 Reserva de emergência

Hoje a reserva atual considera apenas ativos dos tipos:

- `DinheiroEmConta`
- `Investimento`

#### Reserva atual

`ReservaEmergenciaAtual = soma desses ativos líquidos`

#### Base integral da reserva

`BaseReservaEmergenciaIntegral = DespesaMensalAtual * MesesReservaEmergenciaDesejados`

#### Reserva ideal configurada

`ReservaEmergenciaIdealConfigurada = BaseReservaEmergenciaIntegral * (PercentualReservaEmergenciaDesejado / 100)`

#### Cobertura em meses

`CoberturaReservaEmMeses = ReservaEmergenciaAtual / DespesaMensalAtual`

Se `DespesaMensalAtual <= 0`, a cobertura fica `0`.

### 5.4 Comprometimento da renda

`ComprometimentoRendaAtual = (DespesaMensalAtual / ReceitaMensalAtual) * 100`

Se `ReceitaMensalAtual <= 0`:

- se existir despesa, o sistema usa `100`;
- se não existir despesa, usa `0`.

### 5.5 Obrigações futuras e pressão futura

O motor calcula 4 horizontes:

- `30 dias`
- `90 dias`
- `180 dias`
- `365 dias`

#### Obrigações financeiras futuras

Para cada janela:

`ObrigacoesFinanceirasFuturas = soma de despesas pendentes com DataVencimento entre DataReferencia e DataReferencia + N dias`

#### Receita prevista futura

Para cada janela:

`ReceitaPrevista = soma de receitas pendentes com DataVencimento entre DataReferencia e DataReferencia + N dias`

#### Comprometimento financeiro futuro de 30 dias

`ComprometimentoFinanceiroFuturoAtual = (ObrigacoesFinanceirasFuturas30Dias / ReceitaPrevista30Dias) * 100`

Observação importante:

- no código, o método chama o segundo parâmetro de `receitaMensalAtual`, mas o valor realmente passado é `ReceitaPrevista30Dias`.

#### Pressão financeira acumulada de 90, 180 e 365 dias

`ComprometimentoFinanceiroFuturo90DiasAtual = (ObrigacoesFinanceirasFuturas90Dias / ReceitaPrevista90Dias) * 100`

`ComprometimentoFinanceiroFuturo180DiasAtual = (ObrigacoesFinanceirasFuturas180Dias / ReceitaPrevista180Dias) * 100`

`ComprometimentoFinanceiroFuturo365DiasAtual = (ObrigacoesFinanceirasFuturas365Dias / ReceitaPrevista365Dias) * 100`

Se a receita prevista do período for `0`:

- se houver obrigações, o percentual vira `100`;
- se não houver obrigações, vira `0`.

### 5.6 Endividamento

`EndividamentoAtual = (TotalPassivos / TotalAtivos) * 100`

Se `TotalAtivos <= 0`:

- se houver passivos, o sistema usa `100`;
- se não houver passivos, usa `0`.

### 5.7 Patrimônio alvo

O valor vem do `Perfil Financeiro`.

`PercentualPatrimonioAlvoAtual = (PatrimonioLiquidoAtual / PatrimonioAlvo) * 100`

Se `PatrimonioAlvo <= 0`, o percentual fica `0`.

---

## 6. Regras atuais de status dos indicadores

O projeto trabalha com 4 status:

- `Excelente`
- `Bom`
- `Atenção`
- `Crítico`

### 6.1 Meta mínima

Usado quando o valor atual deve ser **maior ou igual** a uma meta.

Regras atuais:

- se `meta <= 0`:
  - `valorAtual > 0` → `Bom`
  - caso contrário → `Atenção`
- se `valorAtual >= meta` → `Excelente`
- se `valorAtual <= 0` → `Crítico`
- caso contrário → `Atenção`

### 6.2 Meta máxima

Usado quando o valor atual deve ser **menor ou igual** a um limite.

Regras atuais:

- se `valorAtual <= 0` → `Excelente`
- se `meta <= 0` → `Atenção`
- se `valorAtual <= meta` → `Excelente`
- caso contrário → `Atenção`

### 6.3 Progresso percentual

Usado para progresso rumo a um alvo.

Regras atuais:

- `percentual >= 100` → `Excelente`
- `percentual > 0` → `Atenção`
- caso contrário → `Crítico`

### 6.4 Observação importante sobre a calibragem atual

O modelo atual quase não utiliza o status `Bom` nas regras principais.

Hoje, na prática:

- muitos indicadores ficam apenas entre `Excelente`, `Atenção` e `Crítico`;
- `Bom` aparece mais em casos específicos de fallback, como ausência de meta, reserva ideal configurada e patrimônio sem alvo negativo.

Isso reduz a granularidade do motor e é um ponto importante para futura recalibração.

---

## 7. Indicadores atuais

## 7.1 Economia Mensal

### O que mede

Quanto sobra ou falta no mês atual em valor absoluto.

### Fórmula atual

`EconomiaMensalAtual = ReceitaMensalAtual - DespesaMensalAtual`

### Meta ideal

`PercentualEconomiaMensalDesejado` do Perfil Financeiro.

Observação:

- o indicador compara **valor absoluto em moeda** contra uma configuração que representa um **percentual desejado**.
- isso é uma limitação conceitual importante do modelo atual.

### Status

Usa `ResolverMetaMinima`.

### Peso oficial

`1.0`

## 7.2 Percentual de Economia

### O que mede

Quanto da renda mensal está sendo preservado.

### Fórmula atual

`PercentualEconomiaAtual = (EconomiaMensalAtual / ReceitaMensalAtual) * 100`

### Meta ideal

`PercentualEconomiaMensalDesejado` do Perfil Financeiro.

### Status

Usa `ResolverMetaMinima`.

### Peso oficial

`1.0`

## 7.3 Reserva de Emergência Atual

### O que mede

Quanto o usuário já possui de reserva líquida.

### Fórmula atual

`ReservaEmergenciaAtual = soma de ativos do tipo DinheiroEmConta + Investimento`

### Meta ideal

`ReservaEmergenciaIdealConfigurada`

### Percentual mostrado

`(ReservaEmergenciaAtual / ReservaEmergenciaIdealConfigurada) * 100`

### Status

Usa `ResolverMetaMinima`.

### Peso oficial

`1.5`

## 7.4 Reserva de Emergência Ideal

### O que mede

Se a meta de reserva foi configurada no Perfil Financeiro e qual é seu tamanho calculado.

### Fórmula atual

`ReservaEmergenciaIdealConfigurada = DespesaMensalAtual * MesesReservaEmergenciaDesejados * (PercentualReservaEmergenciaDesejado / 100)`

### Meta ideal

`BaseReservaEmergenciaIntegral`

### Percentual mostrado

O campo `Percentual` deste indicador não é progresso.

Hoje ele mostra:

`PercentualReservaEmergenciaDesejado`

### Status

- `Bom` se a configuração existir;
- `Atenção` se a configuração não existir.

### Peso oficial

`0.5`

### Observação importante

Este indicador mede principalmente **existência/configuração da meta**, não resultado financeiro alcançado.

## 7.5 Comprometimento da Renda

### O que mede

Quanto da renda do mês atual já está consumido pelas despesas do próprio mês.

### Fórmula atual

`ComprometimentoRendaAtual = (DespesaMensalAtual / ReceitaMensalAtual) * 100`

### Meta ideal

`PercentualMaximoComprometimentoRenda` do Perfil Financeiro.

### Status

Usa `ResolverMetaMaxima`.

### Peso oficial

`1.5`

## 7.6 Comprometimento Financeiro Futuro — 30 dias

### O que mede

A pressão das obrigações pendentes dos próximos 30 dias contra a receita prevista do mesmo horizonte.

### Fórmula atual

`(ObrigacoesFinanceirasFuturas30Dias / ReceitaPrevista30Dias) * 100`

### Meta ideal

`PercentualMaximoComprometimentoRenda`

### Status

Usa `ResolverMetaMaxima`.

### Peso oficial

`1.5`

## 7.7 Comprometimento Financeiro Futuro — 90 dias

### O que mede

Pressão acumulada de obrigações pendentes em 90 dias.

### Fórmula atual

`(ObrigacoesFinanceirasFuturas90Dias / ReceitaPrevista90Dias) * 100`

### Meta ideal

`PercentualMaximoComprometimentoRenda`

### Status

Usa `ResolverMetaMaxima`.

### Peso oficial

`1.0`

## 7.8 Comprometimento Financeiro Futuro — 180 dias

### O que mede

Pressão acumulada de obrigações pendentes em 180 dias.

### Fórmula atual

`(ObrigacoesFinanceirasFuturas180Dias / ReceitaPrevista180Dias) * 100`

### Meta ideal

`PercentualMaximoComprometimentoRenda`

### Status

Usa `ResolverMetaMaxima`.

### Peso oficial

`0.75`

## 7.9 Comprometimento Financeiro Futuro — 365 dias

### O que mede

Pressão acumulada de obrigações pendentes em 365 dias.

### Fórmula atual

`(ObrigacoesFinanceirasFuturas365Dias / ReceitaPrevista365Dias) * 100`

### Meta ideal

`PercentualMaximoComprometimentoRenda`

### Status

Usa `ResolverMetaMaxima`.

### Peso oficial

`0.5`

## 7.10 Endividamento

### O que mede

Relação entre passivos e ativos patrimoniais.

### Fórmula atual

`EndividamentoAtual = (TotalPassivos / TotalAtivos) * 100`

### Meta ideal

`PercentualMaximoEndividamento` do Perfil Financeiro.

### Status

Usa `ResolverMetaMaxima`.

### Peso oficial

`1.5`

## 7.11 Patrimônio Líquido Atual

### O que mede

Diferença entre ativos e passivos.

### Fórmula atual

`PatrimonioLiquidoAtual = TotalAtivos - TotalPassivos`

### Meta ideal

`PatrimonioLiquidoAlvo` do Perfil Financeiro.

### Status

- se houver alvo configurado: usa `ResolverMetaMinima`
- se não houver alvo:
  - `PatrimonioLiquidoAtual >= 0` → `Bom`
  - `PatrimonioLiquidoAtual < 0` → `Crítico`

### Peso oficial

`1.25`

## 7.12 Percentual do Patrimônio Alvo

### O que mede

Quanto do patrimônio-alvo já foi alcançado.

### Fórmula atual

`PercentualPatrimonioAlvoAtual = (PatrimonioLiquidoAtual / PatrimonioAlvo) * 100`

### Meta ideal

`100`

### Status

- se houver alvo configurado: usa `ResolverProgresso`
- se não houver alvo configurado: `Atenção`

### Peso oficial

`0.75`

---

## 8. Conversão de status em nota numérica

Quando o sistema calcula a nota média de um pilar, ele converte cada status em número:

- `Excelente` → `100`
- `Bom` → `80`
- `Atenção` → `55`
- `Crítico` → `25`

Depois aplica média ponderada com os pesos dos indicadores.

---

## 9. Pilares atuais

O modelo atual possui 5 pilares.

## 9.1 Fluxo de Caixa

### Peso do pilar

`30`

### Indicadores usados

- Economia Mensal
- Percentual de Economia
- Comprometimento da Renda
- Comprometimento Financeiro Futuro 30 dias

## 9.2 Liquidez e Reserva

### Peso do pilar

`25`

### Indicadores usados

- Reserva de Emergência Atual
- Reserva de Emergência Ideal

## 9.3 Endividamento e Obrigações

### Peso do pilar

`20`

### Indicadores usados

- Endividamento
- Comprometimento Financeiro Futuro 30 dias
- Comprometimento Financeiro Futuro 90 dias
- Comprometimento Financeiro Futuro 180 dias
- Comprometimento Financeiro Futuro 365 dias

## 9.4 Patrimônio

### Peso do pilar

`15`

### Indicadores usados

- Patrimônio Líquido Atual
- Percentual do Patrimônio Alvo

## 9.5 Planejamento e Disciplina

### Peso do pilar

`10`

### Indicadores usados

- Reserva de Emergência Ideal
- Comprometimento da Renda
- Endividamento
- Percentual do Patrimônio Alvo

### Regra especial

Este pilar não representa um domínio “puro” como os demais.

Ele é calculado como uma combinação proxy:

1. média ponderada desses indicadores;
2. bônus por quantidade de metas/configurações presentes.

### Bônus atual

- `+10` se `4` ou mais indicadores relevantes tiverem `ValorIdeal > 0`
- `+5` se `3` indicadores relevantes tiverem `ValorIdeal > 0`
- `+0` nos demais casos

### Fórmula simplificada

`NotaPlanejamento = clamp(MediaPonderada + BonusConfiguracao, 0, 100)`

### Observação importante

Este é um dos pontos mais sensíveis para recalibração.

Hoje o pilar ainda mede mais “existência de metas e limites configurados + proxies de execução” do que planejamento real executado.

---

## 10. Cálculo atual do score-base

Passo a passo:

1. calcular todos os indicadores;
2. converter status dos indicadores em nota numérica;
3. calcular a nota de cada pilar em `0 a 100`;
4. aplicar peso dos pilares;
5. gerar a pontuação base normalizada.

### Pesos dos pilares

- Fluxo de Caixa → `30`
- Liquidez e Reserva → `25`
- Endividamento e Obrigações → `20`
- Patrimônio → `15`
- Planejamento e Disciplina → `10`

### Fórmula conceitual

`PontuacaoBaseNormalizada = media ponderada das notas dos 5 pilares`

Depois:

`PontuacaoBaseMfScore = PontuacaoBaseNormalizada * 10`

---

## 11. Penalizações críticas atuais

As penalizações críticas são aplicadas **depois** do score-base dos pilares.

Elas existem para capturar situações mais graves ou materializadas, que não deveriam depender apenas da nota média dos indicadores.

## 11.1 Patrimônio líquido negativo

### Gatilho

`PatrimonioLiquidoAtual < 0`

### Penalidade normalizada

`10`

### Penalidade convertida para a escala final

`100`

### Pilar associado

`Patrimônio`

## 11.2 Fluxo mensal negativo no mês atual

### Gatilho

`PossuiFluxoMensalNegativoAtual = true`

Na prática:

`Receitas do mês atual - Despesas do mês atual < 0`

### Penalidade normalizada

`8`

### Penalidade convertida

`80`

### Pilar associado

`Fluxo de Caixa`

## 11.3 Dois meses consecutivos de fluxo negativo

### Gatilho

`MesesConsecutivosFluxoNegativo >= 2`

### Penalidade normalizada

`6`

### Penalidade convertida

`60`

### Pilar associado

`Fluxo de Caixa`

## 11.4 Três ou mais meses consecutivos de fluxo negativo

### Gatilho

`MesesConsecutivosFluxoNegativo >= 3`

### Penalidade normalizada

`12`

### Penalidade convertida

`120`

### Pilar associado

`Fluxo de Caixa`

### Observação importante

O modelo atual aplica a penalidade de `3+ meses` e não a de `2 meses`, porque a regra usa `if/else if`.

## 11.5 Inadimplência

### Gatilho

Existe ao menos um lançamento que seja:

- `Despesa`
- `Pendente`
- `DataVencimento < DataReferencia`

### Penalidade normalizada

`15`

### Penalidade convertida

`150`

### Pilar associado

`Endividamento e Obrigações`

### Observação importante

A inadimplência hoje é binária:

- existe → penaliza;
- não existe → não penaliza.

Ainda não há graduação por:

- dias de atraso;
- valor em atraso;
- reincidência;
- número de compromissos vencidos.

## 11.6 Dados essenciais insuficientes

### Gatilho

O contexto é considerado insuficiente quando:

- não existe nenhum lançamento;
  **ou**
- não existe nenhum ativo **e** nenhum passivo

### Penalidade normalizada

`3`

### Penalidade convertida

`30`

### Pilar associado

`Planejamento e Disciplina`

---

## 12. Fórmula atual do score final

### Etapa 1

Calcular a pontuação base normalizada:

`BaseNormalizada`

### Etapa 2

Somar todas as penalidades críticas:

`PenalidadeTotalNormalizada`

### Etapa 3

Subtrair:

`PontuacaoFinalNormalizada = clamp(round(BaseNormalizada - PenalidadeTotalNormalizada), 0, 100)`

### Etapa 4

Converter para `0 a 1000`:

`MfScoreFinal = PontuacaoFinalNormalizada * 10`

---

## 13. Tendência

O motor também calcula tendência.

### Regra principal

Se houver histórico recente:

- diferença `>= 40` pontos no score final → `Positiva`
- diferença `<= -40` pontos → `Negativa`
- caso contrário → `Neutra`

### Regra de fallback

Se não houver histórico suficiente:

- compara quantidade de indicadores positivos e negativos
- e observa a faixa atual do score

---

## 14. Resumo dos pesos oficiais dos indicadores

| Indicador | Peso |
|---|---:|
| Economia Mensal | 1.00 |
| Percentual de Economia | 1.00 |
| Reserva de Emergência Atual | 1.50 |
| Reserva de Emergência Ideal | 0.50 |
| Comprometimento da Renda | 1.50 |
| Comprometimento Financeiro Futuro 30 dias | 1.50 |
| Comprometimento Financeiro Futuro 90 dias | 1.00 |
| Comprometimento Financeiro Futuro 180 dias | 0.75 |
| Comprometimento Financeiro Futuro 365 dias | 0.50 |
| Endividamento | 1.50 |
| Patrimônio Líquido Atual | 1.25 |
| Percentual do Patrimônio Alvo | 0.75 |

---

## 15. Limitações e pontos sensíveis do modelo atual

Estes pontos são especialmente importantes para qualquer IA que vá propor calibração.

## 15.1 Indicador Economia Mensal usa meta percentual como se fosse meta monetária

Hoje o indicador `Economia Mensal` compara um valor absoluto em moeda com `PercentualEconomiaMensalDesejado`.

Isso cria uma inconsistência semântica.

## 15.2 Status “Bom” é pouco explorado

O modelo atual fica muito polarizado entre:

- `Excelente`
- `Atenção`
- `Crítico`

Isso reduz nuance.

## 15.3 Inadimplência ainda é binária

Não diferencia:

- 1 dia vs 90 dias de atraso;
- R$ 50 vs R$ 20.000 vencidos;
- atraso único vs reincidência.

## 15.4 Pilar Planejamento ainda é proxy

Ele ainda não mede planejamento executado com profundidade.

Hoje mede mais:

- existência de metas;
- alguns limites configurados;
- reflexos indiretos em poucos indicadores.

## 15.5 Indicadores podem aparecer em mais de um pilar

Isso é intencional, mas exige cuidado na recalibração para não gerar reforço excessivo da mesma dimensão.

## 15.6 Penalizações críticas precisam evitar dupla punição

O projeto já trabalha com o princípio de que:

- um problema pode degradar o pilar;
- mas a penalidade crítica deve entrar apenas quando houver gravidade adicional claramente justificável.

## 15.7 Horizonte futuro usa apenas lançamentos pendentes

Isso é coerente para planejamento, mas influencia muito a leitura de pressão futura.

Se a base de lançamentos futuros estiver incompleta, o pilar pode parecer artificialmente melhor.

---

## 16. O que uma IA de calibração deve analisar

Se outra IA for ajudar a evoluir o modelo, ela deve avaliar principalmente:

1. se os pesos dos pilares refletem corretamente risco financeiro pessoal;
2. se os pesos dos indicadores estão equilibrados;
3. se as faixas de status são granulares o suficiente;
4. se as penalizações críticas estão proporcionais;
5. se existe dupla penalização implícita;
6. se a inadimplência precisa deixar de ser binária;
7. se o pilar `Planejamento e Disciplina` precisa ser reformulado;
8. se a escala atual `100 / 80 / 55 / 25` é adequada;
9. se o score final está mais próximo de “risco” ou de “maturidade financeira”;
10. se as fórmulas estão coerentes com o tipo de métrica que pretendem medir.

---

## 17. Perguntas úteis para a IA que vai nos ajudar

- Os pesos dos pilares estão coerentes com um modelo de risco financeiro pessoal?
- Há indicadores com peso excessivo ou insuficiente?
- A regra binária de inadimplência deve ser substituída por faixas graduais?
- O pilar `Planejamento e Disciplina` deveria continuar baseado em proxies?
- A nota por status (`100/80/55/25`) é adequada ou deveria ter maior separação?
- O horizonte futuro de 30 dias merece peso ainda maior do que os demais?
- O modelo atual pune demais ou de menos patrimônio líquido negativo?
- O indicador `Economia Mensal` deveria ter meta em moeda em vez de meta percentual?
- Como reduzir risco de dupla penalização entre pilar e penalidade crítica?
- Como aproximar o modelo de práticas de análise de risco e crédito sem perder explicabilidade?

---

## 18. Arquivos-fonte mais importantes para auditoria técnica

- `MinhasFinancas.Domain/Services/AnaliseFinanceira/IndicadoresFinanceirosService.cs`
- `MinhasFinancas.Domain/Services/AnaliseFinanceira/SaudeFinanceiraService.cs`
- `MinhasFinancas.Application/Services/MfScoreCalculoAppService.cs`
- `MinhasFinancas.Domain/Services/AnaliseFinanceira/Indicadores/*.cs`
- `docs/MF_SCORE.md`
- `docs/INDICADORES_FINANCEIROS.md`
- `docs/MF_SCORE_AUDIT.md`
- `docs/MF_SCORE_VALIDATION.md`
- `docs/MF_SCORE_REVIEW.md`

---

## 19. Conclusão

Hoje o `MF Score` já possui uma estrutura consistente:

- indicadores;
- pilares;
- penalizações críticas;
- histórico;
- tendência.

Porém, ele ainda está em fase de amadurecimento de calibragem.

Os principais pontos para revisão futura são:

- granularidade dos status;
- coerência entre fórmula e tipo de meta;
- severidade das penalizações;
- evolução da inadimplência;
- reformulação do pilar `Planejamento e Disciplina`.

Este documento deve ser usado como base oficial do “estado atual do cálculo” antes de qualquer proposta de recalibração.
