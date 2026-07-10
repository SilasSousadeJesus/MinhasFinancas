# Indicadores Financeiros Oficiais

Este documento registra as fórmulas, intenções e pesos oficiais da camada `AnaliseFinanceira`.

Ele é a referência principal sempre que um indicador mudar.

## Princípios oficiais

- Os indicadores são derivados apenas de dados já persistidos no sistema.
- A camada analítica não consulta interface.
- Indicadores ruins reduzem a nota dos pilares.
- Penalizações críticas não substituem os indicadores; elas só existem para risco grave, materializado ou persistente.
- O modelo oficial de risco financeiro é o `MF Score`.
- O `MF Score` final usa escala `0 a 1000`.
- Os pilares continuam em escala `0 a 100`.
- Um mesmo fato econômico não deve ser penalizado duplamente.

## Escala de status dos indicadores

Todos os indicadores usam a mesma escala de status:

- `Excelente` = 100
- `Bom` = 80
- `Atenção` = 55
- `Crítica` = 25

## Relação entre indicadores, pilares e MF Score

### Indicadores

- geram leitura individual
- alimentam os pilares

### Pilares

- consolidam indicadores por contexto financeiro
- usam escala `0 a 100`

### MF Score

- parte da média ponderada dos cinco pilares
- converte o resultado para escala `0 a 1000`
- só depois aplica penalizações críticas oficiais

## Pesos oficiais por indicador

- `EconomiaMensal` = `1.0`
- `PercentualEconomia` = `1.0`
- `ReservaEmergenciaAtual` = `1.5`
- `ReservaEmergenciaIdeal` = `0.5`
- `ComprometimentoRenda` = `1.5`
- `ComprometimentoFinanceiroFuturo` = `1.5`
- `ComprometimentoFinanceiroFuturo90Dias` = `1.0`
- `ComprometimentoFinanceiroFuturo180Dias` = `0.75`
- `ComprometimentoFinanceiroFuturo365Dias` = `0.5`
- `EndividamentoPatrimonial` = `1.5`
- `PatrimonioLiquidoAtual` = `1.25`
- `PercentualPatrimonioAlvo` = `0.75`

## Critério de pontuação dos indicadores

1. cada indicador contribui com sua nota de status convertida para valor numérico
2. cada indicador é multiplicado pelo peso oficial
3. o total ponderado é dividido pela soma dos pesos
4. o resultado compõe a leitura dos pilares
5. o conjunto dos pilares gera o `MF Score Base`
6. o `MF Score Base` é convertido para `0 a 1000`
7. somente depois entram as penalizações críticas oficiais

## Indicadores oficiais

### Economia mensal

- **Finalidade:** mostrar a sobra mensal entre receitas e despesas do mês de referência.
- **Fórmula:** `receitaMensalAtual - despesaMensalAtual`
- **Meta monetária oficial:** `receitaMensalAtual * (percentualEconomiaMensalDesejado / 100)`
- **Fonte:** lançamentos do mês de referência.
- **Formato:** moeda.
- **Leitura:** quanto maior a sobra, melhor a capacidade de planejamento e proteção operacional.
- **Observação importante:** o indicador não compara mais moeda com meta percentual; a meta é convertida para valor monetário do próprio mês.

### Percentual de economia

- **Finalidade:** mostrar qual parte da renda virou economia real.
- **Fórmula:** `(economiaMensalAtual / receitaMensalAtual) * 100`
- **Fonte:** lançamentos do mês de referência.
- **Formato:** percentual.
- **Leitura:** mede eficiência da renda, não apenas seu tamanho absoluto.
- **Faixas oficiais atuais:**
  - `Excelente`: `>= 20%`
  - `Bom`: `>= 10% e < 20%`
  - `Atenção`: `>= 0% e < 10%`
  - `Crítica`: `< 0%`

### Reserva de emergência atual

- **Finalidade:** mostrar a proteção financeira disponível para imprevistos.
- **Fórmula:** soma dos ativos líquidos classificados como dinheiro em conta ou investimento.
- **Fonte:** bens patrimoniais do usuário.
- **Formato:** moeda.
- **Leitura:** reduz o pilar de liquidez quando é baixa, mas não deve gerar penalização crítica automática apenas por estar zerada.
- **Faixas oficiais atuais** com base em `CoberturaReservaEmMeses`:
  - `Excelente`: `>= 6 meses`
  - `Bom`: `>= 4 e < 6 meses`
  - `Atenção`: `>= 2 e < 4 meses`
  - `Crítica`: `< 2 meses`

