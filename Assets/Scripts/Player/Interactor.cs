using UnityEngine;

public class Interactor : MonoBehaviour
{
    public float interactRange = 2f;
    private bool interactionLocked;
    void Update()
    {
        if (DialogueUI.Instance.IsOpen) {
            if (Input.GetKeyDown(KeyCode.E)) {
                DialogueUI.Instance.NextLine();
            }
            return;
        }
        if (interactionLocked) {
            return;
        }
        if(Input.GetKeyDown(KeyCode.E)) {
            TryInteract();
        }
    }

    void TryInteract() {
        Ray ray = new Ray(transform.position + Vector3.down * 1.85f, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            MonoBehaviour[] behaviours = hit.collider.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IInteractable interactable)
                {
                    interactable.Interact();
                    return;
                }
            }
        }
    }

    public void LockInteraction()
    {
        interactionLocked = true;
    }

    public void UnlockInteraction()
    {
        interactionLocked = false;
    }
}