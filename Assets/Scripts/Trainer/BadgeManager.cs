using UnityEngine;

public class BadgeManager : MonoBehaviour
{
    public static BadgeManager Instance;

    public bool badge1;
    public bool badge2;
    public bool badge3;

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

    public int BadgeCount()
    {
        int count = 0;

        if (badge1) count++;
        if (badge2) count++;
        if (badge3) count++;

        return count;
    }

    public bool HasAllBadges()
    {
        return badge1 && badge2 && badge3;
    }

    public void GiveBadge(int badgeNumber)
    {
        if (badgeNumber == 1)
            badge1 = true;

        if (badgeNumber == 2)
            badge2 = true;

        if (badgeNumber == 3)
            badge3 = true;
    }
}