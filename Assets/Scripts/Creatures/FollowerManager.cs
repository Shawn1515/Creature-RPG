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
            GameObject temp = currentFollower;
            CreatureInstance leader = PartyManager.Instance.GetActiveCreature();

            GameObject prefab = leader.species.creaturePrefab;

            Physics.Raycast(temp.transform.position + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 200f, LayerMask.GetMask("Ground"));
            Vector3 pos = hit.point + Vector3.up * leader.species.groundOffset;
            
            currentFollower =
                Instantiate(
                    prefab,
                    pos,
                    Quaternion.identity
                );
            FollowerCreature follower =
                currentFollower.GetComponent<FollowerCreature>();
            follower.player = player;
            Destroy(temp);
        }
        else
        {
            CreatureInstance leader = PartyManager.Instance.GetActiveCreature();

            GameObject prefab = leader.species.creaturePrefab;

            Physics.Raycast(player.position + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 200f, LayerMask.GetMask("Ground"));
            Vector3 pos = hit.point + Vector3.up * leader.species.groundOffset;
            
            currentFollower =
                Instantiate(
                    prefab,
                    pos + Vector3.right * 5f,
                    Quaternion.identity
                );
            FollowerCreature follower =
                currentFollower.GetComponent<FollowerCreature>();
            follower.player = player;
        }
    }
}