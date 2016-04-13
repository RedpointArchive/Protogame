namespace Protogame
{
    using System;

    /// <summary>
    /// The asset attribute.
    /// </summary>
    public class AssetAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public AssetAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }
    }
}