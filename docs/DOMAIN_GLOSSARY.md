# GlossÃ¡rio do DomÃ­nio - Minhas FinanÃ§as

Este documento registra os conceitos oficiais do domÃ­nio financeiro do sistema.

Sempre que surgir um novo conceito relevante, ele deve ser documentado aqui.

## LanÃ§amento

Registro de uma movimentaÃ§Ã£o financeira do usuÃ¡rio.

## Data de lanÃ§amento

Data em que o registro foi criado no sistema.

NÃ£o representa vencimento, pagamento nem recebimento.

## Data de vencimento

Data prevista para a movimentaÃ§Ã£o ocorrer.

- em despesas, representa o vencimento da obrigaÃ§Ã£o
- em receitas, representa a data prevista de entrada do valor

## Data de efetivaÃ§Ã£o

Data em que o dinheiro realmente entrou ou saiu.

- em despesas, representa a data do pagamento
- em receitas, representa a data do recebimento

## Status do lanÃ§amento

Estado atual do ciclo de vida do lanÃ§amento.

## Pendente

LanÃ§amento previsto, mas ainda nÃ£o efetivado.

## Pago

Status final permitido para despesas efetivadas.

## Recebido

Status final permitido para receitas efetivadas.

## Cancelado

LanÃ§amento que nÃ£o deve mais produzir efeito financeiro previsto nem realizado.

## Receita

MovimentaÃ§Ã£o de entrada de recursos.

Pode assumir apenas:

- `Pendente`
- `Recebido`
- `Cancelado`

## Despesa

MovimentaÃ§Ã£o de saÃ­da de recursos.

Pode assumir apenas:

- `Pendente`
- `Pago`
- `Cancelado`

## EfetivaÃ§Ã£o

Ato de registrar que a movimentaÃ§Ã£o realmente ocorreu.

- receita: `Pendente -> Recebido`
- despesa: `Pendente -> Pago`

## Conta

Meio financeiro do usuÃ¡rio usado para organizar saldo e lanÃ§amentos.

## CartÃ£o

Meio de pagamento vinculado ao usuÃ¡rio, utilizado nos lanÃ§amentos e no controle financeiro.

## Categoria

ClassificaÃ§Ã£o principal de um lanÃ§amento.

## Subcategoria

Detalhamento de uma categoria.

## Fluxo de Caixa Previsto

Leitura baseada em datas previstas, especialmente `DataVencimento`.

## Fluxo de Caixa Realizado

Leitura baseada apenas em movimentaÃ§Ãµes efetivadas, usando `DataEfetivacao`.

## PatrimÃ´nio

Conjunto de ativos e passivos do usuÃ¡rio.

## Ativo patrimonial

Bem ou recurso que compÃµe positivamente o patrimÃ´nio.

## Passivo patrimonial

ObrigaÃ§Ã£o financeira que reduz o patrimÃ´nio.

## PatrimÃ´nio lÃ­quido

Resultado de:

`Total de Ativos - Total de Passivos`

## Snapshot patrimonial

Fotografia congelada do patrimÃ´nio em uma data de referÃªncia.

## EvoluÃ§Ã£o patrimonial

Leitura histÃ³rica construÃ­da a partir dos snapshots salvos.

## Meta

Objetivo financeiro acumulativo do usuÃ¡rio.

## ProjeÃ§Ã£o

SimulaÃ§Ã£o orientada a objetivo financeiro, com acumulado inicial, renda base, renda extra e horizonte temporal.

## SimulaÃ§Ã£o Financeira

CenÃ¡rio hipotÃ©tico persistido por usuÃ¡rio para testar decisÃµes futuras sem alterar dados reais.

## Perfil Financeiro

Conjunto de parÃ¢metros pessoais usados para definir como o usuÃ¡rio deseja medir e avaliar sua prÃ³pria saÃºde financeira.

## Indicador Financeiro

InformaÃ§Ã£o derivada dos dados financeiros do sistema para mostrar desempenho, risco, equilÃ­brio ou progresso do usuÃ¡rio.

NÃ£o Ã© um dado bruto persistido.

