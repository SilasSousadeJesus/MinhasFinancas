# Indicadores Financeiros Oficiais

Este documento registra a referência oficial da camada `AnaliseFinanceira`.

Ele explica:

- o que cada indicador mede;
- como cada leitura se conecta aos pilares;
- quais mudanças conceituais já foram consolidadas;
- quais regras devem permanecer estáveis para futuras calibrações.

## Princípios oficiais

- Os indicadores derivam apenas de dados já persistidos.
- Indicador ruim reduz pilar antes de virar penalização crítica.
- A camada analítica não depende da interface.
- O `MF Score` continua sendo o modelo oficial de saúde financeira com apetite de risco `moderado`.
- O score final usa escala `0 a 1000`.
- Os pilares permanecem em `0 a 100`.
- Um mesmo fato econômico não deve sofrer dupla penalização.

## Escala de status

Todos os indicadores usam a mesma escala de status:

- `Excelente` = `100`
- `Bom` = `80`
- `Atenção` = `55`
- `Crítico` = `25`

## Relação entre indicadores, pilares e score

### Indicadores

- produzem leituras específicas
- alimentam pilares

### Pilares

- consolidam risco por contexto financeiro
- usam escala `0 a 100`

### MF Score

- nasce da média ponderada dos cinco pilares
- é convertido para `0 a 1000`
- só depois recebe penalizações críticas e persistência temporal

## Pesos oficiais por indicador

- `EconomiaMensal` = `1.0`
- `PercentualEconomia` = `1.0`
- `ReservaEmergenciaAtual` = `1.5`
- `ReservaEmergenciaIdeal` = `0.5`
- `ComprometimentoRenda` = `1.5`
- `ComprometimentoFinanceiroFuturo30Dias` = `1.5`
- `ComprometimentoFinanceiroFuturo90Dias` = `0.75`
- `ComprometimentoFinanceiroFuturo180Dias` = `0.5`
- `ComprometimentoFinanceiroFuturo365Dias` = `0.25`
- `Endividamento` = `1.5`
- `PatrimonioLiquidoAtual` = `1.25`
- `PercentualPatrimonioAlvo` = `0.75`
- `CapacidadeFormacaoReserva` = indicador auxiliar do pilar `Liquidez e Reserva`

## Indicadores oficiais

### Economia Mensal

- **Finalidade:** medir a sobra operacional do mês.
- **Fórmula:** `receitaMensalAtual - despesaMensalAtual`
- **Meta monetária oficial:** `receitaMensalAtual * (percentualEconomiaMensalDesejado / 100)`
- **Formato:** moeda
- **Pilar principal:** `Fluxo de Caixa`
- **Leitura oficial:** mostra se o mês fecha com folga, no limite ou no vermelho.

### Percentual de Economia

- **Finalidade:** medir qual parte da renda virou economia real.
- **Fórmula:** `(economiaMensalAtual / receitaMensalAtual) * 100`
- **Formato:** percentual
- **Pilar principal:** `Fluxo de Caixa`
- **Faixas oficiais:**
  - `Excelente`: `>= 20%`
  - `Bom`: `>= 10% e < 20%`
  - `Atenção`: `>= 0% e < 10%`
  - `Crítico`: `< 0%`

### Comprometimento da Renda

- **Finalidade:** medir quanta renda do mês já está comprometida por despesas correntes.
- **Fórmula:** `(despesaMensalAtual / receitaMensalAtual) * 100`
- **Fallback:** se a renda for zero e houver despesas, considera `100%`
- **Formato:** percentual
- **Pilar principal:** `Fluxo de Caixa`
- **Posição conceitual oficial:** continua sendo leitura operacional do mês, não eixo principal de planejamento e nem sinônimo de dívida.
- **Faixas oficiais:**
  - `Excelente`: `<= 55%`
  - `Bom`: `> 55% e <= 75%`
  - `Atenção`: `> 75% e <= 95%`
  - `Crítico`: `> 95%`

### Reserva de Emergência Atual

