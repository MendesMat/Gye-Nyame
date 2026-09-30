# Interface (`GyeNyame.UI`)

Pasta: `Assets/_Project/Scripts/UI`. Usa **uGUI** (Canvas). Segue o padrão **Presenter + View**: o presenter escuta o jogo e decide o que mostrar; a view só desenha.

| Arquivo | Papel |
|---|---|
| `Views/UIHealthBar.cs` | View: `UpdateFill(0..1)` no `Image.fillAmount` |
| `Presenters/PlayerHealthPresenter.cs` | Barra do jogador |
| `Presenters/EnemyHealthPresenter.cs` | Barra do último inimigo atingido |

## Presenters

- **`PlayerHealthPresenter`**: no `Start`, acha o player por **tag** (`targetTag`, com dropdown `[TagSelector]`) e preenche a barra. A cada `EntityDamagedMessage` cujo alvo tem essa tag, atualiza.
- **`EnemyHealthPresenter`**: começa oculto. A cada `EntityDamagedMessage` de alvo com a tag de inimigo, passa a mostrar esse inimigo e atualiza. Esconde quando esse inimigo morre.
- Os dois assinam no `Start` e desassinam no `OnDestroy` (e não em `OnEnable`/`OnDisable`), porque o `EnemyHealthPresenter` se desativa e precisa continuar ouvindo.

## Cena

```
HUDCanvas (Canvas, CanvasScaler, GraphicRaycaster)
├ PlayerHUD  (UIHealthBar, PlayerHealthPresenter)  └ HealthFill (Image)
└ EnemyHUD   (UIHealthBar, EnemyHealthPresenter)   └ HealthFill (Image)
EventSystem
```

## Limitações atuais

- Supõe um único player (busca por tag).
- Não há menus, pausa nem tela de game over. O jogo recarrega a cena sozinho.

O HUD de coop e os menus estão em [multiplayer/session-flow.md](../multiplayer/session-flow.md).
