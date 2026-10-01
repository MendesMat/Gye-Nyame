# 0023 — Protocolo das três sessões: comandos, estados e revisão que só relata

- **Status:** Aceita
- **Data:** 2026-10-01
- **Complementa:** [0017](0017-fluxo-de-trabalho-com-agentes.md)

## Contexto

A [0017](0017-fluxo-de-trabalho-com-agentes.md) definiu três fases em janelas de contexto separadas. Faltava definir como cada sessão começa com uma única instrução, como uma sessão sabe que é a vez dela e o que a revisão pode fazer. A fase antes chamada de "pesquisa" passa a se chamar **levantamento**.

## Decisão

1. **Três comandos de projeto** em `.claude/skills/`: `/levantar-issue N`, `/executar-issue N` e `/revisar-issue N`. Cada um confere o estado, carrega o guia e segue o roteiro da fase.
2. **Os comandos são finos.** O roteiro de cada fase fica só em [workflow/agents.md](../workflow/agents.md). Os comandos apontam para ele.
3. **Passagem de bastão por labels de estado**, com recusa. Estados: `estado:levantamento`, `estado:pronta-para-execução`, `estado:em-execução`, `estado:em-revisão`, `estado:aprovada`. Issue sem label de estado é backlog. Uma sessão **recusa começar** se a issue não estiver no estado de entrada da fase.
4. **A label é a fonte da verdade; o quadro é espelho.** O utilitário `tools/workflow/issue-state.sh` troca a label e move o cartão no mesmo passo.
5. **O levantamento recusa issue com bloqueadora aberta.** Levantamento feito antes da hora envelhece.
6. **`needs-decision` bloqueia todos os comandos** até o arquiteto responder e remover a label.
7. **O levantamento produz** um comentário com: commit de referência, skills necessárias, arquivos envolvidos, decisões aplicáveis, estado atual, plano, plano de testes, roteiro de verificação em Play Mode, armadilhas, critérios de aceite refinados e perguntas em aberto.
8. **Roteiro de Play Mode em duas partes:** "verificável pelo agente" (a execução roda pela Unity CLI e registra no PR) e "verificável jogando" (o arquiteto roda antes do merge).
9. **A revisão só relata.** Não altera código, assets nem documentos. As correções vão para uma nova sessão de execução, em **modo correção**: mesma branch, só os achados bloqueantes.
10. **Achados classificados em bloqueantes e não bloqueantes.** Só os bloqueantes reprovam. Os não bloqueantes ficam listados e o arquiteto decide no merge.
11. **Se a execução descobrir que a issue ou o plano estão errados, ela para e comenta**, com `needs-decision`, e a issue volta ao levantamento.

## Alternativas consideradas

- **Revisão que corrige o que for pequeno:** poupa uma sessão em defeitos triviais, mas o revisor que também escreve perde a independência que justifica a sessão separada. A classificação de achados reduz esse custo sem o revisor tocar no código.
- **Só o quadro como estado, sem labels:** um lugar só, mas conferir exige o escopo `project` do token e uma busca do cartão. A label é uma consulta simples.
- **Roteiro dentro de cada comando:** duplicaria o protocolo em dois lugares, que divergiriam.

## Consequências

- Um defeito bloqueante trivial custa uma sessão de execução a mais.
- Uma sessão que cai no meio deixa a issue em `estado:em-execução` ou `estado:levantamento`. O levantamento pode ser retomado pelo mesmo comando; uma execução interrompida exige que o arquiteto devolva o estado com `issue-state.sh set`.
- O estado "Concluída" do quadro depende do fluxo automático do GitHub Projects para itens fechados, configurado uma vez na interface.
- Os comandos são específicos do Claude Code. Outros agentes seguem o mesmo roteiro lendo `docs/workflow/agents.md`.
