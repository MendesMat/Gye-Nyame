# Gye-Nyame

Gye-Nyame é um jogo de ação no estilo **beat 'em up 2.5D**, desenvolvido na **Unity 6**. O jogo está em transição de singleplayer para **coop online de 2 jogadores**, mantendo o modo solo.

## Documentação

Toda a documentação do projeto fica em [`docs/`](docs/README.md):

- **Como o código funciona hoje:** [docs/architecture](docs/architecture/overview.md)
- **Para onde vai o multiplayer:** [docs/multiplayer](docs/multiplayer/overview.md)
- **Por que cada decisão foi tomada:** [docs/decisions](docs/decisions/README.md)
- **Valores ajustáveis pelo game designer:** [docs/design/tuning.md](docs/design/tuning.md)
- **Como trabalhar no repositório:** [docs/workflow](docs/workflow/agents.md)

Agentes de IA começam por [`CLAUDE.md`](CLAUDE.md).

## Requisitos

- Unity **6000.3.9f1**
- Git LFS (`git lfs install` antes de clonar)
- Unity CLI, para automação do Editor ([docs/workflow/unity-cli.md](docs/workflow/unity-cli.md))
- Unity Smart Merge registrado na máquina ([docs/workflow/git.md](docs/workflow/git.md#unity-smart-merge))

## Módulos

O código fica em `Assets/_Project/Scripts`, dividido em assemblies independentes: **Core** (alicerce), **Physics**, **Entities**, **Combat**, **Player** (Input, Movement, Combat), **Enemy**, **AI** e **UI**. A visão geral de como eles se conectam está em [docs/architecture/overview.md](docs/architecture/overview.md).
