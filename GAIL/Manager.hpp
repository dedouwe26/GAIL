#pragma once

#include "GAIL.hpp"

namespace GAIL
{   

    #pragma region Graphics


    // All the levels of MSAA.
    enum MSAA {
        MSAAx1 = VK_SAMPLE_COUNT_1_BIT, // No MSAA
        MSAAx2 = VK_SAMPLE_COUNT_2_BIT,
        MSAAx4 = VK_SAMPLE_COUNT_4_BIT,
        MSAAx8 = VK_SAMPLE_COUNT_8_BIT,
        MSAAx16 = VK_SAMPLE_COUNT_16_BIT,
        MSAAx32 = VK_SAMPLE_COUNT_32_BIT,
        MSAAx64 = VK_SAMPLE_COUNT_64_BIT
    };

    struct AppInfo;
    class InstancedModel;
    class Model;

    /*
     * This handles all the graphics of GAIL.
     */
    class GraphicsManager
    {
        public:
            // Vulkan Instance
            vk::Instance instance;
            // Vulkan Physical Device
            vk::PhysicalDevice physicalDevice;
            // Vulkan Logical Device
            vk::Device device;

            // Current MSAA size, read-only.
            MSAA MSAASize = MSAA::MSAAx1;

            GraphicsManager(AppInfo info);
            ~GraphicsManager();
            // Returns the max supported MSAA size.
            MSAA GetMaxMSAA();

            // Sets the anti-aliasing (MSAA) to that size. Returns true if successful.
            bool SetMSAA(MSAA MSAASize);

            /*
            Renders the models to the 3D pipeline in the current frame on that layer.
            Returns if successful.
            */
            bool Render3D(std::vector<Model> models);
            /*
            Renders the instanced (Used for models that are rendered multiple times in different places) models to the 3D pipeline in the current frame on that layer.
            Returns if successful.
            */
            bool Render3DInstanced(std::vector<InstancedModel> models);
            /*
            Renders the models to the 2D pipeline (On top of the 3d) in the current frame on that layer.
            Returns if successful.
            */
            bool Render2D(std::vector<Model> models);
            /*
            Renders the instanced (Used for models that are rendered multiple times in different places) models to the 2D pipeline (On top of the 3d) in the current frame on that layer.
            Returns if successful.
            */
            bool Render2DInstanced(std::vector<InstancedModel> models);
            
    };

    #pragma endregion

    #pragma region Audio

    

    // Handles all audio in the Application.
    class AudioManager
    {
        public:
            // Returns all the audio devices for custom audio device selection.
            static std::vector<string> GetAudioDevices();

            // OpenAL Context, for custom usage.
            ALCcontext* ALcontext = nullptr;
            // OpenAL Device, for custom usage.
            ALCdevice* ALdevice = nullptr;

            AudioManager();
            // For a custom audio device selection.
            AudioManager(string audioDevice);
            ~AudioManager();
            // Plays a sound.
            void PlaySound(Sound sound);
            // Plays a sound in a 3D space.
            void PlaySound3D(Sound sound, Vector3 position, Vector3 velocity = Vector3{0, 0, 0});
            // Stops playing a sound.
            void StopSound(Sound sound);
            // (Un)pauses a sound.
            void PauseSound(Sound sound);
            // Goes to the specified sample.
            void Goto(Sound sound, int sample);
            
    };

    #pragma endregion

    #pragma region Input
    
