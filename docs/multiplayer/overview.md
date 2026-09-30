# Multiplayer — visão geral da arquitetura-alvo

> Este documento descreve **para onde** o código vai. Nada disto está implementado ainda. Quando uma parte for implementada, o texto migra para `docs/architecture/` e sai daqui.

## Em uma frase

Coop online de **2 jogadores**, **um jogador é o host** (roda a simulação dos inimigos e as regras) e o outro se conecta a ele. **Cada jogador manda no próprio personagem.** O solo é o mesmo jogo com um host sozinho e offline.

Decisões de base: [0002](../decisions/0002-coop-online-p2p-com-host.md), [0003](../decisions/0003-netcode-for-gameobjects.md), [0004](../decisions/0004-modelo-de-autoridade.md), [0005](../decisions/0005-servico-de-conexao-e-descoberta.md), [0006](../decisions/0006-modo-solo-como-host-local.md).

## Pilha técnica

| Camada | Tecnologia |
|---|---|
| Biblioteca de rede | Netcode for GameObjects 2.x (`com.unity.netcode.gameobjects`) |
| Transporte | Unity Transport (`com.unity.transport`), que vem com o NGO |
| Sessões, código de sala e retransmissão | Unity Multiplayer Services (`com.unity.services.multiplayer`) |
| Identidade | Login anônimo da Unity Authentication (automático, sem conta) |
| Teste local com vários jogadores | Multiplayer Play Mode (`com.unity.multiplayer.playmode`) |
| Câmera | Cinemachine 3 (já instalado) |

As versões são fixadas no `Packages/manifest.json` na issue que instalar cada pacote.

## Módulo de rede: `GyeNyame.Networking`

Novo assembly em `Assets/_Project/Scripts/Networking`. Ele concentra **tudo que conhece o NGO e os serviços**.

```
                       GyeNyame.Core  (continua sem saber de rede)
                             ▲
     Entities, Combat, Player, Enemy, AI, UI  (gameplay, sem NGO)
                             ▲
                    GyeNyame.Networking  (NGO + serviços)
```

**Padrão: adaptadores de rede.** Os componentes de gameplay continuam `MonoBehaviour` comuns e testáveis sem rede. Cada entidade recebe, no próprio prefab, componentes `NetworkBehaviour` do assembly de rede que:

- sincronizam o estado que precisa ser visto pelo outro lado (posição, `EntityStateCategory`, direção do rosto, vida);
- traduzem eventos locais em RPCs e RPCs em eventos locais;
- informam à gameplay, por uma **interface em `Core/Contracts`**, se esta máquina tem autoridade sobre a entidade.

Interface prevista em Core (nome e forma finais definidos na issue que a criar):

```csharp
public interface IEntityAuthority
{
    bool IsLocallyControlled { get; } // esta máquina simula esta entidade
    bool IsHost { get; }              // esta máquina é o host
    int OwnerPlayerIndex { get; }     // 0 ou 1 para personagens; -1 para entidades do host
}
```

A gameplay consulta `IsLocallyControlled` antes de rodar lógica de simulação (estados, IA, detecção de golpe). A réplica só exibe.

## Objetos de rede

| Objeto | Prefab | Dono | Quem cria |
|---|---|---|---|
| Personagem jogável (um por jogador) | um por `CharacterDefinition` | o jogador | host, ao iniciar a fase, com a ficha escolhida |
| Inimigo | um por tipo de inimigo | host | host, nos pontos de spawn da fase |
| Estado da partida (vidas, dificuldade, fluxo) | `MatchState` | host | host, ao iniciar a fase |
| Estado da sala (jogadores, seleção, pronto) | `RoomState` | host | host, na cena de sala |

Tudo que é de rede **é prefab** e está registrado na lista de prefabs de rede do `NetworkManager`. A cena de fase contém só cenário, pontos de spawn e objetos locais (câmera, HUD).

## Cenas

| Cena | Conteúdo |
|---|---|
| `MainMenu` | Menu principal. Contém o `NetworkManager` (persistente entre cenas) |
| `Room` | Sala: código, jogadores conectados, seleção de personagem, "Pronto" |
| `Level_01` | A fase atual (hoje `SampleScene`) |

No online, a troca de cena é feita pelo gerenciador de cenas do NGO, para as duas máquinas trocarem juntas.

## Serviço de sessão

Toda criação e entrada de partida passa por uma interface própria, para que o provedor possa mudar sem tocar em nada fora de `Networking` ([0005](../decisions/0005-servico-de-conexao-e-descoberta.md)):

```csharp
public interface ISessionService
{
    Task<string> HostOnlineAsync();          // cria sessão e devolve o código de sala
    Task JoinByCodeAsync(string code);
    void HostDirect(ushort port);            // eventos: rede local, sem internet
    void JoinDirect(string address, ushort port);
    void StartSolo();                        // host local, sem contatar serviço
    Task LeaveAsync();
    event Action<SessionEndReason> SessionEnded;
}
```

A implementação online usa a API de sessões do pacote `com.unity.services.multiplayer` (criação com rede retransmitida, entrada por código). A direta e a solo configuram o Unity Transport com endereço e porta e chamam `StartHost`/`StartClient` do NGO sem inicializar os serviços.

## Onde continuar

- Quem é dono de quê, sistema por sistema: [authority.md](authority.md)
- Regras de coop: [game-rules.md](game-rules.md)
- Diretor de combate: [combat-director.md](combat-director.md)
- Menus, sala, seleção, HUD: [session-flow.md](session-flow.md)
- Testes e checklists: [testing.md](testing.md)
- O que muda em cada arquivo atual: [migration-map.md](migration-map.md)
