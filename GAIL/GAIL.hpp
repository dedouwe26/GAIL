#pragma once

#include <string>
#include <vector>
#include <map>

#include <al.h>
#include <alc.h>

#define GLFW_INCLUDE_VULKAN
#include <vulkan/vulkan.hpp>

#include <glfw3.h>

namespace GAIL
{
    using string = std::string;
}

#include "Structs.hpp"
#include "Audio.hpp"
#include "Shader.hpp"
#include "Model.hpp"
#include "Manager.hpp"
#include "Application.hpp"