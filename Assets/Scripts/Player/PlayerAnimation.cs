using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    
    Animator animator;
    private int idle;
    private int run;
    private int walk;
    private int jump;
    private PlayerMovement movement;

    void Start() {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        idle = 0;
        run = 1;
        jump = 2;
        walk = 3;
    }

    void Update() {
        if(GameManager.Instance.CurrentState != GameState.Exploration)
        {
            animator.SetInteger("num", idle);
        }
        else if(!movement.IsGrounded && movement.VerticalVelocity > 0.1f) {
            animator.SetInteger("num", jump);
        }
        else if(movement.IsRunning) {
            animator.SetInteger("num", run);
        }
        else if(movement.IsMoving) {
            animator.SetInteger("num", walk);
        }
        else
        {
            animator.SetInteger("num", idle);
        }
    }
}
