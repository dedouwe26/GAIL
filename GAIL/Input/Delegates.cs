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
    /// Callback for when a mouse button is released.
    /// </summary>
    /// <param name="button">The button that is released.</param>
    public delegate void MouseButtonUpCallback(MouseButton button);
    /// <summary>
    /// Callback for when a mouse button is pressed.
    /// </summary>
    /// <param name="button">The button that is pressed.</param>
    public delegate void MouseButtonDownCallback(MouseButton button);
    /// <summary>
    /// Callback for when the mouse scrolled.
    /// </summary>
    /// <param name="offset">How much it scrolled.</param>
    public delegate void ScrollCallback(Vector2 offset);
}