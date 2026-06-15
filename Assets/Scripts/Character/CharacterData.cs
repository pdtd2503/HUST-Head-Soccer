using UnityEngine;

public enum SkillType
{
    SEEE = 0,
    SME = 1,
    SOICT = 2,
    SCLS = 3
}

[CreateAssetMenu(
    fileName = "CharacterData",
    menuName = "HUST Head Soccer/Character Data"
)]
public class CharacterData : ScriptableObject
{
    [Header("Basic Info")]
    public string characterName;
    public string skillDisplayName;

    [TextArea(2, 4)]
    public string skillDescription;

    [Header("Head Sprites")]
    public Sprite headRightSprite;
    public Sprite headLeftSprite;

    [Header("Foot Sprites")]
    public Sprite footRightSprite;
    public Sprite footLeftSprite;

    [Header("Stats")]
    [Range(1, 5)] public int jumpStars;
    [Range(1, 5)] public int speedStars;
    [Range(1, 5)] public int kickStars;
    [Range(1, 5)] public int massStars;

    [Header("Skill")]
    public SkillType skillType;
}