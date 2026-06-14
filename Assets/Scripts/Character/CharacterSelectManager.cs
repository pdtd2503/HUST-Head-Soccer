using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    public TMP_Text seeeLabel;
    public TMP_Text sclsLabel;
    public TMP_Text smeLabel;
    public TMP_Text soictLabel;

    public CharacterData seeeData;
    public CharacterData sclsData;
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

        CharacterData selectedData = GetCharacterData(characterName);

        if (selectedData == null)
        {
            Debug.LogWarning($"No CharacterData found for {characterName}");
            return;
        }

        if (choosingPlayer1)
        {
            GameData.player1Character = characterName;
            GameData.player1Data = selectedData;

            AddLabel(characterName, "P1");

            choosingPlayer1 = false;

            Debug.Log($"Player 1 selected: {characterName}");
        }
        else
        {
            GameData.player2Character = characterName;
            GameData.player2Data = selectedData;

            AddLabel(characterName, "P2");

            Debug.Log($"Player 2 selected: {characterName}");
        }
    }

    CharacterData GetCharacterData(string characterName)
    {
        switch (characterName)
        {
            case "SEEE":
                return seeeData;

            case "SCLS":
                return sclsData;

            case "SME":
                return smeData;

            case "SOICT":
                return soictData;
        }

        return null;
    }

    void AddLabel(string characterName, string playerTag)
    {
        TMP_Text targetLabel = GetLabel(characterName);

        if (targetLabel == null)
        {
            Debug.LogWarning($"No label found for {characterName}");
            return;
        }

        if (targetLabel.text == "")
        {
            targetLabel.text = playerTag;
        }
        else
        {
            targetLabel.text += " " + playerTag;
        }
    }

    TMP_Text GetLabel(string characterName)
    {
        switch (characterName)
        {
            case "SEEE":
                return seeeLabel;

            case "SCLS":
                return sclsLabel;

            case "SME":
                return smeLabel;

            case "SOICT":
                return soictLabel;
        }

        return null;
    }

    public void ResetSelection()
    {
        GameData.player1Character = "";
        GameData.player2Character = "";

        GameData.player1Data = null;
        GameData.player2Data = null;

        choosingPlayer1 = true;

        ClearLabel(seeeLabel);
        ClearLabel(sclsLabel);
        ClearLabel(smeLabel);
        ClearLabel(soictLabel);

        Debug.Log("Selection reset.");
    }

    void ClearLabel(TMP_Text label)
    {
        if (label != null)
        {
            label.text = "";
        }
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