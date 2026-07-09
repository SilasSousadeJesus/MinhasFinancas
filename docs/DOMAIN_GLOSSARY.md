# Glossário do Domínio - Minhas Finanças

Este documento registra os conceitos oficiais do domínio financeiro do sistema.

Sempre que surgir um novo conceito relevante, ele deve ser documentado aqui.

## Lançamento

Registro de uma movimentação financeira do usuário.

## Data de lançamento

Data em que o registro foi criado no sistema.

Não representa vencimento, pagamento nem recebimento.

## Data de vencimento

Data prevista para a movimentação ocorrer.

- em despesas, representa o vencimento da obrigação
- em receitas, representa a data prevista de entrada do valor

## Data de efetivação

Data em que o dinheiro realmente entrou ou saiu.

- em despesas, representa a data do pagamento
- em receitas, representa a data do recebimento

## Status do lançamento

Estado atual do ciclo de vida do lançamento.

## Pendente

Lançamento previsto, mas ainda não efetivado.

## Pago

Status final permitido para despesas efetivadas.

## Recebido

Status final permitido para receitas efetivadas.

## Cancelado

Lançamento que não deve mais produzir efeito financeiro previsto nem realizado.

## Receita

Movimentação de entrada de recursos.

Pode assumir apenas:

- `Pendente`
- `Recebido`
- `Cancelado`

## Despesa

Movimentação de saída de recursos.

Pode assumir apenas:

- `Pendente`
- `Pago`
- `Cancelado`

## Efetivação

Ato de registrar que a movimentação realmente ocorreu.

- receita: `Pendente -> Recebido`
- despesa: `Pendente -> Pago`

## Conta

Meio financeiro do usuário usado para organizar saldo e lançamentos.

## Cartão

Meio de pagamento vinculado ao usuário, utilizado nos lançamentos e no controle financeiro.

## Categoria

Classificação principal de um lançamento.

## Subcategoria

Detalhamento de uma categoria.

## Fluxo de Caixa Previsto

Leitura baseada em datas previstas, especialmente `DataVencimento`.

## Fluxo de Caixa Realizado

Leitura baseada apenas em movimentações efetivadas, usando `DataEfetivacao`.

## Patrimônio

Conjunto de ativos e passivos do usuário.

## Ativo patrimonial

Bem ou recurso que compõe positivamente o patrimônio.

## Passivo patrimonial

Obrigação financeira que reduz o patrimônio.

## Patrimônio líquido

Resultado de:

`Total de Ativos - Total de Passivos`

## Snapshot patrimonial

Fotografia congelada do patrimônio em uma data de referência.

## Evolução patrimonial

Leitura histórica construída a partir dos snapshots salvos.

## Meta

Objetivo financeiro acumulativo do usuário.

## Projeção

Simulação orientada a objetivo financeiro, com acumulado inicial, renda base, renda extra e horizonte temporal.

## Simulação Financeira

Cenário hipotético persistido por usuário para testar decisões futuras sem alterar dados reais.

## Perfil Financeiro

Conjunto de parâmetros pessoais usados para definir como o usuário deseja medir e avaliar sua própria saúde financeira.

## Indicador Financeiro

Informação derivada dos dados financeiros do sistema para mostrar desempenho, risco, equilíbrio ou progresso do usuário.

Não é um dado bruto persistido.

É resultado de cálculo sobre lançamentos, patrimônio, perfil financeiro, metas, projeções ou outras bases existentes.

## Análise Financeira

Módulo responsável por transformar dados em indicadores reutilizáveis.

Essa camada não deve depender de tela específica.

Ela existe para servir dashboard, relatórios, exportações, APIs, módulos de saúde financeira e futuras integrações.

## Saúde Financeira

Leitura interpretativa construída a partir dos indicadores financeiros e dos parâmetros definidos no perfil financeiro do usuário.

## Pontuação de Saúde Financeira

Medida sintética de 0 a 100 calculada a partir do conjunto de indicadores financeiros.

Ela resume, de forma simples, o equilíbrio financeiro atual do usuário.

