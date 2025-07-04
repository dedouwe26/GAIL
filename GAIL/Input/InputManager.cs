using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GAIL.Core;
using LambdaKit.Logging;
using Silk.NET.GLFW;

namespace GAIL.Input
{
    /// <summary>
    /// Handles all input in the Application.
    /// </summary>
    public class InputManager {
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
        /// Event for when a mouse button is pressed.
        /// </summary>
        public event MouseButtonDownCallback? OnMouseButtonDown;
        /// <summary>
        /// Event for when a mouse button is released.
        /// </summary>
        public event MouseButtonUpCallback? OnMouseButtonUp;
        /// <summary>
        /// Event for when the window resized.
        /// </summary>
        public event ScrollCallback? OnScroll;

        private Application.Globals globals;

        /// <summary>
        /// The logger corresponding to the graphics part of the application.
        /// </summary>
        public readonly Logger Logger;

        /// <summary>
        /// Creates an input manager.
        /// </summary>
        /// <param name="globals">The globals for this application.</param>
        /// <param name="logger">The logger to use.</param>
        public InputManager(Application.Globals globals, Logger logger) {
            Logger = logger;
            this.globals = globals;
        }

        /// <summary>
        /// Initializes the input manager.
        /// </summary>
        public void Initialize() {
            Logger.LogDebug("Setting all GLFW callbacks.");
            unsafe {
                API.Glfw.SetKeyCallback(globals.windowManager.Window, 
                    (WindowHandle* window, Keys key, int scanCode, InputAction action, KeyModifiers mods) => {
                        if (action == InputAction.Press) {
                            OnKeyDown?.Invoke((Key)key);
                        } else if (action == InputAction.Release) {
                            OnKeyUp?.Invoke((Key)key);
                        } else if (action == InputAction.Repeat) {
                            OnKeyRepeat?.Invoke((Key)key);
                        }
                    }
                );
                API.Glfw.SetMouseButtonCallback(globals.windowManager.Window,
                    (WindowHandle* window, Silk.NET.GLFW.MouseButton button, InputAction action, KeyModifiers mods) => {
                        if (action == InputAction.Press) {
                            OnMouseButtonDown?.Invoke((MouseButton)button);
                        } else if (action == InputAction.Release) {
                            OnMouseButtonUp?.Invoke((MouseButton)button);
                        }
                    }
                );
                API.Glfw.SetCursorPosCallback(globals.windowManager.Window,
                    (WindowHandle* window, double xpos, double ypos) => {
                        OnMouseMoved?.Invoke(new Vector2((float)xpos, (float)ypos));
                    }
                );
                API.Glfw.SetScrollCallback(globals.windowManager.Window,
                    (WindowHandle* window, double xoffset, double yoffset) => {
                        OnScroll?.Invoke(new Vector2((float)xoffset, (float)yoffset));
                    }
                );
                
            }
        }

        /// <summary>
        /// Converts a key to its name (printable character).
        /// </summary>
        /// <param name="key">The key to convert</param>
        /// <returns>The name (printable character).</returns>
        public static string ToName(Key key) {
            return API.Glfw.GetKeyName((int)key, ToScanCode(key));
        }
        /// <summary>
        /// Converts a key to its platform-specific scan code.
        /// </summary>
        /// <param name="key">The key to convert.</param>
        /// <returns>The platform-specific scan code.</returns>
        public static int ToScanCode(Key key) {
            return API.Glfw.GetKeyScancode((int)key);
        }
        /// <summary>
        /// Returns whether the given key is pressed.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Whether the given key is pressed or not.</returns>
        public bool IsKeyPressed(Key key) {
            unsafe {
                int state = API.Glfw.GetKey(globals.windowManager.Window, (Keys)(int)key);
                return state == (int)InputAction.Press || state == (int)InputAction.Release;
            }
        }
        /// <summary>
        /// Checks if the given mouse button is pressed.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>Whether the given mouse button is pressed or not.</returns>
        public bool IsMouseButtonPressed(MouseButton button) {
            unsafe {
                return API.Glfw.GetMouseButton(globals.windowManager.Window, (int)button) == (int)InputAction.Press;
            }
        }
        /// <summary>
        /// Get the position of the cursor (relative to the top-left side of the window).
        /// </summary>
        /// <returns>The position of the cursor in pixels (relative to the top-left side of the window).</returns>
        public Vector2 GetCursorPosition() {
            unsafe {
                API.Glfw.GetCursorPos(globals.windowManager.Window, out double x, out double y);
                Vector2 cursorPos = new((float)x, (float)y);
                return cursorPos;
            }
            
            
        }
        /// <summary>
        /// If the cursor is locked and hidden (you can still get input).
        /// </summary>
        public bool CursorLocked {
            get {
                unsafe {
                    return API.Glfw.GetInputMode(globals.windowManager.Window, CursorStateAttribute.Cursor) == (int)CursorModeValue.CursorDisabled;
                }
            } set {
                unsafe {
                    API.Glfw.SetInputMode(globals.windowManager.Window, CursorStateAttribute.Cursor, value ? CursorModeValue.CursorDisabled : CursorModeValue.CursorNormal);
                }
            }
        }
    }
}