- **Finalidade:** medir a proteção imediata contra imprevistos.
- **Fórmula:** soma dos ativos líquidos classificados como dinheiro em conta ou investimento.
- **Formato:** moeda
- **Pilar principal:** `Liquidez e Reserva`
- **Faixas oficiais por cobertura em meses:**
  - `Excelente`: `>= 6 meses`
  - `Bom`: `>= 4 e < 6 meses`
  - `Atenção`: `>= 2 e < 4 meses`
  - `Crítico`: `< 2 meses`

### Reserva de Emergência Ideal

- **Finalidade:** representar a meta pessoal de proteção configurada no perfil financeiro.
- **Fórmula:** `despesaMensalAtual * mesesDesejados * percentualDesejado`
- **Formato:** moeda
- **Pilar principal:** `Liquidez e Reserva`

### Capacidade de Formação de Reserva

- **Finalidade:** estimar em quanto tempo a reserva ideal poderia ser formada no ritmo atual.
- **Base de cálculo:** sobra mensal disponível em relação à reserva ideal faltante.
- **Pilar principal:** `Liquidez e Reserva`
- **Regra de apresentação:** quando o fluxo atual não permite projeção realista, a interface não deve exibir valores técnicos como `999 meses`; deve comunicar que a formação não é projetável no ritmo atual.

### Comprometimento Financeiro Futuro - 30 dias

- **Finalidade:** medir a pressão operacional do próximo ciclo.
- **Fórmula:** `(obrigacoesFinanceirasFuturas30Dias / receitaPrevista30Dias) * 100`
- **Formato:** percentual
- **Pilar principal:** `Endividamento e Obrigações`
- **Campos auxiliares:** `ValorObrigacoesPrevistas`, `ValorReceitaPrevista`, `PercentualComprometimento`
- **Faixas oficiais:**
  - `Excelente`: `<= 60%`
  - `Bom`: `> 60% e <= 85%`
  - `Atenção`: `> 85% e <= 105%`
  - `Crítico`: `> 105%`

### Pressão Financeira Acumulada - 90 dias

- **Finalidade:** medir pressão trimestral acumulada.
- **Fórmula:** `(obrigacoesFinanceirasFuturas90Dias / receitaPrevista90Dias) * 100`
- **Formato:** percentual
- **Pilar principal:** `Endividamento e Obrigações`
- **Leitura oficial:** complementa o curto prazo, sem substituí-lo.
- **Faixas oficiais:**
  - `Excelente`: `<= 70%`
  - `Bom`: `> 70% e <= 95%`
  - `Atenção`: `> 95% e <= 110%`
  - `Crítico`: `> 110%`

### Pressão Financeira Acumulada - 180 dias

- **Finalidade:** medir pressão estrutural do médio prazo.
- **Fórmula:** `(obrigacoesFinanceirasFuturas180Dias / receitaPrevista180Dias) * 100`
- **Formato:** percentual
- **Pilar principal:** `Endividamento e Obrigações`
- **Regra qualitativa oficial:** percentuais acima de `100%` não podem ficar apenas em `Atenção`.
- **Faixas oficiais:**
  - `Excelente`: `<= 80%`
  - `Bom`: `> 80% e <= 105%`
  - `Atenção`: `> 105% e <= 120%`
  - `Crítico`: `> 120%`

### Pressão Financeira Acumulada - 365 dias

- **Finalidade:** medir sustentabilidade do horizonte anual.
- **Fórmula:** `(obrigacoesFinanceirasFuturas365Dias / receitaPrevista365Dias) * 100`
- **Formato:** percentual
- **Pilar principal:** `Endividamento e Obrigações`
- **Regra qualitativa oficial:** percentuais acima de `100%` não podem ficar apenas em `Atenção`.
- **Faixas oficiais:**
  - `Excelente`: `<= 85%`
  - `Bom`: `> 85% e <= 110%`
  - `Atenção`: `> 110% e <= 130%`
  - `Crítico`: `> 130%`

### Exposição a Dívidas e Passivos

