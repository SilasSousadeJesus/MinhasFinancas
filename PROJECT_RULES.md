# Project Rules

Use este arquivo para registrar acordos operacionais e arquiteturais do projeto.

## Filosofia do projeto

- Toda regra de negocio fica na camada Application.

# Regras Permanentes do Projeto

## Principios Gerais

- Toda implementacao deve priorizar rastreabilidade, organizacao, historico e controle financeiro.
- Evitar solucoes improvisadas que resolvem apenas o problema imediato.
- Sempre que possivel, modelar os dados pensando em consultas futuras, auditoria, agrupamento e relatorios.
- Nao apagar informacoes financeiras importantes sem deixar historico ou rastreabilidade.
- Preferir modelos explicitos em vez de regras escondidas em texto, descricao ou convencoes frageis.

## Arquitetura

- Controller nao deve conter regra de negocio.
- Services/Application devem concentrar as regras de negocio.
- Repository deve ser responsavel por acesso a dados, nao por decisoes de negocio.
- Evitar duplicacao de logica entre frontend e backend.
- Regras criticas devem ser validadas no backend, mesmo que tambem existam validacoes no frontend.
- Infraestruturas globais devem ser reutilizaveis, desacopladas e nao especificas de uma tela.

## Frontend

- Evitar loading manual espalhado pelas paginas.
- Preferir componentes globais e reutilizaveis.
- Labels devem ser claros e nao induzir o usuario ao erro.
- Filtros devem deixar explicito qual campo estao filtrando.
- Toda operacao demorada deve dar feedback visual ao usuario.

## Dados Financeiros

- Lancamentos gerados automaticamente devem manter vinculo com sua origem.
- Lancamentos parcelados devem possuir identificador de grupo, numero da parcela e total de parcelas.
- Lancamentos fixos ou programados devem permitir rastrear que foram gerados a partir de uma mesma configuracao.
- Alteracoes em lancamentos financeiros devem preservar o maximo possivel de contexto historico.
- Campos importantes para relatorios futuros devem ser estruturados, nao derivados apenas da descricao.

## Manutencao com IA

- Antes de implementar qualquer feature, ler `AI_CONTEXT.md` e `PROJECT_RULES.md`.
- Sempre que uma regra de negocio, entidade, fluxo ou infraestrutura mudar, avaliar se `AI_CONTEXT.md` precisa ser atualizado.
- `AI_CONTEXT.md` deve refletir o estado atual do sistema.
- `PROJECT_RULES.md` deve conter apenas regras permanentes do projeto.
- Ao final de cada implementacao, informar se `AI_CONTEXT.md` ou `PROJECT_RULES.md` foram atualizados.
- O arquivo `AI_CONTEXT.md` sera a principal documentacao tecnica deste projeto e deve ser tratado como um documento vivo.
- Sempre que uma funcionalidade for implementada, removida ou alterada de forma relevante, este arquivo deve ser atualizado para refletir o estado atual do sistema.
- Ao implementar qualquer feature, alem das alteracoes de codigo, verificar se e necessario atualizar o `AI_CONTEXT.md`.
- Nunca deixar o documento desatualizado em relacao ao projeto.

## Roadmap e Evolucao

- Ideias de funcionalidades futuras devem ser registradas no `AI_CONTEXT.md`, em uma secao especifica chamada `Roadmap do Projeto`.
- O Roadmap deve conter apenas funcionalidades futuras e nunca o estado atual do sistema.
- O Roadmap deve ser organizado por modulo, como Dashboard, Lancamentos, Projecoes, Metas, Patrimonio, Cartoes etc.
- Funcionalidades ainda nao implementadas devem ficar claramente marcadas como futuras.
- Ao implementar uma funcionalidade listada no Roadmap, remove-la da lista de pendencias e atualizar a documentacao tecnica nas demais secoes do `AI_CONTEXT.md`.
- O historico da implementacao nao pertence ao Roadmap; ele deve ser registrado no `CHANGELOG.md`.
