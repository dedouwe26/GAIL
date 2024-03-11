using Silk.NET.GLFW;

namespace GAIL.Input
{
    /// <summary>
    /// A key on a keyboard or mouse.
    /// </summary>
    public enum Key {
        /// <summary>
        /// A key that is not known to GLFW.
        /// </summary>
        Unknown = Keys.Unknown,
        A = Keys.A,
        B = Keys.B,
        C = Keys.C,
        D = Keys.D,
        E = Keys.E,
        F = Keys.F,
        G = Keys.G,
        H = Keys.H,
        I = Keys.I,
        J = Keys.J,
        K = Keys.K,
        L = Keys.L,
        M = Keys.M,
        N = Keys.N,
        O = Keys.O,
        P = Keys.P,
        Q = Keys.Q,
        R = Keys.R,
        S = Keys.S,
        T = Keys.T,
        U = Keys.U,
        V = Keys.V,
        W = Keys.W,
        X = Keys.X,
        Y = Keys.Y,
        Z = Keys.Z,
        /// <summary>
        /// AKA esc
        /// </summary>
        ESCAPE = Keys.Escape,
        F1 = Keys.F1,
        F2 = Keys.F2,
        F3 = Keys.F3,
        F4 = Keys.F4,
        F5 = Keys.F5,
        F6 = Keys.F6,
        F7 = Keys.F7,
        F8 = Keys.F8,
        F9 = Keys.F9,
        F10 = Keys.F10,
        F11 = Keys.F11,
        F12 = Keys.F12,
        F13 = Keys.F13, 
        F14 = Keys.F14,
        F15 = Keys.F15,
        F16 = Keys.F16,
        F17 = Keys.F17,
        F18 = Keys.F18,
        F19 = Keys.F19,
        F20 = Keys.F20,
        F21 = Keys.F21,
        F22 = Keys.F22,
        F23 = Keys.F23,
        F24 = Keys.F24,
        /// <summary>
        /// ;)
        /// </summary>
        F25 = Keys.F25,
        /// <summary>
        /// Print Screen
        /// </summary>
        PrintScreen = Keys.PrintScreen,
        /// <summary>
        /// Scroll Lock
        /// </summary>
        ScrollLock = Keys.ScrollLock,
        /// <summary>
        /// Pause Break
        /// </summary>
        Pause = Keys.Pause,
        /// <summary>
        /// (`)
        /// </summary>
        GraveAccent = Keys.GraveAccent,
        Number1 = Keys.Number1,
        Number2 = Keys.Number2,
        Number3 = Keys.Number3,
        Number4 = Keys.Number4,
        Number5 = Keys.Number5,
        Number6 = Keys.Number6,
        Number7 = Keys.Number7,
        Number8 = Keys.Number8,
        Number9 = Keys.Number9,
        Number0 = Keys.Number0,
        /// <summary>
        /// (-)
        /// </summary>
        Minus = Keys.Minus,
        /// <summary>
        /// (=)
        /// </summary>
        Equals = Keys.Equal,
        Backspace = Keys.Backspace, 
        Tab = Keys.Tab,
        /// <summary>
        /// ([)
        /// </summary>
        LeftBracket = Keys.LeftBracket,
        /// <summary>
        /// (])
        /// </summary>
        RightBracket = Keys.RightBracket,
        Backslash = Keys.BackSlash,
        /// <summary>
        /// AKA caps
        /// </summary>
        CapsLock = Keys.CapsLock,
        /// <summary>
        /// (;)
        /// </summary>
        Semicolon = Keys.Semicolon,
        /// <summary>
        /// (')
        /// </summary>
        Apostrophe = Keys.Apostrophe,
        /// <summary>
        /// AKA return.
        /// </summary>
        Enter = Keys.Enter,
        LeftShift = Keys.ShiftLeft,
        Comma = Keys.Comma,
        PERIOD = Keys.Period,
        Slash = Keys.Slash,
        RightShift = Keys.ShiftRight,
        /// <summary>
        /// AKA ctrl.
        /// </summary>
        LeftControl = Keys.ControlLeft,
        LeftAlt = Keys.AltLeft,
        Space = Keys.Space,
        RightAlt = Keys.AltRight,
        /// <summary>
        /// or context menu key.
        /// </summary>
        Menu = Keys.Menu,
        /// <summary>
        /// AKA ctrl.
        /// </summary>
        RightControl = Keys.ControlRight,
        /// <summary>
        /// like left Windows key.
        /// </summary>
        LeftSuper = Keys.SuperLeft,
        /// <summary>
        /// like right Windows key.
        /// </summary>
        RightSuper = Keys.SuperRight,
        Insert = Keys.Insert,
        Home = Keys.Home,
        PageUp = Keys.PageUp,
        PageDown = Keys.PageDown,
        Delete = Keys.Delete,
        End = Keys.End,
        ArrowUp = Keys.Up,
        ArrowLeft = Keys.Left,
        ArrowDown = Keys.Down,
        ArrowRight = Keys.Right,
        NumLock = Keys.NumLock,
        /// <summary>
        /// (/)
        /// </summary>
        NumpadDivide = Keys.KeypadDivide,
        /// <summary>
        /// (*)
        /// </summary>
        NumpadMultiply = Keys.KeypadMultiply,
        /// <summary>
        /// (-)
        /// </summary>
        NumpadSubstract = Keys.KeypadSubtract,
        /// <summary>
        /// Numpad 0
        /// </summary>
        Numpad0 = Keys.Keypad0,
        /// <summary>
        /// Numpad 1
        /// </summary>
        Numpad1 = Keys.Keypad1,
        /// <summary>
        /// Numpad 2
        /// </summary>
        Numpad2 = Keys.Keypad2,
        /// <summary>
        /// Numpad 3
        /// </summary>
        Numpad3 = Keys.Keypad3,
        /// <summary>
        /// Numpad 4
        /// </summary>
        Numpad4 = Keys.Keypad4,
        /// <summary>
        /// Numpad 5
        /// </summary>
        Numpad5 = Keys.Keypad5,
        /// <summary>
        /// Numpad 6
        /// </summary>
        Numpad6 = Keys.Keypad6,
        /// <summary>
        /// Numpad 7
        /// </summary>
        Numpad7 = Keys.Keypad7,
        /// <summary>
        /// Numpad 8
        /// </summary>
        Numpad8 = Keys.Keypad8,
        /// <summary>
        /// Numpad 9
        /// </summary>
        Numpad9 = Keys.Keypad9,
        /// <summary>
        /// (+)
        /// </summary>
        NumpadAdd = Keys.KeypadAdd,
        NumpadEnter = Keys.KeypadEnter,
        /// <summary>
        /// (.)
        /// </summary>
        NumpadDecimal = Keys.KeypadDecimal,
        /// <summary>
        /// (=)
        /// </summary>
        NumpadEquals = Keys.KeypadEqual,

        /// <summary>
        /// AKA MOUSE_1
        /// </summary>
        MOUSE_LEFT = MouseButton.Left, 
        /// <summary>
        /// AKA MOUSE_2
        /// </summary>
        MOUSE_RIGHT = MouseButton.Right,
        /// <summary>
        /// AKA MOUSE_3
        /// </summary>
        MOUSE_MIDDLE = MouseButton.Middle,
        Mouse4 = MouseButton.Button4,
        Mouse5 = MouseButton.Button5,
        Mouse6 = MouseButton.Button6,
        Mouse7 = MouseButton.Button7,
        Mouse8 = MouseButton.Button8,
        /// <summary>
        /// World key for foreign languages (non-US).
        /// </summary>
        WORLD_1 = Keys.World1,
        /// <summary>
        /// World key for foreign languages (non-US).
        /// </summary>
        WORLD_2 = Keys.World2
    }
}