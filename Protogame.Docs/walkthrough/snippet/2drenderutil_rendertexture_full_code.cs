public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
{
    gameContext.Graphics.GraphicsDevice.Clear(Color.Purple);

    _renderUtilities.RenderText(
        renderContext,
        new Vector2(10, 10),
        "Hello MyGame!",
        this._defaultFont);

    _renderUtilities.RenderText(
        renderContext,
        new Vector2(10, 30),
        "Running at " + gameContext.FPS + " FPS; " + gameContext.FrameCount + " frames counted so far",
        this._defaultFont);

    _renderUtilities.RenderTexture(
        renderContext,
        new Vector2(100, 200),
        _playerTexture);
}