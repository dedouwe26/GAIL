namespace GAIL.Serializing.Tests;

public class SerializableTests {
    [Fact]
    public void SerializingAndParsing() {
        StringSerializable str1 = new("some name");
        
        byte[] rawStr1 = str1.Serialize();

        StringSerializable str2 = (StringSerializable.Info.Creator(rawStr1) as StringSerializable)!;

        Assert.Equal(str1.Value, str2.Value);

        LongSerializable long1 = new(Random.Shared.NextInt64());
        
        byte[] rawLong1 = long1.Serialize();

        LongSerializable long2 = (LongSerializable.Info.Creator(rawLong1) as LongSerializable)!;

        Assert.Equal(long1.Value, long2.Value);
    }
}