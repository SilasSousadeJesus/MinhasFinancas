# Guia dos Módulos - Minhas Finanças

Este documento explica o papel funcional de cada módulo do sistema.

Ele existe para responder, de forma simples:

- para que a tela serve
- o que o usuário faz nela
- quais dados ela produz
- como esses dados impactam o restante do sistema

Este arquivo não substitui:

- `AI_CONTEXT.md`, que documenta arquitetura e fluxo técnico
- `docs/DOMAIN_GLOSSARY.md`, que documenta conceitos oficiais do domínio
- `docs/ROADMAP.md`, que documenta evolução futura do produto

## Dashboard

### Finalidade
É a visão geral da situação financeira do usuário. O dashboard existe para transformar dados espalhados em uma leitura rápida do momento atual, dos riscos próximos e dos principais números de acompanhamento.

### O que o usuário faz aqui
- visualiza os principais agregados financeiros
- acompanha gráficos e comparativos
- identifica próximos vencimentos e contas atrasadas
- acessa atalhos para operações importantes, como gerenciamento de contas e cartões

### Dados gerados ou mantidos
- não é um módulo de cadastro principal
- consome dados agregados de lançamentos, contas, cartões e categorias
- consome indicadores calculados pela camada analítica
- consome parâmetros do perfil financeiro para contextualizar indicadores

### Impacto no restante do sistema
- depende fortemente de lançamentos, contas, cartões e categorias
- deverá consumir dados de patrimônio para mostrar patrimônio líquido e evolução patrimonial
- já consome dados do perfil financeiro para calcular indicadores e deverá ampliar esse uso em alertas e leituras de saúde financeira
- poderá consumir metas, projeções e simulações para mostrar risco, progresso e cenário futuro

### O que já está funcional
- agregados principais
- gráficos do dashboard
- radar financeiro com próximos vencimentos, contas atrasadas, alertas e fluxo de caixa próximo
- o dashboard deixou de repetir a leitura analítica detalhada da saúde financeira
- atalhos operacionais

### Evoluções futuras
- patrimônio líquido e evolução patrimonial no dashboard
- saldo projetado
- metas em risco
- resumo financeiro inteligente

## Saúde Financeira

### Finalidade
É a tela analítica da saúde financeira. Seu papel é traduzir os indicadores da análise financeira em uma leitura detalhada da situação atual do usuário.

### O que o usuário faz aqui
- visualiza a pontuação geral da saúde financeira
- entende a classificação atual
- identifica os principais pontos de atenção
- acompanha todos os indicadores com valor atual, valor ideal, status e observação
- consulta insights financeiros centralizados na própria tela
- enxerga a área preparada para gráficos analíticos futuros

### Dados gerados ou mantidos
- não cria dados próprios
- consome o painel de indicadores financeiros
- consome o resumo interpretativo da saúde financeira

### Impacto no restante do sistema
- reutiliza a mesma camada analítica do dashboard
- reforça o uso do perfil financeiro como régua pessoal
- já serve de base para a primeira versão de insights financeiros e do `ResumoFinanceiroIA`
- continuará servindo de base para relatórios, alertas e leituras futuras de saúde financeira

### O que já está funcional
- tela dedicada de saúde financeira
- pontuação geral de 0 a 100
- classificação textual
- pontos de atenção
- cards com todos os indicadores calculados
- valor atual, valor ideal, status e observação por indicador
- insights financeiros centralizados nesta tela
- área reservada para gráficos de evolução patrimonial, economia mensal e reserva de emergência
- geração backend de insights financeiros e de resumo consolidado para futuras interfaces e consumo por IA

### Evoluções futuras
- explicações mais profundas por indicador
- recomendações automáticas
- integração com relatórios e insights inteligentes

## Assistente Financeiro

Observações adicionais da fase técnica:

- a infraestrutura backend já monta um contexto estruturado por seções antes de qualquer chamada externa para IA
- a integração técnica já registra tempo, tokens e custo estimado por chamada sem expor prompt completo nem dados financeiros pessoais em logs

