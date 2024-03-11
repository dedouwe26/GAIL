using System.Numerics;

namespace GAIL.Input
{
    /// <summary>
    /// Callback for when a key is pressed.
    /// </summary>
    /// <param name="key">The key that is pressed.</param>
    public delegate void KeyDownCallback(Key key);
    /// <summary>
    /// Callback for when a key is released.
    /// </summary>
    /// <param name="key">The key that is released.</param>
    public delegate void KeyUpCallback(Key key);
    /// <summary>
    /// Callback for when a key is repeated (held down).
    /// </summary>
    /// <param name="key">The key that is repeated.</param>
    public delegate void KeyRepeatCallback(Key key);
    /// <summary>
    /// Callback for when the mouse has moved.
    /// </summary>
    /// <param name="pos"></param>
    public delegate void MouseMovedCallback(Vector2 pos);
    /// <summary>
    /// Callback for when the mouse scrolled.
    /// </summary>
    /// <param name="offset">How much it scrolled.</param>
    public delegate void ScrollCallback(Vector2 offset);
    /// <summary>
    /// Callback for when the window resized.
    /// </summary>
    /// <param name="width">The new width.</param>
    /// <param name="height">The new height.</param>
    /// <param name="maximized">If it was maximized.</param>
    /// <param name="minimized">If it was minimized.</param>
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
}