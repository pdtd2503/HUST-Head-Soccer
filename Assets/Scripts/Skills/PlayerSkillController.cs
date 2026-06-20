using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    private const KeyCode PLAYER_1_SKILL_KEY = KeyCode.V;
    private const KeyCode PLAYER_2_SKILL_KEY = KeyCode.O;

    private const float SOICT_COOLDOWN = 10f;
    private const float SME_COOLDOWN = 12f;
    private const float SCLS_COOLDOWN = 14f;
    private const float SEEE_COOLDOWN = 8f;

    private PlayerController2D playerController;
    private PlayerController2D opponentController;
    private MatchManager matchManager;

    private Transform ball;
    private Rigidbody2D ballRb;

    private KeyCode skillKey;

    private float skillCharge;
    private float currentCooldownDuration;

    private void Awake()
    {
        playerController = GetComponent<PlayerController2D>();

        if (playerController != null && playerController.isPlayer1)
        {
            skillKey = PLAYER_1_SKILL_KEY;
        }
        else
        {
            skillKey = PLAYER_2_SKILL_KEY;
        }

        matchManager = FindFirstObjectByType<MatchManager>();

        FindBall();
    }

    private void Start()
    {
        FindOpponentController();

        currentCooldownDuration = GetCooldownDuration();
        skillCharge = 0f;
    }

    private void Update()
    {
        UpdateSkillCharge();

        if (!Input.GetKeyDown(skillKey))
        {
            return;
        }

        if (!CanUseSkill())
        {
            return;
        }

        RefreshReferences();

        SkillType skillType = playerController.characterData.skillType;

        UseSkill(skillType);
        ResetSkillCharge();
    }

    private void UpdateSkillCharge()
    {
        if (matchManager != null && !matchManager.CanUsePlayerActions())
        {
            return;
        }

        if (currentCooldownDuration <= 0f)
        {
            currentCooldownDuration = GetCooldownDuration();
        }

        if (skillCharge >= currentCooldownDuration)
        {
            skillCharge = currentCooldownDuration;
            return;
        }

        skillCharge += Time.deltaTime;

        if (skillCharge > currentCooldownDuration)
        {
            skillCharge = currentCooldownDuration;
        }
    }

    private bool CanUseSkill()
    {
        if (matchManager != null && !matchManager.CanUsePlayerActions())
        {
            return false;
        }

        if (playerController == null)
        {
            Debug.LogWarning($"{name} missing PlayerController2D.");
            return false;
        }

        if (playerController.characterData == null)
        {
            Debug.LogWarning($"{name} missing CharacterData.");
            return false;
        }

        if (skillCharge < currentCooldownDuration)
        {
            float remainingTime = currentCooldownDuration - skillCharge;
            Debug.Log($"{name} skill charging: {remainingTime:F1}s remaining");
            return false;
        }

        return true;
    }

    private void RefreshReferences()
    {
        if (ball == null || ballRb == null)
        {
            FindBall();
        }

        if (opponentController == null)
        {
            FindOpponentController();
        }
    }

    private void FindBall()
    {
        GameObject ballObject = GameObject.FindGameObjectWithTag("Ball");

        if (ballObject == null)
        {
            ball = null;
            ballRb = null;
            return;
        }

        ball = ballObject.transform;
        ballRb = ballObject.GetComponent<Rigidbody2D>();
    }

    private void FindOpponentController()
    {
        PlayerController2D[] players =
            FindObjectsByType<PlayerController2D>(
                FindObjectsSortMode.None
            );

        opponentController = null;

        foreach (PlayerController2D player in players)
        {
            if (player != playerController)
            {
                opponentController = player;
                return;
            }
        }
    }

    private void UseSkill(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.SOICT:
                SOICTSkill.UseSkill(playerController, ballRb);
                break;

            case SkillType.SME:
                SMESkill.UseSkill(playerController, matchManager);
                break;

            case SkillType.SCLS:
                SCLSSkill.UseSkill(opponentController);
                break;

            case SkillType.SEEE:
                SEEESkill.UseSkill(playerController, ball);
                break;

            default:
                Debug.LogWarning($"Unhandled skill type: {skillType}");
                break;
        }
    }

    private void ResetSkillCharge()
    {
        currentCooldownDuration = GetCooldownDuration();
        skillCharge = 0f;
    }

    private float GetCooldownBySkillType(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.SOICT:
                return SOICT_COOLDOWN;

            case SkillType.SME:
                return SME_COOLDOWN;

            case SkillType.SCLS:
                return SCLS_COOLDOWN;

            case SkillType.SEEE:
                return SEEE_COOLDOWN;

            default:
                return 10f;
        }
    }

    public float GetCooldownDuration()
    {
        if (playerController == null || playerController.characterData == null)
        {
            return 1f;
        }

        SkillType skillType = playerController.characterData.skillType;
        return GetCooldownBySkillType(skillType);
    }

    public float GetCooldownTimer()
    {
        return Mathf.Max(0f, currentCooldownDuration - skillCharge);
    }

    public bool IsSkillReady()
    {
        return skillCharge >= currentCooldownDuration;
    }

    public float GetSkillChargeRatio()
    {
        if (currentCooldownDuration <= 0f)
        {
            currentCooldownDuration = GetCooldownDuration();
        }

        if (currentCooldownDuration <= 0f)
        {
            return 1f;
        }

        return Mathf.Clamp01(skillCharge / currentCooldownDuration);
    }
}