## Insight Financeiro

Leitura acionável gerada a partir dos indicadores e da saúde financeira.

Pode representar:

- alerta
- oportunidade
- destaque positivo
- necessidade de configuração

## ResumoFinanceiroIA

Objeto consolidado da inteligência financeira do sistema.

Ele organiza, em um único payload:

- indicadores
- saúde financeira
- insights
- prioridades imediatas
- destaques positivos
- resumo executivo textual

## Contexto para IA

Representação textual e estruturada do `ResumoFinanceiroIA`, preparada pelo sistema para consumo externo sem expor dados desnecessários do banco.

## Provedor de IA

Abstração de infraestrutura responsável por receber um contexto já preparado e, futuramente, enviar esse material para um serviço externo de IA.

O provedor nunca deve acessar o banco diretamente.

## Assistente Financeiro

Experiência executiva do sistema que organiza saúde financeira, indicadores, insights e prioridades a partir do `ResumoFinanceiroIA`.

Na fase atual, funciona sem IA real e sem chat.

## Parâmetro Financeiro

Valor configurado pelo usuário para servir de referência em análises, indicadores, alertas e comparações futuras.

## Configuração vigente

Registro atualmente ativo dentro de um histórico de configurações.

Em módulos históricos, representa a configuração que deve ser considerada como referência atual do usuário.

## Histórico do Perfil Financeiro

Conjunto temporal de configurações anteriores do perfil financeiro do usuário.

Permite saber quais critérios estavam válidos em cada período.

## Ação de Simulação

Evento hipotético dentro de uma simulação financeira.

Na V1 pode representar:

- receita única
- despesa única
- receita recorrente mensal
- despesa recorrente mensal
- despesa parcelada

## Resultado simulado

Consolidação mensal que combina base real de lançamentos com ações hipotéticas.

## Saldo real

Saldo mensal obtido apenas dos lançamentos reais.

## Saldo simulado

Saldo mensal obtido ao aplicar as ações simuladas sobre a base real.

## Memória Financeira

Histórico persistido das análises financeiras geradas pelo Assistente Financeiro.

Esse histórico pertence ao sistema, e não ao provedor de IA.

## Análise Financeira Histórica

Fotografia analítica preservada em determinado momento.

Ela registra:

- o período de referência
- o estado resumido da inteligência financeira naquele instante
- a pergunta do usuário
- a resposta da IA
- as métricas técnicas da geração

## Histórico de Análises

Coleção de análises financeiras históricas de um usuário.

Permite auditoria, comparação temporal, evolução mensal e futura construção de memória para a IA.

## Base de Conhecimento Financeira

Conjunto de registros e interpretações persistidas que permitem ao sistema acompanhar a evolução financeira do usuário ao longo do tempo.

Ela pertence ao domínio do sistema e pode ser consumida pela IA, mas não pertence ao provedor externo.

## Memória Consultiva

Resumo estruturado de análises anteriores usado para dar continuidade às novas análises do Assistente Financeiro.

Não representa o histórico completo.

Serve para economizar tokens e preservar contexto recente.

## Plano Estratégico Financeiro

Direção financeira deliberadamente escolhida pelo usuário para um horizonte mais longo.

Ele registra:

- objetivos
- prioridades
- princípios
- decisões importantes
- direção de longo prazo

Exemplos:

- construir reserva de emergência
- reduzir endividamento
- iniciar investimentos
- planejar compra de imóvel

## Interpretador Estratégico

Camada que transforma o Plano Estratégico Financeiro em narrativa compreensível para IA e interfaces.

Na fase atual, ela já é usada para preparar o contexto do Assistente Financeiro antes da chamada de IA.

## Consistência Estratégica

Leitura que avalia se uma decisão está alinhada com o Plano Estratégico Financeiro vigente.

## Avaliador de Consistência Estratégica

Componente determinístico que calcula a consistência entre a pergunta do usuário, a situação financeira atual e o plano estratégico vigente.

## Nível de Consistência Estratégica

Classificação gerada pelo avaliador para indicar o grau de alinhamento.

