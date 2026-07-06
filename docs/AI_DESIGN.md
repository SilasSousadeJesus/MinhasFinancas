# AI Design

Este documento registra as decisões de design da camada de IA do projeto.

Ele complementa:

- `AI_CONTEXT.md`, que documenta arquitetura e fluxo técnico
- `docs/MODULE_GUIDE.md`, que documenta o papel funcional do Assistente Financeiro
- `docs/ROADMAP.md`, que documenta a evolução futura da IA no produto

## Visão geral

A IA do projeto não consulta diretamente o banco de dados nem opera sobre entidades brutas.

Ela recebe um contexto já consolidado pelo sistema, preparado a partir da inteligência financeira existente.

O objetivo é transformar esse contexto em uma análise executiva mais rica, clara e educativa para o usuário.

## Filosofia

Princípios obrigatórios:

- a IA deve explicar antes de recomendar
- a IA deve ensinar antes de aconselhar
- a IA deve atuar como consultora financeira prudente, e não como promotora de decisões impulsivas
- a IA deve respeitar os limites do contexto recebido
- a IA deve complementar a inteligência do sistema, nunca substituí-la

O Assistente Financeiro deve contribuir para educação financeira.

Sempre que possível, a resposta deve mostrar por que uma recomendação importa e qual impacto ela tende a gerar.

## Fluxo oficial

O fluxo oficial da análise com IA é:

`Dados -> Indicadores Financeiros -> Saúde Financeira -> Insights Financeiros -> ResumoFinanceiroIA -> ConstrutorContextoIA -> ConstrutorPromptIA -> IA -> Relatório Executivo`

Esse fluxo garante separação entre:

- dados persistidos
- cálculos analíticos
- interpretação baseada em regras
- geração textual por IA

## Responsabilidades

### Backend

O backend é responsável por:

- consolidar dados financeiros
- calcular indicadores
- montar a saúde financeira
- gerar insights baseados em regras
- produzir o `ResumoFinanceiroIA`
- transformar esse resumo em contexto seguro para uso externo
- construir o prompt final
- chamar o provedor de IA

### IA

A IA é responsável por:

- interpretar o contexto consolidado recebido
- produzir uma análise executiva em linguagem natural
- conectar causas, riscos, oportunidades e prioridades
- organizar a resposta no formato esperado pelo sistema

A IA não deve:

- inventar dados
- recalcular indicadores
- contradizer o contexto preparado pelo backend
- prometer resultados financeiros

### Frontend

O frontend é responsável por:

- exibir o relatório executivo
- preservar a separação entre conteúdo técnico e leitura executiva
- apresentar a análise sem recalcular nada localmente

## Contexto enviado para a IA

O contexto enviado para a IA nasce exclusivamente do `ResumoFinanceiroIA`.

Ele é estruturado pelo `ConstrutorContextoIA` em blocos organizados, incluindo:

- data de referência
- pontuação da saúde financeira
- classificação
- resumo executivo do sistema
- prioridades imediatas
- destaques positivos
- insights prioritários

O contexto deve ser suficiente para gerar uma boa resposta sem expor a base inteira do sistema.

## Engenharia de prompt

O prompt oficial da Fase 4.2 deve orientar a IA a agir como consultora financeira experiente, com as seguintes características:

- linguagem clara
- tom respeitoso
- postura prudente
- foco educativo
- orientação prática

Estrutura obrigatória da resposta:

1. Diagnóstico
2. Principais riscos
3. Pontos positivos
4. Recomendações
5. Plano de ação
6. Conclusão

O prompt também deve forçar:

- separação entre explicação e recomendação
- plano de ação com no máximo 5 prioridades
- ausência de listas excessivas
- ausência de repetição do contexto recebido
- linguagem natural e não robótica

## Segurança

Diretrizes obrigatórias:

- a IA nunca consulta diretamente o banco
- a IA nunca recebe credenciais
- logs técnicos não devem registrar prompt completo nem resposta completa
- a chave da API deve ficar fora do repositório
- o contexto deve ser sempre preparado pelo backend antes da chamada externa

## Especialistas futuros

A infraestrutura foi desenhada para permitir especialistas futuros com a mesma base técnica.

Exemplos:

- especialista em dívidas
- especialista em patrimônio
- especialista em metas
- especialista em fluxo de caixa
- especialista em simulações

Nesses casos, a infraestrutura permanece a mesma e muda principalmente:

- o contexto enviado
- o prompt utilizado
- o objetivo da análise

## Evolução futura

### Interpretador Financeiro

Melhoria futura já identificada:

Criar uma camada entre `Saúde Financeira` e `Insights Financeiros` para transformar indicadores em interpretações humanas baseadas exclusivamente em regras.

Fluxo futuro desejado:

`Dados -> Indicadores Financeiros -> Saúde Financeira -> Interpretador Financeiro -> Insights Financeiros -> ResumoFinanceiroIA -> Assistente Financeiro -> IA`

Essa camada ajudará a:

- reduzir repetição textual
- enriquecer a linguagem executiva
- servir como base ainda melhor para respostas com IA

## Estado atual

Situação atual da evolução:

- Fase 4.1 concluída: integração técnica com IA já existe
- Fase 4.2 iniciada: primeiro prompt oficial de análise financeira com IA implementado

Nesta etapa, o foco é melhorar:

- qualidade do prompt
- formato do relatório executivo
- consistência do tom do assistente
- valor consultivo das respostas
