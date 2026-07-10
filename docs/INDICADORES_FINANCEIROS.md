# Indicadores Financeiros Oficiais

Este documento registra as fÃ³rmulas, intenÃ§Ãµes e pesos oficiais da camada `AnaliseFinanceira`.

Ele Ã© a referÃªncia principal sempre que um indicador mudar.

## PrincÃ­pios oficiais

- Os indicadores sÃ£o derivados apenas de dados jÃ¡ persistidos no sistema.
- A camada analÃ­tica nÃ£o consulta interface.
- Indicadores ruins reduzem a nota dos pilares.
- PenalizaÃ§Ãµes crÃ­ticas nÃ£o substituem os indicadores; elas sÃ³ existem para risco grave, materializado ou persistente.
- O modelo oficial de risco financeiro Ã© o `MF Score`.
- O `MF Score` final usa escala `0 a 1000`.
- Os pilares continuam em escala `0 a 100`.
- Um mesmo fato econÃ´mico nÃ£o deve ser penalizado duplamente.

## Escala de status dos indicadores

Todos os indicadores usam a mesma escala de status:

- `Excelente` = 100
- `Bom` = 80
- `AtenÃ§Ã£o` = 55
- `CrÃ­tica` = 25

## RelaÃ§Ã£o entre indicadores, pilares e MF Score

### Indicadores

- geram leitura individual
- alimentam os pilares

### Pilares

- consolidam indicadores por contexto financeiro
- usam escala `0 a 100`

### MF Score

- parte da mÃ©dia ponderada dos cinco pilares
- converte o resultado para escala `0 a 1000`
- sÃ³ depois aplica penalizaÃ§Ãµes crÃ­ticas oficiais

## Pesos oficiais por indicador

- `EconomiaMensal` = `1.0`
- `PercentualEconomia` = `1.0`
- `ReservaEmergenciaAtual` = `1.5`
- `ReservaEmergenciaIdeal` = `0.5`
- `ComprometimentoRenda` = `1.5`
- `ComprometimentoFinanceiroFuturo` = `1.5`
- `ComprometimentoFinanceiroFuturo90Dias` = `0.75`
- `ComprometimentoFinanceiroFuturo180Dias` = `0.5`
- `ComprometimentoFinanceiroFuturo365Dias` = `0.25`
- `EndividamentoPatrimonial` = `1.5`
- `PatrimonioLiquidoAtual` = `1.25`
- `PercentualPatrimonioAlvo` = `0.75`

## CritÃ©rio de pontuaÃ§Ã£o dos indicadores

1. cada indicador contribui com sua nota de status convertida para valor numÃ©rico
2. cada indicador Ã© multiplicado pelo peso oficial
3. o total ponderado Ã© dividido pela soma dos pesos
4. o resultado compÃµe a leitura dos pilares
5. o conjunto dos pilares gera o `MF Score Base`
6. o `MF Score Base` Ã© convertido para `0 a 1000`
7. somente depois entram as penalizaÃ§Ãµes crÃ­ticas oficiais

## Indicadores oficiais

### Economia mensal

- **Finalidade:** mostrar a sobra mensal entre receitas e despesas do mÃªs de referÃªncia.
- **FÃ³rmula:** `receitaMensalAtual - despesaMensalAtual`
- **Meta monetÃ¡ria oficial:** `receitaMensalAtual * (percentualEconomiaMensalDesejado / 100)`
- **Fonte:** lanÃ§amentos do mÃªs de referÃªncia.
- **Formato:** moeda.
- **Leitura:** quanto maior a sobra, melhor a capacidade de planejamento e proteÃ§Ã£o operacional.
- **ObservaÃ§Ã£o importante:** o indicador nÃ£o compara mais moeda com meta percentual; a meta Ã© convertida para valor monetÃ¡rio do prÃ³prio mÃªs.

### Percentual de economia

- **Finalidade:** mostrar qual parte da renda virou economia real.
- **FÃ³rmula:** `(economiaMensalAtual / receitaMensalAtual) * 100`
- **Fonte:** lanÃ§amentos do mÃªs de referÃªncia.
- **Formato:** percentual.
- **Leitura:** mede eficiÃªncia da renda, nÃ£o apenas seu tamanho absoluto.
- **Faixas oficiais atuais:**
  - `Excelente`: `>= 20%`
  - `Bom`: `>= 10% e < 20%`
  - `AtenÃ§Ã£o`: `>= 0% e < 10%`
  - `CrÃ­tica`: `< 0%`

