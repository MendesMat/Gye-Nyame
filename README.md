# Gye-Nyame
Gye-Nyame é um jogo de ação no estilo beat 'em up 2.5D, desenvolvido na Unity 6. Este documento tem como objetivo apresentar o projeto de forma amigável e acessível. A ideia é fornecer uma visão clara de como o código está organizado e o que cada parte do sistema faz, sem jargões excessivos, para que o trabalho desenvolvido possa ser facilmente compreendido por todos.

## Organização e Módulos
Para manter tudo organizado e fácil de trabalhar, o código do jogo foi separado em blocos independentes chamados de módulos. Cada módulo tem uma responsabilidade única e conversa com os demais de maneira controlada. 
Abaixo listamos esses módulos seguindo uma ordem lógica de construção, começando pela fundação do jogo até chegar na camada que interage diretamente com quem está jogando.

### Núcleo do Sistema (GyeNyame.Core)
Este é o alicerce de todo o projeto. Ele contém as regras fundamentais e ferramentas essenciais que serão usadas por todas as outras partes do jogo. Pense neste módulo como as engrenagens principais e a infraestrutura básica que mantêm o mundo funcionando. O projeto também possui um submódulo focado no editor da Unity 6, que existe unicamente para facilitar a criação de fases e o balanceamento do jogo pela equipe de desenvolvimento.

### Física (GyeNyame.Physics)
Aqui moram as leis da natureza do jogo. Este módulo controla a gravidade, detecção de colisões e os limites espaciais. Em um jogo 2.5D, ele é vital para garantir que os personagens consigam andar pelo chão corretamente e que não atravessem paredes ou caiam fora do cenário.

### Entidades (GyeNyame.Entities)
Nesta parte definimos o que significa existir no mundo do jogo. Qualquer coisa que tenha vida, receba dano ou possua um estado ativo é considerada uma entidade. Este módulo provê a base para criar o jogador, inimigos, aliados ou até objetos quebráveis no cenário.

### Combate (GyeNyame.Combat)
Como se espera de um jogo focado em lutas, a ação é primordial. Este módulo gerencia as regras universais de combate. Ele é responsável por calcular quem acertou quem, quanto de dano foi causado, reações a golpes recebidos e momentos de invulnerabilidade. Ele garante que as lutas sejam justas e responsivas para todos os envolvidos.

### Controles do Jogador (GyeNyame.Player.Input)
Este é o tradutor entre o mundo real e o jogo. O módulo fica escutando cada botão apertado no controle ou no teclado e converte essas ações em intenções claras, dizendo ao sistema do jogo que quem está com o controle quer pular ou que deseja atacar, etc.

### Movimentação do Jogador (GyeNyame.Player.Movement)
Pegando as intenções vindas dos controles e respeitando as leis do módulo de física, este sistema executa os deslocamentos do jogador. Ele calcula a velocidade da caminhada, o momento exato de um pulo e garante que movimentar o personagem pelo cenário seja fluido e ágil.

### Combate do Jogador (GyeNyame.Player.Combat)
Se aproveitando das regras universais de combate, este módulo traz as habilidades exclusivas do jogador. É aqui que definimos como funcionam os combos, ataques especiais e o ritmo dos golpes desferidos.

### Inteligência Artificial (GyeNyame.AI)
Este é o cérebro dos adversários. Ele avalia constantemente o campo de batalha, mede as distâncias, observa os ataques do jogador e toma decisões lógicas. É este módulo que decide quando um inimigo deve se aproximar e atacar ou quando ele deve recuar para tentar uma nova abordagem mais segura.

### Inimigos (GyeNyame.Enemy)
Enquanto a Inteligência Artificial toma as decisões, este módulo atua como o corpo e as habilidades específicas de cada oponente. Ele define quanta vida um inimigo tem, quão forte ele bate e que animações ele usa para agir, dando características próprias a cada inimigo encontrado.

### Interface (GyeNyame.UI)
Por fim, este sistema capta tudo o que está acontecendo por baixo dos panos e traduz em informações visuais na tela. As barras de vida, pontuações, alertas, menus e números de dano. É a linha de comunicação visual principal, responsável por informar e guiar o olhar de quem está jogando.
