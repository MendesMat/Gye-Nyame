# Registros de decisão (ADRs)

Cada arquivo registra **uma** decisão de arquitetura, de design ou de processo: o contexto, o que foi decidido, as alternativas rejeitadas e as consequências.

**Para agentes de IA:** uma decisão com status *Aceita* não é rediscutida dentro de uma issue. Se a tarefa parecer exigir contrariar uma ADR, pare e sinalize com a label `needs-decision`. Só o arquiteto (Matheus) muda uma decisão, criando uma ADR nova que substitui a antiga.

## Índice

| # | Decisão | Status |
|---|---|---|
| [0001](0001-refatorar-em-vez-de-reescrever.md) | Refatorar e migrar em vez de reescrever | Aceita |
| [0002](0002-coop-online-p2p-com-host.md) | Coop online P2P de 2 jogadores, com um jogador como host | Aceita |
| [0003](0003-netcode-for-gameobjects.md) | Netcode for GameObjects como biblioteca de rede | Aceita |
| [0004](0004-modelo-de-autoridade.md) | Dono do personagem manda nele; host manda no resto | Aceita |
| [0005](0005-servico-de-conexao-e-descoberta.md) | Serviço de conexão da Unity, código de sala e IP direto | Aceita |
| [0006](0006-modo-solo-como-host-local.md) | Modo solo como host local offline | Aceita |
| [0007](0007-eventbus-local-e-identidade.md) | EventBus continua local; mensagens ganham identidade | Aceita |
| [0008](0008-hitstop-local-por-entidade.md) | Hitstop local por entidade; fim do `timeScale` global | Aceita |
| [0009](0009-diretor-de-combate-com-tokens-por-alvo.md) | Diretor de combate com tokens por alvo | Aceita |
| [0010](0010-vidas-compartilhadas-e-renascimento.md) | Vidas compartilhadas e renascimento | Aceita |
| [0011](0011-dois-personagens-e-selecao.md) | Dois personagens jogáveis e seleção na sala | Aceita |
| [0012](0012-camera-compartilhada.md) | Câmera única enquadrando os dois | Aceita |
| [0013](0013-escalonamento-de-dificuldade.md) | Dificuldade pelo número de jogadores conectados | Aceita |
| [0014](0014-fluxo-de-menus-cenas-e-hud.md) | Fluxo de menus, cenas e HUD | Aceita |
| [0015](0015-valores-de-design-em-scriptableobjects.md) | Todo valor de design em ScriptableObjects | Aceita |
| [0016](0016-documentacao-em-docs.md) | Documentação em `/docs` como fonte única | Aceita |
| [0017](0017-fluxo-de-trabalho-com-agentes.md) | Pesquisa → execução → revisão, com arquiteto humano | Aceita |
| [0018](0018-testes-e-definicao-de-pronto.md) | Estratégia de testes e definição de pronto | Aceita |
| [0019](0019-agentes-operam-o-editor-via-unity-cli.md) | Agentes operam o Editor pela Unity CLI | Aceita |
| [0020](0020-escopo-da-transicao.md) | Escopo da transição e não-objetivos | Aceita |

## Modelo

```markdown
# NNNN — Título

- **Status:** Proposta | Aceita | Substituída por NNNN
- **Data:** AAAA-MM-DD

## Contexto
## Decisão
## Alternativas consideradas
## Consequências
```
