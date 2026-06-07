public static class CharacterStats
{
    public static float GetJumpReach(int stars)
    {
        return stars switch
        {
            1 => 1.50f,
            2 => 1.75f,
            3 => 2.00f,
            4 => 2.25f,
            5 => 2.50f,
            _ => 2.00f
        };
    }

    public static float GetSpeed(int stars)
    {
        return stars switch
        {
            1 => 2.00f,
            2 => 2.50f,
            3 => 3.00f,
            4 => 3.50f,
            5 => 4.00f,
            _ => 3.00f
        };
    }

    public static float GetKickSpeed(int stars)
    {
        return stars switch
        {
            1 => 2.00f,
            2 => 2.50f,
            3 => 3.00f,
            4 => 3.50f,
            5 => 4.00f,
            _ => 3.00f
        };
    }

    public static float GetMass(int stars)
    {
        return stars switch
        {
            1 => 1.00f,
            2 => 1.50f,
            3 => 2.00f,
            4 => 2.50f,
            5 => 3.00f,
            _ => 2.00f
        };
    }
}