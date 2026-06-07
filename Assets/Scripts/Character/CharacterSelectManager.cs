using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    public TMP_Text seeeLabel;
    public TMP_Text sclcLabel;
    public TMP_Text smeLabel;
    public TMP_Text soictLabel;

    public CharacterData seeeData;
    public CharacterData sclcData;
    public CharacterData smeData;
    public CharacterData soictData;

    private bool choosingPlayer1 = true;

    void Start()
    {
        ResetSelection();
    }

    public void SelectCharacter(string characterName)
    {
        if (GameData.player1Character != "" &&
            GameData.player2Character != "")
        {
            return;
        }

        if (choosingPlayer1)
        {
            GameData.player1Character = characterName;
            GameData.player1Data = GetCharacterData(characterName);

            AddLabel(characterName, "P1");

            choosingPlayer1 = false;
        }
        else
        {
            GameData.player2Character = characterName;
            GameData.player2Data = GetCharacterData(characterName);

            AddLabel(characterName, "P2");
        }
    }

    CharacterData GetCharacterData(string characterName)
    {
        switch (characterName)
        {
            case "SEEE":
                return seeeData;

            case "SCLC":
                return sclcData;

            case "SME":
                return smeData;

            case "SOICT":
                return soictData;
        }

        return null;
    }

    void AddLabel(string characterName, string playerTag)
    {
        TMP_Text targetLabel = null;

        switch (characterName)
        {
            case "SEEE":
                targetLabel = seeeLabel;
                break;

            case "SCLC":
                targetLabel = sclcLabel;
                break;

            case "SME":
                targetLabel = smeLabel;
                break;

            case "SOICT":
                targetLabel = soictLabel;
                break;
        }

        if (targetLabel == null)
            return;

        if (targetLabel.text == "")
        {
            targetLabel.text = playerTag;
        }
        else
        {
            targetLabel.text += " " + playerTag;
        }
    }

    public void ResetSelection()
    {
        GameData.player1Character = "";
        GameData.player2Character = "";

        GameData.player1Data = null;
        GameData.player2Data = null;

        choosingPlayer1 = true;

        seeeLabel.text = "";
        sclcLabel.text = "";
        smeLabel.text = "";
        soictLabel.text = "";
    }

    public void StartMatch()
    {
        if (GameData.player1Data == null ||
            GameData.player2Data == null)
        {
            Debug.Log("Chua chon du nhan vat");
            return;
        }

        SceneManager.LoadScene("Prototype_Level");
    }
}