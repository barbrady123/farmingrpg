public static class Conversions
{
    public static Direction DirectionFromAxisInput(float x, float y)
    {
        if (x < 0) return Direction.Left;
        else if (x > 0) return Direction.Right;
        else if (y < 0) return Direction.Down;
        return Direction.Up;
    }
}
