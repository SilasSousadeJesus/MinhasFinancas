# MF Score: Framework Definitivo de Calibração e Engenharia de Risco

Este documento consolida a revisão analítica completa, propostas de faixas operacionais, matrizes de penalização e o roadmap estratégico para o motor de risco do **MF Score** (Escala 0 a 1000).

---

## 1. Leitura do Modelo Atual: Posicionamento e Crítica Conceitual

### 1.1 Enquadramento do MF Score
O **MF Score** posiciona-se estritamente como um **Score de Risco Financeiro Pessoal**. Ele se difere fundamentalmente das outras abordagens do seguinte modo:
* **Score de Riqueza:** Foca no estoque bruto de capital ou renda isolada. O modelo atual refuta essa métrica, pois um usuário com alta renda, mas fluxo de caixa negativo e alta alavancagem de curto prazo, possui risco de insolvência elevado.
* **Score de Saúde Financeira:** Avalia o bem-estar e o comportamento de consumo (adequação a orçamentos, felicidade com as finanças). Possui um viés predominantemente educacional e subjetivo.
* **Score de Risco Financeiro Pessoal (Foco do MF Score):** Mede a **sustentabilidade estocástica da trajetória do usuário**. Responde à pergunta central do produto: *Qual é o risco financeiro pessoal do usuário se ele continuar seguindo a trajetória atual?* É inspirado em técnicas de *credit scoring* bancário ($P(D)$ — probabilidade de default), mas foca na capacidade intrínseca do indivíduo de honrar seus compromissos e absorver choques exógenos antes de entrar em colapso financeiro.

### 1.2 Arquitetura em Quatro Camadas
A arquitetura estruturada em **Pilares (Camada 1), Penalizações Críticas (Camada 2), Persistência Temporal (Camada 3) e Histórico Mensal (Camada 4)** é altamente robusta:
* **Risco Estrutural:** Fragilidades permanentes ou de longo prazo no "chassi" financeiro do usuário (Ex: falta de reserva de emergência). Deve afetar **apenas a nota do pilar**.
* **Risco Operacional:** Descompassos de curto prazo na rotina financeira diária (Ex: taxa de poupança apertada). Impacta a pontuação de base do pilar.
* **Risco Materializado:** O risco deixou de ser uma possibilidade e virou um fato contábil ou contratual consumado (Ex: inadimplência, fluxo de caixa mensal finalizado no vermelho). Dispara gatilhos na camada de **Penalizações Críticas**.
* **Risco Persistente:** A perpetuação do comportamento nocivo ou a incapacidade de reverter o dano ao longo do tempo (Ex: meses consecutivos operando em déficit). Processado exclusivamente na camada de **Persistência Temporal**.

### 1.3 A Regra de Não Dupla Penalização (*Double Counting*)
A regra de blindagem contra a dupla penalização é o mecanismo que impede o derretimento injustificado do score. No MF Score, uma reserva de emergência zerada reduz a nota do pilar de Liquidez. Ela **não** se transformará em uma penalização crítica na nota final a menos que essa vulnerabilidade latente vire uma **ruptura materializada** (o usuário precisou de dinheiro, não tinha reserva e acabou gerando caixa negativo ou inadimplência).

---

## 2. Avaliação Conceitual por Indicador (Pergunta 03)

### 2.1 Economia Mensal e Percentual de Economia
* **O que medem:** A capacidade de geração líquida de caixa de curto prazo após o consumo. É o indicador de eficiência do motor operacional do usuário.
* **Camada de Impacto:** Pilar de Fluxo de Caixa.
* **Severidade Ideal:** Moderada-Alta (Peso base 1.0).
* **Erro de Calibragem a Evitar:** Ativar penalizações críticas na nota final imediatamente em caso de oscilações pontuais comuns em despesas sazonais (como IPVA em janeiro). Deixe que a nota do pilar absorva a variação.
* **Relação com Análise de Crédito:** Equivale ao indicador de fluxo de caixa livre descontado (*Free Cash Flow*) corporativo.

### 2.2 Reserva de Emergência Atual e Reserva de Emergência Ideal
* **O que medem:** O tamanho do colchão de liquidez estática do indivíduo medida em meses de custo de vida. É a primeira linha de defesa contra sinistros econômicos.
* **Camada de Impacto:** Pilar de Liquidez.
* **Severidade Ideal:** Crítica (Peso 1.5 para a atual, 0.5 para a ideal).
* **Erro de Calibragem a Evitar:** Punir a ausência de reserva diretamente na camada de penalizações críticas. Reserva zero é um risco estrutural latente, não uma quebra de contrato consumada.
* **Relação com Análise de Crédito:** Funciona de forma análoga ao *Quick Ratio* (Índice de Liquidez Imediata).

