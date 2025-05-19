using UnityEngine;

/// <summary>
/// Extension metgods for classes Vector2, Vector3 and Vector4.
/// </summary>
public static class VectorExtensions
{
    /// <summary>
    /// THreshold bellow which is float value considered as zero.
    /// </summary>
    private const float zeroThreshold = 1e-1f;

    /// <summary>
    /// Extend vector2 to Vector3 by adding z component with a specified value.
    /// </summary>
    public static Vector3 Extend(this Vector2 vector, float z) => new(vector.x, vector.y, z);

    /// <summary>
    /// Drop the Z dimension from the vector 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector2 Truncate(this Vector3 vector) => (Vector2)vector;

    /// <summary>
    /// Determine if vector is zero.
    /// </summary>
    public static bool IsZero(this Vector3 vector) => vector.sqrMagnitude < zeroThreshold;
    /// <summary>
    /// Determine if vector is zero.
    /// </summary>
    public static bool IsZero(this Vector2 vector) => vector.sqrMagnitude < zeroThreshold;
}
