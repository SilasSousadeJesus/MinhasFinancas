# Indicadores Financeiros Oficiais

Este documento registra as fórmulas, a intenção e os pesos oficiais da camada `AnaliseFinanceira`.

Ele é a referência principal sempre que um indicador mudar.

## Princípios oficiais

- Os indicadores são derivados de dados já persistidos no sistema.
- A camada analítica não consulta a interface.
- A pontuação da saúde financeira usa uma média ponderada dos indicadores.
- Indicadores de configuração continuam visíveis quando faltam parâmetros no perfil financeiro, mas não devem distorcer a leitura como se fossem crise financeira real.
- Compromissos futuros fazem parte da leitura analítica porque o sistema já possui lançamentos programados, parcelados e recorrentes.

## Escala de status

Todos os indicadores usam a mesma escala:

- `Excelente` = 100
- `Bom` = 80
- `Atenção` = 55
- `Crítica` = 25

## Pontuação da Saúde Financeira

A pontuação geral é calculada a partir dos indicadores disponíveis, usando pesos diferentes conforme o impacto de cada um na situação financeira do usuário.

### Peso atual por indicador

- `EconomiaMensal` = 1.0
- `PercentualEconomia` = 1.0
- `ReservaEmergenciaAtual` = 1.5
- `ReservaEmergenciaIdeal` = 0.5
- `ComprometimentoRenda` = 1.5
- `ComprometimentoFinanceiroFuturo` = 1.5
- `ComprometimentoFinanceiroFuturo90Dias` = 1.25
- `ComprometimentoFinanceiroFuturo180Dias` = 1.0
- `ComprometimentoFinanceiroFuturo365Dias` = 0.75
- `Endividamento patrimonial` = 1.5
- `PatrimonioLiquidoAtual` = 1.25
- `PercentualPatrimonioAlvo` = 0.75

### Critério da pontuação

1. cada indicador contribui com sua nota de status convertida para valor numérico;
2. cada indicador é multiplicado pelo seu peso oficial;
3. o total ponderado é dividido pela soma dos pesos;
4. o resultado é arredondado para a escala de 0 a 100;
5. a classificação textual é aplicada sobre essa pontuação.

## Indicadores oficiais

### Economia mensal

- **Finalidade:** mostrar a sobra mensal obtida entre receitas e despesas do mês atual.
- **Fórmula:** `receitaMensalAtual - despesaMensalAtual`
- **Fonte:** lançamentos do mês de referência.
- **Formato:** moeda.
- **Leitura:** quanto maior a sobra, melhor a capacidade de planejamento.

### Percentual de economia

- **Finalidade:** mostrar a parcela da renda que realmente virou economia no mês.
- **Fórmula:** `(economiaMensalAtual / receitaMensalAtual) * 100`
- **Fonte:** lançamentos do mês de referência.
- **Formato:** percentual.
- **Leitura:** mede disciplina de poupança.

### Reserva de emergência atual

- **Finalidade:** mostrar o valor já disponível para proteção de curto prazo.
- **Fórmula:** soma dos ativos líquidos classificados como dinheiro em conta ou investimento.
- **Fonte:** bens patrimoniais do usuário.
- **Formato:** moeda.
- **Leitura:** quanto maior a reserva, maior a proteção contra imprevistos.

### Reserva de emergência ideal

- **Finalidade:** mostrar a meta de reserva calculada a partir do perfil financeiro.
- **Fórmula:** `despesaMensalAtual * mesesDesejados * percentualDesejado`
- **Fonte:** lançamentos e configuração vigente do perfil financeiro.
- **Formato:** moeda.
- **Leitura:** se a meta não existir, o indicador funciona como lembrete de configuração.

### Comprometimento da renda

- **Finalidade:** medir quanto da renda mensal já está comprometido com despesas do mês atual.
- **Fórmula:** `(despesaMensalAtual / receitaMensalAtual) * 100`
- **Fallback quando a renda é zero e existem despesas:** considera 100% para não mascarar pressão financeira.
- **Fonte:** lançamentos do mês de referência.
- **Formato:** percentual.
- **Leitura:** indica a folga real do orçamento mensal.

### Comprometimento financeiro futuro

