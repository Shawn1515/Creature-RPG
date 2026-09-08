using System.Collections.Generic;
using UnityEngine;

public class CameraOcclusion : MonoBehaviour
{
    public Transform player;
    public Camera mainCamera;

    public LayerMask occlusionLayers;

    public Material occlusionMaterial;

    private Dictionary<Renderer, Material[]> originalMaterials =
        new Dictionary<Renderer, Material[]>();

    private List<Renderer> currentlyOccluded =
        new List<Renderer>();

    void Update()
    {
        FindOccludedObjects();
        RestoreObjects();
    }

    void FindOccludedObjects()
    {
        currentlyOccluded.Clear();

        Vector3 direction =
            player.position - mainCamera.transform.position;

        float distance = direction.magnitude;

        Ray ray = new Ray(
            mainCamera.transform.position,
            direction.normalized
        );

        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            distance,
            occlusionLayers
        );

        foreach (RaycastHit hit in hits)
        {
            Renderer renderer =
                hit.collider.GetComponent<Renderer>();

            if (renderer == null)
            {
                renderer =
                    hit.collider.GetComponentInParent<Renderer>();
            }

            if (renderer == null)
            {
                continue;
            }

            if (!currentlyOccluded.Contains(renderer))
            {
                currentlyOccluded.Add(renderer);
            }

            if (!originalMaterials.ContainsKey(renderer))
            {
                originalMaterials.Add(
                    renderer,
                    renderer.materials
                );

                Material[] newMaterials =
                    new Material[renderer.materials.Length];

                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = occlusionMaterial;
                }

                renderer.materials = newMaterials;
            }
        }
    }

    void RestoreObjects()
    {
        List<Renderer> objectsToRestore =
            new List<Renderer>(originalMaterials.Keys);

        foreach (Renderer renderer in objectsToRestore)
        {
            if (renderer == null)
            {
                originalMaterials.Remove(renderer);
                continue;
            }

            if (!currentlyOccluded.Contains(renderer))
            {
                renderer.materials =
                    originalMaterials[renderer];

                originalMaterials.Remove(renderer);
            }
        }
    }
}