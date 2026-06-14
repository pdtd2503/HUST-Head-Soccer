using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("P1/P2 Selection Labels")]
    public TMP_Text seeeLabel;
    public TMP_Text sclsLabel;
    public TMP_Text smeLabel;
    public TMP_Text soictLabel;

    [Header("Character Info Texts")]
    public TMP_Text seeeInfoText;
    public TMP_Text sclsInfoText;
    public TMP_Text smeInfoText;
    public TMP_Text soictInfoText;

    [Header("Character Data")]
    public CharacterData seeeData;
    public CharacterData sclsData;
    public CharacterData smeData;
    public CharacterData soictData;

    [Header("Selected Character Detail")]
    public TMP_Text descriptionText;

    private bool choosingPlayer1 = true;

    private void Start()
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
        }
        else
        {
            GameData.player2Character = characterName;
            GameData.player2Data = selectedData;

            AddLabel(characterName, "P2");
        }

        ShowDescription(selectedData);
    }

    private CharacterData GetCharacterData(string characterName)
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

            default:
                return null;
        }
    }

    private void AddLabel(string characterName, string playerTag)
    {
        TMP_Text targetLabel = GetLabel(characterName);

        if (targetLabel == null)
        {
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

    private TMP_Text GetLabel(string characterName)
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

            default:
                return null;
        }
    }

    private void ShowDescription(CharacterData data)
    {
        if (descriptionText == null || data == null)
        {
            return;
        }

        descriptionText.text = BuildCharacterInfoText(data);
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

        if (descriptionText != null)
        {
            descriptionText.text = "Chon nhan vat de xem chi tiet.";
        }

        RenderAllCharacterInfo();
    }

    private void RenderAllCharacterInfo()
    {
        SetInfoText(seeeInfoText, seeeData);
        SetInfoText(sclsInfoText, sclsData);
        SetInfoText(smeInfoText, smeData);
        SetInfoText(soictInfoText, soictData);
    }

    private void SetInfoText(TMP_Text targetText, CharacterData data)
    {
        if (targetText == null)
        {
            return;
        }

        if (data == null)
        {
            targetText.text = "";
            return;
        }

        targetText.text = BuildCharacterInfoText(data);
    }

    private string BuildCharacterInfoText(CharacterData data)
    {
        if (data == null)
        {
            return "";
        }

        string displayName = data.characterName;

        if (string.IsNullOrEmpty(displayName))
        {
            displayName = data.skillType.ToString();
        }

        return
            displayName + "\n" +
            "Jump: " + data.jumpStars + "/5\n" +
            "Speed: " + data.speedStars + "/5\n" +
            "Mass: " + data.massStars + "/5\n" +
            "Skill: " + GetSkillDescription(data);
    }

    private string GetSkillDescription(CharacterData data)
    {
        if (data == null)
        {
            return "";
        }

        if (!string.IsNullOrEmpty(data.skillDescription) &&
            data.skillDescription.Trim().Length > 0)
        {
            return data.skillDescription.Trim();
        }

        switch (data.skillType)
        {
            case SkillType.SOICT:
                return "Sut bong bay nhanh theo chieu ngang va tam thoi khong bi anh huong boi trong luc.";

            case SkillType.SME:
                return "Dung tuong tam thoi truoc khung thanh nha de chan bong.";

            case SkillType.SCLS:
                return "Lam doi thu bi nho lai trong thoi gian ngan.";

            case SkillType.SEEE:
                return "Dich chuyen ngay den vi tri gan bong.";

            default:
                return "Chua co mo ta skill.";
        }
    }

    private void ClearLabel(TMP_Text label)
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