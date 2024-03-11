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

        private Application.Globals globals;

        public InputManager(Application.Globals globals) {
            this.globals = globals;
            Glfw glfw = Glfw.GetApi();

            unsafe {
                glfw.SetKeyCallback(globals.windowManager.window, 
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
            }
        }
        ~InputManager() {
            Dispose();
        }
        public void Dispose() {
            
        }

        public char ToChar(Key key) {

        }
        public int ToScanCode(Key key) {

        }

        public bool IsKeyPressed(Key key) {

        }
        public Vector2 GetMousePosition() {

        }
        public bool MouseLocked {get {return MouseLocked;} set {
            MouseLocked = value;
        }}
        public void SetWindowSize(int width, int height, bool maximized, bool minimized) {

        }
        public void SetWindowPosition(int x, int y) {
            
        }
        public void SetWindowTitle(string newTitle) {

        }
        public void SetWindowIcon(List<Texture> newIcon) {

        }
    }
}