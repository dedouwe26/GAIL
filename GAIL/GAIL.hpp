#pragma once

#include "Window.hpp"
#include "Manager.hpp"
#include "Model.hpp"
#include "Shader.hpp"

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
            GraphicsManager GetGraphicsManager() {return graphicsManager;};
            InputManager GetInputManager() {return inputManager;};
            AudioManager GetAudioManager() {return audioManager;};
            Window GetWindow() {return window;};
    };

    
    
} // namespace GAIL