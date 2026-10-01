# Documentação do Gye-Nyame

Esta pasta é a **fonte única da verdade** sobre como o jogo funciona e para onde ele vai. Foi escrita para dois públicos: **agentes de IA** que executam issues e o **game designer** que ajusta valores no Editor.

## Como navegar

| Quero entender… | Leia |
|---|---|
| A visão geral: assemblies, dependências, padrões de comunicação | [architecture/overview.md](architecture/overview.md) |
| Um módulo específico do código atual | [architecture/](#arquitetura-atual) |
| Como a cena, os prefabs, as layers e os assets estão montados | [architecture/scene-and-assets.md](architecture/scene-and-assets.md) |
| Problemas e dívidas técnicas já conhecidos | [architecture/known-issues.md](architecture/known-issues.md) |
| Para onde a arquitetura vai com o multiplayer | [multiplayer/overview.md](multiplayer/overview.md) |
| Por que algo foi decidido de um jeito | [decisions/](decisions/README.md) |
| Que valores o game designer pode ajustar | [design/tuning.md](design/tuning.md) |
| Como trabalhar numa issue (agentes) | [workflow/agents.md](workflow/agents.md) |

## Arquitetura atual

O que existe **hoje** no código (singleplayer):

- [overview.md](architecture/overview.md): assemblies, grafo de dependências, padrões de comunicação, anatomia de uma entidade
- [core.md](architecture/core.md): EventBus, máquina de estados, InputBuffer, contratos, fluxo de jogo, tempo
- [physics.md](architecture/physics.md): física cinemática própria
- [entities.md](architecture/entities.md): movimento base, estados genéricos, animação, visuais
- [combat.md](architecture/combat.md): hitbox, hurtbox, vida, dados de ataque, diretor de combate
- [player.md](architecture/player.md): input, movimento e combate do jogador
- [enemy.md](architecture/enemy.md): corpo dos inimigos
- [ai.md](architecture/ai.md): Behavior Graph e nós customizados
- [ui.md](architecture/ui.md): HUD
- [scene-and-assets.md](architecture/scene-and-assets.md): cena, layers, ScriptableObjects, animações
- [known-issues.md](architecture/known-issues.md): bugs e dívidas técnicas

## Multiplayer (arquitetura-alvo)

- [overview.md](multiplayer/overview.md): topologia, pilha técnica, módulo de rede, cenas
- [authority.md](multiplayer/authority.md): quem é dono de quê e como cada sistema sincroniza
- [game-rules.md](multiplayer/game-rules.md): regras de coop (vidas, personagens, câmera, dificuldade, desconexão)
- [combat-director.md](multiplayer/combat-director.md): diretor de combate com tokens por alvo
- [session-flow.md](multiplayer/session-flow.md): menus, sala, seleção de personagem, HUD
- [testing.md](multiplayer/testing.md): como testar o multiplayer e checklists
- [migration-map.md](multiplayer/migration-map.md): o que muda em cada módulo atual

## Decisões, design e fluxo de trabalho

- [decisions/](decisions/README.md): registros de decisão (ADRs)
- [design/tuning.md](design/tuning.md): guia de valores ajustáveis para o game designer
- [workflow/agents.md](workflow/agents.md): protocolo das três sessões (levantamento → execução → revisão), estados e comandos
- [workflow/skills.md](workflow/skills.md): quais skills usar em cada caso (clean code obrigatória)
- [workflow/git.md](workflow/git.md): branches, PRs, Smart Merge, LFS
- [workflow/unity-cli.md](workflow/unity-cli.md): como agentes operam o Editor

## Regra de manutenção

Todo PR que muda comportamento **atualiza o documento correspondente na mesma PR**. Quando uma parte da arquitetura-alvo (`multiplayer/`) for implementada, o texto correspondente migra para `architecture/`, e `multiplayer/` passa a descrever só o que ainda falta.
