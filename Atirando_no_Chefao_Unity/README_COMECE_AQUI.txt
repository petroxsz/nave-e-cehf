ATIVIDADE 5 - ATIRANDO NO CHEFAO
Unity 6.5 / projeto 2D (scripts e gerador de fase)

COMO ABRIR
1. Extraia a pasta Atirando_no_Chefao_Unity.
2. No Unity Hub > Add > Add project from disk, selecione a pasta extraida.
3. Abra com Unity 6.5 (ou versao compativel com Unity 6).
4. Aguarde a importacao e compilacao dos scripts.
5. No MENU SUPERIOR, clique: Atividade 5 > Gerar jogo completo.
6. Abra a aba GAME e aperte PLAY.

CONTROLES
- WASD ou setas: mover livremente
- Espaco: tiro imediato e tiro automatico a cada 0,35 segundo enquanto pressionado
- Botao Tentar Novamente no fim da partida.

CONFORMIDADE COM O ENUNCIADO
- Um unico jogador e um unico chefe, campo vertical, camera fixa.
- ObjectPool<Projetil> real da Unity para tiros de ambos.
- Nave suporta 3 danos e morre no 4o (conforme resumo final do professor).
- Chefe tem 180 pontos de vida. Frequencia maxima de tiros da nave: 0,35 s.
  Em acerto continuo isso representa pelo menos 63 segundos, sem contar esquivas e escudos.
- Vida alta > 50%, media de 20% ate 50%, baixa < 20%.
- 9 estados distintos no Animator, cada um com ComportamentoChefe : StateMachineBehaviour.
- Estado tecnico extra de transicao de 2,3 segundos, imovel, sem ataques e invulneravel.
- Barra HUD de chefe muda de cor, vidas do jogador, telas de vitoria e derrota.

IMPORTANTE
O projeto inclui um gerador via Editor porque o Unity precisa criar e serializar
internamente o Animator Controller e a cena com seu proprio editor.
A geracao e feita inteiramente na Unity por um unico clique no menu.
Nao foi possivel executar/compilar o Unity neste ambiente. Se um erro aparecer
na sua versao do Unity, envie uma captura do Console para correcoes pontuais.

NOTA
O disparo da nave aponta somente para cima. O chefe possui padroes de rajada
mirada, leque e parede em tres faixas de vida com velocidades e frequencias crescentes.
