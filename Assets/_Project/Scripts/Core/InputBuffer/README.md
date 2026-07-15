# Sistema de Input Buffer

Este documento descreve o funcionamento e a arquitetura do sistema de buffer de entradas (Input Buffer) localizado em `Assets/_Project/Scripts/Core/InputBuffer`.

O sistema armazena intenções de comandos do jogador por uma pequena janela de tempo. Isso é essencial para jogos de ação (especialmente *Hack and Slash*), pois melhora substancialmente o *game feel*. Ele "perdoa" o jogador caso ele pressione o botão de ataque alguns *frames* antes da animação atual terminar, garantindo que o golpe saia na sequência correta sem exigir *inputs frame-perfect*.

---

## Fluxo de Funcionamento

O ciclo de vida de um comando no buffer segue três etapas lógicas principais:

### 1. Armazenamento (Buffering)
Quando uma ação é solicitada (geralmente ao escutar uma mensagem do `EventBus`), o comando é envelopado com um *timestamp* de validade (janela de tempo) e inserido na lista do buffer. Caso o mesmo tipo de comando já exista, o tempo dele é estendido/atualizado.

### 2. Validação (Polling & Cleaning)
A cada quadro (no `Update`), o componente varre a lista de comandos internos. Ele verifica se o tempo de expiração do comando já ultrapassou o `Time.time` atual da Unity. Se sim, o comando é descartado silenciosamente.

### 3. Consumo
Sistemas lógicos consumidores (como uma State Machine de Combate) perguntam ao buffer se um comando desejado está na fila e válido. Caso afirmativo, o sistema processa a ação e imediatamente **consome** o comando. Isso remove o comando da fila para evitar que ele dispare a ação mais de uma vez.

---

## Arquitetura e Scripts

O sistema é composto por duas partes principais nesta pasta:

### 1. [BufferedCommand.cs]
**Papel:** Estrutura de dados (*Struct*) que envelopa o tipo da mensagem e controla sua longevidade.

- **Funcionalidades:**
  - Armazena o `MessageType` (qual ação está aguardando, ex: `PlayerAttackLightMessage`).
  - Guarda o momento exato em que foi criado (`Timestamp`) e o momento limite de expiração (`ExpirationTime`).
  - Oferece um método auxiliar `IsValid(float currentTime)` para checar facilmente se o comando ainda está "vivo".

### 2. [InputBuffer.cs]
**Papel:** Componente `MonoBehaviour` que atua como um contêiner vivo gerenciando a lista de `BufferedCommand`.

- **Funcionalidades:**
  - **`BufferCommand<T>`**: Adiciona um comando do tipo `T` (que deve herdar de `IMessage`) na fila, calculando sua expiração com base no tempo atual + `bufferTime`.
  - **`HasCommand<T>`**: Percorre a lista checando se há um comando válido do tipo `T`.
  - **`ConsumeCommand<T>`**: Encontra a primeira ocorrência do comando `T` e o destrói, completando seu ciclo de vida.
  - **Limpeza (`Update`)**: Chama a rotina interna `CleanExpiredCommands` para matar comandos velhos e liberar memória.

---

## Como Utilizar o Buffer

Para usar este sistema de modo eficiente, o fluxo comum envolve injetar ações no buffer em resposta a mensagens do usuário, e então ler o buffer nas transições da sua State Machine.

### Adicionando comandos ao Buffer
Isso normalmente ocorre quando a classe ouve o input bruto (via EventBus):

```csharp
public class PlayerCombat : MonoBehaviour {
    public InputBuffer inputBuffer; // Referência no Inspector
    public float attackBufferTime = 0.2f; // Janela de 200ms

    void OnEnable() {
        EventBus.Subscribe<PlayerAttackLightMessage>(OnAttackLight);
    }

    void OnAttackLight(PlayerAttackLightMessage msg) {
        // Guarda o comando com uma janela de tempo de 0.2s
        inputBuffer.BufferCommand<PlayerAttackLightMessage>(attackBufferTime);
    }
}
```

### Checando e Consumindo comandos
Geralmente ocorre no `Update` da State Machine que pode usar essa ação:

```csharp
void Update() {
    // Só checamos se estamos em um estado onde podemos atacar (ex: Idle/Walk)
    if (CurrentState is PlayerIdleState || CurrentState is PlayerWalkState) {
        
        // Pergunta: O jogador pediu pra atacar recentemente?
        if (inputBuffer.HasCommand<PlayerAttackLightMessage>()) {
            
            // Consome o comando para ele sumir da fila
            inputBuffer.ConsumeCommand<PlayerAttackLightMessage>();
            
            // Executa a ação
            stateMachine.ChangeState(stateMachine.GetOrCreateState<PlayerAttackLightState>());
        }
    }
}
```
