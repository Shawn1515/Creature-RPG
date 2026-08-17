using System.Collections.Generic;
using UnityEngine;

public class TrainerEncounter : MonoBehaviour, IInteractable
{
    public TrainerData trainer;

    public Transform trainerTransform;

    private List<CreatureInstance> party = new List<CreatureInstance>();

    private bool defeated;

    void Start()
    {
        for(int i = 0; i < trainer.creatures.Length; i++)
        {
            party.Add(new CreatureInstance(trainer.creatures[i], trainer.creatureLevels[i]));
        }
    }

    public void Interact()
    {
        FacePlayer();
        if(defeated)
        {
            DialogueUI.Instance.StartDialogue(
                trainer.defeatDialogue
            );
            return;
        }

        DialogueUI.Instance.SetPendingTrainer(this);

        DialogueUI.Instance.StartDialogue(
            trainer.introDialogue
        );
    }

    void FacePlayer() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            return;
        }

        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void MarkDefeated()
    {
        defeated = true;
    }

    public List<CreatureInstance> GetParty()
    {
        return party;
    }
}