### Reserva de emergÃªncia atual

- **Finalidade:** mostrar a proteÃ§Ã£o financeira disponÃ­vel para imprevistos.
- **FÃ³rmula:** soma dos ativos lÃ­quidos classificados como dinheiro em conta ou investimento.
- **Fonte:** bens patrimoniais do usuÃ¡rio.
- **Formato:** moeda.
- **Leitura:** reduz o pilar de liquidez quando Ã© baixa, mas nÃ£o deve gerar penalizaÃ§Ã£o crÃ­tica automÃ¡tica apenas por estar zerada.
- **Faixas oficiais atuais** com base em `CoberturaReservaEmMeses`:
  - `Excelente`: `>= 6 meses`
  - `Bom`: `>= 4 e < 6 meses`
  - `AtenÃ§Ã£o`: `>= 2 e < 4 meses`
  - `CrÃ­tica`: `< 2 meses`

### Reserva de emergÃªncia ideal

- **Finalidade:** mostrar a meta ideal configurada pelo prÃ³prio usuÃ¡rio.
- **FÃ³rmula:** `despesaMensalAtual * mesesDesejados * percentualDesejado`
- **Fonte:** lanÃ§amentos e configuraÃ§Ã£o vigente do perfil financeiro.
- **Formato:** moeda.
- **Leitura:** funciona como rÃ©gua pessoal e como lembrete quando a meta nÃ£o estÃ¡ configurada.

### Comprometimento da renda

- **Finalidade:** medir quanto da renda mensal jÃ¡ estÃ¡ comprometido com despesas do mÃªs.
- **FÃ³rmula:** `(despesaMensalAtual / receitaMensalAtual) * 100`
- **Fallback quando a renda Ã© zero e existem despesas:** `100%`
- **Fonte:** lanÃ§amentos do mÃªs de referÃªncia.
- **Formato:** percentual.
- **Leitura:** afeta principalmente o pilar de fluxo de caixa, sem gerar penalizaÃ§Ã£o crÃ­tica automÃ¡tica sÃ³ por estar alto.
- **PosiÃ§Ã£o conceitual oficial:** permanece como indicador primÃ¡rio de `Fluxo de Caixa`, e nÃ£o como medida principal de `Planejamento e Disciplina`.
- **Faixas oficiais atuais:**
  - `Excelente`: `<= 20%`
  - `Bom`: `> 20% e <= 35%`
  - `AtenÃ§Ã£o`: `> 35% e <= 50%`
  - `CrÃ­tica`: `> 50%`

### Comprometimento financeiro futuro

- **Finalidade:** medir quanto da renda prevista para os prÃ³ximos 30 dias jÃ¡ estÃ¡ comprometido com obrigaÃ§Ãµes.
- **FÃ³rmula:** `(obrigacoesFinanceirasFuturas30Dias / receitaPrevista30Dias) * 100`
- **Fallback quando a renda Ã© zero e existem obrigaÃ§Ãµes futuras:** `100%`
- **Fonte:** lanÃ§amentos pendentes no horizonte de 30 dias.
- **Formato:** percentual.
- **Leitura:** mostra a folga do curto prazo.
- **Campos de transparÃªncia:** `ValorObrigacoesPrevistas`, `ValorReceitaPrevista` e `PercentualComprometimento`.
- **Faixas oficiais atuais:**
  - `Excelente`: `<= 25%`
  - `Bom`: `> 25% e <= 40%`
  - `AtenÃ§Ã£o`: `> 40% e <= 55%`
  - `CrÃ­tica`: `> 55%`

### PressÃ£o financeira acumulada - 90 dias

- **Finalidade:** medir pressÃ£o financeira do trimestre.
- **FÃ³rmula:** `(obrigacoesFinanceirasFuturas90Dias / receitaPrevista90Dias) * 100`
- **Fallback quando a renda Ã© zero e existem obrigaÃ§Ãµes futuras:** `100%`
- **Fonte:** lanÃ§amentos pendentes no horizonte de 90 dias.
- **Formato:** percentual.
- **Leitura:** complementa a visÃ£o de curto prazo, mas nÃ£o deve gerar penalizaÃ§Ã£o crÃ­tica automÃ¡tica isoladamente.
- **Peso oficial atual:** menor que o horizonte de 30 dias, porque o curto prazo continua sendo a referÃªncia principal de pressÃ£o operacional.

