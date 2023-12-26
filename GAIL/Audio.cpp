#include "GAIL.hpp"

namespace GAIL {
    Sound SoundEffect::Use(Sound baseSound) {
        return baseSound;
    };

    Sound::~Sound() {
        this->soundEffects.clear();
        this->rawData.clear();
        delete &this->rawData;
        delete &this->soundEffects;
    };
    ALuint Sound::CreateSource() {
        // ALC in arg??

        // Apply effects

        // Create ALC source
        // Set volume etc.

        // Return
        return 0;
    };
    Sound Sound::FromOGG(string path) {
        return Sound();
    };
    Sound Sound::FromWAV(string path) {
        return Sound();
    };
}   