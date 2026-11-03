using UnityEngine;

public class Jogador : MonoBehaviour
{
    public float velocidade = 7f;
    public int danosRecebidos { get; private set; }
    private float proximoTiro;
    private float invulneravelAte;
    private SpriteRenderer desenho;

    private void Awake() { desenho = GetComponent<SpriteRenderer>(); }
    private void Update()
    {
        if (Jogo.instancia == null || Jogo.instancia.encerrado) return;
        Vector2 entrada = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        Vector3 p = transform.position + (Vector3)(entrada * velocidade * Time.deltaTime);
        transform.position = new Vector3(Mathf.Clamp(p.x, -7.9f, 7.9f), Mathf.Clamp(p.y, -4.5f, 1.55f), 0);

        // GetKeyDown garante o primeiro tiro imediato. GetKey mantém o disparo a cada 0,35 s.
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Space))
        {
            if (Time.time >= proximoTiro)
            {
                proximoTiro = Time.time + 0.35f;
                Jogo.instancia.piscinas.AtirarJogador((Vector2)transform.position + Vector2.up * 0.5f);
            }
        }
        desenho.color = Time.time < invulneravelAte && Mathf.FloorToInt(Time.time * 12) % 2 == 0
            ? new Color(1, 1, 1, 0.3f) : Color.white;
    }

    public void ReceberDano()
    {
        if (Jogo.instancia.encerrado || Time.time < invulneravelAte) return;
        danosRecebidos++;
        invulneravelAte = Time.time + 1.2f;
        // Três danos suportados; derrota no quarto.
        if (danosRecebidos >= 4) Jogo.instancia.Encerrar(false);
    }
}
