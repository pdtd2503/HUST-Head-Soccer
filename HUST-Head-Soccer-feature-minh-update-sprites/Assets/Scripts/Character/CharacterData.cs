using UnityEngine;

public enum SkillType
{
    PowerShot,
    DoubleJump,
    Wall,
    BallBlink
}

[CreateAssetMenu(fileName = "CharacterData",
menuName = "HUST Head Soccer/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;

    [Header("Head Sprites")]
    public Sprite headRightSprite; // Dùng cho Player 1
    public Sprite headLeftSprite;  // Dùng cho Player 2

    [Header("Foot Sprites")]
    public Sprite footRightSprite; // Dùng cho Player 1
    public Sprite footLeftSprite;  // Dùng cho Player 2

    [Header("Stats")]
    [Range(1, 5)]
    public int jumpStars;

    [Range(1, 5)]
    public int speedStars;

    [Range(1, 5)]
    public int kickStars;

    [Range(1, 5)]
    public int massStars;

    public SkillType skillType;
}