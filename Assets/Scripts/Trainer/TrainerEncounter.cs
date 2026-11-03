
using System.Collections.Generic;
using UnityEngine;

public class TrainerEncounter : MonoBehaviour, IInteractable
{
    [Header("Trainer Data")]
    public TrainerData trainer;
    public Transform trainerTransform;

    [Header("Trainer Reset")]
    public float resetDistance = 100f;

    private Vector3 originalPosition;
    private Vector3 lastPlayerPosition;

    private List<CreatureInstance> party = new List<CreatureInstance>();

    private bool defeated;
    private float distanceTraveled;

    private Transform player;
    private Interactor interactor;
    private TrainerWander trainerWander;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError("TrainerEncounter: Player not found.");
            return;
        }

        player = playerObject.transform;
        interactor = player.Find("player").GetComponent<Interactor>();

        originalPosition = trainerTransform.position;
        lastPlayerPosition = player.position;

        trainerWander = GetComponent<TrainerWander>();

        GenerateRandomParty();
    }

    private void Update()
    {
        if (!defeated || player == null)
            return;

        if (GameManager.Instance.CurrentState != GameState.Exploration)
            return;

        Vector3 currentPosition = player.position;
        Vector3 previousPosition = lastPlayerPosition;

        // Ignore vertical movement.
        currentPosition.y = 0f;
        previousPosition.y = 0f;

        distanceTraveled += Vector3.Distance(
            previousPosition,
            currentPosition
        );

        lastPlayerPosition = player.position;

        if (distanceTraveled >= resetDistance)
        {
            ResetTrainer();
        }
    }

    public bool Defeated()
    {
        return defeated;
    }

    public void ResetTrainerPosition()
    {
        trainerTransform.position = originalPosition;
    }

    public void Interact()
    {
        if (interactor == null)
            return;

        interactor.LockInteraction();
        FacePlayer();

        if (defeated)
        {
            interactor.UnlockInteraction();

            DialogueUI.Instance.StartDialogue(
                trainer.defeatDialogue,
                trainer.trainerName,
                false
            );

            return;
        }

        // Heal the trainer's creatures before battle.
        foreach (CreatureInstance creature in party)
        {
            creature.HealFull();
        }

        DialogueUI.Instance.SetPendingTrainer(this);

        DialogueUI.Instance.StartDialogue(
            trainer.introDialogue,
            trainer.trainerName,
            false
        );
    }

    private void FacePlayer()
    {
        if (player == null)
            return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void MarkDefeated()
    {
        defeated = true;
        distanceTraveled = 0f;

        if (player != null)
            lastPlayerPosition = player.position;
    }

    private void ResetTrainer()
    {
        defeated = false;
        distanceTraveled = 0f;

        GenerateRandomParty();

        if (trainerWander != null)
        {
            trainerWander.ResetWandering();
        }

        lastPlayerPosition = player.position;
    }

    public List<CreatureInstance> GetParty()
    {
        return party;
    }

    private void GenerateRandomParty()
    {
        party.Clear();

        if (trainer.possibleCreatures == null ||
            trainer.possibleCreatures.Length == 0)
        {
            Debug.LogWarning(
                $"{trainer.trainerName} has no possible creatures assigned."
            );
            return;
        }

        int strongestLevel = 1;

        foreach (CreatureInstance creature in PartyManager.Instance.party)
        {
            if (creature != null)
            {
                strongestLevel = Mathf.Max(
                    strongestLevel,
                    creature.level
                );
            }
        }

        int minSize = Mathf.Max(1, trainer.minPartySize);
        int maxSize = Mathf.Max(minSize, trainer.maxPartySize);

        int partySize = Random.Range(minSize, maxSize + 1);

        for (int i = 0; i < partySize; i++)
        {
            CreatureData species = trainer.possibleCreatures[
                Random.Range(0, trainer.possibleCreatures.Length)
            ];

            if (species == null)
                continue;

            int level = Mathf.Clamp(
                strongestLevel + Random.Range(-2, 3),
                1,
                100
            );

            party.Add(new CreatureInstance(species, level));
        }
    }
}
