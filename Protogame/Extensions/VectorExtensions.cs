// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Vector extensions.
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        /// Creates a Quaternion representing the rotation to look from the original direction
        /// vector to the target direction vector.
        /// </summary>
        /// <param name="originalDirection">The original direction vector.</param>
        /// <param name="targetDirection">The target direction vector.</param>
        /// <param name="up">The up vector.</param>
        /// <returns>A quaternion representing the rotation.</returns>
        /// <remarks>
        /// Sourced from http://gamedev.stackexchange.com/questions/15070/orienting-a-model-to-face-a-target.
        /// </remarks>
        public static Quaternion CreateLookQuaternion(this Vector3 originalDirection, Vector3 targetDirection, Vector3 up)
        {
            float dot = Vector3.Dot(originalDirection, targetDirection);

            if (Math.Abs(dot - (-1.0f)) < 0.000001f)
            {
                // vector a and b point exactly in the opposite direction, 
                // so it is a 180 degrees turn around the up-axis
                return new Quaternion(up, MathHelper.ToRadians(180.0f));
            }
            if (Math.Abs(dot - 1.0f) < 0.000001f)
            {
                // vector a and b point exactly in the same direction
                // so we return the identity quaternion
                return Quaternion.Identity;
            }

            var rotAngle = (float)Math.Acos(dot);
            var rotAxis = Vector3.Cross(originalDirection, targetDirection);
            rotAxis = Vector3.Normalize(rotAxis);
            return Quaternion.CreateFromAxisAngle(rotAxis, rotAngle);
        }
    }
}