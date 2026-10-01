# Protocolo das três sessões

> Base: [ADR 0017](../decisions/0017-fluxo-de-trabalho-com-agentes.md) e [ADR 0023](../decisions/0023-protocolo-das-tres-sessoes.md). Papéis: **Matheus é o arquiteto** (decide, aprova e faz merge); **agentes executam**.

Cada issue passa por **três sessões de contexto zerado**, cada uma iniciada por um comando. Nenhuma sessão faz duas fases da mesma issue.

| Fase | Comando | Produz |
|---|---|---|
| 1. Levantamento | `/levantar-issue N` | Um comentário na issue |
| 2. Execução | `/executar-issue N` | Um PR |
| 3. Revisão | `/revisar-issue N` | Um relatório no PR |
| 4. Merge | (arquiteto) | Issue fechada |

Os comandos ficam em `.claude/skills/`. Eles só conferem o estado e apontam para a seção da fase **neste documento**, que é a fonte da verdade do roteiro.

## Estados e passagem de bastão

A **label `estado:*` da issue é a fonte da verdade**. A coluna do quadro ([GitHub Projects](https://github.com/users/MendesMat/projects/1)) é espelhada no mesmo passo pelo utilitário. Issue sem label de estado está no backlog.

```
backlog ──/levantar──► estado:levantamento ──► estado:pronta-para-execução
                              ▲  │ (pergunta em aberto: + needs-decision, espera o arquiteto)
                              │  ▼
        ┌─────────────────────┘
        │ (levantamento envelhecido ou issue errada)
estado:pronta-para-execução ──/executar──► estado:em-execução ──► estado:em-revisão
        ▲                                                                │
        └──────────── achados bloqueantes (até 2 ciclos) ◄──/revisar─────┤
                                                                         ▼
                                                              estado:aprovada ──► merge (arquiteto)
```

| Estado | Significa | Quem coloca |
|---|---|---|
| *(sem label)* | Backlog | — |
| `estado:levantamento` | Levantamento em andamento, ou aguardando resposta do arquiteto | `/levantar-issue` |
| `estado:pronta-para-execução` | Levantamento completo, sem perguntas em aberto; ou PR reprovado aguardando correção | `/levantar-issue`, `/revisar-issue` |
| `estado:em-execução` | Uma sessão está implementando | `/executar-issue` |
| `estado:em-revisão` | PR aberto, aguardando revisão | `/executar-issue` |
| `estado:aprovada` | Revisão sem achados bloqueantes; o arquiteto decide o merge | `/revisar-issue` |
| `needs-decision` | Há uma pergunta para o arquiteto. Vale em qualquer fase e **bloqueia todos os comandos** | qualquer sessão |

### Utilitário de estado

```bash
bash tools/workflow/issue-state.sh status <N>        # estado, needs-decision, bloqueadoras abertas, PR aberto
bash tools/workflow/issue-state.sh set <N> <estado>  # troca a label e move o cartão
bash tools/workflow/issue-state.sh clear <N>         # devolve ao backlog
```

Estados aceitos: `levantamento`, `pronta-para-execução`, `em-execução`, `em-revisão`, `aprovada`.

### Regra de recusa

Toda sessão começa com `status`. **Se a issue não estiver no estado de entrada da fase, a sessão recusa**: explica o motivo ao usuário, não altera nada e termina.

| Comando | Só começa se |
|---|---|
| `/levantar-issue` | Sem label de estado, **ou** `estado:levantamento` (retomada). Sem `needs-decision`. **Nenhuma issue bloqueadora aberta** |
| `/executar-issue` | `estado:pronta-para-execução`. Sem `needs-decision` |
| `/revisar-issue` | `estado:em-revisão`. Sem `needs-decision`. Existe PR aberto da issue |

## Labels de classificação

| Label | Uso |
|---|---|
| `core`, `physics`, `entities`, `combat`, `player`, `enemy`, `ai`, `ui`, `camera`, `network`, `docs`, `infra` | Módulo |
| `feature`, `refactor`, `bug`, `docs`, `infra`, `content` | Tipo |
| `design-tuning` | Cria ou muda valores de design; o game designer valida |

---

## 1. Levantamento

**Entrada:** a issue. **Saída:** um comentário na issue. **Não altera código nem assets.**

1. `status`; aplique a regra de recusa. Depois `set <N> levantamento`.
2. Leia a issue, o [`CLAUDE.md`](../../CLAUDE.md), os documentos de `docs/` citados e as ADRs relacionadas.
3. **Defina as skills necessárias** para a execução, pelo catálogo em [skills.md](skills.md). `clean-code` entra sempre que houver código. Carregue as skills de domínio também agora, se ajudarem a planejar.
4. Leia o código que será tocado e o que depende dele.
5. Se precisar do Editor para entender cena, prefab ou asset, use a Unity CLI só para **ler** ([unity-cli.md](unity-cli.md)).
6. Publique o comentário no formato abaixo (`gh issue comment <N> --body-file <arquivo>`).
7. Encerramento:
   - **Sem perguntas em aberto:** `set <N> pronta-para-execução`.
   - **Com perguntas em aberto:** adicione `needs-decision` (`gh issue edit <N> --add-label needs-decision`), mantenha `estado:levantamento` e **pare**.

**Retomada:** quando o arquiteto responde e remove `needs-decision`, um novo `/levantar-issue N` lê o levantamento anterior e as respostas, e publica um **levantamento consolidado** (novo comentário que substitui o anterior, com o commit de referência atualizado).

### Formato do comentário de levantamento

```markdown
## Levantamento

**Commit de referência:** <SHA da main no momento do levantamento>

### Skills necessárias
- `anthropic-skills:clean-code`: todo o código da issue
- `unity:<skill>`: para qual parte da tarefa

### Arquivos envolvidos
- `caminho/arquivo.cs` (linhas X–Y): o que tem ali e por que importa
- Cenas, prefabs e assets que serão tocados (via Unity CLI)

### Decisões aplicáveis
- ADR NNNN: o que ela obriga nesta tarefa
- `docs/...#seção`: regra ou contrato relevante

### Estado atual
Como funciona hoje, no nível necessário para a tarefa.

### Plano proposto
Passos concretos, na ordem.

### Plano de testes
- Testes EditMode/PlayMode a criar ou alterar, e o que cada um prova.

### Roteiro de verificação em Play Mode
**Verificável pelo agente** (Unity CLI: entrar em Play Mode e ler o console):
- [ ] ...

**Verificável jogando** (arquiteto, antes do merge):
- [ ] ...

### Armadilhas e riscos
O que pode dar errado; invariantes que não podem quebrar.

### Critérios de aceite refinados
- [ ] Verificáveis, incluindo os documentos de `docs/` a atualizar.

### Perguntas em aberto
(vazio, ou perguntas que só o arquiteto responde)
```

---

## 2. Execução

**Entrada:** a issue e o levantamento. **Saída:** um PR.

1. `status`; aplique a regra de recusa. Depois `set <N> em-execução`.
2. Identifique o modo:
   - **Novo:** não há PR aberto da issue.
   - **Correção:** há PR aberto com relatório de revisão reprovando. Continue **na mesma branch** e trate **só os achados bloqueantes** do último relatório.
3. (Modo novo) Compare a `main` com o commit de referência (`git diff <SHA>..origin/main --stat`). Se algo relevante mudou nos arquivos envolvidos, **não execute**: comente o motivo, `clear <N>` (volta ao backlog para novo levantamento) e pare.
4. **Carregue todas as skills listadas em "Skills necessárias"** antes de escrever qualquer coisa. Se descobrir que falta uma, carregue-a e registre no PR.
5. (Modo novo) Crie a branch a partir da `main` ([git.md](git.md#branches)).
6. Leia **só** o que o levantamento indicou (e o que descobrir que é necessário, registrando no PR).
7. Implemente seguindo o plano e as orientações das skills.
8. Cena, prefab e asset: **sempre pela Unity CLI** com Editor conectado ([unity-cli.md](unity-cli.md)).
9. Execute o plano de testes; rode `unity test`.
10. Execute a parte **"verificável pelo agente"** do roteiro de Play Mode e registre o resultado no PR.
11. Atualize `docs/` (e `known-issues.md`, se resolveu algum item).
12. Abra o PR com `Closes #<N>` ([git.md](git.md#pull-requests)), ou atualize o existente no modo correção. Depois `set <N> em-revisão`.

**Se a issue ou o plano estiverem errados, pare.** Não improvise uma mudança de arquitetura nem amplie o escopo. Comente na issue o que encontrou, adicione `needs-decision`, `set <N> levantamento` e termine. Depois da resposta do arquiteto, o fluxo recomeça por `/levantar-issue N`.

Limites: **uma issue por PR, até ~400 linhas alteradas** (sem contar assets gerados). Se não couber, pare e proponha dividir a issue (mesmo procedimento acima).

---

## 3. Revisão

**Entrada:** o PR, a issue e o levantamento. **Saída:** um relatório no PR. **A revisão só relata: não altera código, assets nem documentos, e não faz commit.**

1. `status`; aplique a regra de recusa.
2. Carregue as mesmas skills listadas no levantamento (inclusive `clean-code`).
3. Revise o diff do PR contra a lista abaixo e rode `unity test`.
4. Publique o relatório no PR (`gh pr comment <PR> --body-file <arquivo>`). O número do ciclo é a quantidade de relatórios de revisão já publicados no PR, mais um.
5. Encerramento:
   - **Sem achados bloqueantes:** `set <N> aprovada`.
   - **Com achados bloqueantes, ciclo 1:** `set <N> pronta-para-execução` (uma nova sessão de execução corrige).
   - **Com achados bloqueantes, ciclo 2:** adicione `needs-decision`, mencione o arquiteto no relatório e pare. O estado fica `em-revisão`.

### O que conferir

- Todos os critérios de aceite atendidos.
- O código segue a skill `clean-code` e as orientações das skills da Unity da issue.
- Nenhuma ADR contrariada; regras de dependência entre assemblies respeitadas ([overview](../architecture/overview.md#assemblies-e-dependências)).
- Regras de autoridade respeitadas em código de rede ([authority.md](../multiplayer/authority.md)).
- Nenhum valor de design fixo em código ([0015](../decisions/0015-valores-de-design-em-scriptableobjects.md)).
- O plano de testes foi cumprido e os testes passam.
- O projeto compila sem erros e sem warnings novos.
- `docs/` atualizado e coerente com o código.
- Nenhum segredo, nenhum arquivo de `Library/`, `Temp/`, `Logs/` ou `UserSettings/`.
- O PR não saiu do escopo da issue.

### Bloqueante ou não bloqueante

| Classe | Critério | Efeito |
|---|---|---|
| **Bloqueante** | Critério de aceite não atendido; bug; teste faltando ou falhando; ADR ou regra de arquitetura contrariada; documentação ausente ou errada; segredo ou arquivo indevido; escopo extrapolado | Reprova. Volta para a execução |
| **Não bloqueante** | Nome que poderia ser melhor, comentário, pequena simplificação, estilo sem impacto | **Não reprova.** Fica listado; o arquiteto decide no merge se pede ajuste |

### Formato do relatório de revisão

```markdown
## Revisão — ciclo <1|2>

**Resultado:** Aprovada | Reprovada

### Achados bloqueantes
1. `arquivo:linha` — o problema, por que bloqueia, o que se espera.

### Achados não bloqueantes
1. `arquivo:linha` — a observação.

### Verificações
- Critérios de aceite: N de M atendidos
- `unity test`: resultado
- Documentação: ok | faltando <o quê>

### Pendente para o arquiteto
- Roteiro "verificável jogando" do levantamento.
```

---

## 4. Merge

Só o arquiteto aprova e faz o merge (squash). Antes, ele roda a parte **"verificável jogando"** do roteiro de Play Mode e, quando a milestone exigir, os checklists de [multiplayer/testing.md](../multiplayer/testing.md). O merge fecha a issue (`Closes #N`).

## Regras gerais para qualquer sessão

- **Estado errado, sessão recusa.** Não "conserte" o estado para poder começar.
- **Skills antes de código.** `clean-code` sempre; skills da Unity em cada caso adequado ([skills.md](skills.md)).
- **Não decida no lugar do arquiteto.** ADR aceita não se rediscute; dúvida de escopo, design ou arquitetura vira `needs-decision`.
- **Não amplie o escopo.** Encontrou outro problema? Comente na issue ou proponha uma issue nova; não corrija de carona.
- **Não edite `.unity`/`.prefab`/`.asset` à mão** se houver Editor conectado.
- **Nunca** faça force-push na `main`, nem merge.
- Relate falhas como falhas: teste quebrado, passo pulado, algo não verificado.