- **Finalidade:** medir a pressão dos próximos 30 dias sobre a renda disponível.
- **Fórmula:** `(obrigacoesFinanceirasFuturas30Dias / receitaMensalAtual) * 100`
- **Fallback quando a renda é zero e existem obrigações futuras:** considera 100% para não esconder risco.
- **Fonte:** lançamentos pendentes com vencimento entre a data de referência e os próximos 30 dias.
- **Formato:** percentual.
- **Leitura:** mostra a folga do caixa no curto prazo.

### Comprometimento financeiro futuro - 90 dias

- **Finalidade:** medir a pressão dos próximos 90 dias sobre a renda disponível.
- **Fórmula:** `(obrigacoesFinanceirasFuturas90Dias / receitaMensalAtual) * 100`
- **Fallback quando a renda é zero e existem obrigações futuras:** considera 100% para não esconder risco.
- **Fonte:** lançamentos pendentes com vencimento entre a data de referência e os próximos 90 dias.
- **Formato:** percentual.
- **Leitura:** complementa a visão de curto prazo com uma leitura de trimestre.

### Comprometimento financeiro futuro - 180 dias

- **Finalidade:** medir a pressão dos próximos 180 dias sobre a renda disponível.
- **Fórmula:** `(obrigacoesFinanceirasFuturas180Dias / receitaMensalAtual) * 100`
- **Fallback quando a renda é zero e existem obrigações futuras:** considera 100% para não esconder risco.
- **Fonte:** lançamentos pendentes com vencimento entre a data de referência e os próximos 180 dias.
- **Formato:** percentual.
- **Leitura:** ajuda a enxergar o médio prazo com mais antecedência.

### Comprometimento financeiro futuro - 12 meses

- **Finalidade:** medir a pressão dos próximos 12 meses sobre a renda disponível.
- **Fórmula:** `(obrigacoesFinanceirasFuturas365Dias / receitaMensalAtual) * 100`
- **Fallback quando a renda é zero e existem obrigações futuras:** considera 100% para não esconder risco.
- **Fonte:** lançamentos pendentes com vencimento entre a data de referência e os próximos 12 meses.
- **Formato:** percentual.
- **Leitura:** mostra se o longo prazo ainda preserva folga ou já pede revisão estrutural.

### Endividamento patrimonial

- **Finalidade:** medir o peso dos passivos patrimoniais em relação à base patrimonial ativa.
- **Fórmula:** `(totalPassivos / totalAtivos) * 100`
- **Fallback quando não há ativos e existem passivos:** considera 100%.
- **Fonte:** bens patrimoniais e passivos patrimoniais.
- **Formato:** percentual.
- **Leitura:** quanto menor, melhor a liberdade financeira.

### Patrimônio líquido atual

- **Finalidade:** mostrar a diferença entre ativos e passivos no momento atual.
- **Fórmula:** `totalAtivos - totalPassivos`
- **Fonte:** bens patrimoniais e passivos patrimoniais.
- **Formato:** moeda.
- **Leitura:** é a fotografia do patrimônio líquido atual.

### Percentual do patrimônio alvo

- **Finalidade:** mostrar o avanço atual em relação ao patrimônio alvo.
- **Fórmula:** `(patrimonioLiquidoAtual / patrimonioAlvo) * 100`
- **Fonte:** patrimônio líquido atual e configuração vigente do perfil financeiro.
- **Formato:** percentual.
- **Leitura:** quando não há patrimônio alvo configurado, o indicador continua visível como lembrete de configuração.

## Regras de interpretação

- Indicadores com status `Atenção` ou `Crítica` alimentam pontos de atenção e insights.
- Indicadores com status `Excelente` ou `Bom` podem gerar destaques positivos.
- Indicadores de configuração ausente devem aparecer como lembrete de régua pessoal, não como falha financeira absoluta.
- O indicador de comprometimento financeiro futuro existe para complementar o comprometimento da renda, não para substituí-lo.

## Relação com as telas

- **Dashboard:** consome o resumo da saúde financeira, sem recalcular fórmulas.
- **Saúde Financeira:** exibe o painel detalhado dos indicadores.
- **Assistente Financeiro:** consome o resumo consolidado e os indicadores para compor leitura executiva.
- **Insights Financeiros:** usa os indicadores para gerar alertas, oportunidades e destaques.

## Regra de manutenção

Sempre que uma fórmula, peso, classificação, prioridade ou texto oficial mudar, este documento deve ser atualizado na mesma entrega.
