using GAIL.Serializing.Streams;

namespace GAIL.Serializing.Tests;

public class TestReducer : IReducer {
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
}

public class ReducerTests {
    [Fact]
    public void Serializing() {
        TestReducer reducer = new("admin", 6, true);

        Serializer serializer = new();

        serializer.WriteReducer(reducer);

        byte[] gotten = (serializer.BaseStream as MemoryStream)!.ToArray();
    }
}