### Finalidade
É a tela executiva da inteligência financeira. Seu papel é organizar a situação atual do usuário em formato de prioridades, destaques e direcionamento prático para tomada de decisão.

### O que o usuário faz aqui
- acompanha a saúde financeira consolidada em formato executivo
- lê um resumo corrido com prioridades, principais indicadores, leitura estratégica e conclusão
- usa links para aprofundar a análise completa em Saúde Financeira
- futuramente poderá solicitar uma análise aprofundada com IA

### Dados gerados ou mantidos
- não criará dados financeiros brutos
- consumirá `ResumoFinanceiroIA` como fonte oficial de contexto consolidado
- futuramente poderá produzir relatórios executivos gerados a partir desse resumo

### Impacto no restante do sistema
- reutiliza a cadeia `Dados -> Indicadores -> Saúde Financeira -> Insights -> ResumoFinanceiroIA`
- dependerá da camada analítica e do resumo consolidado, sem recalcular nada na interface
- servirá como ponto central para futuras integrações com IA e especialistas temáticos

### O que já está funcional
- tela executiva do Assistente Financeiro já implementada
- consumo exclusivo de `ResumoFinanceiroIA`
- um único bloco principal de resumo executivo
- saúde financeira consolidada com pontuação e classificação em destaque
- seções internas por título: resumo, prioridades, principais indicadores, leitura estratégica e conclusão
- cada seção possui responsabilidade textual própria:
  - resumo explica a situação
  - prioridades mostram ações curtas
  - principais indicadores sustentam a leitura com números
  - leitura estratégica destaca forças e riscos
  - conclusão fecha o parecer do período
- os blocos textuais não reutilizam literalmente as descrições técnicas dos indicadores; o assistente usa frases interpretativas próprias para transformar os dados em linguagem executiva
- os insights permanecem como camada separada de alertas, oportunidades, configuração e destaques positivos
- principais indicadores em formato textual
- leitura estratégica com pontos fortes e pontos de atenção em formato textual
- conclusão dinâmica construída por regras com base na saúde financeira atual, sem IA generativa
- a conclusão usa interpretações executivas próprias por indicador, em vez de repetir literalmente descrições técnicas ou prioridades
- a abertura da conclusão varia conforme a classificação da saúde financeira
- link para a análise completa em `Saúde Financeira`
- próximas prioridades
- card final de análise aprofundada com IA com botão desabilitado
- a infraestrutura backend da Fase 2 continua preparada para uso futuro com provedores externos
- o backend já possui integração técnica real com OpenAI quando a chave estiver configurada no ambiente
- existe endpoint técnico para geração da análise, sem mudança obrigatória na experiência visual desta fase

### Evoluções futuras
- Fase 4.2: construção da primeira análise financeira aprofundada com IA, usando `ResumoFinanceiroIA` como única base de contexto
- futura camada de `Interpretador Financeiro` entre `Saúde Financeira` e `Insights Financeiros` para qualificar ainda mais a linguagem natural baseada em regras
- especialistas temáticos reutilizando a mesma infraestrutura
- conversa contínua com contexto financeiro consolidado

## Lançamentos

### Finalidade
É o núcleo operacional do sistema. Aqui o usuário registra receitas e despesas, organiza o passado e o futuro financeiro e constrói a base de quase todas as leituras do produto.

### O que o usuário faz aqui
- cria lançamentos
- edita lançamentos
- exclui lançamentos
- efetiva rapidamente pagamentos e recebimentos
- filtra, ordena e pagina a listagem
- exporta os resultados filtrados para Excel
- gerencia parcelamentos em lote a partir de qualquer parcela vinculada ao grupo

### Dados gerados ou mantidos
- receitas
- despesas
- status do lançamento
- datas de lançamento, vencimento e efetivação
- categoria e subcategoria
- conta ou cartão vinculado
- agrupamentos de parcelamento e programação

### Impacto no restante do sistema
- alimenta dashboard e radar financeiro
- alimenta fluxo de caixa simples
- alimenta projeções atreladas a despesas
- alimenta simulações financeiras como base real
- impacta relatórios
- influencia leituras de patrimônio e metas em versões futuras

