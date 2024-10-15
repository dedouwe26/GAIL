using System.Runtime.InteropServices;
using GAIL.Core;
using OxDED.Terminal.Logging;
using Silk.NET.GLFW;

namespace GAIL.Window
{
    /// <summary>
    /// Handles everything for the window (GLFW).
    /// </summary>
    public class WindowManager : IDisposable {
        /// <summary>
        /// If this manager is already disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }
        /// <summary>
        /// The logger corresponding to the graphics part of the application.
        /// </summary>
        public readonly Logger Logger;

        /// <summary>
        /// Event for when the framebuffer has resized.
        /// </summary>
        public event FramebufferResizeCallback? OnFramebufferResize;
        /// <summary>
        /// Event for when the window has resized.
        /// </summary>
        public event WindowResizeCallback? OnWindowResize;
        /// <summary>
        /// Event for when the window has moved.
        /// </summary>
        public event WindowMoveCallback? OnWindowMove;
        /// <summary>
        /// Event for when a file(s) has been dropped onto the window.
        /// </summary>
        public event PathDropCallback? OnPathDrop;

        /// <summary>
        /// Creates a window manager.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <exception cref="APIBackendException"></exception>
        public WindowManager(Logger logger) {
            Logger = logger;
            API.Glfw.SetErrorCallback((ErrorCode error, string description) => {
                Logger.LogError($"GLFW: ({error}): {description}");
                throw new APIBackendException("GLFW", $"({error}): {description}");
            });
        }

        /// <summary>
        /// Initializes the window manager.
        /// </summary>
        /// <param name="title">The window name.</param>
        /// <param name="width">The width of the window (horizontal).</param>
        /// <param name="height">The height of the window (vertical).</param>
        /// <exception cref="APIBackendException"></exception>
        public void Init(string title, int width, int height) {

            if (!API.Glfw.Init())
            {
                throw new APIBackendException("GLFW", "initialization failed!");
            }

            Logger.LogDebug("Creating window.");

            API.Glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);

            unsafe {
                Window = API.Glfw.CreateWindow(width, height, title, null, null);
            }
            if (Window.IsNull) {
                Logger.LogFatal("GLFW: Window creation failed!");
                throw new APIBackendException("GLFW", "window creation failed!");
            }
            unsafe {
                
                API.Glfw.SetWindowPosCallback(Window,
                    (WindowHandle* window, int xpos, int ypos) => {
                        OnWindowMove?.Invoke(xpos, ypos);
                    }
                );
                API.Glfw.SetWindowSizeCallback(Window,
                    (WindowHandle* window, int width, int height) => {
                        OnWindowResize?.Invoke(width, height, 0, 0);
                    }
                );
                API.Glfw.SetFramebufferSizeCallback(Window,
                    (WindowHandle* window, int width, int height) => {
                        OnFramebufferResize?.Invoke(width, height);
                    }
                );
                API.Glfw.SetWindowIconifyCallback(Window,
                    (WindowHandle* window, bool iconified) => {
                        OnWindowResize?.Invoke(0, 0, 0, (byte)(iconified ? 1 : 2));
                    }
                );
                API.Glfw.SetWindowMaximizeCallback(Window,
                    (WindowHandle* window, bool maximized) => {
                        OnWindowResize?.Invoke(0, 0, (byte)(maximized? 1 : 2), 0);
                    }
                );
                API.Glfw.SetDropCallback(Window, 
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

        /// <summary>
        /// The GLFW window instance for custom usage.
        /// </summary>
        public Pointer<WindowHandle> Window { get; private set; } = Pointer<WindowHandle>.FromNull();

        /// <summary>
        /// Updates GLFW (polls events).
        /// </summary>
        public void Update() {
            API.Glfw.PollEvents();
        }

        /// <summary>
        /// True if the window should close.
        /// </summary>
        public bool ShouldClose{get{
            unsafe {
                return API.Glfw.WindowShouldClose(Window);
            }
        }}

        /// <summary>
        /// Sets the size of the window, optionally maximized and/or minimized.
        /// </summary>
        /// <param name="width">The new width of the window (pixels).</param>
        /// <param name="height">The new height of the window (pixels).</param>
        /// <param name="maximized">If it is maximized.</param>
        /// <param name="minimized">If it is minimized.</param>
        public void SetWindowSize(int width, int height, bool maximized = false, bool minimized = false) {
            unsafe {
                if (!maximized && !minimized) { API.Glfw.SetWindowSize(Window, width, height); }
                else if (maximized) { API.Glfw.MaximizeWindow(Window); }
                else if (minimized) { API.Glfw.IconifyWindow(Window); }
            }
        }

        /// <summary>
        /// Sets the window position to the given coordinates.
        /// </summary>
        /// <param name="x">The x (horizontal) position (pixels).</param>
        /// <param name="y">The y (vertical) position (pixels).</param>
        public void SetWindowPosition(int x, int y) {
            unsafe {
                API.Glfw.SetWindowSize(Window, x, y);
            }
        }

        /// <summary>
        /// Sets the window title to the given string.
        /// </summary>
        /// <param name="newTitle">The new title name.</param>
        public void SetWindowTitle(string newTitle) {
            unsafe {
                API.Glfw.SetWindowTitle(Window, newTitle);
            }
        }
        
        /// <summary>
        /// Sets the window icon to the given image (list used for scaling purposes).
        /// </summary>
        /// <remarks>
        /// Some recommended sizes include: 16x16, 32x32, 48x48.
        /// </remarks>
        /// <param name="newIcon">The list of Textures with different sizes for scaling purposes.</param>
        public void SetWindowIcon(List<Texture> newIcon) {
            unsafe {
                Image[] icons = [.. newIcon.Select(x => x.ToGLFW())];
                API.Glfw.SetWindowIcon(Window, icons.Length, Pointer<Image>.FromArray(ref icons));
            }
        }
        /// <summary>
        /// Gets / Sets if the window is resizable or not.
        /// </summary>
        public bool Resizable { get {
            unsafe {
                return API.Glfw.GetWindowAttrib(Window, WindowAttributeGetter.Resizable);
            }
        } set {
            unsafe {
                API.Glfw.SetWindowAttrib(Window, WindowAttributeSetter.Resizable, value);
            }
        } }

        /// <summary>
        /// Gets / sets the opacity (alpha value) of the window.
        /// Can only be between 0 and 1.
        /// </summary>
        public float Opacity { get {
            unsafe {
                return API.Glfw.GetWindowOpacity(Window);
            }
        } set {
            unsafe {
                API.Glfw.SetWindowOpacity(Window, Math.Clamp(value, 0f, 1f));
            }
        } }

        // TODO: public bool Decorated {get set}

        /// <summary>
        /// The current time on the GLFW Timer.
        /// </summary>
        public static double Time { get { return API.Glfw.GetTime(); } }

        /// <inheritdoc/>
        public void Dispose() {
            if (IsDisposed) { return; }

            Logger.LogDebug("Terminating GLFW.");
            unsafe {
                API.Glfw.DestroyWindow(Window);
            }
            API.Glfw.Terminate();

            IsDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}