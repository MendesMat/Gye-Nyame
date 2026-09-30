# Core (`GyeNyame.Core`)

Pasta: `Assets/_Project/Scripts/Core`. É o alicerce: não depende de nenhum outro assembly do jogo e todos dependem dele.

| Subpasta | Conteúdo |
|---|---|
| `Events/` | `EventBus`, `IMessage` |
| `StateMachine/` | `StateMachine`, `BaseState`, fábricas, depurador |
| `InputBuffer/` | `InputBuffer`, `BufferedCommand` |
| `Contracts/Interfaces/` | Interfaces compartilhadas entre módulos |
| `Contracts/Messages/` | Mensagens do EventBus |
| `Contracts/Data/` | `DamageData` |
| `Contracts/Enums/` | `EntityStateCategory` |
| `GameFlow/` | `GameResetHandler`, `LevelEnemyTracker` |
| `TimeManagement/` | `TimeManager` |
| `Attributes/` + `Editor/` | `[TagSelector]` e o drawer que mostra um dropdown de tags no Inspector |

## EventBus

`Core/Events/EventBus.cs`. Pub/sub **estático** e tipado por mensagem.

```csharp
EventBus.Subscribe<EntityDeadMessage>(OnEntityDead);   // em OnEnable
EventBus.Unsubscribe<EntityDeadMessage>(OnEntityDead); // em OnDisable
EventBus.Publish(new EntityDeadMessage(root));
EventBus.Clear();                                      // usado ao recarregar a cena
```

- Toda mensagem implementa `IMessage` (interface de marcação). Prefira `readonly struct`.
- A entrega é **síncrona e imediata**, na ordem de inscrição. `Publish` itera sobre uma cópia da lista, então é seguro assinar ou desassinar dentro de um handler.
- Internamente, cada handler é embrulhado num `Action<IMessage>`, e o embrulho fica num dicionário indexado pelo delegate original. É isso que permite o `Unsubscribe`.
- **Não há escopo:** toda mensagem chega a todos os ouvintes. Quem assina filtra pelo alvo.

Limitações conhecidas (boxing de structs, `Clear<T>` incompleto, handlers sobrevivendo sem domain reload): [known-issues.md](known-issues.md).

### Catálogo de mensagens

| Mensagem | Dados | Publicada por | Ouvida por |
|---|---|---|---|
| `PlayerMoveMessage` | `Vector2 MoveInput` | PlayerInputHandler | PlayerMovement |
| `PlayerJumpMessage` | — | PlayerInputHandler | PlayerMovement, PlayerCombat |
| `PlayerDashMessage` | — | PlayerInputHandler | PlayerMovement, PlayerCombat |
| `PlayerAttackLightMessage` | — | PlayerInputHandler | PlayerCombat |
| `PlayerAttackHeavyMessage` | — | PlayerInputHandler | PlayerCombat |
| `EndCombatMessage` | `GameObject Entity` | BaseEntityAttackState, PlayerCombat | PlayerMovement, EnemyMovement |
| `EntityDamagedMessage` | `GameObject Target`, `DamageData Damage` | EntityHealth | TimeManager, DamageFlashFeedback, BaseEnemy, presenters de UI |
| `EntityDeadMessage` | `GameObject Target` | EntityHealth | LevelEnemyTracker, CombatDirector, EnemyHealthPresenter |
| `AnimationFinishDeathMessage` | `GameObject Target` | EntityAnimationHandler | EntityDeadState |
| `PlayerDiedMessage` | — | PlayerDeadState | PlayerInputHandler, CombatDirector, BaseEnemy |
| `GameOverMessage` | — | PlayerDeadState | GameResetHandler |
| `AllEnemiesDefeatedMessage` | — | LevelEnemyTracker | GameResetHandler |
| `SlowMotionRequestMessage` | `TimeScale`, `Duration` | PlayerDeadState | TimeManager |
| `StateChangedMessage` | `Previous`, `Next` | StateMachine | ninguém (disponível para depuração) |
| `AINodeStateMessage` | `Agent`, `NodeName`, `Status` | nós de IA | AINodesDebugger |
| `AnimationCancelWindowMessage`, `AnimationFinishAttackMessage` | — | **ninguém** | **ninguém** (código morto) |

## Máquina de estados

`Core/StateMachine/`. Máquina de estados **baseada em classes**, com fábrica e cache.

- `StateMachine` é um `MonoBehaviour` que repassa `Update` e `FixedUpdate` para `CurrentState`.
- `BaseState` define `Enter`, `Update`, `FixedUpdate`, `Exit`, e expõe `StateCategory` (usada pela animação) e o evento `OnStateExit`.
- `RegisterState<T>(IStateFactory)` associa um tipo a uma fábrica. `StateFactory<T>` aceita uma lambda, que é como os componentes **injetam dependências** nos estados:

  ```csharp
  stateMachine.RegisterState<EntityIdleState>(
      new StateFactory<EntityIdleState>(sm => new EntityIdleState(sm, this)));
  ```

