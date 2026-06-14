using UnityEngine;
using System.Collections;

public class PlayerSkillController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode skillKey = KeyCode.LeftShift;

    [Header("References")]
    [SerializeField] private PlayerController2D playerController;
    [SerializeField] private PlayerController2D opponentController;
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private Transform ball;
    [SerializeField] private Rigidbody2D ballRb;

    private SOICTSkill soictSkill;
    private SMESkill smeSkill;
    private SCLSSkill sclsSkill;
    private SEEESkill seeeSkill;

    private float skillCharge;
    private float currentCooldownDuration;
    private const float SOICT_COOLDOWN = 10f;
    private const float SME_COOLDOWN = 12f;
    private const float SCLS_COOLDOWN = 14f;
    private const float SEEE_COOLDOWN = 8f;

    private void Awake()
{
    ResolveReferences();
    SetupSkillComponents();

    currentCooldownDuration = GetCooldownDuration();
    skillCharge = currentCooldownDuration;
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

        RefreshBallReferenceIfNeeded();
        UseSkill(playerController.characterData.skillType);
        ResetSkillCharge();
    }

    private void ResolveReferences()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController2D>();
        }

        if (matchManager == null)
        {
            matchManager = FindFirstObjectByType<MatchManager>();
        }

        FindBall();
        FindOpponentController();
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
            return false;
        }

        return true;
    }

    private void RefreshBallReferenceIfNeeded()
    {
        if (ball == null || ballRb == null)
        {
            FindBall();
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
        PlayerController2D[] players = FindObjectsByType<PlayerController2D>(
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

    private void SetupSkillComponents()
    {
        soictSkill = GetOrAddSkill<SOICTSkill>();
        smeSkill = GetOrAddSkill<SMESkill>();
        sclsSkill = GetOrAddSkill<SCLSSkill>();
        seeeSkill = GetOrAddSkill<SEEESkill>();
    }

    private T GetOrAddSkill<T>() where T : Component
    {
        T skill = GetComponent<T>();

        if (skill == null)
        {
            skill = gameObject.AddComponent<T>();
        }

        return skill;
    }

        private void UseSkill(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.SOICT:
                soictSkill.UseSkill(playerController, ballRb); // không freeze
                break;

            case SkillType.SME:
                StartCoroutine(UseSkillWithFreeze(0.3f, () =>
                    smeSkill.UseSkill(playerController, matchManager)
                ));
                break;

            case SkillType.SCLS:
                sclsSkill.UseSkill(opponentController); // không freeze
                break;

            case SkillType.SEEE:
            seeeSkill.UseSkill(playerController, ball); // không freeze
            break;

            default:
                Debug.LogWarning($"Unhandled skill type: {skillType}");
                break;
        }
    }

    private IEnumerator UseSkillWithFreeze(float freezeDuration, System.Action skillAction)
    {
        Time.timeScale = 0f;

        skillAction?.Invoke(); // gọi skill trong lúc freeze

        yield return new WaitForSecondsRealtime(freezeDuration);

        Time.timeScale = 1f;
    }
    private void UpdateSkillCharge()
{
    if (currentCooldownDuration <= 0f)
    {
        currentCooldownDuration = GetCooldownDuration();
    }

    if (skillCharge < currentCooldownDuration)
    {
        skillCharge += Time.deltaTime;

        if (skillCharge > currentCooldownDuration)
        {
            skillCharge = currentCooldownDuration;
        }
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

    return GetCooldownBySkillType(
        playerController.characterData.skillType
    );
}

public float GetSkillChargeRatio()
{
    if (currentCooldownDuration <= 0f)
    {
        return 1f;
    }

    return Mathf.Clamp01(
        skillCharge / currentCooldownDuration
    );
}
}