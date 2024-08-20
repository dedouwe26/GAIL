using System.Security.Cryptography;
using System.Text;
using GAIL.Serializing.Streams;

namespace GAIL.Serializing.Formatters;

/// <summary>
/// Can encrypt / decrypt using AES.
/// </summary>
public class AESFormatter : IFormatter {
    private readonly byte[] Key;
    private readonly byte[] IV;
    /// <summary>
    /// Creates a new AES formatter.
    /// </summary>
    /// <param name="key">The key used for encrypting / decrypting.</param>
    /// <param name="iv">The initialization vector used by the AES algorithm.</param>
    public AESFormatter(byte[] key, byte[] iv) {
        Key = key;
        IV = iv;
    }
    /// <inheritdoc/>
    public byte[] Decode(byte[] encoded) {
        using Aes aes = Aes.Create();

        aes.Key = Key;
        aes.IV = IV;

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        byte[] decoded;
        using (MemoryStream encodedStream = new(encoded)) {
            using CryptoStream decryptedStream = new(encodedStream, decryptor, CryptoStreamMode.Read);
            using MemoryStream decodedStream = new();

            decryptedStream.CopyTo(decodedStream);
            decoded = decodedStream.ToArray();
        }
        return decoded;
    }

    /// <inheritdoc/>
    public byte[] Encode(byte[] original) {
        using Aes aes = Aes.Create();

        aes.Key = Key;
        aes.IV = IV;

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        byte[] encoded;
        using (MemoryStream encodedStream = new()) {
            using (CryptoStream encryptedStream = new(encodedStream, encryptor, CryptoStreamMode.Write)) {
                encryptedStream.Write(original, 0, original.Length);
            }

            encoded = encodedStream.ToArray();
        }

        return encoded;
    }
}