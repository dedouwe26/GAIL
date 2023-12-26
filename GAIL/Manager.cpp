#include "GAIL.hpp"

namespace GAIL
{
    GraphicsManager::GraphicsManager(AppInfo info) {
        VkApplicationInfo appInfo{};
        appInfo.sType = VK_STRUCTURE_TYPE_APPLICATION_INFO;
        appInfo.pApplicationName = info.AppName.c_str();
        appInfo.applicationVersion = VK_MAKE_VERSION(info.AppVersion[0], info.AppVersion[1], info.AppVersion[2]);
        appInfo.pEngineName = info.EngineName.c_str();
        appInfo.engineVersion = VK_MAKE_VERSION(info.EngineVersion[0], info.EngineVersion[1], info.EngineVersion[2]);
        appInfo.apiVersion = VK_API_VERSION_1_0;

        uint32_t extensionCount = 0;
        const char** extensions;

        extensions = glfwGetRequiredInstanceExtensions(&extensionCount);

        VkInstanceCreateInfo createInfo{};
        createInfo.sType = VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO;
        createInfo.pApplicationInfo = &appInfo;
        createInfo.enabledExtensionCount = extensionCount;
        createInfo.ppEnabledExtensionNames = extensions;
        createInfo.enabledLayerCount = 0;

        this->instance = vk::createInstance(createInfo);
    };

    GraphicsManager::~GraphicsManager() {
        
    };

    string AudioManager::GetDefaultAudioDevice() {

    };
    std::vector<string> AudioManager::GetAudioDevices() {

    };

    AudioManager::AudioManager(string audioDevice) {
        
    };
    AudioManager::AudioManager() : AudioManager(GetDefaultAudioDevice()) {};

    AudioManager::~AudioManager() {
        
    };

    bool AudioManager::PlaySound(Sound sound) {

    };
    bool AudioManager::PlaySound3D(Sound sound, Vector3 position) {
        
    };

    InputManager::InputManager(GLFWwindow *window) {
        this->window=window;

        glfwSetWindowUserPointer(this->window, this);
        
        glfwSetKeyCallback(this->window, [](GLFWwindow* window, int key, int scancode, int action, int mods){
            if (action == GLFW_PRESS) {
                static_cast<InputManager*>(glfwGetWindowUserPointer(window))->KeyDownFunction(static_cast<Key>(key));
            } else if (action == GLFW_RELEASE) {
                static_cast<InputManager*>(glfwGetWindowUserPointer(window))->KeyUpFunction(static_cast<Key>(key));
            };
        });
        glfwSetCursorPosCallback(window, [](GLFWwindow* window, double xpos, double ypos) {
            static_cast<InputManager*>(glfwGetWindowUserPointer(window))->MouseMovedFunction(Vector2{xpos, ypos});
        });
        glfwSetScrollCallback(window, [](GLFWwindow* window, double xoffset, double yoffset){
            static_cast<InputManager*>(glfwGetWindowUserPointer(window))->ScrollFunction(Vector2{xoffset, yoffset});
        });
        glfwSetWindowSizeCallback(window, [](GLFWwindow* window, int width, int height){
            static_cast<InputManager*>(glfwGetWindowUserPointer(window))->WindowResizeFunction(width, height, false, false);
        });
        glfwSetWindowIconifyCallback(window, [](GLFWwindow* window, int iconified) {
            static_cast<InputManager*>(glfwGetWindowUserPointer(window))->WindowResizeFunction(0, 0, false, iconified);
        });
        glfwSetWindowMaximizeCallback(window, [](GLFWwindow* window, int maximized){
            static_cast<InputManager*>(glfwGetWindowUserPointer(window))->WindowResizeFunction(0, 0, maximized, false);
        });
    };
    InputManager::~InputManager() {
        
    };

    char InputManager::ToChar(Key key) {return *glfwGetKeyName(key, ToScanCode(key));};
    int InputManager::ToScanCode(Key key) {return glfwGetKeyScancode(key);};
    bool InputManager::IsKeyPressed(Key key) {
        return glfwGetKey(this->window, key)==GLFW_PRESS;
    };
    void InputManager::SetOnKeyDown(void (*KeyDownFunction)(Key key)) {
        this->KeyDownFunction = KeyDownFunction;
    };
    void InputManager::SetOnKeyUp(void (*KeyUpFunction)(Key key)) {
        this->KeyUpFunction = KeyUpFunction;
    };
    Vector2 InputManager::GetMousePosition() {
        Vector2 pos;
        glfwGetCursorPos(window, &pos.x, &pos.y);
        return pos;
    };
    void InputManager::SetOnMouseMoved(void (*MouseMovedFunction)(Vector2 pos)) {
        this->MouseMovedFunction = MouseMovedFunction;
    };
    void InputManager::SetOnScroll(void (*ScrollFunction)(Vector2 offset)) {
        this->ScrollFunction = ScrollFunction;
    };
    void InputManager::LockMouse(bool lock) {
        this->MouseLocked = lock;

    };
    void InputManager::SetWindow(int x, int y, int width, int height, bool maximized, bool minimized) {
        glfwSetWindowPos(window, 100, 100);
        glfwSetWindowSize(window, 640, 480);
        glfwMaximizeWindow(window);
        glfwIconifyWindow(window);
    };
    void InputManager::SetTitle(string newTitle) {
        glfwSetWindowTitle(this->window, newTitle.c_str());
    };
    void InputManager::SetIcon(std::vector<Texture> newIcons) {
        std::vector<GLFWimage> glfwImages;
        glfwImages.resize(newIcons.size());
        for (int i = 0; i < newIcons.size(); i++) {
            // TODO
            glfwImages[i] = GLFWimage{newIcons[i].width, newIcons[i].height, NULL};
        }

        glfwSetWindowIcon(this->window, glfwImages.size(), glfwImages.data());
    };
    void InputManager::SetOnWindowResize(void (*WindowResizeFunction)(int width, int height, bool maximized, bool minimized)) {
        this->WindowResizeFunction = WindowResizeFunction;
    };
    void InputManager::SetOnWindowMove(void (*WindowMoveFunction)(Vector2 newPos)) {
        this->WindowMoveFunction = WindowMoveFunction;
    };
    void InputManager::SetOnPathDrop(void (*PathDropFunction)(std::vector<string> paths)) {
        this->PathDropFunction = PathDropFunction;
    };
} // namespace GAIL
