namespace GAIL
{
    /// <summary>
    /// The update callback.
    /// </summary>
    public delegate void UpdateCallback(Application app, double deltaTime);
    /// <summary>
    /// The load callback.
    /// </summary>
    public delegate void LoadCallback(Application app);
    /// <summary>
    /// The stop callback.
    /// </summary>
    public delegate void StopCallback(Application app);
}