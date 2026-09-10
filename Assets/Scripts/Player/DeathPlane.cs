using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        ScreenFade.Instance.FadeToBlack(2f, BattleManager.Instance.BlackoutComplete);
    }
}