using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// A time machine for transforms.
    /// </summary>
    /// <module>Network</module>
    public class TransformTimeMachine : TimeMachine<ITransform>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformTimeMachine"/> class.
        /// </summary>
        /// <param name="history">
        /// The amount of history to store in this time machine.
        /// </param>
        public TransformTimeMachine(int history)
            : base(history)
        {
        }

        public sealed override ITransform Get(int tick)
        {
            int previousTick, nextTick;

            this.FindSurroundingTickValues(
                this.KnownKeys,
                tick,
                out previousTick,
                out nextTick);

            if (previousTick != -1)
            {
                previousTick = this.KnownKeys[previousTick];
            }

            if (nextTick != -1)
            {
                nextTick = this.KnownKeys[nextTick];
            }

            if (previousTick != -1 && nextTick != -1)
            {
                // If they are the same, skip and return the value.
                if (previousTick == nextTick)
                {
                    return this.KnownValues[previousTick];
                }

                // Construct a new transform based on linearly interpolating
                // the values within the transform.
                var previousValue = this.KnownValues[previousTick];
                var nextValue = this.KnownValues[nextTick];

                if (previousValue.IsSRTMatrix != nextValue.IsSRTMatrix)
                {
                    // We can't interpolate this.
                    return this.KnownValues[previousTick];
                }

                var ratio = (tick - previousTick) / (nextTick - (float)previousTick);

                if (previousValue.IsSRTMatrix && nextValue.IsSRTMatrix)
                {
                    // Interpolate based on local position, rotation and scale.
                    return new DefaultTransform
                    {
                        LocalPosition = Vector3.Lerp(previousValue.LocalPosition, nextValue.LocalPosition, ratio),
                        LocalRotation = Quaternion.Lerp(previousValue.LocalRotation, nextValue.LocalRotation, ratio),
                        LocalScale = Vector3.Lerp(previousValue.LocalScale, nextValue.LocalScale, ratio),
                    };
                }

                if (!previousValue.IsSRTMatrix && !nextValue.IsSRTMatrix)
                {
                    // Interpolate using the custom matrix.  This will probably give a nonsensical
                    // result in most cases?
                    var transform = new DefaultTransform();
                    transform.SetFromCustomMatrix(Matrix.Lerp(previousValue.LocalMatrix, nextValue.LocalMatrix, ratio));
                    return transform;
                }
            }

            if (previousTick != -1 && nextTick == -1)
            {
                // Return the previous value and don't attempt to predict the future.
                return this.KnownValues[previousTick];
            }

            if (nextTick == -1 && previousTick != -1)
            {
                // TODO: Extrapolation
                return this.KnownValues[previousTick];
            }
            
            return new DefaultTransform();
        }
    }
}