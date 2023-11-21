#pragma once

#include <string>

using string = std::string;

namespace GAIL
{
    // Handles the window.
    class Window
    {
        public:
            Window(string name, int width, int height);
            ~Window();
    };
    
} // namespace GAIL
