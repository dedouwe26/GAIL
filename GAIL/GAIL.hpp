#pragma once

#include "Structs.hpp"
#include "Window.hpp"
#include "Audio.hpp"
#include "Model.hpp"
#include "Shader.hpp"
#include "Manager.hpp"

namespace GAIL
{
    // The central part of GAIL. Includes all the Managers.
    class Application
    {
        private:
            GraphicsManager graphicsManager;
            InputManager inputManager;
            AudioManager audioManager;
            Window window;
        public:
            Application(Window window);
            ~Application();

            // Sets the Update function, with delta time (in seconds): CurrentTime - PreviousFrameTime (calls every frame).
            void SetUpdate(void (*updateFunction)(double deltaTime));
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