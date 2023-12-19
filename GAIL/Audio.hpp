#pragma once

#include "GAIL.hpp"

namespace GAIL
{

    // A sound format: how many channels and the bandwidth.
    enum SoundFormat {
        Stereo16 = AL_FORMAT_STEREO16,
        Mono16 = AL_FORMAT_MONO16,
        Stereo8 = AL_FORMAT_STEREO8,
        Mono8 = AL_FORMAT_MONO8
    };

    class Sound;

    // An effect for sounds (like pitch, etc).
    class SoundEffect
    {
        public:
            SoundEffect();
            ~SoundEffect();
            // Applies effects to the sound.
            // Returns the effected sound.
            Sound Use(Sound baseSound);
    };

    // Represents a sound (audio) with effects.
    class Sound
    {
        public:
            SoundFormat format;
            double duration;
            int sampleRate;
            std::vector<char> rawData;
            // If this sound loops
            bool isLooping;
            // The volume of this sound
            float volume;

            std::vector<SoundEffect> soundEffects;

            // sampleRate in Hertz (44100Hz), rawData in the specified format.
            Sound(int sampleRate, std::vector<char> rawData, SoundFormat format);
            ~Sound();
            // Returns a configured Source (OpenAL) created from this class.
            ALuint CreateSource();

            /*
            Apllies sound effects to this sound (when CreateSource is called).
            Applied in the order from the first to the last of the list (vector).
            */
            void SetSoundEffects(std::vector<SoundEffect> soundEffects);
            
            /*
            Loads a sound from a audio file with a path to it.
            Supported types: WAV, ogg, RAW.
            */
            static Sound FromFile(string path);
    };

    /*
    Reads audio from a microphone or sound output.
    */
    class SoundCapture
    {
        public:
            static int GetDefaultDevice();
            // Returns all the capture devices' names.
            static std::vector<string> GetCaptureDevices();
            // For the captureDevice int, use index from GetCaptureDevice or GetDefaultDevice return value, the format of the sound.
            SoundCapture(int captureDevice, SoundFormat format);
            ~SoundCapture();
            // Starts capturing the audio.
            void Start();
            // Stops capturing the audio and returns the sound.
            Sound Stop();
    };
    

} // namespace GAIL
