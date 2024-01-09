#include "GAIL.hpp"

#include <iostream>

namespace GAIL
{
    void Application::GLFWErrorCallback(int error, const char* description) {
        std::cerr << "GAIL: GLFW Error (" << error << "): " << description << std::endl;
    };

    Application::Application(string windowName, int width, int height, Texture icon, AppInfo info) : graphicsManager(GraphicsManager(info)), inputManager(InputManager(this->window)) {
        glfwSetErrorCallback(Application::GLFWErrorCallback);
        if (!glfwInit()) {
            std::cerr << "GAIL: GLFW: initialization failed!" << std::endl;
            this->~Application();
        }
        glfwWindowHint(GLFW_CLIENT_API, GLFW_NO_API); // Sets no graphics api (opengl) for vulkan usage.
        
        this->window = glfwCreateWindow(width, height, windowName.c_str(), NULL, NULL);

        glfwGetWindowSize(this->window, this->width, this->height);
        
        std::vector<Texture> icons;
        icons[0] = icon;
        this->inputManager.SetIcon(icons);

        if (!window) {
            std::cerr << "GAIL: GLFW: window creation failed!" << std::endl;
            this->~Application();
        }

        this->loadFunction(*this);

        double currentTime = glfwGetTime();
        double lastTime = currentTime;
        while (!glfwWindowShouldClose(this->window)) {
            glfwPollEvents();
            currentTime = glfwGetTime();
            this->updateFunction(*this, currentTime - lastTime);
            lastTime = currentTime;
        }
        this->Stop();
    }
    Application::~Application() {
        this->Stop();
    }
    void Application::Stop() {
        this->stopFunction(*this);
        glfwDestroyWindow(window);
        glfwTerminate();
    }
    void Application::SetOnUpdate(void (*updateFunction)(Application app, double deltaTime)) {this->updateFunction = updateFunction;};
    void Application::SetOnLoad(void (*loadFunction)(Application app)) {this->loadFunction = loadFunction;};
    void Application::SetOnStop(void (*stopFunction)(Application app)) {this->stopFunction = stopFunction;};
    GraphicsManager Application::GetGraphicsManager() {return graphicsManager;};
    InputManager Application::GetInputManager() {return inputManager;};
    AudioManager Application::GetAudioManager() {return audioManager;};
    
} // namespace GAIL
