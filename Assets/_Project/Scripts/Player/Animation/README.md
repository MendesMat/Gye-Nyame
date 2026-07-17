# Módulo: Player Animation (`GyeNyame.Player.Animation`)

Este módulo é o elo visual. Ele traduz os estados lógicos (da máquina de estados do jogador) em reprodução de clipes de animação, através da ponte com o `Animator` do Unity, e envia *feedbacks* de *timing* (por *Animation Events*) para ajudar lógicas de tempo críticas (como combos de combate).

## Mecânicas e Funcionalidades

- **Sincronização de Estado/Animação:** Quando a `StateMachine` transita, este módulo capta e reproduz a animação exata baseada no nome do estado, de forma genérica (utilizando o hash do nome para performance).
- **Rosto e Inversão de Sprite (Flipping):** Monitora a direção de movimento através da interface `IPlayerLocomotion`. Se o jogador andou para a esquerda (X < 0), ele vira (flipX) o componente `SpriteRenderer`.
- **Ancoragem de Eventos na Timeline:** Usando *Unity Animation Events*, possibilita avisar ao `Core` o exato *frame* onde um ataque permite "cancelamento" ou o momento exato em que a animação acabou, libertando o estado.

## Como Usar

1. No GameObject, anexe o `PlayerAnimationHandler` (e `PlayerAnimationEventHandler` caso os eventos já tenham sido separados).
2. É obrigatório ter um `Animator` (com os *States* configurados e nomeados com o mesmo nome que consta no *Script* — Ex: `"Walk"`, `"Jump"`) e um `SpriteRenderer`.
3. O componente deve estar no mesmo *GameObject* (ou ser filho direto) onde repousa a `StateMachine` e os scripts que implementam `IPlayerLocomotion` (Geralmente o script de *Movement*).

## Fluxo de Comunicação e Arquitetura

Trabalha de forma bidirecional e fracamente acoplada. É reativo em relação aos estados da lógica, mas proativo ao enviar sinais de *timing* (eventos visuais).

### De onde recebe informação?
- **StateMachine (Lógica Direta):** Escuta o evento local de C# nativo (`_stateMachine.OnStateChanged`).
- **IPlayerLocomotion (Contexto):** Lê via *interface* a variável de direção (`FacingDirectionX`) provida pelo sistema de movimento para virar o sprite visualmente.
- **Unity Animator (Pipeline C++ -> C#):** Lê *Animation Events* (marcadores de tempo colocados fisicamente no `.anim`) através de funções públicas interceptadas.

### O que faz com a informação?
- Se o evento de mudança de estado disparou (ex: de `PlayerIdleState` para `PlayerWalkState`), usa o *dictionary* de *Hashes* para pedir ao `Animator` que dê um `Play()` na animação respectiva de `"Walk"`.
- Modifica o booleano `spriteRenderer.flipX` a cada frame baseando-se em `_facingDirectionX < 0`.

### Para onde envia informação?
- Funciona como um **Emissor** publicando na arquitetura via `EventBus`:
    - `EventBus.Publish(new AnimationCancelWindowMessage(true/false))` 
    - `EventBus.Publish(new AnimationFinishAttackMessage())`
- Tais sinais vão ser engolidos principalmente pelo `PlayerCombat` e afins para orquestrar as *StateMachines* dependentes de tempo visual.

---

- **StateMachine** ➔ *OnStateChanged (C# Event)* ➔ **PlayerAnimationHandler**
- **IPlayerLocomotion** ➔ *Provê X Direction* ➔ **PlayerAnimationHandler**
- **PlayerAnimationHandler** ➔ *Play(Hash) & FlipX* ➔ **Visual (Animator & SpriteRenderer)**
- **Visual** ➔ *Animation Events (Timeline)* ➔ **PlayerAnimationEventHandler**
- **PlayerAnimationEventHandler** ➔ *Publish AnimationCancelWindowMessage* ➔ **EventBus**
- **PlayerAnimationEventHandler** ➔ *Publish AnimationFinishAttackMessage* ➔ **EventBus**
