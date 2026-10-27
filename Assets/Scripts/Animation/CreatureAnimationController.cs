
using UnityEngine;

public class CreatureAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void SetMoving(bool isMoving)
    {
        animator.SetBool("IsMoving", isMoving);
    }

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }
}
