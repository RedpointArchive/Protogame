namespace Protogame.ATFLevelEditor
{
    /// <summary>
    /// An enumeration which defines whether the editor query interface is being used
    /// to query information about the entity for baking the schema, or whether it is
    /// being used at runtime to provide the entity with information that is defined
    /// in the level.
    /// </summary>
    /// <module>Editor</module>
    public enum EditorQueryMode
    {
        BakingSchema,

        LoadingConfiguration,

        ManuallySpawned,
    }
}