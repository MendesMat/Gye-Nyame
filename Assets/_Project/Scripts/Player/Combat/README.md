# Módulo: Player Combat (`GyeNyame.Player.Combat`)

Este módulo é encarregado de toda a lógica de ataque do jogador, processando combos e utilizando o `InputBuffer` para perdoar pequenos atrasos (delay) ou adiantamentos nos comandos do usuário, deixando a *game feel* mais fluida.

## Mecânicas e Funcionalidades

- **Ataques Leves e Pesados:** Suporte para diferentes sequências de ataque (Light 1, Light 2, etc.), baseadas em ScriptableObjects de Dados (`AttackDataSO`).
- **Input Buffer:** Lê as vontades de ataque estocadas no componente `InputBuffer`. Se o jogador apertar o botão de ataque um pouco antes da animação atual terminar, o golpe será guardado e executado automaticamente na sequência.
- **Janelas de Cancelamento (Cancel Windows):** Sistema que lê marcações nas animações para permitir que um ataque em andamento seja interrompido abruptamente por certas ações prioritárias (como um `Dash` ou um `Jump`).

## Como Usar

1. O GameObject precisa de um componente `StateMachine`, `InputBuffer` e `PlayerCombat`.
2. Configure os arrays e as referências aos *ScriptableObjects* (`AttackDataSO`) no *Inspector* de `PlayerCombat` para definir dados como tempo de buffer do ataque e dano (caso haja).
3. Os estados próprios do combate (`PlayerAttackLight1State`, `PlayerAttackLight2State`, etc.) acessam a interface `IPlayerCombatContext` para ler essas informações.

## Fluxo de Comunicação e Arquitetura

O módulo de Combate atua como **Consumidor** de comandos de Input e **Emissor** de alertas de conclusão de combate, coordenando estreitamente com a Animação e o Movimento.

### De onde recebe informação?
- **EventBus (Input):** Ouve `PlayerAttackLightMessage` e `PlayerAttackHeavyMessage`.
- **EventBus (Animation):** Escuta ativamente a `AnimationCancelWindowMessage` e `AnimationFinishAttackMessage` (disparadas provavelmente via eventos na própria timeline de animação).
- **EventBus (Movement Interrupt):** Lê intenções de `PlayerDashMessage` e `PlayerJumpMessage` apenas para verificar se elas quebrarão um combo ativo (caso a janela de cancelamento esteja aberta).

### O que faz com a informação?
- Comandos de ataque são estocados no `InputBuffer` (`InputBuffer.BufferCommand`).
- A cada `Update`, caso o jogador esteja apto (em `PlayerIdleState` ou `PlayerWalkState`), ele checa o buffer e, se houver um ataque válido, manda a `StateMachine` transitar para o estado de ataque correspondente.
- Se uma mensagem de interrupção (Dash/Jump) chegar durante uma `CancelWindow`, ele reseta o combate.

### Para onde envia informação?
- Dispara um `EndCombatMessage` via `EventBus` assim que um ataque é encerrado (seja por conclusão de animação ou cancelamento), avisando ao módulo de Movimento que é possível voltar a transitar livremente (Walk/Idle).

---

- **EventBus** ➔ *PlayerAttackLightMessage / HeavyMessage* ➔ **PlayerCombat**
- **EventBus** ➔ *AnimationCancelWindowMessage / FinishAttackMessage* ➔ **PlayerCombat**
- **PlayerCombat** ➔ *Guarda no* ➔ **InputBuffer**
- **InputBuffer** ➔ *Valida* ➔ **PlayerCombat**
- **PlayerCombat** ➔ *Lê Contexto* ➔ **States (AttackLight1, AttackLight2, AttackHeavy)**
- **States** ➔ *Bloqueia Rosto* ➔ **IPlayerLocomotion**
- **PlayerCombat** ➔ *Publish EndCombatMessage* ➔ **EventBus**
