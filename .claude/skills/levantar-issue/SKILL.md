---
name: levantar-issue
description: Fase 1 do protocolo das três sessões do Gye-Nyame. Faz o levantamento de uma issue do GitHub e publica um comentário estruturado nela, sem alterar código. Use quando o usuário pedir para levantar, pesquisar ou preparar uma issue para execução.
argument-hint: <número da issue>
disable-model-invocation: true
---

# Levantar a issue #$ARGUMENTS

Você é a **sessão de levantamento**. Você **não altera código, assets nem documentos**: o único resultado é um comentário na issue.

## 1. Confira o estado (regra de recusa)

```bash
bash tools/workflow/issue-state.sh status $ARGUMENTS
```

Só continue se **todas** forem verdadeiras:

- `estado` é `backlog` (primeiro levantamento) ou `levantamento` (retomada);
- `needs-decision` é `false`;
- `bloqueadoras abertas` está vazio.

Se alguma falhar, **recuse**: diga ao usuário qual condição falhou e o que precisa acontecer antes, não altere nada e termine.

## 2. Assuma a issue

```bash
bash tools/workflow/issue-state.sh set $ARGUMENTS levantamento
```

## 3. Siga o roteiro

Leia `CLAUDE.md` e depois execute a seção **"1. Levantamento"** de `docs/workflow/agents.md`, incluindo o **formato do comentário de levantamento**. Aquele documento é a fonte da verdade; não improvise outro formato.

Na retomada, leia também o levantamento anterior e as respostas do arquiteto, e publique um levantamento consolidado.

## 4. Encerre

- Sem perguntas em aberto: `bash tools/workflow/issue-state.sh set $ARGUMENTS pronta-para-execução`
- Com perguntas em aberto: `gh issue edit $ARGUMENTS --add-label needs-decision` e mantenha o estado `levantamento`.

Termine dizendo ao usuário o estado final e, se houver, as perguntas que ele precisa responder.
