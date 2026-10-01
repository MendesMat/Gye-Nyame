---
name: executar-issue
description: Fase 2 do protocolo das três sessões do Gye-Nyame. Implementa uma issue já levantada, seguindo o comentário de levantamento, e abre ou atualiza o PR. Use quando o usuário pedir para executar ou implementar uma issue.
argument-hint: <número da issue>
disable-model-invocation: true
---

# Executar a issue #$ARGUMENTS

Você é a **sessão de execução**. Você implementa o que o levantamento planejou e abre um PR. Você **não** decide arquitetura nem amplia o escopo.

## 1. Confira o estado (regra de recusa)

```bash
bash tools/workflow/issue-state.sh status $ARGUMENTS
```

Só continue se **todas** forem verdadeiras:

- `estado` é `pronta-para-execução`;
- `needs-decision` é `false`.

Se alguma falhar, **recuse**: diga ao usuário qual condição falhou e qual comando ou ação vem antes, não altere nada e termine.

## 2. Assuma a issue

```bash
bash tools/workflow/issue-state.sh set $ARGUMENTS em-execução
```

## 3. Siga o roteiro

Leia `CLAUDE.md`, a issue e o **último comentário de levantamento** dela. Depois execute a seção **"2. Execução"** de `docs/workflow/agents.md`.

- `pr aberto` vazio no status: **modo novo**.
- `pr aberto` preenchido: **modo correção**. Continue na mesma branch e trate só os achados bloqueantes do último relatório de revisão.

Carregue **todas** as skills listadas no levantamento antes de escrever qualquer coisa. `clean-code` é obrigatória em todo código.

## 4. Se a issue ou o plano estiverem errados

Pare. Não improvise. Comente na issue o que encontrou e então:

```bash
gh issue edit $ARGUMENTS --add-label needs-decision
bash tools/workflow/issue-state.sh set $ARGUMENTS levantamento
```

Se o motivo for só levantamento envelhecido (a `main` mudou nos arquivos envolvidos), comente e use `bash tools/workflow/issue-state.sh clear $ARGUMENTS`.

## 5. Encerre

Com o PR aberto ou atualizado (`Closes #$ARGUMENTS`):

```bash
bash tools/workflow/issue-state.sh set $ARGUMENTS em-revisão
```

Termine dizendo ao usuário o link do PR, o resultado dos testes e o que ficou pendente para verificação jogando.
