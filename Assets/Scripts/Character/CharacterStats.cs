using UnityEngine;

public static class CharacterStats
{
    private const int MIN_STARS = 1;
    private const int MAX_STARS = 5;

    private const float MIN_SPEED = 3.0f;
    private const float SPEED_STEP = 0.5f;

    private const float MIN_HEAD_REACH = 2.0f;
    private const float HEAD_REACH_STEP = 0.25f;

    private const float MIN_MASS = 10.0f;
    private const float MASS_STEP = 5.0f;

    public static float GetSpeed(int stars)
    {
        int clampedStars = ClampStars(stars);
        return MIN_SPEED + (clampedStars - 1) * SPEED_STEP;
    }

    public static float GetJumpHeadReach(int stars)
    {
        int clampedStars = ClampStars(stars);
        return MIN_HEAD_REACH + (clampedStars - 1) * HEAD_REACH_STEP;
    }

    public static float GetMass(int stars)
    {
        int clampedStars = ClampStars(stars);
        return MIN_MASS + (clampedStars - 1) * MASS_STEP;
    }

    private static int ClampStars(int stars)
    {
        return Mathf.Clamp(stars, MIN_STARS, MAX_STARS);
    }
}