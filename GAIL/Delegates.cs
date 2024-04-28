namespace GAIL
{
    /// <summary>
    /// The update callback (calls every frame).
    /// </summary>
    /// <param name="app">The current application.</param>
    /// <param name="deltaTime">The time between frames in seconds (CurrentTime - PreviousFrameTime).</param>
    public delegate void UpdateCallback(Application app, double deltaTime);
    /// <summary>
    /// The load callback (calls at the start).
    /// </summary>
    /// <param name="app">The current application.</param>
    public delegate void LoadCallback(Application app);
    /// <summary>
    /// The stop callback (calls at disposal).
    /// </summary>
    /// <param name="app">The current application.</param>
    public delegate void StopCallback(Application app);
}