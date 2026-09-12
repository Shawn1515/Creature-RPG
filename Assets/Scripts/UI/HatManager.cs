using UnityEngine;
using TMPro;

public class HatManager : MonoBehaviour
{
    public static HatManager Instance;

    public TextMeshProUGUI hatCountText;

    public int hatCount = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool HasHat()
    {
        return hatCount > 0;
    }

    public bool UseHat()
    {
        if (hatCount <= 0)
        {
            return false;
        }

        hatCount--;
        hatCountText.text = "x" + hatCount;

        return true;
    }

    public void AddHats(int amount)
    {
        hatCount += amount;
        hatCountText.text = "x" + hatCount;
    }
}