### Reserva de emergência ideal

- **Finalidade:** mostrar a meta ideal configurada pelo próprio usuário.
- **Fórmula:** `despesaMensalAtual * mesesDesejados * percentualDesejado`
- **Fonte:** lançamentos e configuração vigente do perfil financeiro.
- **Formato:** moeda.
- **Leitura:** funciona como régua pessoal e como lembrete quando a meta não está configurada.

### Comprometimento da renda

- **Finalidade:** medir quanto da renda mensal já está comprometido com despesas do mês.
- **Fórmula:** `(despesaMensalAtual / receitaMensalAtual) * 100`
- **Fallback quando a renda é zero e existem despesas:** `100%`
- **Fonte:** lançamentos do mês de referência.
- **Formato:** percentual.
- **Leitura:** afeta principalmente o pilar de fluxo de caixa, sem gerar penalização crítica automática só por estar alto.
- **Faixas oficiais atuais:**
  - `Excelente`: `<= 20%`
  - `Bom`: `> 20% e <= 35%`
  - `Atenção`: `> 35% e <= 50%`
  - `Crítica`: `> 50%`

### Comprometimento financeiro futuro

- **Finalidade:** medir quanto da renda prevista para os próximos 30 dias já está comprometido com obrigações.
- **Fórmula:** `(obrigacoesFinanceirasFuturas30Dias / receitaPrevista30Dias) * 100`
- **Fallback quando a renda é zero e existem obrigações futuras:** `100%`
- **Fonte:** lançamentos pendentes no horizonte de 30 dias.
- **Formato:** percentual.
- **Leitura:** mostra a folga do curto prazo.
- **Campos de transparência:** `ValorObrigacoesPrevistas`, `ValorReceitaPrevista` e `PercentualComprometimento`.
- **Faixas oficiais atuais:**
  - `Excelente`: `<= 25%`
  - `Bom`: `> 25% e <= 40%`
  - `Atenção`: `> 40% e <= 55%`
  - `Crítica`: `> 55%`

### Pressão financeira acumulada - 90 dias

- **Finalidade:** medir pressão financeira do trimestre.
- **Fórmula:** `(obrigacoesFinanceirasFuturas90Dias / receitaPrevista90Dias) * 100`
- **Fallback quando a renda é zero e existem obrigações futuras:** `100%`
- **Fonte:** lançamentos pendentes no horizonte de 90 dias.
- **Formato:** percentual.
- **Leitura:** complementa a visão de curto prazo, mas não deve gerar penalização crítica automática isoladamente.

### Pressão financeira acumulada - 180 dias

- **Finalidade:** medir pressão financeira acumulada do médio prazo.
- **Fórmula:** `(obrigacoesFinanceirasFuturas180Dias / receitaPrevista180Dias) * 100`
- **Fallback quando a renda é zero e existem obrigações futuras:** `100%`
- **Fonte:** lançamentos pendentes no horizonte de 180 dias.
- **Formato:** percentual.
- **Leitura:** ajuda a identificar deterioração estrutural em formação.

### Pressão financeira acumulada - 12 meses

- **Finalidade:** medir a pressão financeira acumulada do horizonte anual.
- **Fórmula:** `(obrigacoesFinanceirasFuturas365Dias / receitaPrevista365Dias) * 100`
- **Fallback quando a renda é zero e existem obrigações futuras:** `100%`
- **Fonte:** lançamentos pendentes no horizonte de 365 dias.
- **Formato:** percentual.
- **Leitura:** mostra sustentabilidade do longo prazo.

### Endividamento patrimonial

- **Finalidade:** medir o peso dos passivos sobre a base patrimonial ativa.
- **Fórmula:** `(totalPassivos / totalAtivos) * 100`
- **Fallback quando não há ativos e existem passivos:** `100%`
- **Fonte:** bens patrimoniais e passivos patrimoniais.
- **Formato:** percentual.
- **Leitura:** afeta o pilar de endividamento; não deve ser automaticamente uma penalização crítica sem evidência de risco materializado.
- **Faixas oficiais atuais:**
  - `Excelente`: `<= 15%`
  - `Bom`: `> 15% e <= 30%`
  - `Atenção`: `> 30% e <= 50%`
  - `Crítica`: `> 50%`

