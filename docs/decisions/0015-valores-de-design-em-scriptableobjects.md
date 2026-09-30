# 0015 — Todo valor de design em ScriptableObjects

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

O time tem um **game designer** que ajusta valores no Editor e comita as próprias alterações no repositório. Hoje muitos valores estão espalhados em campos de componentes na cena (velocidade, vida, cooldowns) ou em constantes no código (duração da câmera lenta, espera do combo na morte). Editar a cena para ajustar um número gera conflitos de merge e esconde os valores.

## Decisão

- **Todo número ajustável de design vive num ScriptableObject de configuração**, separado por assunto. Exemplos: regras da partida (vidas, renascimento), dificuldade (multiplicadores), fichas de personagem (atributos), fichas de inimigo, ataques (`AttackDataSO`, que já existe).
- **Componentes e cenas referenciam esses assets**; não guardam valores de design próprios.
- **Nenhuma constante mágica de design em código de gameplay.** Constantes técnicas (tolerâncias, limites de segurança) podem ficar no código, com nome e comentário.
- Cada valor ajustável é listado em [design/tuning.md](../design/tuning.md), com local, unidade e efeito.
- O designer altera sobretudo assets, raramente cenas. O **Unity Smart Merge** fica configurado para os casos inevitáveis ([workflow/git.md](../workflow/git.md)).

## Consequências

- A migração de cada módulo inclui mover os valores de campos de componente para ScriptableObjects.
- Issues que mudam ou criam valores de design recebem a label `design-tuning`, para o designer validar.
- Valores que o host usa (vidas, dificuldade) são lidos só no host; valores do personagem são lidos pelo dono. Os assets são os mesmos nas duas máquinas, porque fazem parte da build.
