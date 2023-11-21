#pragma once

#include <list>
#include <string>

using string = std::string;

namespace GAIL
{

    // An effect for sounds (like pitch, etc).
    class SoundEffect
    {
        public:
            SoundEffect();
            ~SoundEffect();
    };

    // Represents a sound with effects.
    class Sound
    {
        public:
            Sound(char* rawData);
            ~Sound();

            /*
            Adds sound effects to this sound.
            Applied in the order from 0 to the end of the list.
            */
            void SetSoundEffects(std::list<SoundEffect> soundEffects);
            
            // 
            /*
            Loads a sound from a audio file with a path to it.
            Supported types: WAV, ogg, RAW.
            */
            static Sound FromFile(string path);
    };
    /*
    Reads audio from a microphone or sound output.
    For the captureDevice int, use index from GetCaptureDevice or GetDefaultDevice return value.
    */
    class SoundCapture
    {
        public:
            static int GetDefaultDevice();
            // Returns all the capture devices' names.
            static std::list<string> GetCaptureDevices();

            SoundCapture(int captureDevice);
            ~SoundCapture();
            // Starts capturing the audio.
            void Start();
            // Stops capturing the audio and returns the sound.
            Sound Stop();
    };
    

} // namespace GAIL
