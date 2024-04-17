namespace GAIL.Networking.Parser;

public record FormatData(byte[] Data, bool IsObject);

public static class PacketParser {
    private static readonly byte Seperator = 0x1D;
    private static readonly byte Allocator = 0x1E;
    private static readonly byte Scope = 0x1F;
    private static readonly byte[] IllegalCharacters = [Seperator, Allocator, Scope];
    private static readonly Dictionary<Type, IParser<object>> FieldParsers = [];
    public static bool ContainsFieldParser(Type type) {
        return FieldParsers.ContainsKey(type);
    }
    public static bool ContainsFieldParser<T>() {
        return ContainsFieldParser(typeof(T));
    }
    public static IParser<object> GetFieldParser(Type type) {
        if (!FieldParsers.TryGetValue(type, out IParser<object>? parser)) {
            throw new ArgumentException("No parser found for type: "+type.Name, nameof(type));
        }
        if (parser!.GetParserType()!=type) { throw new Exception("Corrupt parser: registered parser type: "+type.Name+", actual type: "+parser.GetParserType().Name); }
        return parser;
    }
    public static IParser<T> GetFieldParser<T>() {
        return (GetFieldParser(typeof(T)) as IParser<T>)!;
    }
    public static bool RegisterFieldParser<T>(IParser<T> parser) {
        if (parser == null) {return false;}
        if (FieldParsers.ContainsKey(typeof(T))) {return false;}
        FieldParsers.Add(typeof(T), (IParser<object>)parser);
        return true;
    }
    public static List<IField<object>> Parse(byte[] data, List<object> format) {

    }
    public static byte[] Format(List<IField<object>> data) {

    }
    public static byte[] Encode(IField<object> field, Type type) {
        if (field.Type!=type) { throw new InvalidOperationException("Type "+field.Type+" is not "+type.Name+"."); }
        (byte[] data, bool isObject) = GetFieldParser(type).Format(field.Value!);
        List<byte> newData = [];
        if (isObject) newData.Add(Scope);
        foreach (byte b in data) {
            newData.Add(b);
            if (IllegalCharacters.Contains(b)) newData.Add(b);
        }
        if (isObject) newData.Add(Scope);
        return [.. newData];
    }
    public static byte[] Encode<T>(IField<T> field) {
        return Encode(new IField<object>(field.Value!), typeof(T));
    }
    public static IField<object> Decode(byte[] data, Type type) {
        byte? previousByte = null;
        List<byte> newData = [];
        bool isObject = false;
        foreach (byte b in data) {
            if (previousByte == null && b == Scope && data[^1] == Scope) {
                isObject = true;
                continue;
            } 
            if (IllegalCharacters.Contains(b)) {
                if (previousByte == b||isObject) {
                    newData.Add(b);
                } else {
                    continue;
                }
            } else {
                newData.Add(b);
            }
            previousByte = b;
        }
        return new IField<object>(GetFieldParser(type).Parse([.. newData]));
    }
    public static IField<T> Decode<T>(byte[] data) {
        return (Decode(data, typeof(T)) as IField<T>)!;
    }

}