using UnityEngine;
using UnityEngine.SceneManagement;

public class Jogo : MonoBehaviour
{
    public static Jogo instancia { get; private set; }
    public Jogador jogador;
    public Chefe chefe;
    public PiscinaProjeteis piscinas;
    public bool encerrado { get; private set; }
    public bool vitoria { get; private set; }
    private GUIStyle titulo, texto, botao, barra;
    private Texture2D pixel;

    private void Awake() { instancia = this; }
    public void Encerrar(bool ganhou) { encerrado = true; vitoria = ganhou; }

    private void CriarEstilos()
    {
        pixel = Texture2D.whiteTexture;
        titulo = new GUIStyle(GUI.skin.label) { fontSize = 34, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        texto = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold };
        barra = new GUIStyle(GUI.skin.label) { fontSize = 16, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        botao = new GUIStyle(GUI.skin.button) { fontSize = 22, fontStyle = FontStyle.Bold };
    }

    private void OnGUI()
    {
        if (titulo == null) CriarEstilos();
        float escala = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
        Matrix4x4 original = GUI.matrix;
        GUI.matrix = Matrix4x4.TRS(new Vector3((Screen.width - 1280 * escala) / 2, (Screen.height - 720 * escala) / 2, 0), Quaternion.identity, new Vector3(escala, escala, 1));
        GUI.color = Color.white;
        GUI.Label(new Rect(24, 16, 320, 32), "NAVE  |  DANOS: " + jogador.danosRecebidos + "/4", texto);
        for (int i = 0; i < 3; i++)
        {
            GUI.color = i < Mathf.Max(0, 3 - jogador.danosRecebidos) ? new Color(0.25f, 1f, 0.82f) : new Color(0.22f, 0.23f, 0.3f);
            GUI.DrawTexture(new Rect(28 + i * 39, 51, 28, 20), pixel);
        }
        GUI.color = Color.white;
        GUI.Label(new Rect(400, 15, 480, 26), "CHEFAO  -  " + (chefe.faseAtual == 0 ? "VIDA ALTA" : chefe.faseAtual == 1 ? "VIDA MEDIA" : "VIDA BAIXA"), barra);
        GUI.color = new Color(0.1f, 0.11f, 0.2f);
        GUI.DrawTexture(new Rect(410, 48, 460, 24), pixel);
        GUI.color = chefe.faseAtual == 0 ? new Color(0.65f, 0.4f, 1f) : chefe.faseAtual == 1 ? new Color(1f, 0.65f, 0.22f) : new Color(1f, 0.2f, 0.3f);
        GUI.DrawTexture(new Rect(414, 52, 452f * chefe.vidaAtual / chefe.vidaMaxima, 16), pixel);
        GUI.color = Color.white;
        if (chefe.invulneravel) GUI.Label(new Rect(440, 82, 400, 35), "ESCUDO ATIVO - INVULNERAVEL!", barra);
        GUI.Label(new Rect(20, 677, 850, 30), "WASD / SETAS: MOVER     ESPACO: ATIRAR", texto);
        if (encerrado)
        {
            GUI.color = new Color(0.03f, 0.04f, 0.11f, 0.92f);
            GUI.DrawTexture(new Rect(310, 190, 660, 310), pixel);
            GUI.color = vitoria ? new Color(0.32f, 1f, 0.76f) : new Color(1f, 0.4f, 0.45f);
            GUI.Label(new Rect(330, 227, 620, 85), vitoria ? "VITORIA!" : "DERROTA!", titulo);
            GUI.color = Color.white;
            if (GUI.Button(new Rect(465, 354, 350, 68), "TENTAR NOVAMENTE", botao)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        GUI.matrix = original;
        GUI.color = Color.white;
    }
}
