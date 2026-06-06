using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    public PlayerController2D player1;
    public PlayerController2D player2;

    void Start()
    {
        player1.characterData = GameData.player1Data;
        player2.characterData = GameData.player2Data;

        player1.ApplyCharacterData();
        player2.ApplyCharacterData();
    }
}