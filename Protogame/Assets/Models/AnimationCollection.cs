namespace Protogame
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An implementation of <see cref="IAnimationCollection"/>.
    /// </summary>
    public class AnimationCollection : IAnimationCollection
    {
        /// <summary>
        /// The dictionary that maps animation names to animations.
        /// </summary>
        private readonly Dictionary<string, IAnimation> m_Dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationCollection"/> class.
        /// </summary>
        /// <param name="animations">
        /// The animations.
        /// </param>
        public AnimationCollection(IEnumerable<IAnimation> animations)
        {
            this.m_Dictionary = animations.ToDictionary(key => key.Name, value => value);
        }

        /// <summary>
        /// Retrieve an animation by the animation's name.
        /// </summary>
        /// <param name="name">
        /// The name of the animation.
        /// </param>
        /// <returns>
        /// The <see cref="IAnimation"/>, or null if there's no animation with this name.
        /// </returns>
        public IAnimation this[string name]
        {
            get
            {
                if (!this.m_Dictionary.ContainsKey(name))
                {
                    return null;
                }

                return this.m_Dictionary[name];
            }
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        public IEnumerator<IAnimation> GetEnumerator()
        {
            return this.m_Dictionary.Values.GetEnumerator();
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.m_Dictionary.Values.GetEnumerator();
        }
    }
}