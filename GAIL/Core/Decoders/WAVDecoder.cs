using GAIL.Audio;

namespace GAIL.Core
{
    public static partial class WAV {
        public partial class Decoder : IDecoder<Sound> {
            public void Dispose() {
                throw new NotImplementedException();
            }

            public Sound Parse(Stream stream) {
                string path = "Unknown path";
                if (stream is FileStream fs) {
                    path = fs.Name;
                }
                byte[] buffer = new byte[4];
                stream.Read(buffer, 0, 4);
                if (buffer != "RIFF".ToCharArray().Select(c => (byte)c)) {
                    throw new FileFormatException(path, "RIFF header doesn't match");
                }
                stream.Read(buffer, 8, 4);
                if (buffer != "WAVE".ToCharArray().Select(c => (byte)c)) {
                    throw new FileFormatException(path, "file format isn't WAVE");
                }
                stream.Read(buffer, 12, 4);
                if (buffer != "fmt ".ToCharArray().Select(c => (byte)c)) {
                    throw new FileFormatException(path, "'fmt ' header doesn't match");
                }
                stream.Read(buffer, 22, 2); // num channels
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                short channels = BitConverter.ToInt16(buffer, 0);
                stream.Read(buffer, 24, 4); // Sample rate
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                int sampleRate = BitConverter.ToInt32(buffer, 0);
                stream.Read(buffer, 34, 2); // BitsPerSample
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                short bitsPerSample = BitConverter.ToInt16(buffer, 0);

                stream.Read(buffer, 36, 4);
                if (buffer != "data".ToCharArray().Select(c => (byte)c)) {
                    throw new FileFormatException(path, "data header doesn't match");
                }
                stream.Read(buffer, 0, 4);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                int size = BitConverter.ToInt32(buffer, 0);
                byte[] data = new byte[size]; 
                stream.Read(data, 44, size);

                return new(sampleRate, channels==1&&bitsPerSample==8?SoundFormat.Mono8:channels==2&&bitsPerSample==8?SoundFormat.Stereo8:channels==1&&bitsPerSample==16?SoundFormat.Mono16:channels==2&&bitsPerSample==16?SoundFormat.Stereo16:SoundFormat.Stereo16, [.. data]);
            }
        }
    }
}