### 2.3 Comprometimento da Renda
* **O que mede:** A rigidez do orçamento pessoal corrente. Representa o percentual da receita comprometido com obrigações fixas e variáveis contratadas.
* **Camada de Impacto:** Pilar de Endividamento.
* **Severidade Ideal:** Crítica (Peso 1.5).
* **Erro de Calibragem a Evitar:** Ignorar a natureza da despesa. Se o comprometimento decorre de aportes recorrentes de investimentos programados, o risco de rigidez é menor do que se derivar de empréstimos consignados ou faturas de cartão consolidadas.
* **Relação com Análise de Crédito:** É o clássico indicador DTI (*Debt-to-Income Ratio*).

### 2.4 Comprometimento Financeiro Futuro (30 dias) e Pressões Financeiras Acumuladas (90, 180 e 365 dias)
* **O que medem:** A alavancagem temporal futura e o encadeamento de despesas parceladas ou recorrentes de longo prazo. Fornece visibilidade sobre o horizonte de insolvência antes que ele ocorra.
* **Camada de Impacto:** Combinação dos pilares de Endividamento e Fluxo de Caixa.
* **Severidade Ideal:** Progressivamente Decrescente (30 dias = Peso 1.5; 90 dias = Peso 1.0; 180 dias = Peso 0.75; 365 dias = Peso 0.5).
* **Erro de Calibragem a Evitar:** Tratar compromissos de longo prazo (como o saldo devedor de um financiamento imobiliário saudável) como uma pressão financeira iminente de alta gravidade. 
* **Relação com Análise de Crédito:** Assemelha-se ao cronograma de amortização de dívidas corporativas de médio e longo prazo.

### 2.5 Endividamento Patrimonial, Patrimônio Líquido Atual e Percentual do Patrimônio Alvo
* **O que medem:** A solvência estrutural profunda do indivíduo no longo prazo. Representa a capacidade de liquidação total de passivos recorrendo à venda de bens e ativos.
* **Camada de Impacto:** Pilar de Patrimônio.
* **Severidade Ideal:** Alta (Peso 1.25 a 1.5).
* **Erro de Calibragem a Evitar:** Permitir que oscilações de marcação a mercado de curto prazo na carteira de investimentos do usuário causem volatilidade ou reduções agressivas na nota do pilar.
* **Relação com Análise de Crédito:** É a relação *Debt-to-Equity* (Passivo/Patrimônio Líquido).

---

## 3. Proposta de Faixas de Risco por Indicador (Pergunta 04)

Esta matriz traduz as faixas operacionais de risco mapeadas para a escala padronizada de status do motor (`Excelente = 100`, `Bom = 80`, `Atenção = 55`, `Crítica = 25`).

| Indicador | Saudável (Nota 100) | Atenção (Nota 80) | Risco Moderado (Nota 55) | Risco Alto (Nota 25) | Risco Grave (Nota 0) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Percentual de Economia** | $> 20\%$ | $10\% 	ext{ a } 19.9\%$ | $5\% 	ext{ a } 9.9\%$ | $0\% 	ext{ a } 4.9\%$ | $< 0\%$ (Déficit operacional) |
| **Reserva de Emergência** | $\ge 6 	ext{ meses}$ | $4 	ext{ a } 5.9 	ext{ meses}$ | $2 	ext{ a } 3.9 	ext{ meses}$ | $0.1 	ext{ a } 1.9 	ext{ meses}$ | $0 	ext{ meses}$ (Exposição total) |
| **Comprometimento da Renda**| $\le 20\%$ | $20.1\% 	ext{ a } 35\%$ | $35.1\% 	ext{ a } 50\%$ | $50.1\% 	ext{ a } 70\%$ | $> 70\%$ (Sufocamento) |
| **Comprometimento Futuro (30d)**| $\le 25\%$ | $25.1\% 	ext{ a } 40\%$ | $40.1\% 	ext{ a } 55\%$ | $55.1\% 	ext{ a } 75\%$ | $> 75\%$ (Estresse iminente) |
| **Endividamento Patrimonial**| $\le 15\%$ | $15.1\% 	ext{ a } 30\%$ | $30.1\% 	ext{ a } 50\%$ | $50.1\% 	ext{ a } 80\%$ | $> 80\%$ (Alavancagem extrema) |

