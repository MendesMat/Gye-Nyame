# 0017 — Pesquisa → execução → revisão, com arquiteto humano

- **Status:** Aceita
- **Data:** 2026-09-30
- **Complementada por:** [0023](0023-protocolo-das-tres-sessoes.md), que define os comandos, as labels de estado e o que a revisão pode fazer. A fase "pesquisa" passou a se chamar **levantamento**, e os nomes das colunas do quadro mudaram.

## Contexto

Há um único programador, que atua como **arquiteto**. A execução do código é feita por **agentes de IA**. Um agente que pesquisa, implementa e revisa na mesma janela de contexto acumula ruído da exploração e revisa o próprio trabalho com viés.

## Decisão

Cada issue passa por **três agentes, em três janelas de contexto separadas**, e termina com a aprovação humana:

1. **Pesquisa:** lê tudo que a issue exige e publica um comentário estruturado na issue (commit de referência, skills necessárias ([0022](0022-skills-obrigatorias-para-agentes.md)), leitura obrigatória, estado atual, plano, armadilhas, critérios de aceite refinados, perguntas em aberto).
2. **Execução:** um agente novo lê o comentário, implementa numa branch própria e abre um PR.
3. **Revisão:** um agente novo revisa o PR contra os critérios de aceite, as ADRs e a documentação, e roda os testes.
4. **Merge:** **só o arquiteto aprova e faz o merge.**

Regras:
- **Pergunta em aberto bloqueia:** a issue recebe `needs-decision` e para até o arquiteto responder. Agentes não decidem no lugar dele.
- **Pesquisa envelhecida:** se, entre a pesquisa e a execução, mudou algo relevante desde o commit de referência, a issue volta para pesquisa.
- **Limite de revisão:** depois de 2 ciclos sem aprovação, a issue sobe para o arquiteto.
- **Uma branch e um PR por issue**, com até cerca de 400 linhas alteradas. Maior que isso, a issue é quebrada.
- **Acompanhamento** num quadro do GitHub Projects: Backlog → Pesquisada → Em execução → Em revisão → Aguardando merge → Concluída.

O procedimento completo está em [workflow/agents.md](../workflow/agents.md).

## Alternativas consideradas

- **Um agente faz tudo:** contexto poluído e auto-revisão enviesada.
- **Merge automático após a revisão do agente:** os primeiros PRs definem padrões que os próximos agentes copiam; um erro de arquitetura propagado custa caro. Pode ser reavaliado quando houver confiança no fluxo.

## Consequências

- A pesquisa fica registrada na issue e pode ser auditada.
- A documentação em `docs/` precisa ser boa o suficiente para que um agente sem contexto entenda a tarefa.
- O arquiteto é o gargalo de merge, por escolha.
