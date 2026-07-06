# Roadmap - Minhas Finanças

Este documento contém apenas evolução do produto.

Ele não representa a arquitetura atual nem o histórico técnico detalhado.

## Estado atual do roadmap

O projeto se encontra atualmente na **Fase 4.1 concluída** do roadmap de Inteligência Financeira / Assistente Financeiro.

A **próxima etapa oficial** é a **Fase 4.2 — Primeira Análise Financeira com IA**.

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

Esse botão pode permanecer desabilitado até a Fase 4.1.

### Fase 4.1 — Integração Técnica com IA

**Status:** concluída

**Objetivo:**
Implementar apenas a comunicação técnica com o primeiro provedor de IA.

**Escopo:**

- configuração segura da API Key
- implementação definitiva do `OpenAIProvider`
- comunicação HTTP com a API
- tratamento de erros
- timeout
- retry
- configuração do modelo
- controle de tokens
- configuração por ambiente
- logs técnicos

**Importante:**

- esta fase não altera a experiência do usuário
- esta fase não melhora os textos
- esta fase não cria inteligência nova
- ela apenas faz a infraestrutura preparada na Fase 2 funcionar de verdade

**Resultado esperado:**

Ao final desta fase, o backend já consegue chamar a IA e obter uma resposta técnica quando a chave estiver configurada corretamente no ambiente.

### Fase 4.2 — Primeira Análise Financeira com IA

**Status:** futura

**Objetivo:**
Criar a primeira experiência real de análise financeira utilizando IA.

**Fluxo oficial:**

`ResumoFinanceiroIA`
↓
`ConstrutorContextoIA`
↓
`ConstrutorPromptIA`
↓
`OpenAIProvider`
↓
`Relatório Executivo`

**Nesta fase serão definidos:**

- prompt oficial
- formato do relatório
- estilo de escrita
- tom do assistente
- regras de segurança
- contexto enviado
- comportamento esperado da IA

**Regra central:**

A IA nunca consulta diretamente o banco.

Ela recebe exclusivamente o `ResumoFinanceiroIA`.

**Resultado esperado:**

Transformar o `ResumoFinanceiroIA` em um parecer financeiro muito mais rico do que o produzido apenas pelas regras do sistema.

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

- Fase 4.1 — Integração Técnica com IA
- Fase 4.2 — Primeira Análise Financeira com IA
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
    - `IA`
  - responsabilidades esperadas:
    - traduzir indicadores em linguagem natural
    - explicar impacto financeiro
    - produzir interpretações baseadas em regras
    - reduzir repetição textual
    - servir como base ainda melhor para futuras integrações com IA
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
