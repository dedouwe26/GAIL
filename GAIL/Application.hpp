#pragma once

#include "GAIL.hpp"

namespace GAIL
{

    // The metadata of the application.
    struct AppInfo {
        // App name (UTF-8)
        string AppName = "App";
        // 0: Major  1: Minor  2: Patch.
        unsigned int AppVersion[3] = {1,0,0};
        // Engine name (UTF-8)
        string EngineName = "GAIL";
        // 0: Major  1: Minor  2: Patch.
        unsigned int EngineVersion[3] = {1,0,0};
    };

    // The central part of GAIL. Includes all the Managers.
    class Application
    {
        private:
            static void GLFWErrorCallback(int error, const char* description);
        public:
            GraphicsManager graphicsManager;
            InputManager inputManager;
            AudioManager audioManager;

            // The current update-function.
            void (*updateFunction)(Application app, double deltaTime);
            // The current load-function.
            void (*loadFunction)(Application app);
            // The current stop-function.
            void (*stopFunction)(Application app);

            // The GLFW window instance for custom usage.
            GLFWwindow* window;

            // Window Width (read-only).
            int *width;
            // Window Height (read-only).
            int *height;

            Application(string windowName = "GAIL Window", int width = 1000, int height=600, Texture icon = Texture{}, AppInfo info = {});
            ~Application();

            // Sets the Update function, with delta time (in seconds): CurrentTime - PreviousFrameTime (calls every frame).
            void SetOnUpdate(void (*updateFunction)(Application app, double deltaTime)) {this->updateFunction = updateFunction;};
            // Sets the Load function (calls at the start).
            void SetOnLoad(void (*loadFunction)(Application app)) {this->loadFunction = loadFunction;};
            // Sets the Stop function (calls at close).
            void SetOnStop(void (*stopFunction)(Application app)) {this->stopFunction = stopFunction;};


            // Gets GraphicsManager.
            GraphicsManager GetGraphicsManager() {return graphicsManager;};
            // Gets InputManager.
            InputManager GetInputManager() {return inputManager;};
            // Gets AudioManager.
            AudioManager GetAudioManager() {return audioManager;};
            /*
            Stops the application (some things might break if used certain functions after).
            */
            void Stop();
    };
    
} // namespace GAIL