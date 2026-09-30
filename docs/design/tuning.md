# Guia de ajuste para o game designer

Este guia lista **todo valor de jogo que pode ser ajustado**, onde ele fica e o que faz. Ele é atualizado a cada PR que cria ou move um valor.

> **Estado atual:** hoje muitos valores ainda estão em componentes da cena ou fixos no código. A transição para o multiplayer move todos eles para **ScriptableObjects de configuração** ([ADR 0015](../decisions/0015-valores-de-design-em-scriptableobjects.md)). As seções marcadas como **(planejado)** mostram onde cada valor vai morar.

## Como ajustar com segurança

- **Ajuste assets (ScriptableObjects), não cenas**, sempre que possível. Assets geram menos conflito de merge.
- Faça o ajuste numa branch própria e abra um PR, como qualquer mudança ([workflow/git.md](../workflow/git.md)).
- Issues que mexem em valores de design têm a label `design-tuning`. Elas esperam a sua validação.
- Mudou um valor? Atualize a coluna "Atual" neste guia no mesmo PR.

---

## Ataques (`AttackDataSO`)

Local: `Assets/_Project/ScriptableObjects/<Player|Enemy>/Combat/`. Um asset por golpe.

| Campo | Unidade | Efeito |
|---|---|---|
| `damage` | pontos de vida | Dano do golpe |
| `nextLightCombo` / `nextHeavyCombo` | asset | Qual golpe vem se apertar leve / pesado em seguida. Vazio = fim do combo |
| `animationCategory` | lista | Qual animação toca |
| `bufferTime` | s | Quanto tempo antes da hora o botão ainda é aceito |
| `comboWindowTime` | s | Quanto tempo depois do golpe o próximo do combo ainda vale. 0 = golpe final |
| `cooldownTime` | s | Espera antes de começar outro combo (no inimigo: intervalo entre ataques) |
| `knockbackForce` | força | Empurrão horizontal no alvo |
| `knockupForce` | força | Lançamento para cima no alvo |
| `hitStopTime` | s | Congelamento no impacto e duração da reação do alvo |
| `screenShakeMultiplier` | — | Ainda não faz nada |

| Asset | dano | buffer | janela | cooldown | knockback | knockup | hitstop |
|---|---|---|---|---|---|---|---|
| AttackLight1 | 1 | 0,2 | 0,2 | 0,1 | 0 | 0 | 0,1 |
| AttackLight2 | 1 | 0,2 | 0,2 | 0,1 | 3 | 0 | 0,1 |
| AttackHeavy | 2 | 0,2 | 0,2 | 0,2 | 7 | 5 | 0,2 |
| BasicEnemyAttack | 1 | 0 | 0 | 2 | 0 | 0 | 0,1 |

Combo atual: `AttackLight1` → leve → `AttackLight2`; `AttackLight1` → pesado → `AttackHeavy`.

## Personagem jogável

Hoje: componentes do objeto `Player` na cena. **(Planejado:** ficha `CharacterDefinition`, uma por personagem.**)**

| Valor | Atual | Hoje em | Efeito |
|---|---|---|---|
| Vida máxima | 6 | `PlayerHealth` (filho HurtBox) | |
| Velocidade | 5 | `PlayerMovement.speed` | Unidades por segundo |
| Multiplicador de profundidade | 2,5 | `PlayerMovement.depthSpeedMultiplier` | Velocidade no eixo Z em relação ao X |
| Gravidade | 25 | `PlayerMovement.gravity` | |
| Desaceleração do knockback | 15 | `PlayerMovement.knockbackDeceleration` | Quanto mais alto, mais rápido o empurrão para |
| Força do pulo | 10 | `PlayerMovement.jumpForce` | |
| Espera entre pulos | 0,2 s | `PlayerMovement.jumpCooldown` | Contada a partir do pouso |
| Velocidade no ar | 0,5× | `PlayerMovement.airSpeedMultiplier` | |
| Trava profundidade no pulo | sim | `PlayerMovement.lockDepthDuringJump` | |
| Velocidade do dash | 3× | `PlayerMovement.dashSpeedMultiplier` | |
| Duração do dash | 0,2 s | `PlayerMovement.dashDuration` | |
| Espera entre dashes | 1 s | `PlayerMovement.dashCooldown` | |
| Duração do fade na morte | 1 s | `EntityVisuals.fadeDuration` | |
| Golpes iniciais | AttackLight1 / AttackHeavy | `PlayerCombat` | Início do combo leve e do pesado |

