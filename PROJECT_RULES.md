# Project Rules

Use este arquivo para registrar acordos operacionais e arquiteturais do projeto.

## Filosofia do projeto

- Toda regra de negocio fica na camada Application.

## Regras atuais

- Preencha aqui as regras que a IA deve seguir ao alterar o projeto.

## Exemplo de regras que podem ser adicionadas

- Nunca rodar migrations automaticamente.
- Nunca alterar contratos da API sem alinhar o frontend.
- Sempre manter `RetornoGenerico` como formato padrao de resposta.
- Sempre validar `usuarioId` antes de operar em dados do usuario.
- Toda nova tela autenticada deve usar o fluxo de sessao existente.