- `GetOrCreateState<T>()` devolve a instância em cache ou cria uma. **Uma instância por tipo por máquina.** Chamar sem registrar lança `InvalidOperationException`.
- `ChangeState(novo, saveToHistory)` chama `Exit` do atual, `Enter` do novo, dispara `OnStateChanged` e publica `StateChangedMessage`. `PopState()` volta ao estado salvo no histórico (não usado hoje).
- **Não há guarda de reentrada:** mudar para o mesmo estado chama `Exit` e `Enter` de novo.
- `StateMachineDebugger` loga cada transição no Console (desligável por `showLogs`).

### Como adicionar um estado

1. Crie uma classe herdando de `BaseState` (ou de um estado genérico em `Entities`) e sobrescreva `StateCategory` se ela tiver animação própria.
2. Registre no `Awake` do componente dono do comportamento, com uma lambda que injeta as dependências.
3. Se a categoria é nova, adicione ao enum `EntityStateCategory` e ao mapa em `EntityAnimationHandler.InitializeAnimations`, e crie o estado com o mesmo nome no Animator.

## InputBuffer

`Core/InputBuffer/`. Guarda intenções de comando por uma janela curta, para que um botão apertado alguns frames antes da hora não seja perdido.

- `BufferCommand<T>(bufferTime)`: guarda (ou renova) um comando do tipo `T`.
- `HasCommand<T>()` / `ConsumeCommand<T>()`: o consumidor pergunta e consome.
- `Clear()`: esvazia (usado ao entrar em Hurt ou ao cancelar um ataque).
- Comandos expirados são removidos no `Update`. Usa `Time.time`, então **congela durante o hitstop**.

## Contratos (interfaces)

| Interface | Implementada por | Usada por |
|---|---|---|
| `IEntityLocomotion` | `BaseEntityMovement` (Player/EnemyMovement) | estados genéricos, animação, IA |
| `IEntityHealth` | `EntityHealth` | estados, UI, IA, EnemyCombat |
| `IEntityVisuals` | `EntityVisuals` | `EntityDeadState` (fade) |
| `IDamageable` | `EntityHealth` | `HurtboxComponent` |
| `IHitbox` / `IHurtbox` | `HitboxComponent` / `HurtboxComponent` | PlayerCombat, EnemyCombat, HitboxComponent |
| `IKinematicPhysics` | `KinematicPhysics` | `BaseEntityMovement` |
| `IAttackDirector` | `CombatDirector` | nós de IA |
| `IEnemyCombat` | `EnemyCombat` | IA, `LevelEnemyTracker`, `CombatDirector` |
| `IEnemyMovement` | `EnemyMovement` | nós de IA |
| `IMovementProvider` | **ninguém** (código morto) | — |

`DamageData` (struct): `Amount`, `SourcePosition`, `KnockbackForce`, `KnockupForce`, `HitStopTime`.

`EntityStateCategory`: `None, Idle, Walk, Jump, Fall, Dash, AttackLight1, AttackLight2, AttackHeavy, Hurt, Dead`. O nome de cada valor é o **nome do estado no Animator**.

## Fluxo de jogo

- `LevelEnemyTracker`: no `Start`, conta todos os `MonoBehaviour` que implementam `IEnemyCombat`. A cada `EntityDeadMessage` de um inimigo, decrementa. No zero, publica `AllEnemiesDefeatedMessage`.
- `GameResetHandler`: ao receber `GameOverMessage` ou `AllEnemiesDefeatedMessage`, espera `resetDelay` segundos em tempo real, restaura `Time.timeScale = 1`, limpa o EventBus e recarrega a cena ativa.

## Tempo

`TimeManager` controla o **`Time.timeScale` global**:

- `EntityDamagedMessage` com `HitStopTime > 0`: `timeScale = 0` durante `HitStopTime` segundos reais (hitstop).
- `SlowMotionRequestMessage`: `timeScale = TimeScale` durante `Duration` segundos reais.
- Um pedido novo cancela o anterior.

> Isso **não funciona em rede** e será substituído por hitstop local por entidade. Veja [ADR 0008](../decisions/0008-hitstop-local-por-entidade.md).

Componentes que usam tempo real (`WaitForSecondsRealtime`, `unscaledDeltaTime`) continuam correndo durante o hitstop: `DamageFlashFeedback`, `GameResetHandler`, `PlayerDeadState`.
