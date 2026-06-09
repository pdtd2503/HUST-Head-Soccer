using UnityEngine;

public class SEEESkill : MonoBehaviour
{
    private const float BALL_OFFSET = 0.75f;

    public void UseSkill(PlayerController2D playerController, Transform ball)
    {
        if (playerController == null || ball == null)
        {
            return;
        }

        int attackDirection = playerController.GetAttackDirection();

        Vector3 targetPosition = playerController.transform.position;
        targetPosition.x = ball.position.x - attackDirection * BALL_OFFSET;
        targetPosition.y = ball.position.y;

        Rigidbody2D playerRb = playerController.GetBody();

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
        }

        playerController.transform.position = targetPosition;
    }
}