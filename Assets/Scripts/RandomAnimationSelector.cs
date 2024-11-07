using UnityEngine;

public class RandomAnimationSelector : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Verifica si la animación que acaba de terminar es "Idle"
        if (stateInfo.IsName("Idle"))
        {
            // Genera un número aleatorio entre 0 y 1
            int randomAnim = Random.Range(0, 10);

            // Asigna el valor al parámetro del Animator
            animator.SetInteger("randomAnim", randomAnim);
        }
    }
}
