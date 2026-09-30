# 0014 — Fluxo de menus, cenas e HUD

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

Hoje o jogo tem uma única cena, sem menus, e recarrega sozinho no fim da partida. O coop precisa de um lugar para criar ou entrar numa partida e escolher personagem.

## Decisão

### Fluxo

```
Menu principal
 ├ Jogar sozinho ──────────────► Sala local (escolhe personagem) ─► Fase
 ├ Criar sala (online) ────────► Sala: mostra código, espera P2, seleção, ambos "Pronto" ─► Fase
 ├ Entrar com código ──────────► Sala
 └ Conexão direta (IP) ────────► Sala
Fase ─► Vitória ou Game Over ─► Sala (online) ou Menu (solo)
```

- **Cenas:** no mínimo **Menu**, **Sala** e **Fase**. No online, a troca de cena é conduzida pelo NGO.
- **Pausa:** no solo, pausa o jogo. No online, abre o menu **sem parar a partida**.
- **Menu de opções** (volume, controles): fora do escopo da transição.

### HUD

- Vida do **personagem 1 no canto superior esquerdo** e do **personagem 2 no superior direito**, com retrato e nome, iguais nas duas máquinas, com destaque de "você".
- **Contador de vidas compartilhadas** no centro superior.
- Barra do **último inimigo que você acertou**, calculada localmente.
- **Contagem regressiva de renascimento** para quem morreu.
- Aviso **"Parceiro desconectado"**.
- Posições e textos em **prefab de UI**, editáveis pelo designer ou artista.

## Consequências

- O `GameResetHandler` (recarregar a cena) é substituído pelo fluxo Fase → Sala/Menu.
- Os presenters de UI deixam de achar o player por tag e passam a receber os jogadores da sessão.
- Detalhes em [multiplayer/session-flow.md](../multiplayer/session-flow.md).
