using UnityEngine;
using System.Collections;

public class Celebrate : MonoBehaviour
{
    public Animator animator;

    public static Celebrate Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void CelebrateAnimation()
    {
        animator.SetTrigger("Celebrate");
    }

    public void EndBattle()
    {
        BattleManager.Instance.EndBattle();
    }
}