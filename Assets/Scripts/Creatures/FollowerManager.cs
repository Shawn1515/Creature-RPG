using UnityEngine;

public class FollowerManager : MonoBehaviour
{
    public static FollowerManager Instance;

    public Transform player;

    public GameObject currentFollower;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnFollower();
    }

    public void SpawnFollower()
    {
        if (currentFollower != null)
        {
            Destroy(currentFollower);
        }

        CreatureInstance leader = PartyManager.Instance.GetActiveCreature();

        GameObject prefab = leader.species.creaturePrefab;

        currentFollower =
            Instantiate(
                prefab,
                player.position,
                Quaternion.identity
            );
        FollowerCreature follower =
            currentFollower.GetComponent<FollowerCreature>();
        follower.player = player;
    }
}