### Racionais Técnicos das Faixas e Blindagem
* **Quando afeta apenas o pilar:** As faixas de `Saudável` até `Risco Alto` recalculam apenas o valor interno ponderado do pilar. Elas indicam maior ou menor vulnerabilidade estrutural, mas não representam uma quebra real de contrato ou insolvência consumada.
* **Quando participa de uma regra crítica:** Somente quando o indicador atinge a faixa de **Risco Grave (0)** configurando um fato contábil ou contratual (ex: `Percentual de Economia < 0` indica caixa mensal negativo e acionará a respectiva penalização crítica de fluxo de caixa negativo).

---

## 4. Avaliação por Pilar (Pergunta 05)

### 4.1 Fluxo de Caixa (Peso 30%)
* **Conceito:** Eficiência orçamentária operacional de curto prazo. É o motor diário do score.
* **Pesa Mais:** `Percentual de Economia`. | **Pesa Menos:** `Economia Mensal` nominal.
* **Combinações Perigosas:** `Percentual de Economia` em declínio crônico associado a `Comprometimento Financeiro Futuro (30 dias)` em ascensão rápida.
* **Gatilho de Penalização Adicional:** O pilar baixo apenas corrói o score base. O gatilho crítico só é disparado se o `Percentual de Economia` cruzar o zero contábil (tornando-se negativo), materializando o déficit.

### 4.2 Liquidez (Peso 25%)
* **Conceito:** Resiliência estática e margem de tempo para sobrevivência contra choques imediatos.
* **Pesa Mais:** `Reserva de Emergência Atual`. | **Pesa Menos:** `Reserva de Emergência Ideal`.
* **Combinações Perigosas:** `Reserva de Emergência Atual = 0` com `Pressão Financeira Acumulada 90 dias` elevada.
* **Gatilho Crítico Adicional (Insolvência Operacional Oculta):** Se a nota do pilar de `Liquidez < 25` (reserva praticamente inexistente) **E** a nota do pilar de `Fluxo de Caixa < 25` (geração de caixa nula ou negativa), o motor de risco detecta que o usuário entrou em colapso. Esta combinação força a redução automática do pilar de Planejamento para zero e dispara um alerta de "Risco Estrutural Avançado" na camada de penalidades, subtraindo 100 pontos da nota final.

### 4.3 Endividamento (Peso 20%)
* **Conceito:** Alavancagem e nível de rigidez contratual do orçamento.
* **Pesa Mais:** `Comprometimento da Renda` e `Comprometimento Futuro 30 dias`. | **Pesa Menos:** `Pressão Financeira Acumulada 365 dias`.
* **Combinações Perigosas:** `Comprometimento da Renda` elevado associado a ativos predominantemente ilíquidos no pilar de Patrimônio.
* **Gatilho de Penalização Adicional:** O endividamento severo pulveriza a nota do pilar. A penalização crítica na nota final só ocorre quando há inadimplência consumada (atraso real de títulos).

### 4.4 Patrimônio (Peso 15%)
* **Conceito:** Solvência estrutural profunda e colchão patrimonial de última instância.
* **Pesa Mais:** `Patrimônio Líquido Atual`. | **Pesa Menos:** `Percentual do Patrimônio Alvo`.
* **Combinações Perigosas:** `Endividamento Patrimonial` alto casado com `Liquidez` em nível crítico.
* **Gatilho de Penalização Adicional:** Quando `Patrimônio Líquido Atual < 0`, a insolvência técnica é declarada, forçando a aplicação imediata da penalidade crítica de patrimônio negativo ($-200$ pontos no score final).

### 4.5 Planejamento (Peso 10%)
* **Conceito:** Previsibilidade, governança e aderência às metas financeiras.
* **Pesa Mais:** Lançamento de despesas futuras e regularidade de sincronização.
* **Gatilho de Penalização Adicional:** Se o usuário ficar mais de 45 dias sem atualizar o sistema, o pilar vai a zero e aciona a penalização crítica de *Ausência de Dados Essenciais* ($-80$ pontos no score final).

---

## 5. Proposta de Camada de Penalizações Críticas (Pergunta 06)

Proposta técnica baseada em modelos de deterioração de risco para a revisão das regras críticas:

