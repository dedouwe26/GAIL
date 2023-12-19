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

        vk::createInstance(createInfo, nullptr, *this->instance);
    };

    GraphicsManager::~GraphicsManager() {
        
    }
} // namespace GAIL
