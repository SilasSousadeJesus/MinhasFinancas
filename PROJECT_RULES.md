# Project Rules

Use este arquivo para registrar acordos operacionais e arquiteturais do projeto.

## Filosofia do projeto

- Toda regra de negócio fica na camada `Application`.

# Regras Permanentes do Projeto

## Princípios Gerais

- Toda implementação deve priorizar rastreabilidade, organização, histórico e controle financeiro.
- Evitar soluções improvisadas que resolvem apenas o problema imediato.
- Sempre que possível, modelar os dados pensando em consultas futuras, auditoria, agrupamento e relatórios.
- Não apagar informações financeiras importantes sem deixar histórico ou rastreabilidade.
- Preferir modelos explícitos em vez de regras escondidas em texto, descrição ou convenções frágeis.

## Arquitetura

- `Controller` não deve conter regra de negócio.
- `Services/Application` devem concentrar as regras de negócio.
- `Repository` deve ser responsável por acesso a dados, não por decisões de negócio.
- Evitar duplicação de lógica entre frontend e backend.
- Regras críticas devem ser validadas no backend, mesmo que também existam validações no frontend.
- Infraestruturas globais devem ser reutilizáveis, desacopladas e não específicas de uma tela.

## Evolução incremental

- Sempre que uma funcionalidade fizer parte de um plano maior, implementar primeiro uma versão simples, porém arquiteturalmente correta.
- Evitar implementar toda a visão futura de uma única vez.
- Cada versão deve deixar a arquitetura preparada para a próxima evolução, sem gerar retrabalho ou quebra de compatibilidade.

## Frontend

- Evitar loading manual espalhado pelas páginas.
- Preferir componentes globais e reutilizáveis.
- Labels devem ser claros e não induzir o usuário ao erro.
- Filtros devem deixar explícito qual campo estão filtrando.
- Toda operação demorada deve dar feedback visual ao usuário.

## Organização da Interface

- Toda tela deve ser organizada de acordo com o fluxo mental do usuário, e não pela ordem em que as funcionalidades foram implementadas.
- Campos relacionados devem permanecer agrupados visualmente.
- Sempre que possível, utilizar a seguinte hierarquia:
  1. Pesquisa
  2. Classificação
  3. Períodos
  4. Ordenação
  5. Ações
- Evitar distribuir informações relacionadas em regiões diferentes da tela.
- A interface deve permitir que o usuário compreenda rapidamente onde encontrar determinada informação, reduzindo o esforço cognitivo e melhorando a produtividade.

## Padronização de Interface

- Toda interface deve utilizar português brasileiro.
- Todos os textos devem possuir acentuação correta.
- Utilizar linguagem simples, natural e profissional.
- Manter consistência de nomenclatura em todo o sistema.
- Evitar abreviações desnecessárias.
- Sempre revisar novos textos antes de finalizar uma implementação.

## Encoding dos arquivos

- Todos os arquivos de código-fonte, componentes, arquivos de configuração e documentos devem ser salvos em UTF-8.
- Nunca remover acentos para evitar problemas de encoding.
- Caso seja identificado problema de codificação, a solução deve ser corrigir o encoding do arquivo, e não alterar a grafia das palavras.

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
- O arquivo `AI_CONTEXT.md` será a principal documentação técnica deste projeto e deve ser tratado como um documento vivo.
- Sempre que uma funcionalidade for implementada, removida ou alterada de forma relevante, este arquivo deve ser atualizado para refletir o estado atual do sistema.
- Ao implementar qualquer feature, além das alterações de código, verificar se é necessário atualizar o `AI_CONTEXT.md`.
- Nunca deixar o documento desatualizado em relação ao projeto.

## Roadmap e Evolução

- Ideias de funcionalidades futuras devem ser registradas no `AI_CONTEXT.md`, em uma seção específica chamada `Roadmap do Projeto`.
- O Roadmap deve conter apenas funcionalidades futuras e nunca o estado atual do sistema.
- O Roadmap deve ser organizado por módulo, como Dashboard, Lançamentos, Projeções, Metas, Patrimônio, Cartões etc.
- Funcionalidades ainda não implementadas devem ficar claramente marcadas como futuras.
- Ao implementar uma funcionalidade listada no Roadmap, removê-la da lista de pendências e atualizar a documentação técnica nas demais seções do `AI_CONTEXT.md`.
- O histórico da implementação não pertence ao Roadmap; ele deve ser registrado no `CHANGELOG.md`.
