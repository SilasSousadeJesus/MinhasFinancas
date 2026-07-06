# Roadmap - Minhas Finanças

Este documento contém apenas evolução do produto.

Ele não representa a arquitetura atual nem o histórico técnico detalhado.

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
- saúde financeira
- insights financeiros básicos
- ResumoFinanceiroIA

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

### Dashboard

- patrimônio líquido no dashboard
- evolução patrimonial no dashboard
- saldo projetado
- metas em risco
- resumo financeiro inteligente

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

- geração de insights com base no histórico

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
