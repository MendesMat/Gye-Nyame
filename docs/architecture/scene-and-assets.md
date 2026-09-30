# Cena, layers e assets

## Estrutura de pastas

```
Assets/
├ _Project/                    ← tudo do jogo
│ ├ Art/
│ │ ├ Animations/Agents/       AgentAnimatorController (controlador base)
│ │ ├ Animations/Player/       clipes Player.* e PlayerAnimatorController (override)
│ │ ├ Animations/Enemy/        DummyAnimatorController (override)
│ │ ├ Materials/               DamageTakenFlash.mat
│ │ └ Sprites/                 Player/Placeholders, Scenario
│ ├ BehaviorTrees/BasicEnemy/  BasicEnemyBG (grafo da IA)
│ ├ ScriptableObjects/         AttackDataSO do player e do inimigo
│ ├ Scripts/                   código (ver overview.md)
│ └ UnityInputSystem/          InputSystem_Actions (+ classe gerada)
├ Scenes/SampleScene.unity     única cena do jogo
├ Settings/                    URP (PC e Mobile), Volume, Build Profile
└ _Recovery/0.unity            cena de recuperação de crash (lixo; ver known-issues)
```

**Não existe nenhum prefab.** Player, inimigos, cenário e HUD são objetos da cena.

## Hierarquia da SampleScene

```
Main Camera (tag MainCamera)
├ CameraBoundaryLeft / CameraBoundaryRight   (layer Ground, BoxCollider: paredes invisíveis)
Directional Light
Entities
├ Player            (ver overview.md: anatomia de uma entidade)
├ DummyDoll, DummyDoll (1), (2), (3)
Scenario
├ BackgroundWall, ForegroundWall, Ground     (layer Ground: limites de profundidade e chão)
Global Volume
Managers
└ GameManager       (TimeManager, CombatDirector, GameResetHandler, LevelEnemyTracker)
HUDCanvas           (ver ui.md)
EventSystem
```

- A câmera é fixa. As bordas esquerda e direita são filhas dela.
- `BackgroundWall` e `ForegroundWall` limitam o eixo Z (profundidade).

## Layers e matriz de colisão

| # | Layer | Quem usa |
|---|---|---|
| 3 | `Player` | Raiz e `Visuals` do player |
| 6 | `Enemy` | Raiz e `Visuals` dos inimigos |
| 7 | `Ground` | Chão, paredes e bordas da câmera |
| 8 | `PlayerHitbox` | Hitboxes de ataque do player |
| 9 | `EnemyHitbox` | Hitboxes de ataque dos inimigos |
| 10 | `PlayerHurtbox` | Hurtbox do player |
| 11 | `EnemyHurtbox` | Hurtbox dos inimigos |
| 12 | `Interactables` | Reservada (sem uso) |
| 13 | `Dead` | Hurtbox de entidade morta. Não colide com nada |

Matriz de colisão (Project Settings → Physics):

| Layer | Colide com |
|---|---|
| Player | Player, Enemy, Ground, Interactables |
| Enemy | Player, Enemy, Ground, Interactables |
| Ground | Player, Enemy, Ground, Interactables |
| PlayerHitbox | EnemyHurtbox |
| EnemyHitbox | PlayerHurtbox |
| Dead | nada |

> **É a matriz que decide quem pode acertar quem.** Não há checagem de time no código. Uma hitbox numa layer errada acerta o alvo errado.

**Tags:** `Player` (raiz e hurtbox do player) e `Enemy` (raiz e hurtbox dos inimigos). Os presenters de UI dependem delas.

## Animação

- **Um controlador base** (`AgentAnimatorController`) com um estado por `EntityStateCategory`: Idle, Walk, Jump, Fall, Dash, AttackLight1, AttackLight2, AttackHeavy, Hurt, Dead. Não há transições nem parâmetros: o código chama `Animator.Play` diretamente.
- **Overrides por personagem:** `PlayerAnimatorController` e `DummyAnimatorController` trocam os clipes. Hoje o Dummy usa os **mesmos clipes do Player** (placeholder).
- **Eventos nos clipes** (chamam `CombatAnimationEventHandler` ou `EntityAnimationHandler`):

| Clipe | Eventos, em ordem |
|---|---|
| Player.AttackLight1 / AttackLight2 | OpenHitbox, CloseHitbox, OpenCancelWindow, CloseCancelWindow, FinishAttack |
| Player.AttackHeavy | OpenHitbox, CloseHitbox, FinishAttack |
| Player.Dead | OnDeathAnimationFinish |

Todo clipe de ataque **precisa** terminar com `FinishAttack`, senão a entidade fica presa no estado de ataque. Todo clipe `Dead` precisa de `OnDeathAnimationFinish`, senão a entidade nunca some.

## ScriptableObjects

| Asset | Tipo | Uso |
|---|---|---|
| `ScriptableObjects/Player/Combat/AttackLight1` | AttackDataSO | Início do combo leve e do pesado |
| `…/AttackLight2` | AttackDataSO | 2º golpe leve |
| `…/AttackHeavy` | AttackDataSO | Finalizador pesado |
| `ScriptableObjects/Enemy/Combat/BasicEnemyAttack` | AttackDataSO | Ataque do Dummy |

Valores em [design/tuning.md](../design/tuning.md).

## Configurações de projeto relevantes

- **Render pipeline:** URP com perfis `PC_RPAsset` e `Mobile_RPAsset`.
- **Build profile:** `Windows Prototype Test v0.1.0` (StandaloneWindows64).
- **Serialização:** Force Text (necessário para diff e merge).
- **Git:** LFS ativo para imagens. O `.gitattributes` aponta cenas e prefabs para o Unity Smart Merge, mas o merge driver ainda não está registrado (ver [workflow/git.md](../workflow/git.md)).
- **Nuvem da Unity:** o projeto está ligado a um projeto na nuvem com o nome antigo "Nye-Gyame".
