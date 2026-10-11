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

    public IEnumerator PointAnimation(bool trainer)
    {
        if(trainer)
        {
            yield return new WaitForSeconds(3f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
        animator.SetTrigger("Point");
    }

    public void StartPointing()
    {
        StartCoroutine(RotateToTarget(Quaternion.Euler(0f, -30f, 0f) * playerTransform.rotation));
    }

    public void DonePointing()
    {
        StartCoroutine(RotateToTarget(Quaternion.Euler(0f, 30f, 0f) * playerTransform.rotation));
    }

    IEnumerator RotateToTarget(Quaternion targetRotation)
    {
        while (Quaternion.Angle(playerTransform.rotation, targetRotation) > 1f)
        {
            playerTransform.rotation = Quaternion.Slerp(
                playerTransform.rotation,
                targetRotation,
                6f * Time.deltaTime
            );
        
            yield return null;
        }
        playerTransform.rotation = targetRotation;
    }
}