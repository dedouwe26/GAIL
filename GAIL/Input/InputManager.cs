using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Runtime.InteropServices;
using GAIL.Core;
using Silk.NET.GLFW;

namespace GAIL.Input
{
    /// <summary>
    /// Handles all input in the Application.
    /// </summary>
    public class InputManager : IManager {
        /// <summary>
        /// Event for when a key is pressed.
        /// </summary>
        public event KeyDownCallback? OnKeyDown;
        /// <summary>
        /// Event for when a key is released.
        /// </summary>
        public event KeyUpCallback? OnKeyUp;
        /// <summary>
        /// Event for when a key is repeated (held down).
        /// </summary>
        public event KeyRepeatCallback? OnKeyRepeat;
        /// <summary>
        /// Event for when the mouse has moved.
        /// </summary>
        public event MouseMovedCallback? OnMouseMoved;
        /// <summary>
        /// Event for when the window resized.
        /// </summary>
        public event ScrollCallback? OnScroll;
        /// <summary>
        /// Event for when the window resized.
        /// </summary>
        public event WindowResizeCallback? OnWindowResize;
        /// <summary>
        /// Event for when the window moved.
        /// </summary>
        public event WindowMoveCallback? OnWindowMove;
        /// <summary>
        /// Event for when a file(s) has been dropped onto the window.
        /// </summary>
        public event PathDropCallback? OnPathDrop;
        /// <summary>
        /// The GLFW API instance for custom usage.
        /// </summary>
        public readonly Glfw glfw;

        private Application.Globals globals;

        public InputManager(Application.Globals globals) {
            glfw = Glfw.GetApi();
            this.globals = globals;
        }

