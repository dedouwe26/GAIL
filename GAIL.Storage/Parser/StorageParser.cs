
namespace GAIL.Storage.Parser;

public class StorageParser : Serializing.Streams.Parser
{
    public StorageParser(Stream input, bool shouldCloseStream = true) : base(input, shouldCloseStream) { }

    public StorageParser(byte[] input, bool shouldCloseStream = true) : base(input, shouldCloseStream) { }
}