### O que já está funcional
- CRUD completo
- filtros
- paginação
- ordenação
- exportação para Excel respeitando os filtros aplicados e usando a infraestrutura compartilhada de relatórios
- efetivação rápida
- lançamentos únicos, parcelados, fixos e por dia útil
- gestão de parcelamentos por grupo, com visualização das parcelas e recálculo dos vencimentos das parcelas elegíveis

### Evoluções futuras
- visualização agrupada de parcelas
- edição em lote de parcelamentos
- exclusão parcial ou total de parcelamentos
- histórico de alterações

## Fluxo de Caixa Simples

### Finalidade
É uma tela de conferência rápida do mês. Serve para responder, de forma objetiva, como está o fluxo financeiro do período sem exigir navegação por muitos gráficos ou filtros.

### O que o usuário faz aqui
- navega entre meses
- visualiza receitas do mês
- visualiza despesas do mês
- acompanha o saldo do mês
- confere listas separadas de receitas e despesas
- exporta o mês atual, um intervalo de meses ou um ano inteiro para Excel

### Dados gerados ou mantidos
- não cria dados próprios
- consolida dados dos lançamentos com base na data de vencimento

### Impacto no restante do sistema
- depende dos lançamentos já cadastrados
- pode consumir perfil financeiro no futuro para comparar saldo real com economia desejada
- pode servir como base de leitura para relatórios mensais

### O que já está funcional
- navegação mensal
- resumo com receitas, despesas e saldo
- gráfico comparativo simples
- listagem de receitas
- listagem de despesas
- exportação para Excel com uma aba por mês exportado

### Evoluções futuras
- comparação com metas mensais
- leitura comparativa com meses anteriores
- alertas simples do mês
- consumo dos parâmetros do perfil financeiro

## Projeções

### Finalidade
Permite planejar quando um objetivo financeiro poderá ser alcançado com base em renda, dívidas, acumulado inicial e horizonte de tempo.

### O que o usuário faz aqui
- cria várias projeções independentes
- define objetivo financeiro
- informa acumulado inicial
- cadastra rendas base
- ajusta rendas extras por mês
- escolhe se a projeção será atrelada às despesas reais ou manual
- acompanha a evolução mês a mês

### Dados gerados ou mantidos
- projeções por usuário
- rendas da projeção
- rendas extras mensais
- dívidas manuais mensais
- configuração de vínculo com despesas reais

### Impacto no restante do sistema
- pode consumir lançamentos reais para montar as despesas da projeção
- futuramente pode dialogar com metas e perfil financeiro
- pode ser usada no dashboard como visão de alcance de objetivos

### O que já está funcional
- múltiplas projeções
- cards de resumo
- detalhamento da projeção
- tabela mensal
- gráfico visual
- modo atrelado a despesas ou manual

### Evoluções futuras
- integração mais forte com metas
- leitura comparativa com perfil financeiro
- indicadores de viabilidade
- cenários mais avançados

## Patrimônio

### Finalidade
É o módulo que mostra a fotografia patrimonial do usuário e sua evolução ao longo do tempo. Ele existe para responder quanto o usuário tem em ativos, quanto deve e qual é seu patrimônio líquido.

### O que o usuário faz aqui
- cadastra ativos
- cadastra passivos
- edita registros patrimoniais
- inativa itens
- gera snapshots manuais
- acompanha o histórico patrimonial

### Dados gerados ou mantidos
- ativos patrimoniais
- passivos
- snapshots patrimoniais
- histórico de evolução do patrimônio líquido

### Impacto no restante do sistema
- deverá alimentar o dashboard com patrimônio líquido e evolução patrimonial
- pode ser comparado com patrimônio líquido alvo do perfil financeiro
- poderá ser usado em relatórios e planejamento financeiro

### O que já está funcional
- CRUD de ativos
- CRUD de passivos
- cálculo do patrimônio líquido atual
- geração manual de snapshot
- histórico de snapshots
- gráfico de evolução patrimonial

