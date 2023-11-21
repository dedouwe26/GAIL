#pragma once

#include <list>
#include <string>
#include "GAIL.hpp"
#include "Model.hpp"

using std::string;

namespace GAIL
{   

    #pragma region Graphics

    // Identifies how many dimensions.
    enum Dimensions {
        TWO = 2,
        THREE = 3
    };

    /*
     * This handles all the graphics of GAIL.
     */
    class GraphicsManager
    {
        public:
            GraphicsManager(ApplicationType type, Window window);
            ~GraphicsManager();
            // // Sets the layers (RENDER ApplicationType).
            // void SetLayers(std::list<Layer> layers);
            // 
            /*
            Renders the model to the current frame on that layer (RENDER ApplicationType).
            Returns if successful
            */
            bool Render(Model model);
    };

    #pragma endregion

    #pragma region Audio

    

    // Handles all audio in the Application.
    class AudioManager
    {
        public:
            AudioManager();
            ~AudioManager();
            // Plays a sound with volume.
            bool PlaySound(Sound sound, double volume);
            // Plays a sound in a 3D space with volume.
            bool PlaySound3D(Sound sound, Vector3 position, double volume);
    };

    #pragma endregion

    #pragma region Input
    
    // A key on a keyboard or mouse.
    enum Key {

    };

    // Handles all input in the Application.
    class InputManager
    {
    public:
        InputManager();
        ~InputManager();
        // Checks if that key is pressed.
        bool IsKeyPressed(Key key);
        // Sets an event for when any key is pressed.
        void SetOnKeyDown(void (*KeyDownFunction)(Key key));
        // Sets an event for when any key is released.
        void SetOnKeyUp(void (*KeyUpFunction)(Key key));
        // Returns the mouse position.
        Vector2 GetMousePosition();
        // Sets an event for when the mouse moved.
        void SetOnMouseMoved(void (*MouseMovedFunction)(Vector2 from, Vector2 to));
    };
    
    #pragma endregion

} // namespace GAIL
