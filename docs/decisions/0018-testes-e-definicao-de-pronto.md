# 0018 — Estratégia de testes e definição de pronto

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

Não existe nenhum teste automatizado. Com agentes executando as issues, os testes são a rede de segurança que diz se algo quebrou. Testes automatizados de rede, porém, custam caro para quem trabalha sozinho.

## Decisão

### Testes

- **EditMode** (rápidos, sem cena) para a **lógica pura**: vidas compartilhadas, cálculo de dificuldade, árvore de combo, tokens e escolha de alvo do diretor, buffer de input, regras de seleção de personagem.
- **PlayMode** só onde for barato, como a máquina de estados de uma entidade.
- **Multiplayer:** validado por **checklist manual**, com o **Multiplayer Play Mode** (vários jogadores no mesmo Editor) no dia a dia e **duas instâncias da build** para validar a build real. Inclui **simulação de latência e perda de pacotes**.
- Os testes rodam pela Unity CLI (`unity test`). **Sem CI por enquanto.**
- Para ser testável, a lógica pura fica em classes C# comuns (sem `MonoBehaviour`), chamadas pelos componentes.

### Definição de pronto de uma milestone

1. Todas as issues fechadas via PR aprovado.
2. `docs/` reflete o estado real do código.
3. Uma **build Windows** passa no checklist de fumaça.
4. Nas milestones de rede, o checklist roda com **150 ms de latência e 2% de perda de pacotes** simulados.

As builds de playtest no itch.io saem ao final das milestones que produzirem algo jogável e mostrável (respeitando a restrição da [0011](0011-dois-personagens-e-selecao.md) para builds públicas).

## Consequências

- As primeiras issues de refatoração ficam um pouco maiores, porque criam os testes junto.
- Os checklists manuais são responsabilidade do arquiteto: agentes não jogam.
- Checklists em [multiplayer/testing.md](../multiplayer/testing.md).
