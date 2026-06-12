using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreatureBattleUI : MonoBehaviour
{
    public Canvas battleCanvas;

    public TextMeshProUGUI nameText;

    public Slider hpSlider;


    public void Setup(CreatureInstance creature)
    {
        nameText.text = creature.CreatureName;
        hpSlider.value = (float)creature.currentHP / creature.MaxHP;
    }

    public void Show()
    {
        battleCanvas.gameObject.SetActive(true);
    }


    public void Hide()
    {
        battleCanvas.gameObject.SetActive(false);
    }
}