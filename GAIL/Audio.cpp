#include "GAIL.hpp"
#include <iostream>
#include <fstream>

namespace GAIL {
    int toInt(char* buffer, std::size_t len) {
        int a = 0;
        std::memcpy(&a, buffer, len); // little endian
        return a;
    };
    Sound SoundEffect::Use(Sound baseSound) {
        return baseSound;
    };
    Sound::Sound(int sampleRate = 0, std::vector<char> rawData = std::vector<char>(), SoundFormat format = SoundFormat::Mono16, std::vector<SoundEffect> soundEffects = std::vector<SoundEffect>()) : sampleRate{sampleRate}, format{format}, rawData{rawData}, soundEffects{soundEffects} {
        alGenBuffers(1, &this->buffer);

        Sound sound = *this;
        for (int i = 0; i < this->soundEffects.size(); i++)
        {
            sound = this->soundEffects[i].Use(sound);
        };
        
        alBufferData(this->buffer, sound.format, sound.rawData.data(), sound.rawData.size(), sound.sampleRate);
        
        delete &sound;

        alGenSources(1, &source);
        alSourcef(source, AL_PITCH, 1);
        alSourcef(source, AL_GAIN, 1);
        alSourcei(source, AL_LOOPING, AL_FALSE);
        alSourcei(source, AL_BUFFER, buffer);
    };
    Sound::~Sound() {
        alDeleteSources(1, &this->source);
        alDeleteBuffers(1, &this->buffer);
        this->soundEffects.clear();
        this->rawData.clear();
        delete &this->rawData;
        delete &this->soundEffects;
    };
    void Sound::Update() {
        alDeleteSources(1, &this->source);
        alDeleteBuffers(1, &this->buffer);

        alGenBuffers(1, &this->buffer);

        Sound sound = *this;
        for (int i = 0; i < this->soundEffects.size(); i++)
        {
            sound = this->soundEffects[i].Use(sound);
        };
        
        alBufferData(this->buffer, sound.format, sound.rawData.data(), sound.rawData.size(), sound.sampleRate);
        
        delete &sound;

        alGenSources(1, &source);
        alSourcef(source, AL_PITCH, this->pitch);
        alSourcef(source, AL_GAIN, this->volume);
        alSourcei(source, AL_LOOPING, isLooping ? AL_TRUE : AL_FALSE);
        alSourcei(source, AL_BUFFER, buffer);
    };
    void Sound::SetSoundEffects(std::vector<SoundEffect> soundEffects) { this->soundEffects = soundEffects; };

