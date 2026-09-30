# 0012 — Câmera única enquadrando os dois

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

Hoje a câmera é fixa, com paredes invisíveis nas bordas. Com dois jogadores, é preciso decidir se cada um tem a própria câmera ou se compartilham.

## Decisão

- **Uma câmera única que enquadra os dois jogadores**, igual nas duas máquinas.
- **Ninguém sai da tela:** as bordas funcionam como parede para quem ficou para trás, e a câmera só avança quando os dois avançam.
- A câmera é **calculada localmente em cada máquina** a partir das posições sincronizadas. Não é sincronizada pela rede.
- Implementação com o **Cinemachine**, que já está instalado.

## Alternativas consideradas

- **Uma câmera por jogador:** cada um poderia se afastar, quebrando o "lutar juntos" e a regra de renascer na borda esquerda.
- **Zoom out dinâmico:** mais complexo, e em 2.5D com sprites pixelados distorce a escala.

## Consequências

- A borda esquerda da câmera é o ponto de renascimento ([0010](0010-vidas-compartilhadas-e-renascimento.md)).
- Um jogador pode "prender" o outro. É aceito no gênero.
- As bordas atuais, filhas da câmera, continuam fazendo sentido e passam a acompanhar a câmera que se move.
