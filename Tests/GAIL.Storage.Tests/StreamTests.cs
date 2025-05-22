using GAIL.Serializing.Formatters;
using GAIL.Storage.Members;

namespace GAIL.Storage.Tests;

public class StreamTests {
    [Fact]
    public void SerializingAndParsing() {
        Storage original = new();
            Container container = new("test");
                container.AddChild(new IntField("BEEF", 0xBEEF));
                container.AddChild(new StringField("key", "value"));
        original.AddChild(container);

        Stream stream = new MemoryStream();
        original.Save(stream);

        stream.Position = 0;
        Storage latest = new();
        latest.Load(stream);

        Assert.Equal(0xBEEF, latest.Get<IntField>("test.BEEF")!.Value);
        Assert.Equal("value", latest.Get<StringField>("test.key")!.Value);
    }
    
    [Fact]
    public void Formatter() {
        IFormatter formatter = new GZipFormatter();
        Storage original = new(formatter);
            Container container = new("test");
                container.AddChild(new IntField("BEEF", 0xBEEF));
                container.AddChild(new StringField("key", "value"));
        original.AddChild(container);

        Stream stream = new MemoryStream();
        original.Save(stream);

        stream.Position = 0;
        Storage latest = new(formatter);
        latest.Load(stream);

        Assert.Equal(0xBEEF, latest.Get<IntField>("test.BEEF")!.Value);
        Assert.Equal("value", latest.Get<StringField>("test.key")!.Value);
    }
}