using UnityEngine;
using UnityEngine.Pool;

public class PiscinaProjeteis : MonoBehaviour
{
    public Sprite spriteProjetil;
    private ObjectPool<Projetil> piscinaJogador;
    private ObjectPool<Projetil> piscinaChefe;

    private void Awake()
    {
        piscinaJogador = CriarPiscina("Tiro_Jogador");
        piscinaChefe = CriarPiscina("Tiro_Chefe");
    }

    private ObjectPool<Projetil> CriarPiscina(string nome)
    {
        ObjectPool<Projetil> nova = null;
        nova = new ObjectPool<Projetil>(
            () =>
            {
                var obj = new GameObject(nome);
                obj.transform.SetParent(transform);
                var sr = obj.AddComponent<SpriteRenderer>();
                sr.sprite = spriteProjetil;
                sr.sortingOrder = 10;
                var p = obj.AddComponent<Projetil>();
                obj.SetActive(false);
                return p;
            },
            p => p.gameObject.SetActive(true),
            p => p.gameObject.SetActive(false),
            p => { if (p) Destroy(p.gameObject); },
            true, 40, 250);
        return nova;
    }

    public void AtirarJogador(Vector2 origem)
    {
        Projetil p = piscinaJogador.Get();
        p.Preparar(piscinaJogador, origem, Vector2.up, 11f, true);
    }

    public void AtirarChefe(Vector2 origem, Vector2 direcao, float velocidade)
    {
        Projetil p = piscinaChefe.Get();
        p.Preparar(piscinaChefe, origem, direcao, velocidade, false);
    }
}
