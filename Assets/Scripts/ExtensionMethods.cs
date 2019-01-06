using UnityEngine;

public static class ExtensionMethods
{
    public static float AngleFromZero(this Vector2 vector2)
    {
        return (Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg) - 90;
    }
}
