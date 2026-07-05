# Project Rules

Use este arquivo para registrar apenas regras permanentes de desenvolvimento do projeto.

Não registrar roadmap, histórico de funcionalidades ou changelog neste documento.

## Filosofia do projeto

- Toda regra de negócio fica prioritariamente na camada `Application`.

## Princípios gerais

- Toda implementação deve priorizar rastreabilidade, histórico, clareza e controle financeiro.
- Evitar soluções improvisadas que resolvem apenas o problema imediato.
- Sempre que possível, modelar os dados pensando em consultas futuras, auditoria, agrupamento e relatórios.
- Não apagar informações financeiras relevantes sem preservar rastreabilidade.
- Preferir modelos explícitos em vez de regras escondidas em descrições ou convenções frágeis.

## Arquitetura

- `Controller` não deve conter regra de negócio.
- `Application/Services` devem concentrar a regra de negócio operacional.
- `Repository` deve cuidar de acesso a dados, não de decisões de negócio.
- Evitar duplicação de lógica entre frontend e backend.
- Regras críticas devem ser validadas no backend, mesmo quando existirem no frontend.
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

## Organização da interface

- Toda tela deve ser organizada de acordo com o fluxo mental do usuário, e não pela ordem em que as funcionalidades foram implementadas.
- Campos relacionados devem permanecer agrupados visualmente.
- Sempre que possível, utilizar a seguinte hierarquia:
  1. Pesquisa
  2. Classificação
  3. Períodos
  4. Ordenação
  5. Ações
- Evitar distribuir informações relacionadas em regiões diferentes da tela.
- A interface deve reduzir esforço cognitivo e facilitar produtividade.

## Padronização de interface

- Toda interface deve utilizar português brasileiro.
- Todos os textos devem possuir acentuação correta.
- Utilizar linguagem simples, natural e profissional.
- Manter consistência de nomenclatura em todo o sistema.
- Evitar abreviações desnecessárias.
- Sempre revisar novos textos antes de finalizar uma implementação.

## Encoding dos arquivos

- Todos os arquivos de código-fonte, componentes, arquivos de configuração e documentos devem ser salvos em UTF-8.
- Nunca remover acentos para contornar problema de encoding.
- Caso exista problema de codificação, corrigir o encoding do arquivo em vez de alterar a grafia.

## Dados financeiros

- Lançamentos gerados automaticamente devem manter vínculo com sua origem.
- Lançamentos parcelados devem possuir identificador de grupo, número da parcela e total de parcelas.
- Lançamentos fixos ou programados devem permitir rastrear que foram gerados a partir de uma mesma configuração.
- Alterações em lançamentos financeiros devem preservar o máximo possível de contexto histórico.
- Campos importantes para relatórios futuros devem ser estruturados, não derivados apenas de descrição.

## Manutenção com IA

- Antes de implementar qualquer feature, ler `AI_CONTEXT.md` e `PROJECT_RULES.md`.
- Ao alterar arquitetura, fluxo técnico, infraestrutura ou padrão estrutural, avaliar atualização do `AI_CONTEXT.md`.
- Ao surgir nova regra permanente de desenvolvimento, atualizar `PROJECT_RULES.md`.
- Ao final de cada implementação, informar se a documentação foi atualizada.

## Documentação viva

- Toda implementação relevante deve atualizar a documentação correspondente.
- Não utilizar o `AI_CONTEXT.md` como repositório de todas as informações do projeto.
- Cada informação deve ser registrada apenas no documento responsável por aquele assunto.
- A documentação faz parte do código e deve evoluir junto com ele.