### Patrimônio líquido atual

- **Finalidade:** mostrar a diferença entre ativos e passivos.
- **Fórmula:** `totalAtivos - totalPassivos`
- **Fonte:** bens patrimoniais e passivos patrimoniais.
- **Formato:** moeda.
- **Leitura:** patrimônio líquido negativo pode justificar penalização crítica.

### Percentual do patrimônio alvo

- **Finalidade:** mostrar o avanço atual em relação ao patrimônio alvo configurado.
- **Fórmula:** `(patrimonioLiquidoAtual / patrimonioAlvo) * 100`
- **Fonte:** patrimônio líquido atual e configuração vigente do perfil financeiro.
- **Formato:** percentual.
- **Leitura:** quando não há patrimônio alvo configurado, o indicador continua visível como régua de planejamento.

## Regras de interpretação

- Indicadores com status `Atenção` ou `Crítica` alimentam pontos de atenção e insights.
- Indicadores com status `Excelente` ou `Bom` podem gerar destaques positivos.
- Indicadores de configuração ausente funcionam como maturidade analítica, não como crise financeira absoluta.
- O curto prazo deve ser distinguido da pressão financeira acumulada em horizontes maiores.
- O `MF Score` não deve aplicar dupla penalização sobre o mesmo fato econômico.

## Penalizações críticas e indicadores

As penalizações críticas não substituem a leitura dos indicadores.

Na versão atual, elas devem focar em:

- inadimplência
- fluxo mensal negativo
- meses consecutivos no vermelho
- patrimônio líquido negativo
- ausência de dados essenciais

### Matriz oficial de inadimplência

A inadimplência deixou de ser binária e passa a usar níveis graduais com base em:

- `DiasAtraso`
- `PercentualValorEmAtrasoSobreRenda`

Regras atuais:

- `Nível 1 - Atraso técnico`
  - até `7 dias` de atraso
  - e valor em atraso `< 10%` da renda
  - penalidade: `30 pontos` no score final
- `Nível 2 - Estresse moderado`
  - `8 a 30 dias`
  - ou valor em atraso entre `10% e 25%` da renda
  - penalidade: `90 pontos` no score final
- `Nível 3 - Inadimplência relevante`
  - `31 a 60 dias`
  - ou valor em atraso entre `25% e 50%` da renda
  - penalidade: `170 pontos` no score final
- `Nível 4 - Inadimplência grave`
  - acima de `60 dias`
  - ou valor em atraso `> 50%` da renda
  - penalidade: `250 pontos` no score final

Quando tempo e materialidade caem em níveis diferentes, prevalece o nível mais grave.

Os seguintes fatores devem permanecer prioritariamente na camada dos pilares:

- reserva baixa
- comprometimento alto
- pressão futura
- pressão financeira acumulada

## Relação com as telas

- **Dashboard:** consome apenas resumo consolidado
- **Saúde Financeira:** exibe indicadores e `MF Score`
- **Assistente Financeiro:** consome resumo consolidado e leitura executiva
- **Personas de calibração:** rodam o mesmo motor oficial
- **Auditoria do MF Score:** valida o comportamento do motor contra cenários esperados

## Calibração do MF Score

Mudanças nesses indicadores não devem ser feitas apenas por ajuste numérico.

Cada revisão deve responder se a alteração:

- melhora a capacidade do `MF Score` representar o risco financeiro real
- evita dupla penalização
- mantém coerência com as personas e com a auditoria oficial
- continua respeitando a separação entre indicador ruim e penalização crítica

Toda alteração relevante também deve ser confrontada com:

- `docs/MF_SCORE.md`
- `docs/MF_SCORE_VALIDATION.md`
- `docs/MF_SCORE_AUDIT.md`

## Regra de manutenção

Sempre que uma fórmula, peso, classificação, regra de pilar, penalização crítica ou texto oficial mudar, este documento deve ser atualizado na mesma entrega.
