namespace Masked.Sys;

/// <summary>
/// Provides certain mathemathical utilities
/// </summary>
public sealed partial class Math
{
    /// <summary>
    /// Calculate any root of a given number (Aproximation)
    /// </summary>
    /// <param name="number">Number to which it's root should be calculated, Positive ONLY</param>
    /// <param name="root">The root, square, cube, etc to be calculated</param>
    /// <returns>The root of the number</returns>
    public static double CalculateRoot(ulong number, int root)
        => System.Math.Pow(number, 1.0 / root);

    /// <summary>
    /// Calculates the angle of a rectangle-triangle in degree angles.
    /// </summary>
    /// <returns>An approximate angle based on the Trigonometric inverse tan() function.</returns>
    public static double CalculateAngle(float opposingSide, float adyacentSide)
        => RadianToDegree(System.Math.Atan(opposingSide / adyacentSide));

    /// <summary>
    /// Converts radians into degree angles.
    /// </summary>
    /// <param name="radians">The measurement of radians</param>
    /// <returns>The inputted radians in degrees</returns>
    public static double RadianToDegree(double radians)
        => radians * 57.2957795131;

    // 57.2957795131 is 180*π
    // -> So just use a constant.

    /// <summary>
    /// Converts degree angles into radians
    /// </summary>
    /// <param name="degrees">The measurement of radians</param>
    /// <returns>The inputted degrees in radians</returns>
    public static double DegreeToRadian(double degrees)
        => degrees / 57.2957795131;
}