### Evoluções futuras
- integração automática com contas, cartões e lançamentos
- snapshots automáticos por período
- relatórios patrimoniais dedicados

## Simulações Financeiras

### Finalidade
Serve para testar cenários hipotéticos sem alterar os dados reais do usuário. É o espaço para comparar decisões antes de colocá-las em prática.

### O que o usuário faz aqui
- cria simulações
- define período da simulação
- adiciona ações hipotéticas
- compara fluxo real e fluxo simulado
- acompanha o impacto mês a mês

### Dados gerados ou mantidos
- simulações financeiras
- ações de simulação
- resultado consolidado por período

### Impacto no restante do sistema
- consome lançamentos reais como base de comparação
- pode consumir perfil financeiro para avaliar qualidade do cenário
- poderá conversar com patrimônio, metas e projeções em versões futuras

### O que já está funcional
- overview de simulações
- tela de edição detalhada
- ações simuladas persistidas
- cálculo comparativo mensal entre real e simulado

### Evoluções futuras
- comparação entre múltiplas simulações
- duplicação rápida de cenários
- impacto em patrimônio líquido
- impacto em metas
- importação de ações a partir de lançamentos reais

## Perfil Financeiro

### Finalidade
É a régua pessoal de avaliação da saúde financeira do usuário. Esse módulo não registra movimentações; ele define os parâmetros que dizem o que é saudável, aceitável ou desejado para aquela pessoa.

### O que o usuário faz aqui
- define percentual desejado de economia mensal
- define percentual desejado de reserva de emergência
- define quantidade ideal de meses de reserva
- define limite de comprometimento da renda
- define limite de endividamento
- define investimento mínimo desejado
- define patrimônio líquido alvo
- registra observações e contexto

### Dados gerados ou mantidos
- perfil financeiro do usuário
- histórico de configurações
- configuração vigente
- parâmetros pessoais de leitura financeira

### Impacto no restante do sistema
- já é consumido pela camada analítica para calcular indicadores financeiros
- já é consumido pelo dashboard para contextualizar indicadores
- será consumido pelo radar financeiro para alertas personalizados
- poderá ser consumido por patrimônio para comparar patrimônio atual com patrimônio alvo
- poderá ser consumido por fluxo de caixa simples para comparar saldo do mês com a economia desejada
- poderá ser consumido por projeções para avaliar se a trajetória planejada está alinhada ao padrão desejado
- poderá ser consumido por simulações financeiras para medir qualidade de cenários hipotéticos
- poderá ser consumido por metas como referência estratégica
- poderá ser consumido por relatórios e indicadores futuros

### O que já está funcional
- cadastro dos parâmetros financeiros principais
- atualização do perfil
- histórico de vigência
- leitura da configuração vigente
- consumo inicial pelos indicadores financeiros do dashboard
- consumo real pela tela de Saúde Financeira
- consumo pelos insights financeiros e pelo resumo consolidado da inteligência do sistema

### Evoluções futuras
- integração real com dashboard
- integração com alertas do radar financeiro
- score de saúde financeira
- insights automáticos com base no histórico

## Metas

### Finalidade
É o módulo voltado a objetivos financeiros acumulativos. Ele existe para transformar desejos financeiros em acompanhamento estruturado.

### O que o usuário faz aqui
- cadastra metas
- acompanha progresso
- consulta andamento das metas

### Dados gerados ou mantidos
- metas financeiras
- valores objetivos
- progresso associado

### Impacto no restante do sistema
- poderá ser consumido por dashboard
- poderá conversar com projeções e simulações
- poderá usar patrimônio e perfil financeiro como contexto de prioridade e viabilidade

### O que já está funcional
- existe base no backend
- existe estrutura de dados para o módulo

### Evoluções futuras
- fechamento completo do frontend
- integração com dashboard
- leitura de risco e progresso

## Contas e Cartões

### Finalidade
Organiza os meios financeiros utilizados pelo usuário. Esse módulo dá estrutura para que os lançamentos sejam vinculados a contas e cartões reais.

