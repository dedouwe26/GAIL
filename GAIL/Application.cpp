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

        if (!window) {
            std::cerr << "GAIL: GLFW: window creation failed!" << std::endl;
            this->~Application();
        }

        this->loadFunction(*this);

        while (!glfwWindowShouldClose(this->window)) {
            glfwPollEvents();
            // this->updateFunction();
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
    
} // namespace GAIL
