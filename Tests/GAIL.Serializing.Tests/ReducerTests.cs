using GAIL.Serializing.Streams;

namespace GAIL.Serializing.Tests;

public class TestReducer : IReducer, IEquatable<TestReducer> {
    private static ReducerInfo? info;
    /// <summary>
    /// Information on how to read and create this serializable.
    /// </summary>
    [SerializingInfo]
    public static ReducerInfo Info { get {
        if (info == null) {
            info = IReducer.CreateInfo(()=>new TestReducer("", default, false));
        }
        return info;
    } }
    public SerializableInfo[] Format => [StringSerializable.Info, IntSerializable.Info, BoolSerializable.Info];

    public TestReducer(string name, int id, bool isAdmin) {
        this.name = name;
        this.id = id;
        this.isAdmin = isAdmin;
    }
    public string name;
    public int id;
    public bool isAdmin;

    public void Parse(ISerializable[] serializables) {
        name = (serializables[0] as StringSerializable)!.Value;
        id = (serializables[1] as IntSerializable)!.Value;
        isAdmin = (serializables[2] as BoolSerializable)!.B1;
    }

    public ISerializable[] Serialize() {
        return [new StringSerializable(name), new IntSerializable(id), new BoolSerializable(isAdmin)];
    }

    public bool Equals(TestReducer? other) {
        if (other == null) return false;
        return name == other.name && id == other.id && isAdmin == other.isAdmin;
    }
}

public class ReducerTests {
    [Fact]
    public void Serializing() {
        TestReducer reducer = new("admin", 6, true);
        Serializer serializer = new();

        serializer.WriteReducer(reducer);

        byte[] gotten = (serializer.BaseStream as MemoryStream)!.ToArray();

        serializer.Dispose();

        TestReducer reducer2 = new("admin", 6, true);
        Serializer serializer2 = new();

        serializer2.WriteReducer(reducer);

        byte[] gotten2 = (serializer.BaseStream as MemoryStream)!.ToArray();

        serializer.Dispose();

        Assert.Equal(gotten, gotten2);
    }
    [Fact]
    public void SerializingAndParsing() {
        TestReducer reducer = new("admin", 6, true);
        Serializer serializer = new();
        Parser parser = new(serializer.BaseStream, false);

        serializer.WriteReducer(reducer);
        serializer.BaseStream.Position = 0;
        TestReducer reducer2 = parser.ReadReducer<TestReducer>(TestReducer.Info);

        Assert.True(reducer.Equals(reducer2));

        parser.Dispose();
        serializer.Dispose();
    }
}