### PressÃ£o financeira acumulada - 180 dias

- **Finalidade:** medir pressÃ£o financeira acumulada do mÃ©dio prazo.
- **FÃ³rmula:** `(obrigacoesFinanceirasFuturas180Dias / receitaPrevista180Dias) * 100`
- **Fallback quando a renda Ã© zero e existem obrigaÃ§Ãµes futuras:** `100%`
- **Fonte:** lanÃ§amentos pendentes no horizonte de 180 dias.
- **Formato:** percentual.
- **Leitura:** ajuda a identificar deterioraÃ§Ã£o estrutural em formaÃ§Ã£o.
- **Peso oficial atual:** reduzido para evitar que horizontes mÃ©dios dominem a leitura do risco imediato.

### PressÃ£o financeira acumulada - 12 meses

- **Finalidade:** medir a pressÃ£o financeira acumulada do horizonte anual.
- **FÃ³rmula:** `(obrigacoesFinanceirasFuturas365Dias / receitaPrevista365Dias) * 100`
- **Fallback quando a renda Ã© zero e existem obrigaÃ§Ãµes futuras:** `100%`
- **Fonte:** lanÃ§amentos pendentes no horizonte de 365 dias.
- **Formato:** percentual.
- **Leitura:** mostra sustentabilidade do longo prazo.
- **Peso oficial atual:** o menor entre os horizontes, funcionando como apoio estrutural e nÃ£o como principal driver do score.

### Endividamento patrimonial

- **Finalidade:** medir o peso dos passivos sobre a base patrimonial ativa.
- **FÃ³rmula:** `(totalPassivos / totalAtivos) * 100`
- **Fallback quando nÃ£o hÃ¡ ativos e existem passivos:** `100%`
- **Fonte:** bens patrimoniais e passivos patrimoniais.
- **Formato:** percentual.
- **Leitura:** afeta o pilar de endividamento; nÃ£o deve ser automaticamente uma penalizaÃ§Ã£o crÃ­tica sem evidÃªncia de risco materializado.
- **Faixas oficiais atuais:**
  - `Excelente`: `<= 15%`
  - `Bom`: `> 15% e <= 30%`
  - `AtenÃ§Ã£o`: `> 30% e <= 50%`
  - `CrÃ­tica`: `> 50%`

### PatrimÃ´nio lÃ­quido atual

- **Finalidade:** mostrar a diferenÃ§a entre ativos e passivos.
- **FÃ³rmula:** `totalAtivos - totalPassivos`
- **Fonte:** bens patrimoniais e passivos patrimoniais.
- **Formato:** moeda.
- **Leitura:** patrimÃ´nio lÃ­quido negativo pode justificar penalizaÃ§Ã£o crÃ­tica.

### Percentual do patrimÃ´nio alvo

- **Finalidade:** mostrar o avanÃ§o atual em relaÃ§Ã£o ao patrimÃ´nio alvo configurado.
- **FÃ³rmula:** `(patrimonioLiquidoAtual / patrimonioAlvo) * 100`
- **Fonte:** patrimÃ´nio lÃ­quido atual e configuraÃ§Ã£o vigente do perfil financeiro.
- **Formato:** percentual.
- **Leitura:** quando nÃ£o hÃ¡ patrimÃ´nio alvo configurado, o indicador continua visÃ­vel como rÃ©gua de planejamento.

## Regras de interpretaÃ§Ã£o

- Indicadores com status `AtenÃ§Ã£o` ou `CrÃ­tica` alimentam pontos de atenÃ§Ã£o e insights.
- Indicadores com status `Excelente` ou `Bom` podem gerar destaques positivos.
- Indicadores de configuraÃ§Ã£o ausente funcionam como maturidade analÃ­tica, nÃ£o como crise financeira absoluta.
- O curto prazo deve ser distinguido da pressÃ£o financeira acumulada em horizontes maiores.
- O `MF Score` nÃ£o deve aplicar dupla penalizaÃ§Ã£o sobre o mesmo fato econÃ´mico.

## PenalizaÃ§Ãµes crÃ­ticas e indicadores

