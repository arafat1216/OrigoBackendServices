using System.Security;
using System.Security.Cryptography;

namespace Common.Cryptography;

/// <summary>
///     Hashing values according to best practices. Can be used for password hashing.
///     Taken from https://stackoverflow.com/a/73125177
/// </summary>
public static class SecretHasher
{
    private const int SALT_SIZE = 16; // 128 bits
    private const int KEY_SIZE = 32; // 256 bits
    private const int ITERATIONS = 100000;
    private static readonly HashAlgorithmName _algorithm = HashAlgorithmName.SHA256;

    private const char SEGMENT_DELIMITER = ':';

    /// <summary>
    ///     Hashes a secret 100 000 times to ensure the safety of the hashing.
    /// </summary>
    /// <param name="secret"></param>
    /// <returns></returns>
    public static string Hash(string secret)
    {
        var salt = RandomNumberGenerator.GetBytes(SALT_SIZE);
        var key = Rfc2898DeriveBytes.Pbkdf2(secret, salt, ITERATIONS, _algorithm, KEY_SIZE);
        return string.Join(SEGMENT_DELIMITER, Convert.ToHexString(key), Convert.ToHexString(salt), ITERATIONS,
            _algorithm);
    }

    /// <summary>
    ///     Verifies the secret against a previously hashed value.
    /// </summary>
    /// <param name="secret">The secret to check if it is correct</param>
    /// <param name="hash">The previously hashed value of the original secret</param>
    /// <returns></returns>
    public static bool Verify(string secret, string hash)
    {
        var segments = hash.Split(SEGMENT_DELIMITER);
        var key = Convert.FromHexString(segments[0]);
        var salt = Convert.FromHexString(segments[1]);
        var iterations = int.Parse(segments[2]);
        var algorithm = new HashAlgorithmName(segments[3]);
        var inputSecretKey = Rfc2898DeriveBytes.Pbkdf2(secret, salt, iterations, algorithm, key.Length);
        return key.SequenceEqual(inputSecretKey);
    }
}