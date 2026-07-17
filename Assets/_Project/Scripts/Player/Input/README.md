# Módulo: Player Input (`GyeNyame.Player.Input`)

Este módulo é responsável pela camada mais externa de interação do jogador com o jogo. Ele intercepta as entradas de hardware (teclado, mouse, gamepad) através do Unity Input System e as converte em intenções de domínio, distribuídas por toda a arquitetura através do `EventBus`.

## Mecânicas e Funcionalidades

- **Mapeamento de Ações Básicas:** Captura de sinais de `Move`, `Jump`, `Dash`, `AttackLight` e `AttackHeavy`.
- **Gerenciamento de Contexto:** Possui suporte para alternar mapas de ação (ex.: `SwitchToPlayerMode` e `SwitchToUIMode`), permitindo que a entrada do jogador seja roteada apenas para UI quando necessário (menus abertos, pausa).
- **Tradução de Input para Mensagens:** Remove o acoplamento do Unity Input System do resto do jogo. O input cru é embalado em *Messages* específicas (`Core.Contracts.Messages`).

## Como Usar

1. O componente principal deste módulo é o `PlayerInputHandler`.
2. Adicione-o a um GameObject (geralmente na raiz do Player).
3. Certifique-se de que o arquivo `PlayerInputActions` (gerado pelo Unity Input System) exista e esteja acessível, pois o script o instancia no `Awake()`.
4. Ele cuidará automaticamente da habilitação e desabilitação dos inputs em `OnEnable` e `OnDisable`.

## Fluxo de Comunicação e Arquitetura

Este módulo atua unicamente como um **Emissor (Publisher)** de informações. Ele não consome informações de jogabilidade, o que mantém sua responsabilidade puramente focada na captação de intenções.

### De onde recebe informação?
- **Unity Input System:** Recebe *callbacks* disparados quando o jogador pressiona ou solta um botão físico.

### O que faz com a informação?
- Avalia o contexto da ação (ex: lê um `Vector2` em `OnMovePerformed` ou entende `Vector2.zero` em `OnMoveCanceled`).
- Empacota essa intenção dentro de structs concretas de Mensagem.

### Para onde envia informação?
- Envia a mensagem traduzida para o **Core** usando `EventBus.Publish(...)`.

- **Unity Input System** ➔ *Callbacks* ➔ **PlayerInputHandler**
- **PlayerInputHandler** ➔ *Publish Move/Jump/Dash/Attack Messages* ➔ **EventBus**
- **EventBus** ➔ *Broadcast* ➔ **Módulos Consumidores (Movement, Combat, etc.)**
