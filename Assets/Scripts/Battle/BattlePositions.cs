using UnityEngine;

public class BattlePositions : MonoBehaviour
{
    public static BattlePositions Instance;
    public Transform battleCameraTarget;
    public Transform battleCenter;
    public Transform playerCreatureSpot;
    public Transform enemySpot;

    private void Awake()
    {
        Instance = this;
    }
}