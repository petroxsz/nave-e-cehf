using UnityEngine;

// Este código é anexado DIRETAMENTE a cada um dos NOVE estados do Animator.
// A transição entre estados é feita pelo Animator Controller (parâmetro Estado),
// e NÃO por uma máquina de estados implementada em C#.
public class ComportamentoChefe : StateMachineBehaviour
{
    public int numero;
    public float intervaloTiro = 0.9f;
    private float proximoDisparo;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        proximoDisparo = Time.time + 0.45f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Chefe chefe = animator.GetComponent<Chefe>();
        if (chefe == null) return;
        chefe.ExecutarComportamento(numero, stateInfo.normalizedTime, Time.deltaTime);
        if (Time.time >= proximoDisparo)
        {
            chefe.DispararPadrao(numero);
            proximoDisparo = Time.time + intervaloTiro;
        }
    }
}
