using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterData[] characters;

    [Header("Player 1 UI")]
    public Image player1CharacterImage;
    public TMP_Text player1CharacterNameText;
    public TMP_Text player1StatsText;
    public TMP_Text player1SkillText;

    [Header("Player 2 UI")]
    public Image player2CharacterImage;
    public TMP_Text player2CharacterNameText;
    public TMP_Text player2StatsText;
    public TMP_Text player2SkillText;

    [Header("Scene Settings")]
    public string mapSelectSceneName = "MapSelect";

    private int player1Index = 0;
    private int player2Index = 1;

    private void Start()
    {
        if (characters == null || characters.Length == 0)
        {
            Debug.LogError("CharacterSelectManager: Chưa gán danh sách characters.");
            return;
        }

        if (characters.Length == 1)
        {
            player1Index = 0;
            player2Index = 0;
        }
        else
        {
            player1Index = Mathf.Clamp(player1Index, 0, characters.Length - 1);
            player2Index = Mathf.Clamp(player2Index, 0, characters.Length - 1);

            if (player1Index == player2Index)
            {
                player2Index = (player1Index + 1) % characters.Length;
            }
        }

        UpdateAllUI();
    }

    public void Player1Next()
    {
        player1Index = GetNextIndex(player1Index);
        UpdatePlayer1UI();
    }

    public void Player1Previous()
    {
        player1Index = GetPreviousIndex(player1Index);
        UpdatePlayer1UI();
    }

    public void Player2Next()
    {
        player2Index = GetNextIndex(player2Index);
        UpdatePlayer2UI();
    }

    public void Player2Previous()
    {
        player2Index = GetPreviousIndex(player2Index);
        UpdatePlayer2UI();
    }

    public void StartMatch()
    {
        if (characters == null || characters.Length == 0)
        {
            Debug.LogError("Không thể bắt đầu vì chưa có character data.");
            return;
        }

        CharacterData player1Data = characters[player1Index];
        CharacterData player2Data = characters[player2Index];

        if (player1Data == null || player2Data == null)
        {
            Debug.LogError("Không thể bắt đầu vì Player 1 hoặc Player 2 chưa có dữ liệu nhân vật.");
            return;
        }

        GameData.player1Data = player1Data;
        GameData.player2Data = player2Data;

        GameData.player1Character = GetDisplayName(player1Data);
        GameData.player2Character = GetDisplayName(player2Data);

        SceneManager.LoadScene(mapSelectSceneName);
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    private void UpdateAllUI()
    {
        UpdatePlayer1UI();
        UpdatePlayer2UI();
    }

    private void UpdatePlayer1UI()
    {
        CharacterData data = GetCharacter(player1Index);
        UpdatePlayerUI(data, player1CharacterImage, player1CharacterNameText, player1StatsText, player1SkillText, true);
    }

    private void UpdatePlayer2UI()
    {
        CharacterData data = GetCharacter(player2Index);
        UpdatePlayerUI(data, player2CharacterImage, player2CharacterNameText, player2StatsText, player2SkillText, false);
    }

    private void UpdatePlayerUI(
        CharacterData data,
        Image characterImage,
        TMP_Text characterNameText,
        TMP_Text statsText,
        TMP_Text skillText,
        bool isPlayer1
    )
    {
        if (data == null)
        {
            return;
        }

        if (characterImage != null)
        {
            Sprite spriteToShow = isPlayer1 ? data.headRightSprite : data.headLeftSprite;

            if (spriteToShow == null)
            {
                spriteToShow = data.headRightSprite != null ? data.headRightSprite : data.headLeftSprite;
            }

            characterImage.sprite = spriteToShow;
            characterImage.preserveAspect = true;
        }

        if (characterNameText != null)
        {
            characterNameText.text = GetDisplayName(data);
        }

        if (statsText != null)
        {
            statsText.text = BuildStatsText(data);
        }

        if (skillText != null)
        {
            skillText.text = "SKILL: " + GetSkillName(data);
        }
    }

    private CharacterData GetCharacter(int index)
    {
        if (characters == null || characters.Length == 0)
        {
            return null;
        }

        index = Mathf.Clamp(index, 0, characters.Length - 1);
        return characters[index];
    }

    private int GetNextIndex(int currentIndex)
    {
        if (characters == null || characters.Length == 0)
        {
            return 0;
        }

        return (currentIndex + 1) % characters.Length;
    }

    private int GetPreviousIndex(int currentIndex)
    {
        if (characters == null || characters.Length == 0)
        {
            return 0;
        }

        return (currentIndex - 1 + characters.Length) % characters.Length;
    }

    private string BuildStatsText(CharacterData data)
    {
        return
            "SPD  " + BuildBoxes(data.speedStars) + "\n" +
            "JMP  " + BuildBoxes(data.jumpStars) + "\n" +
            "KCK  " + BuildBoxes(data.kickStars) + "\n" +
            "MAS  " + BuildBoxes(data.massStars);
    }

    private string BuildBoxes(int value)
    {
        value = Mathf.Clamp(value, 0, 5);

        string result = "";

        for (int i = 0; i < 5; i++)
        {
            result += i < value ? "■" : "□";
        }

        return result;
    }

    private string GetDisplayName(CharacterData data)
    {
        if (data == null)
        {
            return "";
        }

        if (!string.IsNullOrWhiteSpace(data.characterName))
        {
            return data.characterName.Trim();
        }

        return data.skillType.ToString();
    }

    private string GetSkillName(CharacterData data)
    {
        if (data == null)
        {
            return "";
        }

        switch (data.skillType)
        {
            case SkillType.SOICT:
                return "DIGITAL SHOT";

            case SkillType.SME:
                return "DEFENSE WALL";

            case SkillType.SCLS:
                return "CHEMICAL FLASK";

            case SkillType.SEEE:
                return "TELEPORT";

            default:
                return "UNKNOWN";
        }
    }
}