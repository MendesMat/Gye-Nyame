# 0008 — Hitstop local por entidade; fim do `timeScale` global

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

Hoje o `TimeManager` zera o `Time.timeScale` durante o hitstop e reduz para 0,2 na câmera lenta da morte. Em rede, isso congelaria a simulação de uma máquina enquanto a outra segue, dessincronizando tudo e atrapalhando o tick do netcode.

## Decisão

- **Hitstop local por entidade:** só o atacante e o alvo congelam (animação e movimento), pelo `HitStopTime` do golpe, em cada máquina que os exibe. O resto do mundo continua.
- **`Time.timeScale` deixa de ser usado** para hitstop e para câmera lenta durante a partida.
- **Câmera lenta** vira **efeito visual local**, e só no **game over** (quando a última vida acaba). Não altera a simulação.

## Alternativas consideradas

- **Manter o `timeScale` global:** dessincroniza as duas máquinas.
- **Câmera lenta a cada morte:** interromperia o parceiro que ainda está jogando.

## Consequências

- Os componentes que hoje dependem do hitstop global (estados, `InputBuffer`, `DamageFlashFeedback`) passam a consultar o "relógio" da própria entidade.
- A duração do hitstop continua vindo de `AttackDataSO.HitStopTime`.
- A pausa do modo solo ([0006](0006-modo-solo-como-host-local.md)) é um mecanismo separado.
