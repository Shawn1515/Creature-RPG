using UnityEngine;
public enum GameState
{
    Exploration,
    Dialogue,
    Battle
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState CurrentState = GameState.Exploration;

    private void Awake()
    {
        Instance = this;
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        Debug.Log("Game State: " + newState);
    }
}