using UnityEngine;

public enum SkillType
{
    SOICT,
    SME,
    SCLS,
    SEEE
}

[CreateAssetMenu(
    fileName = "CharacterData",
    menuName = "HUST Head Soccer/Character Data"
)]
public class CharacterData : ScriptableObject
{
    public string characterName;

    [Header("Head Sprites")]
    public Sprite headRightSprite;
    public Sprite headLeftSprite;

    [Header("Foot Sprites")]
    public Sprite footRightSprite;
    public Sprite footLeftSprite;

    [Header("Stats")]
    [Range(1, 5)] public int jumpStars = 3;
    [Range(1, 5)] public int speedStars = 3;
    [Range(1, 5)] public int massStars = 3;

    [Header("Skill")]
    public SkillType skillType;
}