    Sound Sound::FromWAV(string path) {
        std::ifstream file(path, std::ios::binary);
        if(!file.is_open()) {
            std::cerr << "GAIL: FromWAV (load file): Could not open \"" << path << "\"" << std::endl;
            return NULL;
        }
        char buffer[4];
        // RIFF
        if(!file.read(buffer, 4)) {
            std::cerr << "GAIL: FromWAV (load file): could not read RIFF" << std::endl;
            return NULL;
        }
        if(std::strncmp(buffer, "RIFF", 4) != 0) {
            std::cerr << "GAIL: FromWAV (load file): file is not a valid WAVE file (header doesn't begin with RIFF)" << std::endl;
            return NULL;
        }
        // ChunkSize of RIFF
        if(!file.read(buffer, 4)) {
            std::cerr << "GAIL: FromWAV (load file): could not read ChunkSize" << std::endl;
            return NULL;
        }
        // format
        if(!file.read(buffer, 4)) {
            std::cerr << "GAIL: FromWAV (load file): could not read Format" << std::endl;
            return NULL;
        }
        if(std::strncmp(buffer, "WAVE", 4) != 0) {
            std::cerr << "GAIL: FromWAV (load file): file is not a valid WAVE file (header doesn't contain WAVE)" << std::endl;
            return NULL;
        }
        // "fmt/0"
        if(!file.read(buffer, 4)) {
            std::cerr << "GAIL: FromWAV (load file): could not read Subchunk1ID" << std::endl;
            return NULL;
        }
        // 16, for PCM.
        if(!file.read(buffer, 4)) {
            std::cerr << "GAIL: FromWAV (load file): could not read Subchunk1Size" << std::endl;
            return NULL;
        }
        // Audio format (1=PCM)
        if(!file.read(buffer, 2)) {
            std::cerr << "GAIL: FromWAV (load file): could not read Audio format" << std::endl;
            return NULL;
        }
        // NumChannels
        if(!file.read(buffer, 2)) {
            std::cerr << "GAIL: FromWAV (load file): could not read NumChannels" << std::endl;
            return NULL;
        }
        int channels = toInt(buffer, 2);

        // sample rate
        if(!file.read(buffer, 4)) {
            std::cerr << "GAIL: FromWAV (load file): could not read sample rate" << std::endl;
            return NULL;
        }
        int sampleRate = toInt(buffer, 4);
        // (sampleRate * bitsPerSample * channels) / 8
        if(!file.read(buffer, 4)) {
            std::cerr << "GAIL: FromWAV (load file): could not read (sampleRate * bitsPerSample * channels) / 8" << std::endl;
            return NULL;
        }
        // channels * BitsPerSample/8
        if(!file.read(buffer, 2)) {
            std::cerr << "GAIL: FromWAV (load file): could not read BlockAlign" << std::endl;
            return NULL;
        }
        // bitsPerSample
        if(!file.read(buffer, 2)) {
            std::cerr << "GAIL: FromWAV (load file): could not read bits per sample" << std::endl;
            return NULL;
        }
        int bitsPerSample = toInt(buffer, 2);
        // Subchunk2ID (data)
        if(!file.read(buffer, 4)) {
            std::cerr << "GAIL: FromWAV (load file): could not read Subchunk2ID" << std::endl;
            return NULL;
        }
        if(std::strncmp(buffer, "data", 4) != 0) {
            std::cerr << "GAIL: FromWAV (load file): file is not a valid WAVE file (doesn't have 'data' tag)" << std::endl;
            return NULL;
        }
        // Subchunk2Size
        if(!file.read(buffer, 4)) {
            std::cerr << "GAIL: FromWAV (load file): could not read Subchunk2Size" << std::endl;
            return NULL;
        }
        int size = toInt(buffer, 4);
        /* cannot be at the end of file */
        if(file.eof()) {
            std::cerr << "GAIL: FromWAV (load file): No data." << std::endl;
            return NULL;
        }
        if(file.fail()) {
            std::cerr << "GAIL: FromWAV (load file): fail state set on the file" << std::endl;
            return NULL;
        }

        std::vector<char> data = std::vector<char>(size);
        file.read(data.data(), size);
        file.close();
        return Sound(sampleRate, data, channels==1&&bitsPerSample==8?SoundFormat::Mono8:channels==2&&bitsPerSample==8?SoundFormat::Stereo8:channels==1&&bitsPerSample==16?SoundFormat::Mono16:channels==2&&bitsPerSample==16?SoundFormat::Stereo16:SoundFormat::Stereo16);
    }
    std::vector<string> SoundCapture::GetCaptureDevices() {
        std::vector<string> captureDevices;
        const ALCchar* devices = alcGetString(nullptr, ALC_CAPTURE_DEVICE_SPECIFIER);
        const char* ptr = devices;

        captureDevices.clear();

        do {
            captureDevices.push_back(std::string(ptr));
            ptr += captureDevices.back().size() + 1;
        } while(*(ptr + 1) != '\0');
        return captureDevices;
    };
    SoundCapture::SoundCapture(SoundFormat format, int sampleRate, string captureDevice = nullptr) {
        this->captureDevice = alcCaptureOpenDevice(captureDevice.c_str(), sampleRate, format, sampleRate);
        this->sampleRate = sampleRate;
        this->format = format;
    };
    SoundCapture::~SoundCapture() {
        alcCaptureCloseDevice(this->captureDevice);
    };
    void SoundCapture::Start() {
        alcCaptureStart(this->captureDevice);
    };
    Sound SoundCapture::Stop() {
        alcCaptureStop(captureDevice);
        ALCint samplesAvailable;
        alcGetIntegerv(captureDevice, ALC_CAPTURE_SAMPLES, 1, &samplesAvailable);
        std::vector<char> buffer;
        if (samplesAvailable > 0) {
            buffer.resize(this->format==SoundFormat::Mono16||this->format==SoundFormat::Stereo16 ? samplesAvailable*2 : samplesAvailable);

            alcCaptureSamples(captureDevice, buffer.data(), samplesAvailable);
        }
        return Sound(this->sampleRate, buffer, this->format);
    };
}   