Exemplos:

- Muito alta
- Alta
- Média
- Baixa
- Muito baixa
- Indeterminada

## Decisão Financeira

Intenção do usuário estruturada pelo sistema em uma forma própria de domínio antes da leitura estratégica.

## Interpretador da Decisão Financeira

Camada que transforma a pergunta do usuário em `DecisãoFinanceiraIA`, identificando tipo, categoria, valor estimado, prazo e objetivo relacionado.

## Compromissos Financeiros

Registro de intenções e decisões financeiras que o usuário deseja acompanhar ao longo do tempo.

Esse conceito funciona como memória operacional das ações assumidas com o próprio planejamento financeiro ou com sugestões vindas do Assistente Financeiro.

Campos conceituais principais:

- descrição do compromisso
- origem do compromisso
- status atual
- data de criação
- data de conclusão, quando aplicável
- data de cancelamento, quando aplicável
- observações complementares
- ativo/inativo

## Especialistas Financeiros

Camada interna do Assistente Financeiro responsável por aprofundar a leitura de domínios específicos sem criar uma segunda IA.

Os especialistas retornam pareceres estruturados por assunto, como:

- dívidas
- fluxo de caixa
- patrimônio
- reserva de emergência
- plano estratégico
- compromissos

Eles não substituem o Assistente Financeiro principal. Apenas aprofundam o contexto consolidado antes da resposta final.

Exemplos:

- fortalecer a reserva de emergência
- reduzir uso do cartão de crédito
- adiar uma compra não prioritária

No futuro, os compromissos poderão receber indicadores de execução, alertas e vínculo mais direto com o Plano Estratégico Financeiro.

## Comprometimento financeiro futuro

Leitura analítica que mostra quanto da renda prevista nos próximos 30 dias já está comprometida com despesas e obrigações futuras.

Ela complementa o `Comprometimento da renda` ao observar a pressão do caixa em diferentes horizontes.

A leitura oficial expõe de forma clara o valor das obrigações previstas, o valor da receita prevista e o percentual de comprometimento calculado.

## Pressão financeira acumulada

Leitura analítica que mostra quanto da renda prevista em um horizonte maior já está comprometida com despesas e obrigações futuras.

A camada analítica usa essa leitura para os horizontes de 90 dias, 180 dias e 12 meses.

Assim como o horizonte de 30 dias, essa leitura também expõe obrigações previstas, receita prevista e percentual de comprometimento para tornar a análise transparente.

## Endividamento patrimonial

Indicador que mede o peso dos passivos patrimoniais sobre a base patrimonial ativa.

Ele não representa obrigações futuras de cartão, parcelas ou lançamentos pendentes. Essa leitura é feita pelos indicadores de comprometimento financeiro futuro e de pressão financeira acumulada.

## Pontuação de Saúde Financeira

Medida sintética de 0 a 100 calculada a partir de uma média ponderada dos indicadores financeiros, incluindo leituras de curto prazo e pressão financeira acumulada em horizontes maiores.

Indicadores de contexto ou configuração têm peso reduzido quando servem mais como régua pessoal do que como medida direta de desempenho.

## MF Score

Modelo oficial de avaliação de risco financeiro pessoal do sistema.

Ele substitui conceitualmente a antiga leitura de pontuação simples e passa a organizar os indicadores em cinco pilares:

- Fluxo de Caixa
- Liquidez e Reserva
- Endividamento e Obrigações
- Patrimônio
- Planejamento e Disciplina

O MF Score produz:

- nota base;
- nota final;
- classificação;
- risco textual;
- tendência;
- indicadores críticos que aplicam penalizações.

## Calibração do MF Score

Etapa contínua de amadurecimento do modelo oficial de risco.

Ela revisa:

- indicadores
- fórmulas
- pesos
- pilares
- penalizações
- classificações
- textos e interpretações

## MF Score Potencial

Versão futura do score que representa o ponto de evolução possível do usuário caso a trajetória atual seja mantida e os principais pontos de pressão sejam corrigidos.
