namespace Protogame
{
    using System;
    
    public class UserInterfaceAsset : MarshalByRefObject, IAsset
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserInterfaceAsset"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="userInterfaceData">
        /// The raw user interface.
        /// </param>
        /// <param name="userInterfaceFormat">
        /// The format of the user interface.
        /// </param>
        /// <param name="sourcePath">
        /// The source path.
        /// </param>
        public UserInterfaceAsset(string name, string userInterfaceData, UserInterfaceFormat userInterfaceFormat, string sourcePath)
        {
            this.Name = name;
            this.UserInterfaceData = userInterfaceData;
            this.UserInterfaceFormat = userInterfaceFormat;
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
        /// Gets or sets the user interface data.
        /// </summary>
        /// <value>
        /// The user interface data.
        /// </value>
        public string UserInterfaceData { get; set; }

        /// <summary>
        /// Gets or sets the format of the user interface.
        /// </summary>
        public UserInterfaceFormat UserInterfaceFormat { get; set; }
    }
}