As penalizaÃ§Ãµes crÃ­ticas nÃ£o substituem a leitura dos indicadores.

Na versÃ£o atual, elas devem focar em:

- inadimplÃªncia
- fluxo mensal negativo
- meses consecutivos no vermelho
- patrimÃ´nio lÃ­quido negativo
- ausÃªncia de dados essenciais

### Matriz oficial de inadimplÃªncia

A inadimplÃªncia deixou de ser binÃ¡ria e passa a usar nÃ­veis graduais com base em:

- `DiasAtraso`
- `PercentualValorEmAtrasoSobreRenda`

Regras atuais:

- `NÃ­vel 1 - Atraso tÃ©cnico`
  - atÃ© `7 dias` de atraso
  - e valor em atraso `< 10%` da renda
  - penalidade: `30 pontos` no score final
- `NÃ­vel 2 - Estresse moderado`
  - `8 a 30 dias`
  - ou valor em atraso entre `10% e 25%` da renda
  - penalidade: `90 pontos` no score final
- `NÃ­vel 3 - InadimplÃªncia relevante`
  - `31 a 60 dias`
  - ou valor em atraso entre `25% e 50%` da renda
  - penalidade: `170 pontos` no score final
- `NÃ­vel 4 - InadimplÃªncia grave`
  - acima de `60 dias`
  - ou valor em atraso `> 50%` da renda
  - penalidade: `250 pontos` no score final

Quando tempo e materialidade caem em nÃ­veis diferentes, prevalece o nÃ­vel mais grave.

Os seguintes fatores devem permanecer prioritariamente na camada dos pilares:

- reserva baixa
- comprometimento alto
- pressÃ£o futura
- pressÃ£o financeira acumulada

### PenalizaÃ§Ãµes temporais oficiais de fluxo negativo

As penalizaÃ§Ãµes temporais atuais do `MF Score` foram recalibradas para manter proporcionalidade com apetite de risco `moderado`:

- `1 mÃªs negativo`: `40 pontos` no score final
- `2 meses consecutivos negativos`: `90 pontos` no score final
- `3 ou mais meses consecutivos negativos`: `140 pontos` no score final

Objetivo:

- diferenciar alerta pontual de deterioraÃ§Ã£o recorrente;
- evitar que um Ãºnico mÃªs ruim produza colapso artificial do score;
- manter puniÃ§Ã£o forte quando o desequilÃ­brio vira padrÃ£o.

### Cura e reincidÃªncia da inadimplÃªncia

AlÃ©m da matriz principal de inadimplÃªncia, o motor passou a distinguir:

- `reincidÃªncia`: atraso atual com ocorrÃªncias recentes em meses diferentes;
- `cura recente`: atraso jÃ¡ regularizado, mas ainda recente.

Regras atuais:

- reincidÃªncia agrava a penalizaÃ§Ã£o da inadimplÃªncia atual;
- cura recente sem atraso pendente gera apenas penalidade residual leve;
- o objetivo Ã© nÃ£o tratar um usuÃ¡rio recÃ©m-regularizado como inadimplente ativo, mas tambÃ©m nÃ£o apagar o risco imediatamente.

## Pilar Planejamento e Disciplina

Na versÃ£o atual, o pilar `Planejamento e Disciplina` deixou de depender apenas de proxies genÃ©ricos e passou a considerar explicitamente a configuraÃ§Ã£o mÃ­nima do `Perfil Financeiro`.

### ParÃ¢metros bÃ¡sicos obrigatÃ³rios

O pilar sÃ³ pode ser considerado realmente saudÃ¡vel quando os cinco parÃ¢metros abaixo estiverem configurados:

- `PercentualEconomiaMensalDesejado`
- `PercentualReservaEmergenciaDesejado`
- `MesesReservaEmergenciaDesejados`
- `PercentualMaximoComprometimentoRenda`
- `PercentualMaximoEndividamento`

### Regra operacional atual

O cÃ¡lculo combina:

- nota de configuraÃ§Ã£o bÃ¡sica do perfil financeiro;
- sinais de execuÃ§Ã£o observados em:
  - `PercentualEconomia`
  - `ReservaEmergenciaAtual`
  - `PercentualPatrimonioAlvo`

Quando existirem, o pilar tambÃ©m passa a considerar sinais opcionais de execuÃ§Ã£o estratÃ©gica:

