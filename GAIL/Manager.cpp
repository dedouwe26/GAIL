#include "GAIL.hpp"

#include <iostream>

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
    std::vector<string> AudioManager::GetAudioDevices() {
        std::vector<string> audioDevices;
        const ALCchar* devices = alcGetString(nullptr, ALC_DEVICE_SPECIFIER);

        const char* ptr = devices;

        audioDevices.clear();

        do
        {
            audioDevices.push_back(std::string(ptr));
            ptr += audioDevices.back().size() + 1;
        }
        while(*(ptr + 1) != '\0');
        return audioDevices;
    };

    AudioManager::AudioManager(string audioDevice) {
        this->ALdevice = alcOpenDevice(audioDevice.c_str());
        if (!this->ALdevice) {
            std::cerr << "GAIL: OpenAL: Failed to open device." << std::endl;
            delete this;
            return;
        }
        this->ALcontext = alcCreateContext(this->ALdevice, nullptr);
        if (!this->ALcontext) {
            std::cerr << "GAIL: OpenAL: Could not create audio context." << std::endl;
            delete this;
            return;
        }
        if (!alcMakeContextCurrent(this->ALcontext)!= ALC_TRUE) {
            std::cerr << "GAIL: OpenAL: Could not make audio context current." << std::endl;
            delete this;
            return;
        }

    };
    AudioManager::AudioManager() {
        this->ALdevice = alcOpenDevice(nullptr);
        if (!this->ALdevice) {
            std::cerr << "GAIL: OpenAL: Failed to open device." << std::endl;
            delete this;
            return;
        }
        this->ALcontext = alcCreateContext(this->ALdevice, nullptr);
        if (!this->ALcontext) {
            std::cerr << "GAIL: OpenAL: Could not create audio context." << std::endl;
            delete this;
            return;
        }
        if (!alcMakeContextCurrent(this->ALcontext)!= ALC_TRUE) {
            std::cerr << "GAIL: OpenAL: Could not make audio context current." << std::endl;
            delete this;
            return;
        }
    };

    AudioManager::~AudioManager() {
        alcMakeContextCurrent(nullptr);
        alcDestroyContext(this->ALcontext);
        alcCloseDevice(this->ALdevice);
    };

    void AudioManager::PlaySound(Sound sound) {
        sound.Update();
        alSourcePlay(sound.source);

        ALint state = AL_PLAYING;
        while(state == AL_PLAYING)
        {
            alGetSourcei(sound.source, AL_SOURCE_STATE, &state);
        }
        alDeleteSources(1, &sound.source);
        alDeleteBuffers(1, &sound.buffer);
    };
    void AudioManager::PlaySound3D(Sound sound, Vector3 position, Vector3 velocity = Vector3{0, 0, 0}) {
        sound.Update();
        alSource3f(sound.source, AL_POSITION, position.x, position.y, position.z);
        alSource3f(sound.source, AL_VELOCITY, velocity.x, velocity.y, velocity.z);

        alSourcePlay(sound.source);

        ALint state = AL_PLAYING;
        while(state == AL_PLAYING)
        {
            alGetSourcei(sound.source, AL_SOURCE_STATE, &state);
        }
        alDeleteSources(1, &sound.source);
        alDeleteBuffers(1, &sound.buffer);
    };
    
    void AudioManager::StopSound(Sound sound) {
        alSourceStop(sound.source);
    };
    void AudioManager::PauseSound(Sound sound) {
        ALint state;
        alGetSourcei(sound.source, AL_SOURCE_STATE, &state);
        alSourcei(sound.source, AL_SOURCE_STATE, state==AL_PAUSED ? AL_PLAYING : AL_PAUSED);
    }
    void AudioManager::Goto(Sound sound, int sample) {
        alSourcei(sound.source, AL_SAMPLE_OFFSET, sample);
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
        glfwSetWindowPosCallback(window, [](GLFWwindow* window, int xpos, int ypos){
            static_cast<InputManager*>(glfwGetWindowUserPointer(window))->WindowMoveFunction(xpos, ypos);
        });
        glfwSetWindowSizeCallback(window, [](GLFWwindow* window, int width, int height){
            static_cast<InputManager*>(glfwGetWindowUserPointer(window))->WindowResizeFunction(width, height, NULL, NULL);
        });
        glfwSetWindowIconifyCallback(window, [](GLFWwindow* window, int iconified) {
            static_cast<InputManager*>(glfwGetWindowUserPointer(window))->WindowResizeFunction(0, 0, 0, iconified==GLFW_TRUE ? 1 : 2);
        });
        glfwSetWindowMaximizeCallback(window, [](GLFWwindow* window, int maximized){
            static_cast<InputManager*>(glfwGetWindowUserPointer(window))->WindowResizeFunction(0, 0, maximized==GLFW_TRUE ? 1 : 2, 0);
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
    bool InputManager::GetMouseLocked() {
        return this->MouseLocked;
    };
    void InputManager::LockMouse(bool lock) {
        this->MouseLocked = lock;
        glfwSetInputMode(window, GLFW_CURSOR, lock ? GLFW_CURSOR_DISABLED : GLFW_CURSOR_NORMAL);
    };
    void InputManager::SetWindowSize(int width, int height, bool maximized = false, bool minimized = false) {
        glfwSetWindowSize(this->window, width, height);
        glfwMaximizeWindow(this->window); // TODO
        glfwIconifyWindow(this->window);
    };
    void InputManager::SetWindowPosition(int x, int y) {
        glfwSetWindowPos(this->window, x, y);
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
    void InputManager::SetOnWindowResize(void (*WindowResizeFunction)(int width, int height, char maximized, char minimized)) {
        this->WindowResizeFunction = WindowResizeFunction;
    };
    void InputManager::SetOnWindowMove(void (*WindowMoveFunction)(int x, int y)) {
        this->WindowMoveFunction = WindowMoveFunction;
    };
    void InputManager::SetOnPathDrop(void (*PathDropFunction)(std::vector<string> paths)) {
        this->PathDropFunction = PathDropFunction;
    };
} // namespace GAIL