- **Finalidade:** medir o peso combinado de passivos e dívidas sobre a estrutura patrimonial.
- **Base conceitual oficial:** o indicador separa:
  - dívidas de consumo
  - financiamentos patrimoniais
  - obrigações estruturais
- **Regra importante:** financiamento patrimonial não deve ter a mesma severidade de dívida de consumo.
- **Leitura atual:** a composição usa pesos diferentes por natureza de passivo, preservando explicabilidade e apetite de risco moderado.
- **Pilar principal:** `Endividamento e Obrigações`
- **Faixas oficiais:**
  - `Excelente`: `<= 15%`
  - `Bom`: `> 15% e <= 30%`
  - `Atenção`: `> 30% e <= 50%`
  - `Crítico`: `> 50%`

## Nota oficial da `v2.5`

A versão `mf-score-v2.5-1000` não criou novos indicadores. Ela recalibrou a sensibilidade numérica dos indicadores já existentes para:

- reduzir falso negativo em perfis organizados com pouca folga operacional;
- separar melhor dívida organizada de ruptura financeira;
- evitar colapso prematuro da base da escala em cenários ainda recuperáveis.

### Patrimônio Líquido Atual

- **Finalidade:** medir a situação patrimonial real do usuário.
- **Fórmula:** `totalAtivos - totalPassivos`
- **Leitura principal complementar:** proporção do patrimônio líquido sobre a base de ativos.
- **Pilar principal:** `Patrimônio`
- **Regra conceitual oficial:** este é o centro do pilar patrimonial.

### Percentual do Patrimônio-Alvo

- **Finalidade:** medir avanço rumo ao objetivo patrimonial configurado.
- **Fórmula:** `(patrimonioLiquidoAtual / patrimonioAlvo) * 100`
- **Pilar principal:** `Patrimônio`
- **Regra conceitual oficial:** serve como sinal de evolução, não como base principal da fotografia patrimonial.

## Regras estruturais dos pilares

### Fluxo de Caixa

O pilar deve responder principalmente:

- o mês fecha positivo?
- fecha negativo?
- fecha com folga real?

Por isso, sua leitura privilegia:

- `Economia Mensal`
- `Percentual de Economia`
- `Comprometimento da Renda`

### Endividamento e Obrigações

O pilar deve separar:

- dívida de consumo;
- financiamento patrimonial;
- pressão futura recorrente;
- inadimplência.

### Patrimônio

O pilar deve ser guiado principalmente por:

- ativos;
- passivos;
- patrimônio líquido.

O patrimônio-alvo é complementar.

### Planejamento e Disciplina

O pilar deve combinar:

- base mínima configurada no perfil financeiro;
- sinais reais de execução e consistência;
- plano estratégico e compromissos apenas quando existirem.

## Penalizações críticas e indicadores

As penalizações críticas não substituem os indicadores.

Na versão atual, elas devem focar em:

- inadimplência
- reincidência ou cura recente
- persistência de fluxo negativo
- patrimônio líquido negativo
- ausência de dados essenciais

### Regra temporal oficial

A persistência de fluxo negativo agora substitui a penalização simples, em vez de somar com ela.

O motor aplica apenas o nível mais grave encontrado:

- `1 mês`
- `2 meses`
- `3+ meses`
- `6+ meses`
- `12+ meses`

### Regra de inadimplência

A inadimplência continua gradual e considera:

- dias de atraso;
- materialidade do valor vencido sobre a renda.

## Regra sobre projeção de receitas futuras

As receitas recorrentes precisam ser projetadas corretamente nos horizontes futuros, especialmente em:

- `180 dias`
- `365 dias`

Essa projeção não deve subestimar a receita futura nem inflar artificialmente a pressão acumulada.

## Relação com as telas

- **Saúde Financeira:** exibe os indicadores completos
- **Dashboard:** consome apenas síntese
- **Assistente Financeiro:** usa leitura executiva derivada do mesmo núcleo
- **Laboratório do MF Score:** inspeciona o motor oficial sem recalcular regras por fora