    // A key on a keyboard or mouse.
    enum Key {
        UNKNOWN = GLFW_KEY_UNKNOWN, // A key that is not known to GLFW.
        A = GLFW_KEY_A,
        B = GLFW_KEY_B,
        C = GLFW_KEY_C,
        D = GLFW_KEY_D,
        E = GLFW_KEY_E,
        F = GLFW_KEY_F,
        G = GLFW_KEY_G,
        H = GLFW_KEY_H,
        I = GLFW_KEY_I,
        J = GLFW_KEY_J,
        K = GLFW_KEY_K,
        L = GLFW_KEY_L,
        M = GLFW_KEY_M,
        N = GLFW_KEY_N,
        O = GLFW_KEY_O,
        P = GLFW_KEY_P,
        Q = GLFW_KEY_Q,
        R = GLFW_KEY_R,
        S = GLFW_KEY_S,
        T = GLFW_KEY_T,
        U = GLFW_KEY_U,
        V = GLFW_KEY_V,
        W = GLFW_KEY_W,
        X = GLFW_KEY_X,
        Y = GLFW_KEY_Y,
        Z = GLFW_KEY_Z,
        ESCAPE = GLFW_KEY_ESCAPE, // or ESC
        F1 = GLFW_KEY_F1,
        F2 = GLFW_KEY_F2,
        F3 = GLFW_KEY_F3,
        F4 = GLFW_KEY_F4,
        F5 = GLFW_KEY_F5,
        F6 = GLFW_KEY_F6,
        F7 = GLFW_KEY_F7,
        F8 = GLFW_KEY_F8,
        F9 = GLFW_KEY_F9,
        F10 = GLFW_KEY_F10,
        F11 = GLFW_KEY_F11,
        F12 = GLFW_KEY_F12,
        F13 = GLFW_KEY_F13, 
        F14 = GLFW_KEY_F14,
        F15 = GLFW_KEY_F15,
        F16 = GLFW_KEY_F16,
        F17 = GLFW_KEY_F17,
        F18 = GLFW_KEY_F18,
        F19 = GLFW_KEY_F19,
        F20 = GLFW_KEY_F20,
        F21 = GLFW_KEY_F21,
        F22 = GLFW_KEY_F22,
        F23 = GLFW_KEY_F23,
        F24 = GLFW_KEY_F24,
        F25 = GLFW_KEY_F25, // ;)
        PRINTSCREEN = GLFW_KEY_PRINT_SCREEN, // Print Screen
        SCROLLLOCK = GLFW_KEY_SCROLL_LOCK, // Scroll Lock
        PAUSE = GLFW_KEY_PAUSE, // Pause Break
        GRAVE_ACCENT = GLFW_KEY_GRAVE_ACCENT, // `
        KEY_1 = GLFW_KEY_1, // 1
        KEY_2 = GLFW_KEY_2, // 2
        KEY_3 = GLFW_KEY_3, // 3
        KEY_4 = GLFW_KEY_4, // 4
        KEY_5 = GLFW_KEY_5, // 5
        KEY_6 = GLFW_KEY_6, // 6
        KEY_7 = GLFW_KEY_7, // 7
        KEY_8 = GLFW_KEY_8, // 8
        KEY_9 = GLFW_KEY_9, // 9
        KEY_0 = GLFW_KEY_0, // 0
        MINUS = GLFW_KEY_MINUS, // -
        EQUALS = GLFW_KEY_EQUAL, // =
        BACKSPACE = GLFW_KEY_BACKSPACE, 
        TAB = GLFW_KEY_TAB,
        LEFT_BRACKET = GLFW_KEY_LEFT_BRACKET, // [
        RIGHT_BRACKET = GLFW_KEY_RIGHT_BRACKET, // ]
        BACKSLASH = GLFW_KEY_BACKSLASH,
        CAPSLOCK = GLFW_KEY_CAPS_LOCK, // or CAPS
        SEMICOLON = GLFW_KEY_SEMICOLON, // ;
        APOSTROPHE = GLFW_KEY_APOSTROPHE, // '
        ENTER = GLFW_KEY_ENTER, // or return
        LSHIFT = GLFW_KEY_LEFT_SHIFT,
        COMMA = GLFW_KEY_COMMA,
        PERIOD = GLFW_KEY_PERIOD,
        SLASH = GLFW_KEY_SLASH,
        RSHIFT = GLFW_KEY_RIGHT_SHIFT,
        LEFT_CTRL = GLFW_KEY_LEFT_CONTROL,
        LEFT_ALT = GLFW_KEY_LEFT_ALT,
        SPACE = GLFW_KEY_SPACE,
        RIGHT_ALT = GLFW_KEY_RIGHT_ALT,
        MENU = GLFW_KEY_MENU, // or context menu key
        RIGHT_CTRL = GLFW_KEY_RIGHT_CONTROL,
        LEFT_SUPER = GLFW_KEY_LEFT_SUPER, // like left Windows key 
        RIGHT_SUPER = GLFW_KEY_RIGHT_SUPER, // like right Windows key 
        INSERT = GLFW_KEY_INSERT,
        HOME = GLFW_KEY_HOME,
        PAGE_UP = GLFW_KEY_PAGE_UP,
        PAGE_DOWN = GLFW_KEY_PAGE_DOWN,
        DELETE = GLFW_KEY_DELETE,
        END = GLFW_KEY_END,
        ARROW_UP = GLFW_KEY_UP,
        ARROW_LEFT = GLFW_KEY_LEFT,
        ARROW_DOWN = GLFW_KEY_DOWN,
        ARROW_RIGHT = GLFW_KEY_RIGHT,
        NUMLOCK = GLFW_KEY_NUM_LOCK,
        NUMPAD_SLASH = GLFW_KEY_KP_DIVIDE,
        NUMPAD_ASTRIK = GLFW_KEY_KP_MULTIPLY,
        NUMPAD_MINUS = GLFW_KEY_KP_SUBTRACT,
        NUMPAD_0 = GLFW_KEY_KP_0,  // Numpad 0
        NUMPAD_1 = GLFW_KEY_KP_1, // Numpad 1
        NUMPAD_2 = GLFW_KEY_KP_2, // Numpad 2
        NUMPAD_3 = GLFW_KEY_KP_3, // Numpad 3
        NUMPAD_4 = GLFW_KEY_KP_4, // Numpad 4
        NUMPAD_5 = GLFW_KEY_KP_5, // Numpad 5
        NUMPAD_6 = GLFW_KEY_KP_6, // Numpad 6
        NUMPAD_7 = GLFW_KEY_KP_7, // Numpad 7
        NUMPAD_8 = GLFW_KEY_KP_8, // Numpad 8
        NUMPAD_9 = GLFW_KEY_KP_9, // Numpad 9
        NUMPAD_PLUS = GLFW_KEY_KP_ADD,
        NUMPAD_ENTER = GLFW_KEY_KP_ENTER,
        NUMPAD_PERIOD = GLFW_KEY_KP_DECIMAL,
        NUMPAD_EQUALS = GLFW_KEY_KP_EQUAL,

