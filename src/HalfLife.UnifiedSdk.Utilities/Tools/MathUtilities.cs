using System;
using System.Numerics;

namespace HalfLife.UnifiedSdk.Utilities.Tools
{
    /// <summary> Helpers for performing math operations. </summary>
    public static class MathUtilities
    {
        /// <summary>
        /// Converts angles expressed in degrees to a set of directional unit vectors.
        /// </summary>
        /// <param name="angles">Angles to convert.</param>
        /// <param name="forward">Forward direction.</param>
        /// <param name="right">Right direction.</param>
        /// <param name="up">Up direction.</param>
        public static void AngleVectors(Vector3 angles, out Vector3 forward, out Vector3 right, out Vector3 up)
        {
            float angle = angles.Y * (MathF.PI * 2 / 360);
            float sy = MathF.Sin(angle);
            float cy = MathF.Cos(angle);
            angle = angles.X * (MathF.PI * 2 / 360);
            float sp = MathF.Sin(angle);
            float cp = MathF.Cos(angle);
            angle = angles.Z * (MathF.PI * 2 / 360);
            float sr = MathF.Sin(angle);
            float cr = MathF.Cos(angle);

            forward = new(
                cp * cy,
                cp * sy,
                -sp);

            right = new(
                (-1 * sr * sp * cy) + (-1 * cr * -sy),
                (-1 * sr * sp * sy) + (-1 * cr * cy),
                -1 * sr * cp);

            up = new(
                (cr * sp * cy) + (-sr * -sy),
                (cr * sp * sy) + (-sr * cy),
                cr * cp);
        }
    }
}
