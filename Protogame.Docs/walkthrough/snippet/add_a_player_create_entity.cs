public MyGameWorld(
    I2DRenderUtilities twoDRenderUtilities,
    IAssetManagerProvider assetManagerProvider,
    IEntityFactory entityFactory)
{
    this.Entities = new List<IEntity>();

    _renderUtilities = twoDRenderUtilities;
    _assetManager = assetManagerProvider.GetAssetManager();
    _defaultFont = this._assetManager.Get<FontAsset>("font.Default");

    // You can also save the entity factory in a field and use it, e.g. in the Update
    // loop or anywhere else in your game.
    var entityA = entityFactory.CreateExampleEntity("EntityA");
    entityA.X = 100;
    entityA.Y = 50;
    var entityB = entityFactory.CreateExampleEntity("EntityB");
    entityB.X = 120;
    entityB.Y = 100;

    // Don't forget to add your entities to the world!
    this.Entities.Add(entityA);
    this.Entities.Add(entityB);

    // This pulls in the texture asset via the asset manager.  Note that
    // the folder seperator from "texture/Player" has been translated
    // into a single dot.
    _playerTexture = _assetManager.Get<TextureAsset>("texture.Player");

    // Add the player entity.
    this.Entities.Add(entityFactory.CreatePlayerEntity());
}