        MOUSE_LEFT = GLFW_MOUSE_BUTTON_LEFT, // AKA MOUSE_1
        MOUSE_RIGHT = GLFW_MOUSE_BUTTON_RIGHT, // AKA MOUSE_2
        MOUSE_MIDDLE = GLFW_MOUSE_BUTTON_MIDDLE, // AKA MOUSE_3
        MOUSE_4 = GLFW_MOUSE_BUTTON_4,
        MOUSE_5 = GLFW_MOUSE_BUTTON_5,
        MOUSE_6 = GLFW_MOUSE_BUTTON_6,
        MOUSE_7 = GLFW_MOUSE_BUTTON_7,
        MOUSE_8 = GLFW_MOUSE_BUTTON_8,

        WORLD_1 = GLFW_KEY_WORLD_1, // World key for foreign languages (non-US).
        WORLD_2 = GLFW_KEY_WORLD_2  // World key for foreign languages (non-US).
    };

    // Handles all input in the Application.
    class InputManager
    {
        private:
            bool MouseLocked;
        public:
            // The GLFW window, for custom usage.
            GLFWwindow *window;

            
            void (*KeyDownFunction)(Key key); // Check the corresponding functions.
            void (*KeyUpFunction)(Key key); // Check the corresponding functions.
            void (*MouseMovedFunction)(Vector2 pos); // Check the corresponding functions.
            void (*ScrollFunction)(Vector2 offset); // Check the corresponding functions.
            void (*WindowResizeFunction)(int width, int height, char maximized, char minimized); // Check the corresponding functions.
            void (*WindowMoveFunction)(int x, int y); // Check the corresponding functions.
            void (*PathDropFunction)(std::vector<string> paths); // Check the corresponding functions.

            InputManager(GLFWwindow *window);
            ~InputManager();
            // Converts a key or keypress to the corresponding character (excluding shift check).
            char ToChar(Key key);
            // Converts a key to the platform-specific scancode.
            int ToScanCode(Key key);

            // Checks if that key is pressed.
            bool IsKeyPressed(Key key);
            // Sets an event for when any key is pressed.
            void SetOnKeyDown(void (*KeyDownFunction)(Key key));
            // Sets an event for when any key is released.
            void SetOnKeyUp(void (*KeyUpFunction)(Key key));

            // Returns the mouse position.
            Vector2 GetMousePosition();
            // Sets an event for when the mouse moved.
            void SetOnMouseMoved(void (*MouseMovedFunction)(Vector2 pos));
            // Sets an event for when the mouse wheel scrolled.
            void SetOnScroll(void (*ScrollFunction)(Vector2 offset));
            
            // Returns if the mouse is locked.
            bool GetMouseLocked();
            // Locks the mouse in place and hide it.
            void LockMouse(bool lock);

            // Sets the window size (keep width and height 0 if you're using maximized or minimized).
            void SetWindowSize(int width, int height, bool maximized, bool minimized);
            // Sets the position of the window.
            void SetWindowPosition(int x, int y);
            // Sets the title of the current window (supports UTF-8).
            void SetTitle(string newTitle);
            // Sets the icons of the current window, use different sizes for rescaling purposes.
            void SetIcon(std::vector<Texture> newIcons);
            // Sets an event for when the window is resized (including maximized, minimized: 2=restored, 1=applied, 0=nothing).
            void SetOnWindowResize(void (*WindowResizeFunction)(int width, int height, char maximized, char minimized));
            // Sets an event for when the window is moved.
            void SetOnWindowMove(void (*WindowMoveFunction)(int x, int y));
            // Sets an event for when a path / paths are dropped on the window.
            void SetOnPathDrop(void (*PathDropFunction)(std::vector<string> paths));
    };
    
    #pragma endregion

} // namespace GAIL
