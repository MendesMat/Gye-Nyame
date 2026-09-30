# Problemas conhecidos e dívida técnica

Lista do que já sabemos que está errado ou sobrando no código atual. Cada item indica **onde será tratado**. Pela [ADR 0001](../decisions/0001-refatorar-em-vez-de-reescrever.md), bugs de um módulo são corrigidos na issue de refatoração desse módulo, e não numa milestone separada.

Ao corrigir um item, **remova-o desta lista** no mesmo PR.

## Bugs

| # | Onde | Problema | Efeito |
|---|---|---|---|
| B1 | `Entities/Movement/States/EntityDashState.cs`, `TransitionToGroundState` | Falta `return` depois da troca para Idle | O dash sempre termina em Walk. Sem input, a máquina faz Idle → Walk → Idle em sequência (animação pisca, dois eventos de troca) |
| B2 | Blackboard do `BasicEnemyBG` | A variável `IsStunned` guarda o primeiro ramo do grafo, mas nenhum código a define | O ramo nunca executa |
| B3 | Cena: um dos `DummyDoll` | O filho `Visuals` está na layer `EnemyHitbox` em vez de `Enemy` | Inconsistência que pode gerar colisões inesperadas. Some quando os inimigos virarem prefab |
| B4 | `PlayerCombat` | Os clipes chamam `CloseCancelWindow`, mas `PlayerCombat` não tem esse método (só `EnemyCombat` tem); a janela só fecha em `ResetCombatState` | A janela de cancelamento pode ficar aberta além do previsto |

## Acoplamento e dependências

| # | Problema | Solução prevista |
|---|---|---|
| A1 | `GyeNyame.Entities` referencia `Unity.Behavior` só para desligar o `BehaviorGraphAgent` no `EntityDeadState` | A IA reage à morte por conta própria (mensagem ou evento), e `Entities` perde a dependência |
| A2 | `GyeNyame.Player.Movement` referencia `Unity.InputSystem` sem usar | Remover a referência |
| A3 | `GyeNyame.Player.Combat` referencia `GyeNyame.Player.Movement` sem usar | Remover a referência |
| A4 | `EnemyCombat` procura o diretor com `FindAnyObjectByType` | Injeção explícita (no spawn da rede) |
| A5 | `CombatDirector`, IA e UI supõem **um único player** | Reescritos para N alvos: [multiplayer/combat-director.md](../multiplayer/combat-director.md) |
| A6 | Mensagens de input não dizem de qual jogador vieram | [ADR 0007](../decisions/0007-eventbus-local-e-identidade.md) |
| A7 | `TimeManager` usa `Time.timeScale` global | [ADR 0008](../decisions/0008-hitstop-local-por-entidade.md) |

## Código morto

| Item | Local |
|---|---|
| `AnimationCancelWindowMessage`, `AnimationFinishAttackMessage` | `Core/Contracts/Messages/AnimationMessages.cs` |
| `IMovementProvider` | `Core/Contracts/Interfaces` |
| `IPlayerMovementContext` (interface vazia), `IPlayerCombatContext` (vazia) | `Player/Movement`, `Player/Combat` |
| `AttackDataSO.screenShakeMultiplier` | Nunca lido (o Cinemachine está instalado mas não é usado) |
| `CombatDirector.ReleasePositionSlot` | Nunca chamado. Por isso `_enemyNoises` não é limpo quando um inimigo morre |
| `PlayerDeadState._slowMotionFinished` | Atualizado, nunca lido |
| `ReleaseAttackTokenAction` | Nó existe, mas não está no grafo |
| Ação `Interact` do Input | Mapeada, nunca lida |

## EventBus

- Mensagens `struct` sofrem **boxing** ao passar por `IMessage` (pequena alocação por publish).
- `Clear<T>()` remove os handlers, mas não os embrulhos em `_wrappers`.
- Com *Enter Play Mode* sem domain reload, handlers de uma sessão anterior sobrevivem, porque o bus é estático.

Nenhum dos três causa problema hoje. Viram relevantes com o multiplayer (mais mensagens, mais objetos).

## Estrutura de assets

| Item | Solução prevista |
|---|---|
| Nenhum prefab: Player e 4 Dummies são objetos soltos | Prefabs de rede (primeira milestone técnica) |
| `Assets/_Recovery/0.unity` versionado | Apagar e adicionar `_Recovery/` ao `.gitignore` |
| Nome do build profile com erro de digitação (`Windows  Protype Test v0.1.0`) | Renomear junto com a primeira issue de build |
| Nome do projeto na nuvem da Unity ainda é "Nye-Gyame" | Corrigir na configuração dos serviços online |
| Merge driver do Smart Merge não registrado | `unity vcs merge-setup` ([workflow/git.md](../workflow/git.md)) |
