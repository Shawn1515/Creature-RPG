using UnityEngine;
public class NPCIndicator : MonoBehaviour
{
    public GameObject interactionCanvas;
    private void Start() {
        interactionCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            interactionCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            interactionCanvas.SetActive(false);
        }
    }
}