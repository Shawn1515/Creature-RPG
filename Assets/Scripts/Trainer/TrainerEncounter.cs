using System.Collections.Generic;
using UnityEngine;

public class TrainerEncounter : MonoBehaviour, IInteractable
{
    public TrainerData trainer;

    public Transform trainerTransform;

    private Vector3 originalPosition;

    private List<CreatureInstance> party = new List<CreatureInstance>();

    private bool defeated;

    void Start()
    {
        originalPosition = trainerTransform.position;
        for(int i = 0; i < trainer.creatures.Length; i++)
        {
            party.Add(new CreatureInstance(trainer.creatures[i], trainer.creatureLevels[i]));
        }
    }

    public void ResetTrainerPosition()
    {
        trainerTransform.position = originalPosition;
    }

    public void Interact()
    {
        FacePlayer();
        if(defeated)
        {
            DialogueUI.Instance.StartDialogue(
                trainer.defeatDialogue, trainer.trainerName
            );
            return;
        }

        if(trainer.trainerType == TrainerType.Champion)
        {
            if(!BadgeManager.Instance.HasAllBadges())
            {
                string[] message = {
                    "You aren't ready to challenge me yet.",
                    "Come back when you have all three badges."
                };
                DialogueUI.Instance.StartDialogue(message, "???");
                return;
            }
        }

        for(int i = 0; i < party.Count; i++)
        {
            party[i].currentHP = party[i].MaxHP;
        }

        DialogueUI.Instance.SetPendingTrainer(this);

        DialogueUI.Instance.StartDialogue(
            trainer.introDialogue, trainer.trainerName
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