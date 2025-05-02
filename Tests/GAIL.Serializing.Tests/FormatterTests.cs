using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;

namespace GAIL.Serializing.Tests;

public class FormatterTests {
    [Fact]
    public void SerializingAndParsing() {
        byte[] actual = [.. new byte[10].Select((_, index) => Convert.ToByte(index))];
        {
            GZipFormatter formatter = new();
            
            byte[] encoded = formatter.Encode(actual);
            byte[] decoded = formatter.Decode(encoded);

            Assert.Equal(actual, decoded);
        }
        {
            byte[] key = new byte[32];
            byte[] iv = new byte[16];
            Random.Shared.NextBytes(key);
            Random.Shared.NextBytes(iv);
            AESFormatter formatter = new(key, iv);

            byte[] encoded = formatter.Encode(actual);
            byte[] decoded = formatter.Decode(encoded);

            Assert.Equal(actual, decoded);
        }
    }
    [Fact]
    public void Streams() {
        Serializer serializer = new();
        
        string actual = "This string will probably be compressed.";
        serializer.WriteString(actual, new GZipFormatter());

        serializer.BaseStream.Position = 0;
        Parser parser = new(serializer.BaseStream, false);

        string gotten = parser.ReadString(new GZipFormatter());

        parser.Dispose();
        serializer.Dispose();

        Assert.Equal(actual, gotten);
    }
}