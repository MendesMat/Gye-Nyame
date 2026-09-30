# Física (`GyeNyame.Physics`)

Pasta: `Assets/_Project/Scripts/Physics`. Um único componente: `KinematicPhysics`, que implementa `IKinematicPhysics`.

## Por que física própria

As entidades **não usam a física dinâmica da Unity** (gravidade, forças, `velocity`). O `Rigidbody` existe para colisão e para `MovePosition`, e todo o movimento é calculado no código:

- Previsível e controlado, como o gênero exige (pulo e knockback com curvas exatas).
- Evita que personagens atravessem paredes ou o chão.
- É **determinístico o bastante** para rede: posição é consequência direta de input e estado, não de simulação física.

## O que ele oferece

```csharp
bool CheckGround(Vector3 currentPosition, float fallDistance, out float allowedFallDistance);
Vector3 CalculateAllowedMovement(Vector3 currentPosition, Vector3 intendedMovement);
```

- **`CheckGround`**: `SphereCast` para baixo a partir de `posição + rotação × groundCheckOffset`, com raio `groundCheckRadius`, distância igual à queda pretendida no frame, filtrando por `groundLayerMask` e ignorando triggers. Se bater, devolve a distância permitida (menos `skinWidth`). Isso impede atravessar o chão mesmo em quedas rápidas.
- **`CalculateAllowedMovement`**: `Rigidbody.SweepTest` na direção do movimento (usa o collider real da entidade). Se bater em algo, devolve o movimento cortado no ponto de contato (menos `skinWidth`).

## Quem usa

`BaseEntityMovement.UpdateMovement` chama os dois a cada `FixedUpdate`, e aplica o resultado com `Rigidbody.MovePosition`. Veja [entities.md](entities.md).

## Configuração (Inspector)

| Campo | Valor atual | Função |
|---|---|---|
| `groundLayerMask` | layer `Ground` | O que conta como chão |
| `groundCheckRadius` | 0.1 | Raio da esfera de checagem |
| `groundCheckOffset` | (0, −0.9, 0) | Deslocamento da esfera em relação ao pivô. A base da esfera deve coincidir com o pé do sprite |
| `skinWidth` | 0.01 | Folga para não "grudar" nas superfícies |

Com a entidade selecionada, o Gizmo mostra a esfera (verde) e a direção do cast (vermelho).

## Colisão entre corpos

Quem bloqueia quem é definido pela **Layer Collision Matrix** (Project Settings → Physics). Hoje `Player`, `Enemy`, `Ground` e `Interactables` colidem entre si, inclusive **Player com Player**. Veja [scene-and-assets.md](scene-and-assets.md#layers-e-matriz-de-colisão).
