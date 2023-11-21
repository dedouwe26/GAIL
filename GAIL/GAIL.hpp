#pragma once

#include "Structs.hpp"
#include "Window.hpp"
#include "Audio.hpp"
#include "Model.hpp"
#include "Shader.hpp"
#include "Manager.hpp"

namespace GAIL
{

    /*
        Defines how it's rendered.
        RENDER: Can contain 3D, uses rendering. Recommended for games.
        NONRENDER: Only 2D, doesn't use rendering or shaders.
    */
    enum ApplicationType {
        RENDER, // Can contain 3D, uses rendering. Recommended for games.
        NONRENDER // Only 2D, doesn't use rendering or shaders.
    };
    
    // The central part of GAIL. Includes all the Managers.
    class Application
    {
        private:
            GraphicsManager graphicsManager;
            InputManager inputManager;
            AudioManager audioManager;
            Window window;
        public:
            Application(ApplicationType type, Window window);
            ~Application();

            // Sets the Update function (calls every frame).
            void SetUpdate(void (*updateFunction)());
            // Sets the Load function (calls at the start).
            void SetLoad(void (*loadFunction)());
            // Sets the Stop function (calls at close).
            void SetStop(void (*stopFunction)());


            // Gets GraphicsManager.
            GraphicsManager GetGraphicsManager() {return graphicsManager;};
            // Gets InputManager.
            InputManager GetInputManager() {return inputManager;};
            // Gets AudioManager.
            AudioManager GetAudioManager() {return audioManager;};
            // Gets Window.
            Window GetWindow() {return window;};
            // Stops the application (some things might break if used certain functions after).
            /*
            Stops the application (some things might break if used certain functions after).
            Returns if succeeded.
            */
            bool Stop();
    };
    
} // namespace GAIL