Ã‰ resultado de cÃ¡lculo sobre lanÃ§amentos, patrimÃ´nio, perfil financeiro, metas, projeÃ§Ãµes ou outras bases existentes.

## AnÃ¡lise Financeira

MÃ³dulo responsÃ¡vel por transformar dados em indicadores reutilizÃ¡veis.

Essa camada nÃ£o deve depender de tela especÃ­fica.

Ela existe para servir dashboard, relatÃ³rios, exportaÃ§Ãµes, APIs, mÃ³dulos de saÃºde financeira e futuras integraÃ§Ãµes.

## SaÃºde Financeira

Leitura interpretativa construÃ­da a partir dos indicadores financeiros e dos parÃ¢metros definidos no perfil financeiro do usuÃ¡rio.

## PontuaÃ§Ã£o de SaÃºde Financeira

Medida sintÃ©tica de 0 a 100 calculada a partir do conjunto de indicadores financeiros.

Ela resume, de forma simples, o equilÃ­brio financeiro atual do usuÃ¡rio.

## Insight Financeiro

Leitura acionÃ¡vel gerada a partir dos indicadores e da saÃºde financeira.

Pode representar:

- alerta
- oportunidade
- destaque positivo
- necessidade de configuraÃ§Ã£o

## ResumoFinanceiroIA

Objeto consolidado da inteligÃªncia financeira do sistema.

Ele organiza, em um Ãºnico payload:

- indicadores
- saÃºde financeira
- insights
- prioridades imediatas
- destaques positivos
- resumo executivo textual

## Contexto para IA

RepresentaÃ§Ã£o textual e estruturada do `ResumoFinanceiroIA`, preparada pelo sistema para consumo externo sem expor dados desnecessÃ¡rios do banco.

## Provedor de IA

AbstraÃ§Ã£o de infraestrutura responsÃ¡vel por receber um contexto jÃ¡ preparado e, futuramente, enviar esse material para um serviÃ§o externo de IA.

O provedor nunca deve acessar o banco diretamente.

## Assistente Financeiro

ExperiÃªncia executiva do sistema que organiza saÃºde financeira, indicadores, insights e prioridades a partir do `ResumoFinanceiroIA`.

Na fase atual, funciona sem IA real e sem chat.

## ParÃ¢metro Financeiro

Valor configurado pelo usuÃ¡rio para servir de referÃªncia em anÃ¡lises, indicadores, alertas e comparaÃ§Ãµes futuras.

## ConfiguraÃ§Ã£o vigente

Registro atualmente ativo dentro de um histÃ³rico de configuraÃ§Ãµes.

Em mÃ³dulos histÃ³ricos, representa a configuraÃ§Ã£o que deve ser considerada como referÃªncia atual do usuÃ¡rio.

## HistÃ³rico do Perfil Financeiro

Conjunto temporal de configuraÃ§Ãµes anteriores do perfil financeiro do usuÃ¡rio.

Permite saber quais critÃ©rios estavam vÃ¡lidos em cada perÃ­odo.

## AÃ§Ã£o de SimulaÃ§Ã£o

Evento hipotÃ©tico dentro de uma simulaÃ§Ã£o financeira.

Na V1 pode representar:

- receita Ãºnica
- despesa Ãºnica
- receita recorrente mensal
- despesa recorrente mensal
- despesa parcelada

## Resultado simulado

ConsolidaÃ§Ã£o mensal que combina base real de lanÃ§amentos com aÃ§Ãµes hipotÃ©ticas.

## Saldo real

Saldo mensal obtido apenas dos lanÃ§amentos reais.

## Saldo simulado

Saldo mensal obtido ao aplicar as aÃ§Ãµes simuladas sobre a base real.

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

Leitura analítica que mostra quanto da renda dos próximos 30 dias já está comprometida com despesas pendentes.

Ela complementa o `Comprometimento da renda` ao observar a pressão do caixa no curtíssimo prazo.

## Pontuação de Saúde Financeira

Medida sintética de 0 a 100 calculada a partir de uma média ponderada dos indicadores financeiros.

Indicadores de contexto ou configuração têm peso reduzido quando servem mais como régua pessoal do que como medida direta de desempenho.

