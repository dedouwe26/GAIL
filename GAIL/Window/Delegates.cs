namespace GAIL.Window;

/// <summary>
/// Callback for when the window resized.
/// </summary>
/// <param name="width">The new width.</param>
/// <param name="height">The new height.</param>
/// <param name="maximized">If it was maximized (0=nothing, 1=maximized, 2=restored).</param>
/// <param name="minimized">If it was minimized (0=nothing, 1=minimized, 2=restored).</param>
public delegate void WindowResizeCallback(int width, int height, byte maximized, byte minimized);
/// <summary>
/// Callback for when the window moved.
/// </summary>
/// <param name="x">The new x (horizontal) position.</param>
/// <param name="y">The new y (vertical) position.</param>
public delegate void WindowMoveCallback(int x, int y);
/// <summary>
/// Callback for when a file(s) has been dropped onto the window.
/// </summary>
/// <param name="paths">A list of all the paths to the files.</param>
public delegate void PathDropCallback(List<string> paths);