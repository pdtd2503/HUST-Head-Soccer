using UnityEngine;

public class SOICTSkill : MonoBehaviour
{
    private const float STRAIGHT_SHOT_SPEED = 16f;
    private const float MAX_STRAIGHT_SHOT_TIME = 1.5f;

    public void UseSkill(PlayerController2D playerController, Rigidbody2D ballRb)
    {
        if (playerController == null || ballRb == null)
        {
            return;
        }
        AudioManager.Instance?.PlaySkillSOICT(2.5f); 
        SoictBallStraightShotRuntime straightShotRuntime =
            ballRb.GetComponent<SoictBallStraightShotRuntime>();

        if (straightShotRuntime == null)
        {
            straightShotRuntime =
                ballRb.gameObject.AddComponent<SoictBallStraightShotRuntime>();
        }

        int attackDirection = playerController.GetAttackDirection();

        straightShotRuntime.ActivateStraightShot(
            attackDirection,
            STRAIGHT_SHOT_SPEED,
            MAX_STRAIGHT_SHOT_TIME
        );
    }
}