using UnityEngine;

public struct IntVector2
{
    public int x;
    public int y;

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static implicit operator Vector2(IntVector2 v)
    {
        return new Vector2(v.x, v.y);
    }
    public static implicit operator Vector3(IntVector2 v)
    {
        return new Vector3(v.x, v.y, 0);
    }

    public override string ToString()
    {
        return "(" + this.x + ", " + this.y + ")";
    }
}