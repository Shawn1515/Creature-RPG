using UnityEngine;
public class NPCAnimation : MonoBehaviour
{
    public Animator animator;
    private void Update()
    {
        if(GameManager.Instance.GetState() == GameState.Party)
        {
            animator.speed = 0f;
        }
        else
        {
            animator.speed = 1f;
        }
    }
}