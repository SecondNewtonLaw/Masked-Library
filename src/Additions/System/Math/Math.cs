using SysMath = System.Math;

namespace Masked.Sys;

/// <summary>
/// Provides certain mathemathical utilities
/// </summary>
public sealed partial class Math {
    /// <summary>
    /// Calculate any root of a given number (Aproximation)
    /// </summary>
    /// <param name="number">Number to which it's root should be calculated, Positive ONLY</param>
    /// <param name="root">The root, square, cube, etc to be calculated</param>
    /// <returns>The root of the number</returns>
    public static double CalculateRoot(ulong number, int root) {
        return SysMath.Pow(number, 1.0 / root);
    }

    /// <summary>
    /// Calculates the angle of a rectangle-triangle in degree angles.
    /// </summary>
    /// <returns>An approximate angle based on the Trigonometric inverse tan() function.</returns>
    public static double CalculateAngle(float opposingSide, float adyacentSide) {
        return RadianToDegree(SysMath.Atan(opposingSide / adyacentSide));
    }

    /// <summary>
    /// Converts radians into degree angles.
    /// </summary>
    /// <param name="radians">The measurement of radians</param>
    /// <returns>The inputted radians in degrees</returns>
    public static double RadianToDegree(double radians) {
        return radians * 57.2957795131;
    }

    // 57.2957795131 is 180*π
    // -> So just use a constant.

    /// <summary>
    /// Converts degree angles into radians
    /// </summary>
    /// <param name="degrees">The measurement of radians</param>
    /// <returns>The inputted degrees in radians</returns>
    public static double DegreeToRadian(double degrees) {
        return degrees / 57.2957795131;
    }

    /// <summary>
    /// Calculates Force.
    /// </summary>
    /// <param name="mass">The mass</param>
    /// <param name="acceleration">The Acceleration</param>
    /// <returns></returns>
    public static double CalculateForce(double mass, double acceleration = 9.8d) {
        return mass * acceleration;
    }

    /// <summary>
    /// Solves a quadratic equation given a, b and c.
    /// </summary>
    /// <param name="a">First member of the equation, no X included.</param>
    /// <param name="b">Second member of the equation, no X included.</param>
    /// <param name="c">Third member of the equation, no X included.</param>
    /// <returns>An array containing as value 0 the positive result and the second the negative result.</returns>
    public static double[] QuadraticSolve(double a, double b, double c) {
        //! Reference Equation => x = (-b (+/-) sqrt(b^2 - 4*a*c))/(2*a)
        var negativeB = b * -1;
        var bSquared = SysMath.Pow(b, 2);

        var plusR = (negativeB + SysMath.Sqrt(bSquared - 4 * a * c)) / (2 * a);
        var negR = (negativeB - SysMath.Sqrt(bSquared - 4 * a * c)) / (2 * a);

        return new double[] { plusR, negR };
    }
}