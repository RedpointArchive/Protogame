namespace Protogame
{
    using System;

    /// <summary>
    /// The level asset.
    /// </summary>
    public class LevelAsset : MarshalByRefObject, IAsset
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelAsset"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="levelData">
        /// The raw level data.
        /// </param>
        /// <param name="levelDataFormat">
        /// The format of the level data.
        /// </param>
        /// <param name="sourcePath">
        /// The source path.
        /// </param>
        public LevelAsset(string name, string levelData, LevelDataFormat levelDataFormat, string sourcePath)
        {
            this.Name = name;
            this.LevelData = levelData;
            this.LevelDataFormat = levelDataFormat;
            this.SourcePath = sourcePath;
        }

        /// <summary>
        /// Gets a value indicating whether compiled only.
        /// </summary>
        /// <value>
        /// The compiled only.
        /// </value>
        public bool CompiledOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether source only.
        /// </summary>
        /// <value>
        /// The source only.
        /// </value>
        public bool SourceOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the source path.
        /// </summary>
        /// <value>
        /// The source path.
        /// </value>
        public string SourcePath { get; set; }

        /// <summary>
        /// Gets or sets the level data.
        /// </summary>
        /// <value>
        /// The level data.
        /// </value>
        [Obsolete("Use LevelData instead.", true)]
        public string Value
        {
            get { return LevelData; }
            set { LevelData = value; }
        }

        /// <summary>
        /// Gets or sets the level data.
        /// </summary>
        /// <value>
        /// The level data.
        /// </value>
        public string LevelData { get; set; }

        /// <summary>
        /// Gets or sets the format of the level data.
        /// </summary>
        public LevelDataFormat LevelDataFormat { get; set; }

        /// <summary>
        /// The resolve.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(LevelAsset)))
            {
                return this as T;
            }

            throw new InvalidOperationException("Asset already resolved to LevelAsset.");
        }
    }
}