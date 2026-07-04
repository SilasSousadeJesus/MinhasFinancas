# Project Rules

Use este arquivo para registrar acordos operacionais e arquiteturais do projeto.

## Filosofia do projeto

- Toda regra de negocio fica na camada Application.

# Regras Permanentes do Projeto

## Princípios Gerais

- Toda implementação deve priorizar rastreabilidade, organização, histórico e controle financeiro.
- Evitar soluções improvisadas que resolvem apenas o problema imediato.
- Sempre que possível, modelar os dados pensando em consultas futuras, auditoria, agrupamento e relatórios.
- Não apagar informações financeiras importantes sem deixar histórico ou rastreabilidade.
- Preferir modelos explícitos em vez de regras escondidas em texto, descrição ou convenções frágeis.

## Arquitetura

- Controller não deve conter regra de negócio.
- Services/Application devem concentrar as regras de negócio.
- Repository deve ser responsável por acesso a dados, não por decisões de negócio.
- Evitar duplicação de lógica entre frontend e backend.
- Regras críticas devem ser validadas no backend, mesmo que também existam validações no frontend.
- Infraestruturas globais devem ser reutilizáveis, desacopladas e não específicas de uma tela.

## Frontend

- Evitar loading manual espalhado pelas páginas.
- Preferir componentes globais e reutilizáveis.
- Labels devem ser claros e não induzir o usuário ao erro.
- Filtros devem deixar explícito qual campo estão filtrando.
- Toda operação demorada deve dar feedback visual ao usuário.

## Dados Financeiros

- Lançamentos gerados automaticamente devem manter vínculo com sua origem.
- Lançamentos parcelados devem possuir identificador de grupo, número da parcela e total de parcelas.
- Lançamentos fixos ou programados devem permitir rastrear que foram gerados a partir de uma mesma configuração.
- Alterações em lançamentos financeiros devem preservar o máximo possível de contexto histórico.
- Campos importantes para relatórios futuros devem ser estruturados, não derivados apenas da descrição.

## Manutenção com IA

- Antes de implementar qualquer feature, ler `AI_CONTEXT.md` e `PROJECT_RULES.md`.
- Sempre que uma regra de negócio, entidade, fluxo ou infraestrutura mudar, avaliar se `AI_CONTEXT.md` precisa ser atualizado.
- `AI_CONTEXT.md` deve refletir o estado atual do sistema.
- `PROJECT_RULES.md` deve conter apenas regras permanentes do projeto.
- Ao final de cada implementação, informar se `AI_CONTEXT.md` ou `PROJECT_RULES.md` foram atualizados.
- O arquivo AI_CONTEXT.md será a principal documentação técnica deste projeto e deve ser tratado como um documento vivo.
- Sempre que uma funcionalidade for implementada, removida ou alterada de forma relevante, este arquivo deve ser atualizado para refletir o estado atual do sistema.
- Ao implementar qualquer feature, além das alterações de código, verifique se é necessário atualizar o AI_CONTEXT.md.
-  Nunca deixe o documento desatualizado em relação ao projeto.
