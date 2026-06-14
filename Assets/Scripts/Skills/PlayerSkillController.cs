using UnityEngine;

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

    [Header("Cooldown Settings")]
    [SerializeField] private float soictCooldown = 10f;
    [SerializeField] private float smeCooldown = 12f;
    [SerializeField] private float sclsCooldown = 14f;
    [SerializeField] private float seeeCooldown = 8f;

    private float cooldownTimer;

    private SOICTSkill soictSkill;
    private SMESkill smeSkill;
    private SCLSSkill sclsSkill;
    private SEEESkill seeeSkill;

    private void Awake()
    {
        ResolveReferences();
        SetupSkillComponents();
    }

    private void Update()
    {
        UpdateCooldown();

        if (!Input.GetKeyDown(skillKey))
        {
            return;
        }

        if (!CanUseSkill())
        {
            return;
        }

        RefreshBallReferenceIfNeeded();

        SkillType skillType = playerController.characterData.skillType;
        UseSkill(skillType);
        StartCooldown(skillType);
    }

    private void UpdateCooldown()
    {
        if (cooldownTimer <= 0f)
        {
            return;
        }

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer < 0f)
        {
            cooldownTimer = 0f;
        }
    }

    private void StartCooldown(SkillType skillType)
    {
        cooldownTimer = GetCooldownBySkillType(skillType);
    }

    private float GetCooldownBySkillType(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.SOICT:
                return soictCooldown;

            case SkillType.SME:
                return smeCooldown;

            case SkillType.SCLS:
                return sclsCooldown;

            case SkillType.SEEE:
                return seeeCooldown;

            default:
                return 10f;
        }
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
        if (cooldownTimer > 0f)
        {
            Debug.Log($"{name} skill is cooling down: {cooldownTimer:F1}s left.");
            return false;
        }

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
                soictSkill.UseSkill(playerController, ballRb);
                break;

            case SkillType.SME:
                smeSkill.UseSkill(playerController, matchManager);
                break;

            case SkillType.SCLS:
                sclsSkill.UseSkill(opponentController);
                break;

            case SkillType.SEEE:
                seeeSkill.UseSkill(playerController, ball);
                break;

            default:
                Debug.LogWarning($"Unhandled skill type: {skillType}");
                break;
        }
    }

    public float GetCooldownTimer()
    {
        return cooldownTimer;
    }

    public bool IsSkillReady()
    {
        return cooldownTimer <= 0f;
    }
}