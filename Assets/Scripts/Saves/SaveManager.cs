using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
{
    SaveManager.Instance.SaveGame();
}
if(Input.GetKeyDown(KeyCode.F9))
{
    SaveManager.Instance.LoadGame();
}
    }

    public void SaveGame()
    {
        SaveData save =
            new SaveData();

        foreach(
            CreatureInstance creature
            in PartyManager.Instance.party
        )
        {
            CreatureSaveData data =
                new CreatureSaveData();

            data.speciesName =
                creature.species.creatureName;

            data.level =
                creature.level;

            data.currentHP =
                creature.currentHP;

            data.experience =
                creature.experience;

            save.party.Add(data);
        }

        string json =
            JsonUtility.ToJson(
                save,
                true
            );

        PlayerPrefs.SetString(
            "SaveData",
            json
        );

        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if(!PlayerPrefs.HasKey("SaveData"))
        {
            return;
        }

        string json = PlayerPrefs.GetString("SaveData");

        SaveData save = JsonUtility.FromJson<SaveData>(json);

        PartyManager.Instance.party.Clear();

    foreach(CreatureSaveData creatureData in save.party)
    {
        CreatureData species =
            CreatureDatabase.Instance
            .GetCreatureByName(
                creatureData.speciesName
            );

        CreatureInstance creature =
            new CreatureInstance(
                species,
                creatureData.level
            );

        creature.currentHP =
            creatureData.currentHP;

        creature.experience =
            creatureData.experience;

        PartyManager.Instance
            .AddCreature(
                creature
            );
    }

        FollowerManager.Instance
            .SpawnFollower();
    }
}