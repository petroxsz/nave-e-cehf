using UnityEngine;
using UnityEngine.Pool;

public class Projetil : MonoBehaviour
{
    private IObjectPool<Projetil> pool;
    private bool doJogador;
    private Vector2 direcao;
    private float velocidade;
    private float tempoRestante;
    private bool devolvido;

    public void Preparar(IObjectPool<Projetil> poolOrigem, Vector2 posicao, Vector2 direcaoTiro, float velocidadeTiro, bool tiroJogador)
    {
        pool = poolOrigem;
        transform.position = posicao;
        direcao = direcaoTiro.normalized;
        velocidade = velocidadeTiro;
        doJogador = tiroJogador;
        tempoRestante = 6f;
        devolvido = false;
        transform.localScale = tiroJogador ? new Vector3(0.21f, 0.43f, 1) : new Vector3(0.31f, 0.31f, 1);
        var sr = GetComponent<SpriteRenderer>();
        sr.color = tiroJogador ? new Color(0.25f, 1f, 0.9f) : new Color(1f, 0.36f, 0.35f);
    }

    private void Update()
    {
        if (Jogo.instancia == null || Jogo.instancia.encerrado) { Devolver(); return; }
        transform.position += (Vector3)(direcao * velocidade * Time.deltaTime);
        tempoRestante -= Time.deltaTime;
        if (tempoRestante <= 0 || Mathf.Abs(transform.position.x) > 9.8f || Mathf.Abs(transform.position.y) > 6.3f)
        {
            Devolver(); return;
        }
        if (doJogador)
        {
            if (Jogo.instancia.chefe != null && Vector2.Distance(transform.position, Jogo.instancia.chefe.transform.position) < 0.95f)
            {
                Jogo.instancia.chefe.ReceberDano(1);
                Devolver();
            }
        }
        else if (Jogo.instancia.jogador != null && Vector2.Distance(transform.position, Jogo.instancia.jogador.transform.position) < 0.43f)
        {
            Jogo.instancia.jogador.ReceberDano();
            Devolver();
        }
    }

    public void Devolver()
    {
        if (devolvido) return;
        devolvido = true;
        pool?.Release(this);
    }
}
