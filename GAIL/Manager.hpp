#pragma once

#include <map>
#include <string>
#include "Model.hpp"

using std::string;

namespace GAIL
{

    #pragma region Graphics

    // A Graphical layer, like UI: 2D and World: 3D.
    class Layer
    {
        private:
            string name;
            int dimensions;
		public:
			Layer(string name, int dimensions) : name{name}, dimensions{dimensions} {};
			~Layer();
    };

    /*
     * This handles all the graphics of GAIL.
     */
    class GraphicsManager
    {
        private:
            std::map<string, Layer> layers;
        public:
            GraphicsManager(std::map<string, Layer> layers) : layers{layers} {};
            ~GraphicsManager();
    };

    #pragma endregion

    #pragma region Audio

    // Handles all audio in the Application.
    class AudioManager
    {
        public:
            AudioManager();
            ~AudioManager();
    };

    #pragma endregion

    #pragma region Input
    
    // Handles all input in the Application.
    class InputManager
    {
    public:
        InputManager();
        ~InputManager();
    };
    
    #pragma endregion

} // namespace GAIL
