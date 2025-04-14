using UnityEngine;

/// <summary>
/// Extension metgods for classes Vector2, Vector3 and Vector4.
/// </summary>
public static class VectorExtensions
{
    /// <summary>
    /// Extend vector2 to Vector3 by adding z component with a specified value.
    /// </summary>
    public static Vector3 Extend(this Vector2 vector, float z) => new(vector.x, vector.y, z);
}
