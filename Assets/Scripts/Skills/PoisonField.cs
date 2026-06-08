using UnityEngine;

public class PoisonField : MonoBehaviour
{
    public float lifeTime = 3f;
    public float stunDuration = 1f;
    public float speedBoostDuration = 2f;

    private PlayerController2D owner;
    private PlayerController2D opponent;
    private bool used;

    public void Setup(
        PlayerController2D ownerController,
        PlayerController2D opponentController,
        float poisonLifeTime,
        float poisonStunDuration,
        float ownerSpeedBoostDuration
    )
    {
        owner = ownerController;
        opponent = opponentController;
        lifeTime = poisonLifeTime;
        stunDuration = poisonStunDuration;
        speedBoostDuration = ownerSpeedBoostDuration;

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used)
        {
            return;
        }

        PlayerController2D target = other.GetComponentInParent<PlayerController2D>();

        if (target == null)
        {
            return;
        }

        if (target == owner)
        {
            return;
        }

        if (opponent != null && target != opponent)
        {
            return;
        }

        used = true;

        target.Stun(stunDuration);

        if (owner != null)
        {
            owner.SetTemporarySpeedStars(5, speedBoostDuration);
        }

        Destroy(gameObject);
    }
}