        /// <summary>
        /// Initializes the input manager.
        /// </summary>
        public void Init() {
            unsafe {
                glfw.SetKeyCallback(globals.windowManager.Window, 
                    (WindowHandle* window, Keys key, int scanCode, InputAction action, KeyModifiers mods) => {
                        // OnKeyDown?.Invoke((Key)key);
                        if (action == InputAction.Press) {
                            OnKeyDown?.Invoke((Key)key);
                        } else if (action == InputAction.Release) {
                            OnKeyUp?.Invoke((Key)key);
                        } else if (action == InputAction.Repeat) {
                            OnKeyRepeat?.Invoke((Key)key);
                        }
                    }
                );
                glfw.SetCursorPosCallback(globals.windowManager.Window,
                    (WindowHandle* window, double xpos, double ypos) => {
                        OnMouseMoved?.Invoke(new Vector2((float)xpos, (float)ypos));
                    }
                );
                glfw.SetScrollCallback(globals.windowManager.Window,
                    (WindowHandle* window, double xoffset, double yoffset) => {
                        OnScroll?.Invoke(new Vector2((float)xoffset, (float)yoffset));
                    }
                );
                glfw.SetWindowPosCallback(globals.windowManager.Window,
                    (WindowHandle* window, int xpos, int ypos) => {
                        OnWindowMove?.Invoke(xpos, ypos);
                    }
                );
                glfw.SetWindowSizeCallback(globals.windowManager.Window,
                    (WindowHandle* window, int width, int height) => {
                        OnWindowResize?.Invoke(width, height, 0, 0);
                    }
                );
                glfw.SetWindowIconifyCallback(globals.windowManager.Window,
                    (WindowHandle* window, bool iconified) => {
                        OnWindowResize?.Invoke(0, 0, 0, (byte)(iconified ? 1 : 2));
                    }
                );
                glfw.SetWindowMaximizeCallback(globals.windowManager.Window,
                    (WindowHandle* window, bool maximized) => {
                        OnWindowResize?.Invoke(0, 0, (byte)(maximized? 1 : 2), 0);
                    }
                );
                glfw.SetDropCallback(globals.windowManager.Window, 
                    (WindowHandle* window, int count, nint paths) => {
                        List<string> list = [];
                        for ( int i = 0; i < count; i++ ) {
                            nint? strPtr = (nint?)Marshal.PtrToStructure(paths, typeof(nint));
                            if (strPtr == null) { continue; }
                            list.Add(Marshal.PtrToStringUTF8(strPtr.Value)!);
                            paths = new IntPtr(paths.ToInt64()+IntPtr.Size);
                        }
                        OnPathDrop?.Invoke(list);
                    }
                );
            }
        }
        ~InputManager() {
            Dispose();
        }
        public void Dispose() {
            glfw.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Converts a key to its name (printable character).
        /// </summary>
        /// <param name="key">The key to convert</param>
        /// <returns>The name (printable character).</returns>
        public string ToName(Key key) {
            return glfw.GetKeyName((int)key, ToScanCode(key));
        }
        /// <summary>
        /// Converts a key to its platform-specific scan code.
        /// </summary>
        /// <param name="key">The key to convert.</param>
        /// <returns>The platform-specific scan code.</returns>
        public int ToScanCode(Key key) {
            return glfw.GetKeyScancode((int)key);
        }
        /// <summary>
        /// Returns whether the given key is pressed.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Whether the given key is pressed or not.</returns>
        public bool IsKeyPressed(Key key) {
            unsafe {
                return glfw.GetKey(globals.windowManager.Window, (Keys)(int)key) == (int)InputAction.Press;
            }
        }
        /// <summary>
        /// Checks if the given mouse button is pressed.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>Whether the given mouse button is pressed or not.</returns>
        public bool IsMouseButtonPressed(MouseButton button) {
            unsafe {
                return glfw.GetMouseButton(globals.windowManager.Window, (int)button) == (int)InputAction.Press;
            }
        }
        /// <summary>
        /// Get the position of the mouse (relative to the top-left side of the window).
        /// </summary>
        /// <returns>The position of the mouse in pixels (relative to the top-left side of the window).</returns>
        public Vector2 GetMousePosition() {
            unsafe {
                glfw.GetCursorPos(globals.windowManager.Window, out double x, out double y);
                Vector2 mousePos = new((float)x, (float)y);
                return mousePos;
            }
            
        }
        /// <summary>
        /// If the mouse is locked and hidden (you can still get input).
        /// </summary>
        public bool MouseLocked {
            get {
                unsafe {
                    return glfw.GetInputMode(globals.windowManager.Window, CursorStateAttribute.Cursor) == (int)CursorModeValue.CursorDisabled;
                }
            } set {
                unsafe {
                    glfw.SetInputMode(globals.windowManager.Window, CursorStateAttribute.Cursor, value ? CursorModeValue.CursorDisabled : CursorModeValue.CursorNormal);
                }
            }
        }
        /// <summary>
        /// Sets the size of the window, optionally maximized and/or minimized.
        /// </summary>
        /// <param name="width">The new width of the window (pixels).</param>
        /// <param name="height">The new height of the window (pixels).</param>
        /// <param name="maximized">If it is maximized.</param>
        /// <param name="minimized">If it is minimized.</param>
        public void SetWindowSize(int width, int height, bool maximized = false, bool minimized = false) {
            unsafe {
                glfw.SetWindowSize(globals.windowManager.Window, width, height);
                if (maximized) { glfw.MaximizeWindow(globals.windowManager.Window); }
                if (minimized) { glfw.IconifyWindow(globals.windowManager.Window); }
            }
            
        }
        /// <summary>
        /// Sets the window position to the given coordinates.
        /// </summary>
        /// <param name="x">The x (horizontal) position (pixels).</param>
        /// <param name="y">The y (vertical) position (pixels).</param>
        public void SetWindowPosition(int x, int y) {
            unsafe {
                glfw.SetWindowSize(globals.windowManager.Window, x, y);
            }
        }
        /// <summary>
        /// Sets the window title to the given string.
        /// </summary>
        /// <param name="newTitle">The new title name.</param>
        public void SetWindowTitle(string newTitle) {
            unsafe {
                glfw.SetWindowTitle(globals.windowManager.Window, newTitle);
            }
        }
        /// <summary>
        /// Sets the window icon to the given image ().
        /// </summary>
        /// <param name="newIcon"></param>
        public void SetWindowIcon(List<Texture> newIcon) {
            unsafe {
                fixed (Image* ptr = newIcon.Select(x => x.ToGLFWRGB()).ToArray()) {
                    glfw.SetWindowIcon(globals.windowManager.Window, 1, ptr);
                }
            }
        }
    }
}