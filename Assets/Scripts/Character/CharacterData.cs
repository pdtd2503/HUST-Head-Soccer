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

    public Sprite characterSprite;

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