### 5.1 Fluxo de Caixa Mensal Negativo
* **Gatilho Objetivo:** `Percentual de Economia < 0` no fechamento do mês.
* **Motivo:** Materialização do déficit operacional (gastar mais do que arrecada).
* **Severidade e Natureza:** **-50 pontos** no score final; natureza **Fixa** para a primeira ocorrência do ciclo.
* **Risco de Exagero:** Baixo. Protege o usuário contra variações sazonais legítimas (um único mês negativo por imprevisto isolado não destrói o score se ele tiver pilares de liquidez fortes).

### 5.2 Patrimônio Líquido Negativo
* **Gatilho Objetivo:** `Patrimônio Líquido Atual < 0`.
* **Motivo:** Insolvência técnica profunda (as dívidas superam todos os bens móveis e imóveis).
* **Severidade e Natureza:** **-200 pontos** no score final; natureza **Fixa** (enquanto o balanço permanecer negativo).
* **Risco de Exagero:** Controlado. Funciona como uma trava de segurança que bloqueia o acesso do usuário aos níveis "Bom" ou "Excelente".

### 5.3 Ausência de Dados Essenciais
* **Gatilho Objetivo:** Período $> 45 	ext{ dias}$ sem qualquer atualização de saldos de contas ou lançamentos de despesas.
* **Motivo:** Perda de rastreabilidade do modelo (risco de opacidade estrutural).
* **Severidade e Natureza:** **-80 pontos** no score final; natureza **Fixa** (removida imediatamente após a regularização dos dados).
* **Risco de Exagero:** Mitigado pelo prazo elástico de 45 dias, evitando punir o usuário por hiatos curtos de viagem ou férias.

### 5.4 Penalidades Removidas por Duplicidade
* Propostas como "Penalização automática por Reserva Zero" ou "Penalização automática por Cartão Alto" **devem ser rejeitadas da camada crítica**, pois já corroem integralmente os pilares de Liquidez e Endividamento, respectivamente, violando o princípio de *Double Counting*.

---

## 6. Persistência Temporal do Risco (Pergunta 07)

A modelagem temporal atua aplicando agravadores cumulativos e definindo a velocidade de recuperação do score (*curva de cura*).

### 6.1 Regras de Agravamento Progressivo por Inércia
* **1 Mês Negativo:** Tratado como oscilação padrão de caixa. Aplica-se apenas a penalidade crítica isolada de **-50 pontos**.
* **2 Meses Consecutivos Negativos:** Evidencia a falha do usuário em ajustar o comportamento orçamentário no mês seguinte ao alerta. Ativa o agravador temporal de **-150 pontos** adicionais no score final.
* **3 ou mais Meses Consecutivos Negativos:** Indica trajetória crônica de endividamento ou queima severa de patrimônio. Aplica o agravador temporal máximo de **-300 pontos** adicionais.

### 6.2 Reincidência de Inadimplência
* Se o usuário regularizar suas contas vencidas, a penalidade por inadimplência é removida. No entanto, se ele voltar a ficar inadimplente em um intervalo inferior a 90 dias, a nova penalidade é aplicada com um **fator multiplicador de $1.5\times$**, pois o modelo detecta um padrão de instabilidade crônica de fluxo de caixa.

### 6.3 Curva de Cura e Bônus de Recuperação Coerente (Amortização Temporal)
Para evitar o "efeito iô-iô" (o score oscilar violentamente entre meses positivos e negativos), adota-se uma **Amortização Temporal de Risco**:
* Quando o usuário regulariza uma conta ou fecha um mês no positivo, os pilares recuperam suas notas de base de forma imediata.
* A camada temporal, contudo, retém uma **penalidade amortecida residual** para validar a consistência comportamental: retém $50\%$ da punição temporal no primeiro mês de melhora e $25\%$ no segundo mês. O score só estará totalmente limpo após **90 dias consecutivos** de estabilidade operacional comprovada.

---

## 7. Mecânica de Inadimplência Gradual (Pergunta 08)

### 7.1 Avaliação do Modelo Binário da V1
A regra atual (`Status = Pendente` E `DataVencimento < DataReferencia`) é **suficiente para uma V1** por ser simples, confiável e barata computacionalmente. No entanto, ela carece de realismo econômico porque pune um esquecimento de R$ 10 com o mesmo rigor de um calote substancial de R$ 10.000, gerando oscilações artificiais no score.

### 7.2 Arquitetura de Evolução para uma Inadimplência Gradual
Para evoluir o motor com segurança sem inflar a complexidade do banco de dados, a inadimplência deve ser processada através de uma **Matriz de Gravidade Condicional**:

