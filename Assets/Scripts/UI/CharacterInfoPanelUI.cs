using TMPro;
using UnityEngine;

public class CharacterInfoPanelUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text skillText;
    [SerializeField] private TMP_Text descriptionText;

    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text jumpText;
    [SerializeField] private TMP_Text kickText;
    [SerializeField] private TMP_Text massText;

    private void Start()
    {
        ShowEmpty();
    }

    public void ShowCharacter(CharacterData data)
    {
        if (data == null)
        {
            ShowEmpty();
            return;
        }

        nameText.text = data.characterName;
        skillText.text = "Skill: " + data.skillDisplayName;
        descriptionText.text = data.skillDescription;

        speedText.text = "SPD: " + Stars(data.speedStars);
        jumpText.text = "JMP: " + Stars(data.jumpStars);
        kickText.text = "KCK: " + Stars(data.kickStars);
        massText.text = "MAS: " + Stars(data.massStars);
    }

    public void ShowEmpty()
    {
        nameText.text = "SELECT A CHARACTER";
        skillText.text = "Skill: -";
        descriptionText.text = "Click a character to view stats and skill.";

        speedText.text = "SPD: -";
        jumpText.text = "JMP: -";
        kickText.text = "KCK: -";
        massText.text = "MAS: -";
    }

   private string Stars(int value)
    {
        value = Mathf.Clamp(value, 1, 5);

        string result = "[";

        for (int i = 0; i < 5; i++)
        {
            result += i < value ? "*" : "-";
        }

        result += "]";

        return result;
    }
}