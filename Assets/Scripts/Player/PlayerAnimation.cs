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
    private bool runP;
    private bool jumpP;

    void Start() {
        animator = GetComponent<Animator>();
        idle = 0;
        run = 1;
        jump = 2;
        walk = 3;
    }

    void Update() {
        if(GameManager.Instance.CurrentState != GameState.Exploration)
        {
            animator.SetInteger("num", idle);
            return;
        }
        runP = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        jumpP = Input.GetKey(KeyCode.Space);
        if(jumpP) {
            animator.SetInteger("num", jump);
            return;
        }
        if(runP && Input.GetKey(KeyCode.LeftShift)) {
            animator.SetInteger("num", run);
            return;
        }
        if(runP) {
            animator.SetInteger("num", walk);
            return;
        }
        animator.SetInteger("num", idle);
    }
}
