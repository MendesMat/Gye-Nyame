# Fluxo de trabalho com agentes de IA

> Base: [ADR 0017](../decisions/0017-fluxo-de-trabalho-com-agentes.md). Papéis: **Matheus é o arquiteto** (decide, aprova e faz merge); **agentes executam**.

Cada issue passa por **três agentes em três janelas de contexto separadas**. Nenhum agente faz duas fases da mesma issue.

```
Backlog ─► [1. Pesquisa] ─► Pesquisada ─► [2. Execução] ─► Em revisão ─► [3. Revisão] ─► Aguardando merge ─► [Arquiteto] ─► Concluída
                │                               │                              │
                └─ pergunta em aberto ─► needs-decision (para até o arquiteto responder)
                                                └─ mudou algo desde o SHA ─► volta para pesquisa
                                                                               └─ 2 ciclos sem aprovar ─► arquiteto
```

## Quadro e labels

**GitHub Projects**, colunas: `Backlog` → `Pesquisada` → `Em execução` → `Em revisão` → `Aguardando merge` → `Concluída`. Cada agente move o cartão ao começar e ao terminar a sua fase (`gh project item-edit`).

| Label | Uso |
|---|---|
| `core`, `physics`, `entities`, `combat`, `player`, `enemy`, `ai`, `ui`, `network`, `docs`, `infra` | Módulo |
| `feature`, `refactor`, `bug`, `docs`, `infra` | Tipo |
| `design-tuning` | Cria ou muda valores de design; o game designer valida |
| `needs-decision` | Bloqueada por uma pergunta ao arquiteto |

## 1. Pesquisa

**Entrada:** a issue. **Saída:** um comentário na issue. **Não altera código.**

1. Leia a issue, o [`CLAUDE.md`](../../CLAUDE.md), os documentos de `docs/` citados e as ADRs relacionadas.
2. **Defina as skills necessárias** para a execução, pelo catálogo em [skills.md](skills.md). `clean-code` entra sempre que houver código. Carregue as skills de domínio também na pesquisa, se ajudarem a planejar.
3. Leia o código que será tocado e o que depende dele.
4. Se precisar do Editor para entender cena, prefab ou asset, use a Unity CLI só para **ler** ([unity-cli.md](unity-cli.md)).
5. Publique o comentário no formato abaixo (`gh issue comment <n> --body-file ...`).
6. Se "Perguntas em aberto" não estiver vazia: adicione `needs-decision` e **pare**. Senão, mova para `Pesquisada`.

### Formato do comentário de pesquisa

```markdown
## Pesquisa

**Commit de referência:** <SHA da main no momento da pesquisa>

### Skills necessárias
- `anthropic-skills:clean-code`: todo o código da issue
- `unity:<skill>`: para qual parte da tarefa

### Leitura obrigatória
- `caminho/arquivo.cs` (linhas X–Y): por quê
- `docs/...#seção`: por quê
- ADR NNNN: por quê

### Estado atual
O que existe hoje e como funciona, no nível necessário para a tarefa.

### Plano proposto
Passos concretos, arquivos a criar/alterar, assets e prefabs a mexer (via CLI).

### Armadilhas e riscos
O que pode dar errado; invariantes que não podem quebrar.

### Critérios de aceite refinados
- [ ] Verificáveis, incluindo testes que devem existir e passar.
- [ ] Documentos de `docs/` que devem ser atualizados.

### Perguntas em aberto
(vazio, ou perguntas que só o arquiteto responde)
```

## 2. Execução

**Entrada:** a issue e o comentário de pesquisa. **Saída:** um PR.

1. Compare a `main` com o **commit de referência** (`git diff <SHA>..origin/main --stat`). Se algo relevante mudou nos arquivos da leitura obrigatória, **não execute**: comente o motivo e mova de volta para `Backlog` (nova pesquisa).
2. **Carregue todas as skills listadas em "Skills necessárias"** antes de escrever qualquer coisa. Se descobrir que falta uma, carregue-a e registre no PR.
3. Crie a branch a partir da `main` ([git.md](git.md#branches)).
4. Leia **só** o que a pesquisa indicou (e o que descobrir que é necessário, registrando no PR).
5. Implemente seguindo o plano e as orientações das skills carregadas. Se o plano se mostrar errado, pare e comente na issue em vez de improvisar uma mudança de arquitetura.
6. Cena, prefab e asset: **sempre pela Unity CLI** com Editor conectado ([unity-cli.md](unity-cli.md)).
7. Escreva ou atualize os testes; rode `unity test`.
8. Atualize `docs/` (e `known-issues.md`, se resolveu algum item).
9. Abra o PR ([git.md](git.md#pull-requests)) e mova para `Em revisão`.

Limites: **uma issue por PR, até ~400 linhas alteradas** (sem contar assets gerados). Se não couber, pare e proponha dividir a issue.

## 3. Revisão

**Entrada:** o PR, a issue e o comentário de pesquisa. **Saída:** uma revisão no PR.

Antes de revisar, **carregue as mesmas skills listadas na pesquisa** (inclusive `clean-code`).

Confira:
- [ ] Todos os critérios de aceite atendidos.
- [ ] O código segue a skill `clean-code` e as orientações das skills da Unity da issue.
- [ ] Nenhuma ADR contrariada; regras de dependência entre assemblies respeitadas ([overview](../architecture/overview.md#assemblies-e-dependências)).
- [ ] Regras de autoridade respeitadas em código de rede ([authority.md](../multiplayer/authority.md)).
- [ ] Nenhum valor de design fixo em código ([0015](../decisions/0015-valores-de-design-em-scriptableobjects.md)).
- [ ] Testes existem para a lógica nova e passam (`unity test`).
- [ ] O projeto compila sem erros e sem warnings novos.
- [ ] `docs/` atualizado e coerente com o código.
- [ ] Nenhum segredo, nenhum arquivo de `Library/`, `Temp/`, `Logs/` ou `UserSettings/`.

Resultado:
- **Aprovado:** comente "Revisão aprovada" com o resumo e mova para `Aguardando merge`.
- **Mudanças necessárias:** comentários objetivos no PR; um **novo** agente de execução corrige. Depois do **2º ciclo** sem aprovação, marque o arquiteto e pare.

## 4. Merge

Só o arquiteto aprova e faz o merge (squash). Ele também roda os checklists manuais quando a issue ou a milestone exigir ([multiplayer/testing.md](../multiplayer/testing.md)).

## Regras gerais para qualquer agente

- **Skills antes de código.** `clean-code` sempre; skills da Unity em cada caso adequado ([skills.md](skills.md)).
- **Não decida no lugar do arquiteto.** ADR aceita não se rediscute; dúvida de escopo, design ou arquitetura vira `needs-decision`.
- **Não amplie o escopo.** Encontrou outro problema? Comente na issue ou proponha uma issue nova; não corrija de carona.
- **Não edite `.unity`/`.prefab`/`.asset` à mão** se houver Editor conectado.
- **Nunca** faça force-push na `main`, nem merge.
- Relate falhas como falhas: teste quebrado, passo pulado, algo não verificado.
