// This is our new counter field that we increment every frame.
private float m_Counter = 0;

public void Update(IGameContext gameContext, IUpdateContext updateContext)
{
    // Increments the counter.
    this.m_Counter++;

    // Adjust the X and Y position so that the X position deviates 50
    // pixels either side of 300 as the game updates.
    this.X = 300 + (float)Math.Sin(MathHelper.ToRadians(this.m_Counter)) * 50;
    this.Y = 300;
}