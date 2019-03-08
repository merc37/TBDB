using UnityEngine;

public static class ExtensionMethods
{
    public static float AngleFromZero(this Vector2 vector2)
    {
        return (Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg) - 90;
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static Vector2 Vector2(this Vector3 v)
    {
        return v;
    }

    public static Vector3 Vector3(this Vector2 v)
    {
        return v;
    }

    public static bool IsCircleCollider(this Collider2D collider)
    {
        return collider.GetType() == typeof(CircleCollider2D);
    }

    public static bool IsBoxCollider(this Collider2D collider)
    {
        return collider.GetType() == typeof(BoxCollider2D);
    }
}
