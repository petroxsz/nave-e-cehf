using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GerarJogoCompleto
{
    private const string Base = "Assets/Recursos/";
    private const string Cena = "Assets/Cenas/Jogo.unity";
    private const string Controlador = "Assets/Recursos/Chefao.controller";

    [MenuItem("Atividade 5/Gerar jogo completo")]
    public static void Gerar()
    {
        Directory.CreateDirectory(Base);
        Directory.CreateDirectory("Assets/Cenas");
        Sprite nave = CriarSprite("Nave", 32, 32, 0);
        Sprite chefeSprite = CriarSprite("Chefao", 48, 48, 1);
        Sprite bala = CriarSprite("Bala", 16, 16, 2);
        AnimatorController controller = CriarAnimator();
        var cena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject cameraObj = new GameObject("Camera_Fixa");
        var camera = cameraObj.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5.4f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.025f, 0.028f, 0.088f);
        cameraObj.transform.position = new Vector3(0, 0, -10);
        cameraObj.tag = "MainCamera";
        cameraObj.AddComponent<AudioListener>();

        // Estrelas de fundo como sprites estáticos: camera permanece absolutamente fixa.
        var fundo = new GameObject("Fundo_Estrelado");
        Random.InitState(2210);
        for (int i = 0; i < 105; i++)
        {
            var estrela = new GameObject("Estrela_" + i.ToString("000"));
            estrela.transform.SetParent(fundo.transform);
            estrela.transform.position = new Vector3(Random.Range(-9.3f, 9.3f), Random.Range(-5.2f, 5.2f), 0);
            float escala = Random.Range(0.025f, 0.075f);
            estrela.transform.localScale = new Vector3(escala, escala, 1);
            var sr = estrela.AddComponent<SpriteRenderer>();
            sr.sprite = bala;
            sr.color = new Color(0.35f, 0.65f, 1f, Random.Range(0.28f, 0.75f));
            sr.sortingOrder = -10;
        }

        var naveObj = new GameObject("Nave_Jogador");
        naveObj.transform.position = new Vector3(0, -3.75f, 0);
        var naveSr = naveObj.AddComponent<SpriteRenderer>();
        naveSr.sprite = nave;
        naveSr.sortingOrder = 3;
        naveObj.transform.localScale = Vector3.one * 1.55f;
        Jogador jogador = naveObj.AddComponent<Jogador>();

        var chefeObj = new GameObject("Chefao_Unico");
        chefeObj.transform.position = new Vector3(0, 3.35f, 0);
        var chefeSr = chefeObj.AddComponent<SpriteRenderer>();
        chefeSr.sprite = chefeSprite;
        chefeSr.sortingOrder = 4;
        chefeObj.transform.localScale = Vector3.one * 2.45f;
        Animator anim = chefeObj.AddComponent<Animator>();
        anim.runtimeAnimatorController = controller;
        Chefe chefe = chefeObj.AddComponent<Chefe>();

        var piscinaObj = new GameObject("Pools_Unity_ObjectPool");
        PiscinaProjeteis piscina = piscinaObj.AddComponent<PiscinaProjeteis>();
        piscina.spriteProjetil = bala;

        var controleObj = new GameObject("Controle_Jogo_e_HUD");
        Jogo jogo = controleObj.AddComponent<Jogo>();
        jogo.jogador = jogador;
        jogo.chefe = chefe;
        jogo.piscinas = piscina;

        EditorSceneManager.SaveScene(cena, Cena);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(Cena, true) };
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeGameObject = controleObj;
        EditorUtility.DisplayDialog("Atividade 5", "Jogo criado! Abra a aba Game e pressione Play.\n\nMovimento: WASD / setas\nTiro: Espaco", "OK");
    }

    private static AnimatorController CriarAnimator()
    {
        if (AssetDatabase.LoadAssetAtPath<AnimatorController>(Controlador) != null)
            AssetDatabase.DeleteAsset(Controlador);
        var controller = AnimatorController.CreateAnimatorControllerAtPath(Controlador);
        controller.AddParameter("Estado", AnimatorControllerParameterType.Int);
        var raiz = controller.layers[0].stateMachine;
        AnimationClip clip = new AnimationClip { name = "Pose_Estatica" };
        AssetDatabase.CreateAsset(clip, Base + "Pose_Estatica.anim");

        string[] fases = { "ALTA", "MEDIA", "BAIXA" };
        string[] padroes = { "Perseguicao", "Leque", "Parede" };
        for (int i = 0; i < 9; i++)
        {
            // NOVE estados de comportamento reais no Animator, com Behaviour individual.
            var estado = raiz.AddState(fases[i / 3] + "_" + padroes[i % 3], new Vector3(300 + (i % 3) * 250, 30 + (i / 3) * 130, 0));
            estado.motion = clip;
            estado.writeDefaultValues = false;
            var comportamento = estado.AddStateMachineBehaviour<ComportamentoChefe>();
            comportamento.numero = i;
            comportamento.intervaloTiro = Mathf.Max(0.30f, 1.22f - i / 3 * 0.28f - (i % 3) * 0.08f);
            if (i == 0) raiz.defaultState = estado;
            var transicao = raiz.AddAnyStateTransition(estado);
            transicao.hasExitTime = false;
            transicao.duration = 0f;
            transicao.canTransitionToSelf = false;
            transicao.AddCondition(AnimatorConditionMode.Equals, i, "Estado");
        }
        // Décimo estado técnico: apenas escudo/transição de fase, sem ataque.
        var invul = raiz.AddState("TRANSICAO_ESCUDO_2_3s", new Vector3(1100, 200, 0));
        invul.motion = clip;
        invul.writeDefaultValues = false;
        var entradaEscudo = raiz.AddAnyStateTransition(invul);
        entradaEscudo.hasExitTime = false;
        entradaEscudo.duration = 0;
        entradaEscudo.canTransitionToSelf = false;
        entradaEscudo.AddCondition(AnimatorConditionMode.Equals, 9, "Estado");
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        return controller;
    }

    private static Sprite CriarSprite(string nome, int w, int h, int tipo)
    {
        string caminho = Base + nome + ".png";
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        Color vazio = new Color(0, 0, 0, 0);
        Color[] cores = new Color[w * h];
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            float cx = (x - (w - 1) / 2f) / (w / 2f);
            float cy = (y - (h - 1) / 2f) / (h / 2f);
            Color cor = vazio;
            if (tipo == 0)
            {
                bool fuselagem = Mathf.Abs(cx) < (0.14f + Mathf.Max(0, -cy) * 0.28f) && cy > -0.9f && cy < 0.90f - Mathf.Abs(cx);
                bool asa = cy < -0.05f && cy > -0.70f && Mathf.Abs(cx) < (0.95f - Mathf.Abs(cy + 0.2f) * 0.65f);
                if (asa) cor = new Color(0.08f, 0.66f, 0.85f);
                if (fuselagem) cor = new Color(0.65f, 0.96f, 1f);
                if (Mathf.Abs(cx) < 0.11f && cy > -0.1f && cy < 0.45f) cor = new Color(0.08f, 0.22f, 0.55f);
                if (cy < -0.68f && Mathf.Abs(cx) < 0.19f) cor = new Color(1f, 0.66f, 0.2f);
            }
            else if (tipo == 1)
            {
                float d = Mathf.Abs(cx) + Mathf.Abs(cy) * 0.76f;
                if (d < 0.83f) cor = d > 0.66f ? new Color(0.95f, 0.34f, 0.84f) : new Color(0.28f, 0.13f, 0.57f);
                if (Mathf.Abs(cx) < 0.54f && Mathf.Abs(cy) < 0.18f) cor = new Color(1f, 0.34f, 0.5f);
                if (Mathf.Abs(cx) < 0.18f && Mathf.Abs(cy) < 0.18f) cor = Color.white;
                if (Mathf.Abs(cx) > 0.7f && Mathf.Abs(cy) < 0.32f) cor = new Color(0.7f, 0.29f, 0.96f);
            }
            else { if (cx * cx + cy * cy < 0.63f) cor = Color.white; }
            cores[y * w + x] = cor;
        }
        tex.SetPixels(cores);
        tex.Apply();
        File.WriteAllBytes(caminho, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(caminho);
        var importer = (TextureImporter)AssetImporter.GetAtPath(caminho);
        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = w;
        importer.filterMode = FilterMode.Point;
        importer.mipmapEnabled = false;
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(caminho);
    }
}
