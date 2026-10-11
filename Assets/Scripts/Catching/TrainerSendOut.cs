using UnityEngine;
using System.Collections;

public class TrainerSendOut : MonoBehaviour
{
    public Animator animator;
    public Transform hatSpawnPoint;
    public GameObject catchHatPrefab;

    private GameObject currentHat;
    private Transform target;
    private System.Action onCreatureRevealed;

    public IEnumerator StartSendOut(Transform creatureTarget, System.Action onReveal)
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(RotateToTarget(Quaternion.Euler(0f, -90f, 0f) * gameObject.transform.rotation));
        if (currentHat != null)
            yield break;

        target = creatureTarget;
        onCreatureRevealed = onReveal;

        currentHat = Instantiate(
            catchHatPrefab,
            hatSpawnPoint.position,
            hatSpawnPoint.rotation,
            hatSpawnPoint
        );

        animator.SetTrigger("Throw");
    }
    public void ThrowHat()
    {
        if (currentHat == null || target == null)
            return;

        currentHat.transform.SetParent(null);
        currentHat.transform.rotation = Quaternion.identity;

        CatchHat hat = currentHat.GetComponent<CatchHat>();

        if (hat != null)
        {
            hat.StartSendOut(target, RevealCreature);
        }
    }

    private void RevealCreature()
    {
        onCreatureRevealed?.Invoke();

        if (currentHat != null)
            Destroy(currentHat);

        currentHat = null;
        target = null;
        onCreatureRevealed = null;
    }

    public void DoneThrowing()
    {
        StartCoroutine(RotateToTarget(Quaternion.Euler(0f, 90f, 0f) * gameObject.transform.rotation));
    }

    IEnumerator RotateToTarget(Quaternion targetRotation)
    {
        while (Quaternion.Angle(gameObject.transform.rotation, targetRotation) > 1f)
        {
            gameObject.transform.rotation = Quaternion.Slerp(
                gameObject.transform.rotation,
                targetRotation,
                6f * Time.deltaTime
            );
        
            yield return null;
        }
        gameObject.transform.rotation = targetRotation;
    }
}
