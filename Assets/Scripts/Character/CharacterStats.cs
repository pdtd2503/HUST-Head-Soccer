public static class CharacterStats
{
    public static float GetJumpReach(int stars)
    {
        return stars switch
        {
            1 => 2.00f,
            2 => 2.25f,
            3 => 2.50f,
            4 => 2.75f,
            5 => 3.00f,
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

    public static float GetMass(int stars)
    {
        return stars switch
        {
            1 => 10.00f,
            2 => 15.00f,
            3 => 20.00f,
            4 => 25.00f,
            5 => 30.00f,
            _ => 20.00f
        };
    }
}