## Inimigo (Dummy)

Hoje: componentes de cada `DummyDoll` na cena (4 cópias). **(Planejado:** ficha por tipo de inimigo.**)**

| Valor | Atual | Hoje em |
|---|---|---|
| Vida máxima | 4 | `EntityHealth` (filho Hurtbox) |
| Velocidade | 3 | `EnemyMovement.speed` |
| Multiplicador de profundidade | 2,5 | `EnemyMovement.depthSpeedMultiplier` |
| Gravidade / desaceleração do knockback | 25 / 15 | `EnemyMovement` |
| Ataque | BasicEnemyAttack | `EnemyCombat.attackData` |

## Comportamento da IA

Local: grafo `Assets/_Project/BehaviorTrees/BasicEnemy/BasicEnemyBG` (abrir no editor de Behavior Graph e selecionar o nó).

| Nó | Parâmetro | Atual | Efeito |
|---|---|---|---|
| ApproachCombatSlot | Min/MaxReactionDelay | 0,4 / 0,8 s | De quanto em quanto tempo o inimigo recalcula para onde ir |
| ApproachCombatSlot | SlotArrivalToleranceX / Z | 0,15 / 0,05 | Quão perto do ponto conta como "chegou" |
| ApproachCombatSlot | SpeedSmoothingDistance | 0,5 | Distância em que começa a desacelerar |
| Hesitate (antes de atacar) | Min/MaxHesitationTime | 0,5 / 1,5 s | Pausa antes do golpe |
| TacticalPositioning | RepositionDuration | 3 s | Tempo máximo rondando |
| TacticalPositioning | Min/MaxStandbyDistance | 3 / 7 | Distância de espera em relação ao player |
| TacticalPositioning | Min/MaxLateralOffset | −3 / 3 | Desvio em profundidade |
| TacticalPositioning | AntiClusteringNoise | 0,5 | Aleatoriedade para os inimigos não se amontoarem |
| TacticalPositioning | ArrivalTolerance | 0,15 | |
| Hesitate (após rondar) | Min/MaxHesitationTime | 1 / 2,5 s | |
| Hesitate (ramo "atordoado") | Min/MaxHesitationTime | 1,5 / 2,5 s | Ramo inativo hoje |

## Diretor de combate

Hoje: `CombatDirector` em `Managers/GameManager`. **(Planejado:** `DifficultySettings` e `CombatDirectorSettings`.**)**

| Valor | Atual | Efeito |
|---|---|---|
| Atacantes simultâneos | 2 | Quantos inimigos atacam ao mesmo tempo (planejado: por jogador, com valor de solo e de dupla) |
| Distância do slot | 0,75 | Distância em X em que o inimigo se posiciona para atacar |

## Valores fixos no código (a mover)

Estes valores hoje só mudam editando código. Serão movidos para ScriptableObjects nas issues de cada módulo.

| Valor | Atual | Onde está |
|---|---|---|
| Câmera lenta na morte do player | 0,2× por 2 s | `PlayerDeadState` |
| Espera extra antes de morrer (fim do combo no corpo) | 0,5 s | `EntityHurtState` |
| Chance de o inimigo largar o token após atacar | 1 em 20 | `ProbabilisticTokenReleaseAction` |
| Alcance de ataque além do slot | +0,5 em X, 0,1 em Z | `CombatDirector.IsInAttackRange` |
| Espera até recarregar a cena no fim | 2 s | `GameResetHandler` (na cena) |

## Regras de coop (planejado)

Valores novos que o multiplayer cria. Ficarão em `MatchRules` e `DifficultySettings`.

| Valor | Inicial | Efeito |
|---|---|---|
| Vidas compartilhadas | 3 | Mortes permitidas à dupla antes do game over |
| Espera para renascer | 3 s | |
| Invencibilidade ao renascer | 1,5 s | |
| Atacantes por jogador (solo / dupla) | 2 / a definir | |
| Multiplicador de vida dos inimigos (dupla) | a definir | |
| Multiplicador de dano dos inimigos (dupla) | a definir | |
| Tempo mínimo de um inimigo no mesmo alvo | a definir | Evita troca de alvo a cada instante |

## Decisões pendentes de design

| Assunto | Situação |
|---|---|
| Os dois jogadores colidem entre si? | Hoje colidem. Recomendação técnica: não colidir ([multiplayer/game-rules.md](../multiplayer/game-rules.md#colisão-entre-os-dois-jogadores)) |
| Valores de dupla da tabela acima | A definir durante os playtests |
