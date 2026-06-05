using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    public TMP_Text seeeLabel;
    public TMP_Text sclcLabel;
    public TMP_Text smeLabel;
    public TMP_Text soictLabel;

    private bool choosingPlayer1 = true;

    void Start()
    {
        ResetSelection();
    }

    public void SelectCharacter(string characterName)
    {
        // Đã chọn đủ 2 người thì không cho chọn tiếp
        if (GameData.player1Character != "" &&
            GameData.player2Character != "")
        {
            return;
        }

        if (choosingPlayer1)
        {
            GameData.player1Character = characterName;

            AddLabel(characterName, "P1");

            choosingPlayer1 = false;
        }
        else
        {
            GameData.player2Character = characterName;

            AddLabel(characterName, "P2");
        }
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

        choosingPlayer1 = true;

        seeeLabel.text = "";
        sclcLabel.text = "";
        smeLabel.text = "";
        soictLabel.text = "";
    }

    public void StartMatch()
    {
        if (GameData.player1Character == "" ||
            GameData.player2Character == "")
        {
            Debug.Log("Chua chon du nhan vat");
            return;
        }

        SceneManager.LoadScene("Prototype_Level");
    }
}