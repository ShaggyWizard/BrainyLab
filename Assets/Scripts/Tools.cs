using System.Collections;
using UnityEngine;


public static class Tools
{
    public static Vector2 Vector3ToVector2(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.z);
    }
    public static Vector3 Vector2ToVector3(Vector2 vector2)
    {
        return new Vector3(vector2.x, 0, vector2.y);
    }
}