using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    public PlayerController2D player1;
    public PlayerController2D player2;

    private void Start()
    {
        if (player1 == null || player2 == null)
        {
            Debug.LogError("CharacterLoader chưa được gán Player_1 hoặc Player_2");
            return;
        }

        if (GameData.player1Data != null)
        {
            player1.characterData = GameData.player1Data;
        }

        if (GameData.player2Data != null)
        {
            player2.characterData = GameData.player2Data;
        }

        player1.isPlayer1 = true;
        player2.isPlayer1 = false;

        player1.ApplyCharacterData();
        player2.ApplyCharacterData();
    }
}