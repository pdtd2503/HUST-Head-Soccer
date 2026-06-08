using UnityEngine;

public enum SkillType
{
    SEEE_DashToBall,
    SME_BallUpDoubleJump,
    SOICT_StraightShot,
    SCLS_Poison
}

[CreateAssetMenu(fileName = "CharacterData",
menuName = "HUST Head Soccer/Character Data")]
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
    [Range(1, 5)]
    public int jumpStars;

    [Range(1, 5)]
    public int speedStars;

    [Range(1, 5)]
    public int kickStars;

    [Range(1, 5)]
    public int massStars;

    [Header("Skill")]
    public SkillType skillType;
}