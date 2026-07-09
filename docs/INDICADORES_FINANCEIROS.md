# Indicadores Financeiros Oficiais

Este documento registra as fórmulas, a intenção e os pesos oficiais da camada `AnaliseFinanceira`.

Ele é a referência principal sempre que um indicador mudar.

## Princípios oficiais

- Os indicadores são derivados de dados já persistidos no sistema.
- A camada analítica não consulta a interface.
- A pontuação da saúde financeira usa uma média ponderada dos indicadores.
- O modelo oficial de risco financeiro é o `MF Score`, que organiza os indicadores em pilares, aplica pesos e pode sofrer penalizações críticas.
- Indicadores de configuração continuam visíveis quando faltam parâmetros no perfil financeiro, mas não devem distorcer a leitura como se fossem crise financeira real.
- O comprometimento de curto prazo e a pressão financeira acumulada fazem parte da leitura analítica porque o sistema já possui lançamentos programados, parcelados e recorrentes.
- Os indicadores temporais expõem de forma explícita o valor das obrigações previstas, o valor da receita prevista e o percentual de comprometimento calculado.

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
- `ComprometimentoFinanceiroFuturo90Dias` = 1.0
- `ComprometimentoFinanceiroFuturo180Dias` = 0.75
- `ComprometimentoFinanceiroFuturo365Dias` = 0.5
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

- **Finalidade:** medir quanto da renda prevista para os próximos 30 dias já está comprometido com despesas e obrigações futuras.
- **Fórmula:** `(obrigacoesFinanceirasFuturas30Dias / receitaPrevista30Dias) * 100`
- **Fallback quando a renda é zero e existem obrigações futuras:** considera 100% para não esconder risco.
- **Fonte:** lançamentos pendentes de receita e despesa com vencimento entre a data de referência e os próximos 30 dias.
- **Formato:** percentual.
- **Leitura:** mostra a folga do caixa no curto prazo.
- **Campos de transparência:** `ValorObrigacoesPrevistas`, `ValorReceitaPrevista` e `PercentualComprometimento`.

### Pressão financeira acumulada - 90 dias

- **Finalidade:** medir a pressão financeira acumulada dos próximos 90 dias sobre a renda prevista do mesmo período.
- **Fórmula:** `(obrigacoesFinanceirasFuturas90Dias / receitaPrevista90Dias) * 100`
- **Fallback quando a renda é zero e existem obrigações futuras:** considera 100% para não esconder risco.
- **Fonte:** lançamentos pendentes de receita e despesa com vencimento entre a data de referência e os próximos 90 dias.
- **Formato:** percentual.
- **Leitura:** complementa a visão de curto prazo com uma leitura de trimestre.
- **Campos de transparência:** `ValorObrigacoesPrevistas`, `ValorReceitaPrevista` e `PercentualComprometimento`.

### Pressão financeira acumulada - 180 dias

- **Finalidade:** medir a pressão financeira acumulada dos próximos 180 dias sobre a renda prevista do mesmo período.
- **Fórmula:** `(obrigacoesFinanceirasFuturas180Dias / receitaPrevista180Dias) * 100`
- **Fallback quando a renda é zero e existem obrigações futuras:** considera 100% para não esconder risco.
- **Fonte:** lançamentos pendentes de receita e despesa com vencimento entre a data de referência e os próximos 180 dias.
- **Formato:** percentual.
- **Leitura:** ajuda a enxergar o médio prazo com mais antecedência.
- **Campos de transparência:** `ValorObrigacoesPrevistas`, `ValorReceitaPrevista` e `PercentualComprometimento`.

### Pressão financeira acumulada - 12 meses

- **Finalidade:** medir a pressão financeira acumulada dos próximos 12 meses sobre a renda prevista do mesmo período.
- **Fórmula:** `(obrigacoesFinanceirasFuturas365Dias / receitaPrevista365Dias) * 100`
- **Fallback quando a renda é zero e existem obrigações futuras:** considera 100% para não esconder risco.
- **Fonte:** lançamentos pendentes de receita e despesa com vencimento entre a data de referência e os próximos 12 meses.
- **Formato:** percentual.
- **Leitura:** mostra se o longo prazo ainda preserva folga ou já pede revisão estrutural.
- **Campos de transparência:** `ValorObrigacoesPrevistas`, `ValorReceitaPrevista` e `PercentualComprometimento`.

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
- O indicador de comprometimento financeiro futuro existe para complementar o comprometimento da renda nos próximos 30 dias, enquanto os horizontes maiores representam pressão financeira acumulada.
- A saúde financeira deixa de ser apenas uma média simples e passa a alimentar o `MF Score`, que aplica pilares, pesos e regras críticas documentadas em `docs/MF_SCORE.md`.

## Relação com as telas

- **Dashboard:** consome o resumo da saúde financeira, sem recalcular fórmulas.
- **Saúde Financeira:** exibe o painel detalhado dos indicadores.
- **Assistente Financeiro:** consome o resumo consolidado e os indicadores para compor leitura executiva.
- **Insights Financeiros:** usa os indicadores para gerar alertas, oportunidades e destaques.

## Regra de manutenção

Sempre que uma fórmula, peso, classificação, prioridade ou texto oficial mudar, este documento deve ser atualizado na mesma entrega.

## Calibração do MF Score

As mudanças nesses indicadores não devem ser feitas apenas por ajuste numérico.

Cada revisão deve responder se a alteração melhora a capacidade do `MF Score` representar corretamente o risco financeiro do usuário.

Quando houver dúvida entre duas formulações equivalentes, preferir a que:

- aumente a rastreabilidade;
- deixe a interpretação mais clara;
- produza leitura mais próxima do comportamento financeiro real;
- preserve a consistência com os cenários oficiais de validação.

Toda alteração relevante também deve ser confrontada com a `Suite Oficial de Validação do MF Score`, documentada em `docs/MF_SCORE_VALIDATION.md`.

## Auditoria operacional do motor

Além da validação documental, os indicadores passam a ser conferidos por uma auditoria operacional interna do `MF Score`.

Essa auditoria:

- executa personas sintéticas contra o motor oficial;
- não recalcula indicadores fora da camada `AnaliseFinanceira`;
- gera planilha `.xlsx` com score, pilares, indicadores críticos e dados de entrada;
- deve ser usada como evidência técnica sempre que houver mudança relevante no Motor Financeiro.

Além dessa auditoria automática, existe agora uma auditoria humana das personas, usada para avaliar se a nota calculada faz sentido sob a ótica de um consultor financeiro antes de consolidar faixas esperadas como padrão oficial.

## Governança de lacunas e cobertura

Quando a limitação não estiver na fórmula de um indicador isolado, mas na cobertura conceitual do modelo, ela deve ser registrada em `docs/MF_SCORE_AUDIT.md`.

Exemplo atual:

- o pilar `Planejamento` ainda usa proxies e não mede diretamente toda a execução estratégica do usuário.
