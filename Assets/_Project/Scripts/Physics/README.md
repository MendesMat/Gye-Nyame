# Módulo: Kinematic Physics (`GyeNyame.Physics`)

Este módulo gerencia colisões customizadas e testes de física voltados para movimentações de controle manual e rigoroso (cinemática). Ao invés de delegar a movimentação livremente à gravidade ou colisões padrões da Unity, ele simula passos (Sweep/Casts) e previne comportamentos irreais indesejáveis em um cenário isométrico 2.5D, garantindo a aterrissagem estável e evitando travessias (tunneling).

## Mecânicas e Funcionalidades

- **Ground Check (Verificação de Chão Anti-Tunneling):** Através de um `SphereCast` contínuo baseado na distância exata pretendida de queda por frame, garante que o personagem nunca atravesse o chão. Respeita dinamicamente a rotação visual (espaço local) e utiliza margens de segurança (`skinWidth`).
- **Movement Collision (Colisão Lateral):** Realiza uma varredura com o colisor real do personagem (`SweepTest`) avaliando com precisão distâncias horizontais/profundidade seguras e prevenindo que o player atravesse elementos com que deveria colidir.

## Como Usar

1. O GameObject precisa de um componente `Rigidbody` e, dependendo da necessidade, um `BoxCollider` (para uso horizontal do `SweepTest`).
2. Adicione o script `KinematicPhysics`.
3. No Unity Editor:
   - Defina a `Colliders Layer Mask` contendo os elementos de colisão do cenário.
   - Usando a aba Scene, ajuste o `Ground Check Offset` e `Radius` com base nos Gizmos gerados (linha vermelha e esfera verde). A parte de baixo da esfera deve estar perfeitamente alinhada à sola do pé da Sprite do personagem.
4. Nos scripts Consumidores (como `PlayerMovement`), referencie a interface `IKinematicPhysics` usando um campo `MonoBehaviour` exposto no Inspector e processe o movimento sempre limitando as distâncias através do componente.

## Fluxo de Comunicação e Arquitetura

O sistema atua como um **Prestador de Serviço/Dependência** puramente calculista para módulos core de movimentação, não se acoplando com a gerência geral de eventos do jogo.

### De onde recebe informação?
- **Core (Contratos):** Scripts de lógica superior, como o `PlayerMovement`, invocam os métodos diretamente requerendo cálculos através da interface `IKinematicPhysics` e passando vetores de intenção (distância desejada).

### O que faz com a informação?
- Submete os vetores, offsets locais e colisores ao Motor Físico da Unity (`UnityEngine.Physics`).
- Filtra, converte e compensa o que bateu em uma margem aceitável usando a tolerância definida por `skinWidth`.

### Para onde envia informação?
- Retorna variáveis processadas de volta imediatamente para a classe chamadora, dizendo exatamente qual é o novo vetor reduzido permitido para a movimentação ou a distância de queda máxima.

- **PlayerMovement** ➔ *Injeta Dependência / Passa Intenção* ➔ **KinematicPhysics**
- **KinematicPhysics** ➔ *SweepTest / SphereCast* ➔ **Unity Backend (Physics System)**
- **Unity Backend** ➔ *Retorna Colisões* ➔ **KinematicPhysics**
- **KinematicPhysics** ➔ *Aplica SkinWidth / Retorna Distância Permitida* ➔ **PlayerMovement**
- **KinematicPhysics** ➔ *Implementa* ➔ **IKinematicPhysics**
