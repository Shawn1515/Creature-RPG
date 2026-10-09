using UnityEngine;
using System.Collections;

public class Point : MonoBehaviour
{
    public Transform playerTransform;
    public Animator animator;

    public static Point Instance;

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator PointAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("Point");
    }
}