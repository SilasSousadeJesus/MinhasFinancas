# Roadmap - Minhas Finanças

Este documento contém apenas evolução do produto.

Ele não representa a arquitetura atual nem o histórico técnico detalhado.

## Estado atual do roadmap

O projeto se encontra atualmente na **Fase 3 concluída** do roadmap de Inteligência Financeira / Assistente Financeiro.

A **próxima etapa oficial** é a **Fase 4 — Integração com IA**.

## Roadmap da Inteligência Financeira / Assistente Financeiro

### Fase 1 — Inteligência do Sistema (sem IA)

**Status:** concluída

**Objetivo:**
Fazer o sistema entender a situação financeira do usuário sozinho.

**Entregas:**

- Indicadores Financeiros
- Saúde Financeira
- Insights Financeiros
- ResumoFinanceiroIA

### Fase 2 — Infraestrutura de IA

**Status:** concluída

**Objetivo:**
Criar a infraestrutura de integração com IA sem consumir nenhum provedor.

**Escopo entregue:**

- `IProvedorIA`
- `OpenAIProvider`
- `ConstrutorPromptIA`
- `ConstrutorContextoIA`
- `AssistenteFinanceiroService`

**Resultado desta fase:**

- a infraestrutura já prepara contexto e prompt a partir de `ResumoFinanceiroIA`
- o provedor permanece simulado
- nenhum token é consumido
- nenhuma IA real é chamada

### Fase 3 — Assistente Financeiro

**Status:** concluída

**Objetivo:**
Criar a tela do Assistente Financeiro.

Essa tela deve funcionar completamente mesmo sem IA.

Ela deve consumir apenas o `ResumoFinanceiroIA`.

**Exibir:**

- Saúde Financeira
- Indicadores
- Insights
- Gráficos
- Tendências
- Histórico
- Recomendações produzidas pelo sistema

**Observação:**

Ao final da tela existirá um botão:

`Gerar análise aprofundada com IA`

Esse botão pode permanecer desabilitado até a Fase 4.

### Fase 4 — Integração com IA

**Status:** futura

**Objetivo:**
Integrar o primeiro provedor (`OpenAI`).

**Fluxo:**

`ResumoFinanceiroIA`
↓
`ConstrutorContextoIA`
↓
`ConstrutorPromptIA`
↓
`OpenAIProvider`
↓
`Relatório Executivo`

**Regra central:**

A IA nunca consulta diretamente o banco.

Ela recebe exclusivamente o `ResumoFinanceiroIA`.

### Fase 5 — Especialistas

**Status:** futura

Criar especialistas utilizando a mesma infraestrutura.

**Exemplos:**

- Especialista em Dívidas
- Especialista em Patrimônio
- Especialista em Metas
- Especialista em Fluxo de Caixa
- Especialista em Simulações

**Nesta fase muda apenas:**

- prompt
- contexto enviado

Toda a infraestrutura permanece a mesma.

### Fase 6 — Conversa Contínua

**Status:** futura

Permitir que o usuário converse com o Assistente Financeiro utilizando todo o contexto financeiro já consolidado.

**Exemplos:**

- Vale a pena quitar meu empréstimo?
- E se eu comprar uma casa?
- Quanto posso gastar em um carro?
- Essa simulação é saudável?

## Concluído

### Fundação

- autenticação com JWT
- cliente HTTP padronizado
- loading global de requisições
- migração do banco oficial para MySQL

### Operação financeira

- categorias e subcategorias
- contas e cartões
- lançamentos com CRUD
- filtros, ordenação, paginação e exportação Excel
- efetivação rápida de receitas e despesas
- lançamentos únicos, parcelados, fixos e por dia útil

### Leitura financeira

- dashboard
- radar financeiro
- fluxo de caixa simples
- indicadores financeiros
- saúde financeira
- insights financeiros básicos
- ResumoFinanceiroIA
- infraestrutura de IA preparada sem integração real com provedor
- assistente financeiro consumindo apenas o resumo consolidado do backend

### Planejamento

- projeções financeiras
- simulações financeiras
- perfil financeiro

### Patrimônio

- ativos patrimoniais
- passivos
- snapshots patrimoniais
- evolução patrimonial

## Em desenvolvimento

### Metas

- integração visual completa do módulo no frontend

### Relatórios

- fechamento do frontend para consumo dos relatórios já existentes no backend

### Orçamento

- definição da regra de negócio e integração real com a aplicação

## Próximas implementações

### Inteligência Financeira / Assistente Financeiro

- Fase 4 — Integração com IA
- Interpretador Financeiro
  - criar uma camada entre `Saúde Financeira` e `Insights Financeiros`
  - objetivo: transformar indicadores em interpretações humanas baseadas exclusivamente em regras de negócio
  - motivação: evitar que `Insights`, `ResumoFinanceiroIA` e futuras interfaces reutilizem textos técnicos dos indicadores
  - fluxo desejado:
    - `Dados`
    - `Indicadores Financeiros`
    - `Saúde Financeira`
    - `Interpretador Financeiro`
    - `Insights Financeiros`
    - `ResumoFinanceiroIA`
    - `Assistente Financeiro`
    - `IA (quando utilizada)`
  - responsabilidades esperadas:
    - traduzir indicadores em interpretações humanas
    - explicar o impacto dos números na vida financeira
    - produzir frases contextualizadas sem uso de IA generativa
    - evitar que `Insights` e `ResumoFinanceiroIA` reutilizem textos técnicos dos indicadores
    - servir como camada de conhecimento para futuras integrações com IA
  - esta melhoria é uma evolução futura independente e não faz parte da Fase 4

### Dashboard

- patrimônio líquido no dashboard
- evolução patrimonial no dashboard
- saldo projetado
- metas em risco
- resumo financeiro inteligente na interface, consumindo `ResumoFinanceiroIA`

### Lançamentos

- modal para visualização de parcelas agrupadas
- edição em lote de parcelamentos
- exclusão individual ou total de parcelamentos
- histórico de alterações

### Patrimônio

- integração automática com contas, cartões e lançamentos
- geração automática de snapshots por período
- relatório patrimonial dedicado

### Metas

- experiência completa de uso no frontend
- leitura consolidada junto ao dashboard e ao planejamento financeiro

### Perfil Financeiro

- evolução dos insights com base no histórico

## Ideias futuras

### Simulações Financeiras

- comparação entre múltiplas simulações
- aplicação parcial de simulação aos dados reais com confirmação explícita
- impacto em patrimônio líquido
- impacto em metas
- importação de ações a partir de lançamentos reais
- duplicação rápida de cenários

### Infraestrutura

- melhorias globais de performance
- melhorias de segurança
- melhorias de observabilidade
- melhorias de auditoria
- melhor aproveitamento de Hangfire e SignalR