### O que o usuário faz aqui
- cadastra contas
- cadastra cartões
- edita dados
- remove ou inativa registros
- usa contas e cartões nos lançamentos

### Dados gerados ou mantidos
- contas
- cartões
- saldos e dados cadastrais associados

### Impacto no restante do sistema
- é consumido diretamente pelos lançamentos
- pode ser explorado no dashboard em análises por conta e cartão
- poderá influenciar patrimônio e relatórios futuros

### O que já está funcional
- CRUD de contas
- CRUD de cartões
- uso real no modal de lançamentos
- gerenciamento rápido via dashboard

### Evoluções futuras
- análises mais fortes por conta e cartão
- melhor integração com patrimônio
- relatórios específicos por meio financeiro

## Categorias e Subcategorias

### Finalidade
Classifica financeiramente os lançamentos. Esse módulo existe para dar significado aos números e permitir leitura analítica por tipo de gasto ou receita.

### O que o usuário faz aqui
- cria categorias
- cria subcategorias
- edita classificações
- gerencia a estrutura usada nos lançamentos

### Dados gerados ou mantidos
- categorias por usuário
- subcategorias por usuário
- relacionamento entre classificação e lançamentos

### Impacto no restante do sistema
- é consumido pelos lançamentos
- alimenta gráficos e análises do dashboard
- poderá alimentar relatórios por categoria e subcategoria
- ajuda projeções e simulações a ganharem contexto analítico

### O que já está funcional
- CRUD completo
- seed inicial no cadastro do usuário
- integração ponta a ponta com lançamentos

### Evoluções futuras
- refinamento da taxonomia inicial
- análises comparativas mais profundas
- relatórios mais detalhados por classificação

## Relatórios

### Finalidade
É o módulo voltado à leitura consolidada e histórica do sistema. Deve transformar os dados operacionais em visões mais analíticas e úteis para decisão.

### O que o usuário faz aqui
- consulta consolidações financeiras
- compara períodos
- analisa comportamento financeiro

### Dados gerados ou mantidos
- não deve ser um módulo de cadastro principal
- consome dados de lançamentos, patrimônio, perfil financeiro, metas e demais módulos analíticos

### Impacto no restante do sistema
- depende de quase todos os módulos financeiros
- deve sintetizar leituras do sistema em formatos de análise

### O que já está funcional
- existem serviços e base no backend

### Evoluções futuras
- fechamento do frontend
- relatórios por categoria, conta, cartão e período
- relatórios patrimoniais
- relatórios comparativos com perfil financeiro e metas

## Orçamento

### Finalidade
Será o módulo responsável por definir limites planejados de gasto e acompanhar aderência ao plano financeiro do usuário.

### O que o usuário faz aqui
- definirá valores orçados
- organizará limites por contexto financeiro
- acompanhará desvio entre orçado e realizado

### Dados gerados ou mantidos
- valores planejados de orçamento
- limites e referências para comparação com despesas reais

### Impacto no restante do sistema
- deverá impactar o dashboard
- deverá impactar leituras de lançamentos
- poderá dialogar com perfil financeiro, metas e relatórios

### O que já está funcional
- ainda não existe integração funcional concluída

### Evoluções futuras
- definição formal da regra de negócio
- integração com dashboard e lançamentos
- comparativo entre planejado e realizado

## Atualiza��o do m�dulo Assistente Financeiro

A partir da Fase 4.2, o m�dulo passa a ter tamb�m uma camada oficial de design documentada em `docs/AI_DESIGN.md`.

### Complemento funcional

- a infraestrutura t�cnica da Fase 4.1 permanece a mesma
- a Fase 4.2 iniciou a formaliza��o do comportamento textual da IA
- o primeiro prompt oficial foi estruturado para gerar uma an�lise consultiva, prudente, educativa e organizada em: diagn�stico, principais riscos, pontos positivos, recomenda��es, plano de a��o e conclus�o
- a IA continua consumindo exclusivamente o contexto consolidado derivado de `ResumoFinanceiroIA`
