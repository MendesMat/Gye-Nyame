# 0007 — EventBus continua local; mensagens ganham identidade

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

O `EventBus` é estático e global. As mensagens de input (`PlayerMoveMessage`, `PlayerAttackLightMessage`…) não dizem de qual jogador vieram, e os ouvintes supõem que só existe um player. Com dois personagens na mesma máquina (o próprio e a réplica do parceiro), um botão moveria os dois.

## Decisão

- O **EventBus continua existindo e continua local** a cada máquina. Ele não atravessa a rede.
- **O que precisa atravessar a rede usa as ferramentas do NGO:** variáveis sincronizadas (`NetworkVariable`) para estado e RPCs para eventos pontuais. O código de rede, ao receber, pode republicar no EventBus local.
- **Mensagens ganham identidade:**
  - mensagens de input carregam o **identificador do jogador local** que as gerou;
  - mensagens sobre entidades continuam carregando o GameObject da raiz (`Target`/`Entity`), que já identifica a entidade.
- Os componentes de um personagem **só reagem ao input do próprio dono**. A réplica do parceiro nunca lê input local.

## Alternativas consideradas

- **Tornar o EventBus "de rede":** misturaria transporte com lógica de jogo e esconderia custos de banda atrás de um `Publish`.
- **Substituir o EventBus por referências diretas:** perderia o desacoplamento que já funciona bem.

## Consequências

- Todas as mensagens de input mudam de assinatura. É uma refatoração mecânica, mas atinge Input, Movement e Combat do player.
- Ao escrever um ouvinte, a pergunta passa a ser: "isso deve rodar em toda máquina, só no dono ou só no host?". A resposta está em [multiplayer/authority.md](../multiplayer/authority.md).
- As limitações do bus (boxing, `Clear<T>`, domain reload) ficam registradas em [architecture/known-issues.md](../architecture/known-issues.md) e podem ser tratadas na mesma refatoração.
