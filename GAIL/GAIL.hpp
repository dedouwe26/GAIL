#pragma once

#include "Structs.hpp"
#include "Audio.hpp"
#include "Model.hpp"
#include "Shader.hpp"
#include "Manager.hpp"

namespace GAIL
{
    // The central part of GAIL. Includes all the Managers.
    class Application
    {
        public:
            GraphicsManager graphicsManager;
            InputManager inputManager;
            AudioManager audioManager;

            Application(string windowName = "GAIL Window", int width = 1000, int height=600, Texture icon = Texture{});
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
            /*
            Stops the application (some things might break if used certain functions after).
            Returns true if succeeded.
            */
            bool Stop();
    };
    
} // namespace GAIL