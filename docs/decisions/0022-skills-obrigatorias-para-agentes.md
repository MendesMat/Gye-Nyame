# 0022 — Skills obrigatórias para agentes

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

Os agentes de IA que executam as issues têm acesso a *skills*: pacotes de instruções especializadas que carregam boas práticas para um tipo de tarefa. Há uma skill de código limpo e um conjunto de skills da Unity (UI, multiplayer, física, CLI, pacotes, sprites…). Sem uma regra, cada agente usaria as skills de forma inconsistente, e o código gerado variaria de estilo e qualidade de uma issue para outra.

## Decisão

1. **A skill de clean code é obrigatória para gerar qualquer código.** Todo agente que escreve ou altera C# carrega a skill `clean-code` antes de começar.
2. **As skills da Unity são obrigatórias sempre que a tarefa cair no domínio delas** (UI, multiplayer, física, pacotes, sprites, Editor via CLI…).
3. **As skills são definidas antes de começar o trabalho.** O agente de pesquisa lista, no comentário da issue, quais skills a execução vai usar e por quê. O agente de execução carrega todas elas antes de escrever qualquer coisa. O agente de revisão confere se o código segue as orientações dessas skills.

O catálogo de qual skill usar em cada caso está em [workflow/skills.md](../workflow/skills.md).

## Consequências

- O comentário de pesquisa ganha a seção **"Skills necessárias"** ([workflow/agents.md](../workflow/agents.md)).
- Se a orientação de uma skill conflitar com uma ADR ou com a documentação do projeto, **vale a ADR**, e o conflito é registrado na issue para o arquiteto avaliar.
- Quando uma skill nova relevante for instalada, o catálogo em `workflow/skills.md` é atualizado.
