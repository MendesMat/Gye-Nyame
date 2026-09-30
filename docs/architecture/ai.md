# Inteligência artificial (`GyeNyame.AI`)

Pasta: `Assets/_Project/Scripts/AI`. A IA usa o pacote **Unity Behavior** (Behavior Graph). O grafo fica em `Assets/_Project/BehaviorTrees/BasicEnemy/BasicEnemyBG.asset` e roda no componente `BehaviorGraphAgent` da raiz de cada inimigo.

## Princípio

A IA é o **cérebro** e só decide. Ela nunca move nem anima diretamente:

- movimento → `IEnemyMovement.SetMovementIntent(Vector2)`
- ataque → `IEnemyCombat.TryAttack()`
- coordenação com os outros inimigos → `IAttackDirector` (o `CombatDirector`)
- rosto → `IEntityLocomotion.SetFacingDirectionLock` / `ForceFacingDirectionX`

O assembly só depende de `Core`, então os nós funcionam com qualquer implementação dessas interfaces.

## Blackboard

| Variável | Tipo | Uso |
|---|---|---|
| `Self` | GameObject | O próprio inimigo (entra como `Agent` nos nós) |
| `CombatDirector` | GameObject | O GameObject que tem o `IAttackDirector` (entra como `Director`) |
| `IsStunned` | bool | Condição do primeiro ramo. **Nenhum código define essa variável hoje** |

## Estrutura do grafo

```
Start
└ Try In Order (seletor)
  ├ Sequence ── Guard: IsStunned
  │   └ Hesitate
  ├ Sequence ── Priority Abort: CanAcquireToken
  │   └ AcquireAttackToken
  │     → ApproachCombatSlot
  │     → CheckAttackRange
  │     → Hesitate
  │     → ExecuteAttack
  │     → ProbabilisticTokenRelease
  └ Sequence
      └ TacticalPositioning → Hesitate
```

Em palavras: se conseguir um token, o inimigo se aproxima do slot, confere o alcance, hesita um pouco, ataca e às vezes (1 em 20) devolve o token. Sem token, reposiciona-se taticamente a uma distância de espera e hesita. O *Priority Abort* reavalia `CanAcquireToken` para que um inimigo em espera entre no ataque assim que um token fica livre.

## Nós customizados (`AI/Nodes`)

| Nó | Tipo | O que faz |
|---|---|---|
| `CanAcquireTokenCondition` | Condição | `director.IsTokenAvailableFor(agent)` |
| `AcquireAttackTokenAction` | Ação | `RequestAttackToken`. Falha se negado |
| `ApproachCombatSlotAction` | Ação | Vai até o slot do diretor, recalcula o slot a cada 0,4–0,8 s (tempo de reação), desacelera perto do alvo. Sucesso ao chegar ou já estar no alcance |
| `CheckAttackRangeAction` | Ação | Sucesso se no alcance. Senão **libera o token** e falha |
| `HesitateAction` | Ação | Fica parado um tempo aleatório entre mínimo e máximo, olhando para o player (valores por nó no grafo) |
| `ExecuteAttackAction` | Ação | `TryAttack` e espera `IsAttacking` virar falso |
| `ProbabilisticTokenReleaseAction` | Ação | Libera o token com chance de 1/20 |
| `ReleaseAttackTokenAction` | Ação | Libera o token (não usado no grafo atual) |
| `TacticalPositioningAction` | Ação | Escolhe um ponto a uma distância de espera aleatória, com deslocamento lateral em Z e ruído anti-aglomeração; vai até lá ou até o tempo acabar |

Todos os nós que movem **travam o rosto voltado para o player** enquanto rodam e destravam em `OnEnd`. Os parâmetros numéricos são variáveis do nó, editáveis no grafo (veja [design/tuning.md](../design/tuning.md)).

## Depuração

- Cada nó publica `AINodeStateMessage(agent, nó, status)`.
- `AINodesDebugger` (na raiz do inimigo) loga essas mensagens no Console quando `showLogs` está ligado.

## Como criar um nó

1. Classe `partial` herdando de `Unity.Behavior.Action` (ou `Condition`), com `[Serializable, GeneratePropertyBag]` e `[NodeDescription(name, story, category, id)]`. O `id` precisa ser único e **não pode mudar depois**, porque o grafo referencia o nó por ele.
2. Entradas como `[SerializeReference] public BlackboardVariable<T> Nome`.
3. Obtenha dependências por interface em `OnStart`; devolva `Status.Failure` se faltar alguma.
4. Publique `AINodeStateMessage` nos pontos importantes, para depuração.
5. Pare o movimento e destrave o rosto em `OnEnd`.

## Limitações atuais

- Supõe um único player (tudo passa por `director.GetPlayerTransform()`).
- Morte: o `EntityDeadState` desliga o `BehaviorGraphAgent`, o que acopla `Entities` ao pacote Behavior.

Ambas são tratadas na migração: [multiplayer/combat-director.md](../multiplayer/combat-director.md).
