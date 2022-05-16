using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;

namespace Common.Interfaces
{
    interface IKeyVaultService
    {
        /// <summary>
        /// Gets an encryption key, certificate or other key type from the the Azure key vault.
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        Task<KeyVaultKey> GetKeyAsync(string keyName, CancellationToken cancellationToken = default);

        Task<DeletedKey> GetDeletedKeyAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates an encryption key, certificate or other key type and adds it to the Azure key vault.
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        Task<KeyVaultKey> CreateKeyAsync(string keyName, CancellationToken cancellationToken = default);

        Task<KeyVaultKey> ImportKeyAsync(string name, JsonWebKey keyMaterial, CancellationToken cancellationToken = default);

        Task<DeleteKeyOperation> DeleteKeyAsync(string name, CancellationToken cancellationToken = default);

        Task<RecoverDeletedKeyOperation> RecoverDeletedKeyAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a secret value such as passwords, client secrets or API keys from the Azure key vault.
        /// </summary>
        /// <param name="secretName"></param>
        /// <returns></returns>
        Task<KeyVaultSecret> GetSecretAsync(string secretName, CancellationToken cancellationToken = default);

        Task<DeletedSecret> GetDeletedSecretAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Save a secret value such as passwords, client secrets or API keys in the Azure key vault.
        /// </summary>
        /// <param name="secretName"></param>
        /// <param name="secretValue"></param>
        /// <returns></returns>
        Task<KeyVaultSecret> SetSecretAsync(string secretName, string secretValue, CancellationToken cancellationToken = default);

        Task<DeleteSecretOperation> DeleteSecretAsync(string name, CancellationToken cancellationToken = default);

        Task<RecoverDeletedSecretOperation> RecoverDeletedSecretAsync(string name, CancellationToken cancellationToken = default);
    }
}
