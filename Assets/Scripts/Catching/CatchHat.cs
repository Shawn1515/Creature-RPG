using UnityEngine;

public class CatchHat : MonoBehaviour
{
    public float flightDuration = 0.75f;
    public float rotationSpeed = 720f;
    public float maxScaleMultiplier = 4f;

    private Transform target;
    private Vector3 startPosition;
    private Vector3 originalScale;
    private float timer;

    private bool caught;

    public void StartThrow(Transform newTarget, bool willCatch)
    {
        target = newTarget;

        caught = willCatch;

        startPosition = transform.position;

        timer = 0f;
    }

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (target == null)
        {
            return;
        }

        timer += Time.deltaTime;

        float progress = timer / flightDuration;
        if(progress > flightDuration - 1)
        {
            target.gameObject.SetActive(false);
        }
        float expansionProgress = Mathf.InverseLerp(
            0.4f,
            1f,
            progress
        );

        transform.localScale = Vector3.Lerp(
            originalScale,
            originalScale * maxScaleMultiplier,
            expansionProgress
        );
        Vector3 position = Vector3.Lerp(
            startPosition,
            target.position,
            progress
        );
        transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime
        );

        float arcHeight = 2f;
        position.y += Mathf.Sin(progress * Mathf.PI) * arcHeight;
        transform.position = position;

        if (progress >= 1f)
        {
            ReachCreature();
        }
    }

    void ReachCreature()
    {
        target = null;
        StartCoroutine(BattleManager.Instance.CatchAfterAnimation(caught));
    }
}