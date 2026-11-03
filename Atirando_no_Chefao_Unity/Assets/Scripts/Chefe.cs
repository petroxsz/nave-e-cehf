using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Chefe : MonoBehaviour
{
    public int vidaMaxima = 180;
    public int vidaAtual { get; private set; }
    public bool invulneravel { get; private set; }
    public int faseAtual { get; private set; } = 0;
    private Animator animator;
    private SpriteRenderer desenho;
    private float fimTransicao;
    private float proximaTroca;
    private int comportamentoLocal;
    private readonly Vector3 posicaoCentral = new Vector3(0, 3.35f, 0);

    private void Awake()
    {
        animator = GetComponent<Animator>();
        desenho = GetComponent<SpriteRenderer>();
        vidaAtual = vidaMaxima;
        transform.position = posicaoCentral;
        proximaTroca = Time.time + 7f;
    }

    private void Start() { animator.SetInteger("Estado", 0); }

    private void Update()
    {
        if (Jogo.instancia == null || Jogo.instancia.encerrado) return;
        if (invulneravel)
        {
            transform.position = posicaoCentral;
            desenho.color = Mathf.FloorToInt(Time.time * 9) % 2 == 0 ? Color.white : new Color(0.6f, 0.8f, 1f, 0.3f);
            if (Time.time >= fimTransicao)
            {
                invulneravel = false;
                desenho.color = CorFase();
                comportamentoLocal = 0;
                proximaTroca = Time.time + 6f;
                animator.SetInteger("Estado", faseAtual * 3);
            }
            return;
        }
        desenho.color = CorFase();
        if (Time.time >= proximaTroca)
        {
            comportamentoLocal = (comportamentoLocal + 1) % 3;
            animator.SetInteger("Estado", faseAtual * 3 + comportamentoLocal);
            proximaTroca = Time.time + (faseAtual == 0 ? 7f : faseAtual == 1 ? 6f : 5f);
        }
    }

    private Color CorFase() => faseAtual == 0 ? new Color(0.7f, 0.5f, 1f) : faseAtual == 1 ? new Color(1f, 0.68f, 0.3f) : new Color(1f, 0.3f, 0.43f);

    public void ReceberDano(int quantidade)
    {
        if (Jogo.instancia.encerrado || invulneravel) return;
        vidaAtual = Mathf.Max(0, vidaAtual - quantidade);
        if (vidaAtual == 0) { Jogo.instancia.Encerrar(true); return; }
        float percentual = (float)vidaAtual / vidaMaxima;
        int novaFase = percentual > 0.5f ? 0 : percentual >= 0.2f ? 1 : 2;
        if (novaFase != faseAtual)
        {
            faseAtual = novaFase;
            invulneravel = true;
            fimTransicao = Time.time + 2.3f;
            animator.SetInteger("Estado", 9); // Estado especial: transição invulnerável.
        }
    }

    // Executado EXCLUSIVAMENTE pelos StateMachineBehaviours anexados aos 9 estados do Animator.
    public void ExecutarComportamento(int numero, float tempoNoEstado, float delta)
    {
        if (invulneravel || Jogo.instancia.encerrado) return;
        float velocidadeLateral = numero < 3 ? 1.2f : numero < 6 ? 2.1f : 3.1f;
        float amplitude = numero % 3 == 0 ? 1.5f : numero % 3 == 1 ? 2.9f : 3.9f;
        float x = Mathf.Sin(Time.time * velocidadeLateral) * amplitude;
        transform.position = Vector3.Lerp(transform.position, posicaoCentral + new Vector3(x, 0, 0), delta * 3f);
    }

    public void DispararPadrao(int numero)
    {
        if (invulneravel || Jogo.instancia.encerrado) return;
        Vector2 origem = (Vector2)transform.position + Vector2.down * 0.72f;
        Vector2 alvo = ((Vector2)Jogo.instancia.jogador.transform.position - origem).normalized;
        int fase = numero / 3;
        int padrao = numero % 3;
        float vel = 3.8f + fase * 0.9f;
        if (padrao == 0)
        {
            // Rajada de tiros direcionados ao jogador.
            Jogo.instancia.piscinas.AtirarChefe(origem, alvo, vel);
            if (fase >= 2) Jogo.instancia.piscinas.AtirarChefe(origem, Rotacionar(alvo, 12), vel);
        }
        else if (padrao == 1)
        {
            // Leque: 3, 5 ou 7 disparos.
            int quantidade = 3 + fase * 2;
            for (int i = 0; i < quantidade; i++)
                Jogo.instancia.piscinas.AtirarChefe(origem, Rotacionar(Vector2.down, (i - (quantidade - 1) / 2f) * 15f), vel);
        }
        else
        {
            // Parede oblíqua com abertura central (variável por fase).
            int quantidade = 4 + fase * 2;
            for (int i = 0; i < quantidade; i++)
            {
                float angulo = -65f + 130f * i / (quantidade - 1);
                Jogo.instancia.piscinas.AtirarChefe(origem, Rotacionar(Vector2.down, angulo), vel + 0.4f);
            }
        }
    }

    private static Vector2 Rotacionar(Vector2 v, float graus)
    {
        float r = graus * Mathf.Deg2Rad;
        return new Vector2(v.x * Mathf.Cos(r) - v.y * Mathf.Sin(r), v.x * Mathf.Sin(r) + v.y * Mathf.Cos(r));
    }
}