- `Plano EstratÃ©gico Financeiro` vigente
- objetivos estratÃ©gicos ativos, prioritÃ¡rios e concluÃ­dos
- `Compromissos Financeiros` em andamento
- `Compromissos Financeiros` concluÃ­dos e cancelados

Regra obrigatÃ³ria:

- se o usuÃ¡rio nÃ£o possuir plano estratÃ©gico vigente nem compromissos financeiros, esses elementos sÃ£o ignorados;
- a ausÃªncia deles nÃ£o reduz a nota do pilar;
- eles sÃ³ influenciam o cÃ¡lculo quando realmente existirem.

### Teto por quantidade de parÃ¢metros configurados

- `5 de 5`: teto `100`
- `4 de 5`: teto `75`
- `3 de 5`: teto `60`
- `2 de 5`: teto `45`
- `1 de 5`: teto `35`
- `0 de 5`: teto `30`

ConsequÃªncia prÃ¡tica:

- o usuÃ¡rio pode atÃ© ter boa execuÃ§Ã£o financeira parcial;
- mas nÃ£o alcanÃ§a nota alta em planejamento sem configurar o conjunto mÃ­nimo de referÃªncias do prÃ³prio plano.

## RelaÃ§Ã£o com as telas

- **Dashboard:** consome apenas resumo consolidado
- **SaÃºde Financeira:** exibe indicadores e `MF Score`
- **Assistente Financeiro:** consome resumo consolidado e leitura executiva
- **Personas de calibraÃ§Ã£o:** rodam o mesmo motor oficial
- **Auditoria do MF Score:** valida o comportamento do motor contra cenÃ¡rios esperados

## CalibraÃ§Ã£o do MF Score

MudanÃ§as nesses indicadores nÃ£o devem ser feitas apenas por ajuste numÃ©rico.

Cada revisÃ£o deve responder se a alteraÃ§Ã£o:

- melhora a capacidade do `MF Score` representar o risco financeiro real
- evita dupla penalizaÃ§Ã£o
- mantÃ©m coerÃªncia com as personas e com a auditoria oficial
- continua respeitando a separaÃ§Ã£o entre indicador ruim e penalizaÃ§Ã£o crÃ­tica

Toda alteraÃ§Ã£o relevante tambÃ©m deve ser confrontada com:

- `docs/MF_SCORE.md`
- `docs/MF_SCORE_VALIDATION.md`
- `docs/MF_SCORE_AUDIT.md`

## Regra de manutenÃ§Ã£o

Sempre que uma fÃ³rmula, peso, classificaÃ§Ã£o, regra de pilar, penalizaÃ§Ã£o crÃ­tica ou texto oficial mudar, este documento deve ser atualizado na mesma entrega.


## Perfil Financeiro Inicial

Os indicadores oficiais agora assumem que sempre existe uma configuração vigente do Perfil Financeiro.

Quando o usuário ainda não personalizou sua régua, a camada analítica passa a consumir automaticamente o Perfil Financeiro Inicial.

Isso reduz mensagens de ausência de meta ou limite quando o sistema já possui uma régua padrão válida.


## Atualizacao oficial v2.3

### Capacidade de Formacao de Reserva

- **Finalidade:** medir em quantos meses a sobra mensal atual conseguiria completar a reserva de emergencia ideal restante.
- **Formula:** `ReservaIdealRestante / EconomiaMensalAtual`, quando `EconomiaMensalAtual > 0`.
- **Reserva restante:** `max(0, ReservaEmergenciaIdeal - ReservaEmergenciaAtual)`.
- **Formato:** meses.
- **Faixas oficiais atuais:**
  - `Excelente`: `<= 3 meses`
  - `Bom`: `> 3 e <= 6 meses`
  - `Atencao`: `> 6 e <= 12 meses`
  - `Critica`: `> 12 meses` ou `economia mensal <= 0`
- **Leitura:** nao substitui a reserva atual. Funciona como atenuador da liquidez para evitar falso positivo de risco em usuarios iniciantes com fluxo muito forte.

### Regra adicional de patrimonio neutro

- Quando `total de ativos = 0`, `total de passivos = 0` e `patrimonio liquido = 0`, o sistema trata o caso como `ponto de partida patrimonial neutro`, e nao como insolvencia.
