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

    // An effect for sounds.
    class SoundEffect
    {
        public:
            SoundEffect() {};
            ~SoundEffect() {};
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
        // If this sound loops (default: false).
        bool isLooping = false;
        // The volume of this sound (default: 1).
        float volume = 1.f;
        // The pitch of this sound (default: 1).
        float pitch = 1.f;
        // The source (openAL), for custom usage.
        ALuint source;
        // The buffer (openAL), for custom usage.
        ALuint buffer;

        // The current sound effects.
        std::vector<SoundEffect> soundEffects;

        // sampleRate in Hertz (44100Hz), rawData: vector<char>: the raw sound data in PCM (8bit format: 1x the size, 16bit format: 2x the size), format: mono/stereo 16bit etc., soundEffects: vector of effects to apply
        Sound(int sampleRate = 0, std::vector<char> rawData = std::vector<char>(), SoundFormat format = SoundFormat::Mono16, std::vector<SoundEffect> soundEffects = std::vector<SoundEffect>());
        ~Sound();

        // Updates the current source and buffer (call this when you use OpenAL).
        void Update();

        /*
        Applies sound effects to this sound (when CreateSource is called).
        Applied in the order from the first to the last of the list (vector).
        */
        void SetSoundEffects(std::vector<SoundEffect> soundEffects);
        /*
        Loads a sound from a wav file with a path to it.
        */
        static Sound FromWAV(string path);
    };

    /*
    Reads audio from a microphone or sound output.
    */
    class SoundCapture
    {
        public:
            // the sample rate of this sound capture.
            int sampleRate;
            // The format of this sound capture.
            SoundFormat format;
            // The capture device, used for custom usage.
            ALCdevice *captureDevice;
            // Returns all the capture devices' names.
            static std::vector<string> GetCaptureDevices();
            // For the captureDevice, use GetCaptureDevice return value or nullptr, the format of the sound, sampleRate in Hz.
            SoundCapture(SoundFormat format, int sampleRate, string captureDevice = nullptr);
            ~SoundCapture();
            // Starts capturing the audio.
            void Start();
            // Stops capturing the audio and returns the sound.
            Sound Stop();
    };
    

} // namespace GAIL