* **Nível 1 - Atraso Técnico (Até 7 dias de atraso E Valor Total $< 10\%$ da renda mensal):** Tratado como ruído operacional ou esquecimento. Penalização: **-30 pontos** fixos no score final.
* **Nível 2 - Estresse de Caixa (8 a 45 dias de atraso OU Valor entre $10\%$ e $35\%$ da renda):** Caracteriza incapacidade momentânea de honrar compromissos. Penalização: **-120 pontos** no score final.
* **Nível 3 - Calote Consumado (> 45 dias de atraso OU Valor $> 35\%$ da renda):** Equivalente a uma restrição de crédito oficial. Penalização: **-300 pontos** no score final.

---

## 8. Avaliação das Personas Sintéticas (Pergunta 09 e 10)

Revisão técnica das faixas atuais do sistema para evitar que perfis de alta vulnerabilidade estrutural alcancem notas de risco baixo.

| Persona | Faixa Atual | Faixa Sugerida | Motivo da Mudança |
| :--- | :--- | :--- | :--- |
| **Vida Financeira Excelente** | `900-1000` | **`920-1000`** | **Estreita demais na base.** Um perfil excelente não deve flutuar próximo a 900 se todos os indicadores estiverem no teto. |
| **Boa renda, reserva zero e cartão alto** | `600-740` | **`580-660`** | **Permissiva demais no teto.** 740 indica um risco baixo/moderado, o que é incoerente para quem tem reserva zero e alta rigidez de cartão. |
| **Patrimônio alto com fluxo ruim** | `550-750` | **`640-720`** | **Larga demais.** 550 punia excessivamente o patrimônio alto; 750 ignorava o sufocamento de fluxo. A faixa sugerida estabiliza o perfil em risco moderado. |
| **Excelente fluxo com pouco patrimônio** | `750-900` | **`780-860`** | **Larga demais.** O teto de 900 deve ser blindado para quem possui patrimônio consolidado. O excelente fluxo garante estabilidade na faixa "Bom". |
| **Inadimplência** | `0-490` | **`350-520`** | **Larga e binária demais.** Com a nova regra de inadimplência gradual, atrasos técnicos curtos ficam em `480-520`, enquanto calotes crônicos desabam para `<400`. |
| **Comprometimento extremo** | `0-590` | **`420-540`** | **Larga demais na base.** Se o comprometimento é extremo mas não há inadimplência materializada, o score base é destruído no pilar, mas não deve ir a zero. |

---

## 9. Proposta de Calibragem Prática (Pergunta 11)

### 11.1 Ajustes de Pesos por Pilar e Indicador
* Manutenção dos pesos oficiais estruturais dos pilares (`Fluxo de Caixa: 30%`, `Liquidez: 25%`, `Endividamento: 20%`, `Patrimônio: 15%`, `Planejamento: 10%`).
* Ajuste fino no indicador `Comprometimento Financeiro Futuro (30 dias)`, cujo peso interno no pilar de Endividamento sobe para **`1.75`**, tornando o modelo mais sensível a faturas de cartão explosivas antes mesmo do fechamento do mês.

### 11.2 Ordem Recomendada de Implementação
1. **Sprint 1 (Pilares & Faixas):** Ajustar as funções de cálculo dos Pilares com os novos limites das faixas de risco (Seção 3).
2. **Sprint 2 (Inadimplência Gradual):** Refatorar a query de identificação de inadimplência para extrair `DiasAtraso` e `ValorVencido`, aplicando a Matriz Gradual.
3. **Sprint 3 (Camada Temporal):** Implementar a esteira temporal no Job mensal do Hangfire, injetando as penalidades residuais no histórico.

### 11.3 Riscos de Implementação e Validação (*Shadow Run*)
* **Risco:** Volatilidade em massa nos scores dos usuários atuais após o deploy da inadimplência gradual.
* **Mitigação (Estratégia de Shadow Run):** Crie uma propriedade temporária chamada `ScoreV2` no motor de cálculo. Rode o novo algoritmo em paralelo com a V1 por **30 dias (um ciclo de faturamento completo)**. Use logs para comparar os desvios. Se o desvio médio dos usuários da base ficar contido dentro de $\pm 45 	ext{ pontos}$, o modelo está calibrado e pronto para a virada de chave oficial.

---

## 10. Plano de Evolução do MF Score (Pergunta 12)

### 10.1 Curto Prazo (Sprints Técnicas Imediatas)
* **Correção de Duplicidades Latentes:** Remover travas de código que geravam penalizações em cascata.
* **Inadimplência Gradual:** Substituição da regra booleana simples por dias de atraso e impacto sobre a renda corrente.
* **Shadow Run:** Implementação do ambiente de testes paralelo para validação estatística da calibragem sem impacto em produção.

