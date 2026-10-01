---
name: revisar-issue
description: Fase 3 do protocolo das três sessões do Gye-Nyame. Revisa o PR de uma issue contra os critérios de aceite, as ADRs e a documentação, e publica um relatório no PR, sem alterar nada. Use quando o usuário pedir para revisar uma issue ou o PR dela.
argument-hint: <número da issue>
disable-model-invocation: true
---

# Revisar a issue #$ARGUMENTS

Você é a **sessão de revisão**. Você **só relata**: não altera código, assets nem documentos, não faz commit e não faz merge. A independência em relação a quem escreveu o código é o motivo de esta sessão existir.

## 1. Confira o estado (regra de recusa)

```bash
bash tools/workflow/issue-state.sh status $ARGUMENTS
```

Só continue se **todas** forem verdadeiras:

- `estado` é `em-revisão`;
- `needs-decision` é `false`;
- `pr aberto` está preenchido.

Se alguma falhar, **recuse**: diga ao usuário qual condição falhou, não altere nada e termine.

## 2. Siga o roteiro

Leia `CLAUDE.md`, a issue, o último comentário de levantamento e o PR (`gh pr view`, `gh pr diff`). Depois execute a seção **"3. Revisão"** de `docs/workflow/agents.md`, incluindo a lista do que conferir, a classificação **bloqueante / não bloqueante** e o **formato do relatório de revisão**.

Carregue as mesmas skills listadas no levantamento, inclusive `clean-code`.

## 3. Encerre

Publique o relatório no PR e então:

- Sem achados bloqueantes: `bash tools/workflow/issue-state.sh set $ARGUMENTS aprovada`
- Com achados bloqueantes no **ciclo 1**: `bash tools/workflow/issue-state.sh set $ARGUMENTS pronta-para-execução`
- Com achados bloqueantes no **ciclo 2**: `gh issue edit $ARGUMENTS --add-label needs-decision`, mencione o arquiteto no relatório e mantenha o estado `em-revisão`.

Termine dizendo ao usuário o resultado, a contagem de achados de cada classe e o que ficou pendente para ele verificar jogando.
