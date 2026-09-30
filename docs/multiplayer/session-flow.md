# Fluxo de sessão: menus, sala, seleção e HUD

> Arquitetura-alvo. Base: [ADR 0014](../decisions/0014-fluxo-de-menus-cenas-e-hud.md), [0011](../decisions/0011-dois-personagens-e-selecao.md), [0005](../decisions/0005-servico-de-conexao-e-descoberta.md).

## Fluxo de telas

```
MainMenu
 ├ Jogar sozinho ─────────► Room (local, sem código) ─────────────► Level_01
 ├ Criar sala ────────────► Room (online, mostra código) ─────────► Level_01
 ├ Entrar com código ─────► Room
 └ Conexão direta (IP) ───► Room
Level_01 ─► Vitória / Game Over ─► Room (online)  ou  MainMenu (solo)
Host saiu / erro de conexão ─► MainMenu com aviso
```

## MainMenu

| Botão | Ação |
|---|---|
| Jogar sozinho | `ISessionService.StartSolo()` e vai para `Room` |
| Criar sala | `HostOnlineAsync()`; mostra o código na `Room` |
| Entrar com código | Campo de texto → `JoinByCodeAsync(code)` |
| Conexão direta | Campos de IP e porta → `HostDirect`/`JoinDirect` (para eventos em rede local) |
| Sair | Fecha o jogo |

Erros (código inválido, sala cheia, sem internet) aparecem como mensagem na própria tela, sem travar.

## Room (sala)

Estado do host, sincronizado (`RoomState`):

| Dado | Descrição |
|---|---|
| Jogadores conectados | 1 ou 2, com identificação "você" |
| Personagem de cada jogador | nenhum, personagem A ou B |
| Pronto de cada jogador | sim/não |
| Código da sala | só no online |

Regras:
- O jogador clica num personagem; o pedido vai ao host. Se estiver livre, o host atribui; se não, recusa. Empate: vale quem chegou primeiro ao host.
- Personagem de um jogador fica **travado** para o outro.
- "Pronto" só habilita com personagem escolhido. A fase começa quando **todos os conectados** estão prontos com personagens diferentes. No solo, basta escolher e confirmar.
- Se o convidado sair da sala, a seleção dele é liberada.
- Ao voltar de uma partida, as seleções são mantidas mas podem ser trocadas, e o "Pronto" é zerado.

## Level_01 (fase)

- O host cria os personagens a partir das fichas escolhidas, cada um com o dono certo, e os inimigos nos pontos de spawn.
- A cena não contém mais Player nem Dummies soltos, só cenário, pontos de spawn, câmera e HUD.

### Pausa

| Modo | Comportamento |
|---|---|
| Solo | Abre o menu e **para o jogo** |
| Online | Abre o menu **sem parar a partida**; o personagem fica parado enquanto o menu está aberto |

Opções do menu de pausa: Continuar, Sair para a sala (online, host encerra para os dois), Sair para o menu.

## HUD

```
┌─────────────────────────────────────────────────────────────┐
│ [retrato] Personagem A   ♥♥♥ ×3 ♥♥♥   Personagem B [retrato]│
│ ████████░░ (você)       vidas da dupla       ██████████     │
│                                                             │
│                     Renascendo em 2…                        │
│                                                             │
│                  [barra do último inimigo]                  │
└─────────────────────────────────────────────────────────────┘
```

- Personagem 1 (primeiro slot da sala) à **esquerda**, personagem 2 à **direita**, **iguais nas duas máquinas**. O painel do jogador local tem um destaque de "você".
- **Vidas compartilhadas** no centro superior.
- **Barra do inimigo:** o último inimigo que **você** acertou (calculado localmente).
- **Contagem de renascimento** para quem está morto.
- **"Parceiro desconectado"** quando o convidado cai.
- No solo, só o painel do jogador aparece.
- Tudo em prefab de UI (uGUI). Os presenters recebem os jogadores da sessão, em vez de procurar por tag.