### 10.2 Médio Prazo (3 a 6 Meses - Depende de Maturidade de Dados)
* **Evolução do Pilar de Planejamento:** Substituir o uso de proxies simples por indicadores reais de desvios orçamentários (Orçado vs. Realizado integrado).
* **Maturação da Camada Temporal:** Substituição das regras matemáticas fixas de meses negativos por modelos baseados em médias móveis exponenciais (EMA) do comportamento de fluxo de caixa dos últimos 6 meses.

### 10.3 Longo Prazo (12 Meses ou Mais - Depende de Histórico e Domínio)
* **Integração via Open Finance:** Captura automática do comportamento de crédito externo do usuário (Scoring de birôs de crédito tradicionais integrando como insumo do pilar de Endividamento).
* **Calibragem Dinâmica por IA:** Utilização de modelos de Machine Learning (ex: XGBoost) treinados no histórico anônimo da base de usuários para ajustar dinamicamente os pesos dos indicadores de acordo com padrões reais de inadimplência do sistema.

### 10.4 Decisões Conceituais (Não dependem de código)
* **Definição do Apetite de Risco do Produto:** Alinhamento estratégico sobre quão severo o aplicativo quer ser com o usuário. Um score muito punitivo pode gerar frustração e *churn*; um score muito permissivo perde a utilidade técnica de proteção financeira.

---

## 11. Tabela-Resumo Final do Framework (Calibragem)

Esta matriz consolida a governança, a distribuição de pesos e a severidade de impacto de cada componente do **MF Score** reformulado:

| Indicador / Evento | Tipo de Impacto | Severidade / Peso | Observações Práticas de Calibragem | Risco de Dupla Penalização |
| :--- | :--- | :--- | :--- | :--- |
| **Percentual de Economia** | Pilar (Fluxo de Caixa) | Peso `1.0` | Mede a eficiência diária; use média móvel de 90 dias para suavizar ruídos operacionais. | Baixo. Flutuações reduzem o pilar, mas não devem gerar descontos na nota final. |
| **Reserva de Emergência Atual** | Pilar (Liquidez) | Peso `1.5` | Principal indicador de resiliência estática; a nota zero não aplica punição na nota final. | Alto. Controlado aplicando a punição exclusivamente no pilar e não na nota final. |
| **Comprometimento da Renda** | Pilar (Endividamento) | Peso `1.5` | Mede a rigidez orçamentária; valores acima de $50\%$ derrubam a nota de base do pilar. | Mitigado. A punição em nota final só ocorre caso gere inadimplência real. |
| **Comprometimento Futuro (30d)** | Pilar (Endividamento) | Peso `1.75` | Funciona como o pipeline de curtíssimo prazo; possui alto poder preditivo de estresse de caixa. | Controlado. Elemento preditivo de fluxo futuro dentro do pilar. |
| **Pressões Acumuladas (365d)** | Pilar (Endividamento) | Peso `0.5` | Impacto decrescente no tempo; compromissos distantes geram menor risco de liquidez presente. | Baixo. Peso reduzido atenua o impacto de parcelas longas e saudáveis. |
| **Patrimônio Líquido Atual** | Pilar (Patrimônio) | Peso `1.25` | Estabelece o piso estrutural de solvência de longo prazo do usuário. | Baixo. Serve como amortecedor para o piso do score de investidores. |
| **Inadimplência Materializada** | Penalização Crítica | Gradual (Até -300 pts) | Substitui a regra binária; calcula a punição final com base nos dias de atraso e no percentual da renda. | Zero. Trata-se de um evento contratual quebra de acordo contábil real. |
| **Fluxo Mensal Negativo** | Penalização Crítica | Fixa (-50 pts) | Captura a ruptura operacional pontual da competência atual. | Zero. Disparado apenas se o fechamento consolidado da conta for negativo. |
| **Patrimônio Líquido Negativo** | Penalização Crítica | Fixa (-200 pts) | Bloqueia o acesso do usuário às faixas superiores; sinaliza insolvência técnica total. | Zero. Indica estado técnico de falência patrimonial. |
| **Inércia Recorrente no Vermelho**| Persistência Temporal | Cumulativa (Até -300 pts) | Pune de forma severa a repetição do fluxo negativo por 2 ou mais meses consecutivos. | Zero. Avalia unicamente o fator inercial/temporal do comportamento do usuário. |

