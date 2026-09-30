# Gye-Nyame — Guia para agentes de IA

Beat 'em up 2.5D em **Unity 6000.3.9f1** (URP, sprites 2D em mundo 3D). O jogo está em **transição de singleplayer para coop online de 2 jogadores** (Netcode for GameObjects, um jogador hospeda).

Este arquivo é o ponto de entrada. A fonte da verdade é a pasta [`docs/`](docs/README.md).

## Antes de qualquer tarefa

1. Leia [`docs/README.md`](docs/README.md) para saber onde está cada assunto.
2. Leia o documento do módulo que você vai tocar em [`docs/architecture/`](docs/architecture/).
3. Se a tarefa envolve rede, coop ou regras novas, leia [`docs/multiplayer/`](docs/multiplayer/).
4. Consulte [`docs/decisions/`](docs/decisions/README.md) antes de propor uma alternativa: **decisões registradas não são rediscutidas** dentro de uma issue. Se achar que uma decisão está errada, pare e pergunte (label `needs-decision`).
5. **Defina as skills que a tarefa exige e carregue-as antes de começar** ([`docs/workflow/skills.md`](docs/workflow/skills.md)):
   - **`clean-code` é obrigatória para gerar ou alterar qualquer código;**
   - **as skills da Unity são obrigatórias em cada caso adequado** (UI, rede, física, pacotes, Editor via CLI…).
6. Siga o fluxo de trabalho em [`docs/workflow/agents.md`](docs/workflow/agents.md) (pesquisa → execução → revisão).

## Regras que nunca mudam

- **Todo código segue a skill `clean-code`**, e toda tarefa usa as skills da Unity do seu domínio. Veja [ADR 0022](docs/decisions/0022-skills-obrigatorias-para-agentes.md).
- **Core não depende de ninguém.** Módulos conversam por interfaces em `Core/Contracts` ou mensagens no `EventBus`. Veja [overview](docs/architecture/overview.md).
- **Todo número ajustável de design fica em ScriptableObject** e é documentado em [`docs/design/tuning.md`](docs/design/tuning.md). Nada de constante mágica em código de gameplay.
- **Não edite `.unity`, `.prefab` ou `.asset` à mão** quando houver Editor conectado. Use a Unity CLI. Veja [`docs/workflow/unity-cli.md`](docs/workflow/unity-cli.md).
- **Todo PR atualiza os documentos de `docs/` que o código tocou.** Documentação desatualizada é bug.
- **Nenhum segredo no repositório** (ele é público): chaves Steamworks, tokens, credenciais de serviço.
- Texto de documentação em **português**; identificadores de código em **inglês**.

## Comandos úteis

```bash
unity status                                    # há Editor conectado?
unity test . --mode EditMode                    # roda os testes EditMode
unity command --caller plugin --skill gye-nyame # lista comandos do Editor
```

## Mapa rápido do código

| Pasta | Assembly | Documento |
|---|---|---|
| `Assets/_Project/Scripts/Core` | `GyeNyame.Core` | [core.md](docs/architecture/core.md) |
| `Assets/_Project/Scripts/Physics` | `GyeNyame.Physics` | [physics.md](docs/architecture/physics.md) |
| `Assets/_Project/Scripts/Entities` | `GyeNyame.Entities` | [entities.md](docs/architecture/entities.md) |
| `Assets/_Project/Scripts/Combat` | `GyeNyame.Combat` | [combat.md](docs/architecture/combat.md) |
| `Assets/_Project/Scripts/Player` | `GyeNyame.Player.*` | [player.md](docs/architecture/player.md) |
| `Assets/_Project/Scripts/Enemy` | `GyeNyame.Enemy` | [enemy.md](docs/architecture/enemy.md) |
| `Assets/_Project/Scripts/AI` | `GyeNyame.AI` | [ai.md](docs/architecture/ai.md) |
| `Assets/_Project/Scripts/UI` | `GyeNyame.UI` | [ui.md](docs/architecture/ui.md) |
