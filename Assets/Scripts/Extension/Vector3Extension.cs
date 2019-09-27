using UnityEngine;

public static class MathExtension
{
    public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
    {
        float _x = x ?? vector.x;
        float _y = y ?? vector.y;
        float _z = z ?? vector.z;

        return new Vector3(_x, _y, _z);
    }

    public static float DistanceXZ(this Vector3 origin, Vector3 final)
    {
        Vector3 first = new Vector3(origin.x, 0, origin.z);
        Vector3 second = new Vector3(final.x, 0, final.z);

        return Vector3.Distance(first, second);
    }

    public static float InvertMouseY(float y)
    {
        return Screen.height - y;
    }
}
