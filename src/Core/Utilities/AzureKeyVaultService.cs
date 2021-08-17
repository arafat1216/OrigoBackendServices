using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public class AzureKeyVaultService : IKeyVaultService
    {
        private readonly IConfiguration Configuration;

        private readonly string keyVaultUrl = "https://origov2-keyvault-dev.vault.azure.net/";

        private readonly KeyClient KeyClient;

        private readonly SecretClient SecretClient;

        private readonly string tenantId = "f48f3686-91a4-4a60-8216-0c3a5f878b40";

        private readonly string clientId = "f1f1664c-9206-47e4-a378-e522adea3a7f";

        private readonly string clientSecret = "CX.cW0oiv7fplEdqoRrrCub58iemrHWt.r";

        public AzureKeyVaultService(Configuration configuration = new Configuration(this))
        {
            Configuration = configuration;
            // TODO: put these values in secrets.json
            Environment.SetEnvironmentVariable("AZURE_TENANT_ID", tenantId);
            Environment.SetEnvironmentVariable("AZURE_CLIENT_ID", clientId);
            Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", clientSecret);
            // Create a new key client using the default credential from Azure.Identity using environment variables previously set,
            // including AZURE_CLIENT_ID, AZURE_CLIENT_SECRET, and AZURE_TENANT_ID.
            KeyClient = new KeyClient(vaultUri: new Uri(keyVaultUrl), credential: new DefaultAzureCredential());
            // Create a new secret client using the default credential from Azure.Identity using environment variables previously set,
            // including AZURE_CLIENT_ID, AZURE_CLIENT_SECRET, and AZURE_TENANT_ID.
            SecretClient = new SecretClient(vaultUri: new Uri(keyVaultUrl), credential: new DefaultAzureCredential());
        }

        /// <summary>
        /// Gets an encryption key, certificate or other key type from the the Azure key vault.
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public async Task<KeyVaultKey> GetKeyAsync(string keyName, CancellationToken cancellationToken = default)
        {
            var key = await KeyClient.GetKeyAsync(keyName, cancellationToken: cancellationToken);
            return key;
        }

        public async Task<DeletedKey> GetDeletedKeyAsync(string name, CancellationToken cancellationToken = default)
        {
            var key = await KeyClient.GetDeletedKeyAsync(name, cancellationToken);
            return key;
        }

        /// <summary>
        /// Creates an encryption key, certificate or other key type and adds it to the Azure key vault.
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public async Task<KeyVaultKey> CreateKeyAsync(string keyName, CancellationToken cancellationToken = default)
        {
            // Create a key of any type
            KeyVaultKey key = await KeyClient.CreateKeyAsync(keyName, KeyType.Rsa, cancellationToken: cancellationToken);
            return key;
        }

        public async Task<KeyVaultKey> ImportKeyAsync(string name, JsonWebKey keyMaterial, CancellationToken cancellationToken = default)
        {
            var key = await KeyClient.ImportKeyAsync(name, keyMaterial, cancellationToken);
            return key;
        }

        public async Task<DeleteKeyOperation> DeleteKeyAsync(string name, CancellationToken cancellationToken = default)
        {
            var key = await KeyClient.StartDeleteKeyAsync(name, cancellationToken);
            return key;
        }

        public async Task<RecoverDeletedKeyOperation> RecoverDeletedKeyAsync(string name, CancellationToken cancellationToken = default)
        {
            var key = await KeyClient.StartRecoverDeletedKeyAsync(name, cancellationToken);
            return key;
        }

        /// <summary>
        /// Gets a secret value such as passwords, client secrets or API keys from the Azure key vault.
        /// </summary>
        /// <param name="secretName"></param>
        /// <returns></returns>
        public async Task<KeyVaultSecret> GetSecretAsync(string secretName, CancellationToken cancellationToken = default)
        {
            // Retrieve a secret using the secret client.
            var key = await SecretClient.GetSecretAsync(secretName, cancellationToken: cancellationToken);
            return key;
        }

        public async Task<DeletedSecret> GetDeletedSecretAsync(string name, CancellationToken cancellationToken = default)
        {
            // Retrieve a secret that has been deleted using the secret client.
            var secret = await SecretClient.GetDeletedSecretAsync(name, cancellationToken);
            return secret;
        }

        /// <summary>
        /// Save a secret value such as passwords, client secrets or API keys in the Azure key vault.
        /// </summary>
        /// <param name="secretName"></param>
        /// <param name="secretValue"></param>
        /// <returns></returns>
        public async Task<KeyVaultSecret> SetSecretAsync(string secretName, string secretValue, CancellationToken cancellationToken = default)
        {
            // Create a new secret using the secret client.
            var secret = await SecretClient.SetSecretAsync(secretName, secretValue, cancellationToken);
            return secret;
        }

        public async Task<DeleteSecretOperation> DeleteSecretAsync(string name, CancellationToken cancellationToken = default)
        {
            var secret = await SecretClient.StartDeleteSecretAsync(name, cancellationToken);
            return secret;
        }

        public async Task<RecoverDeletedSecretOperation> RecoverDeletedSecretAsync(string name, CancellationToken cancellationToken = default)
        {
            var secret = await SecretClient.StartRecoverDeletedSecretAsync(name, cancellationToken);
            return secret;
        }
    }
}
