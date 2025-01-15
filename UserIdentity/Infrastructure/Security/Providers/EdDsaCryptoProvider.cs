using System.Security.Cryptography;

using Microsoft.IdentityModel.Tokens;

namespace UserIdentity.Infrastructure.Security.Providers
{
	public class EdDsaCryptoProvider : ICryptoProvider
	{
		public object Create(string algorithm, params object[] args)
		{
			if (algorithm == EdDsaSecurityAlgorithmConstants.EdDsa && args[0] is EdDsaSecurityKey key)
			{
				return new EdDsaSignatureProvider(key, algorithm, (bool)args[1]);
			}

			throw new CryptographicException($"Unsupported algorithm: {algorithm}");
		}

		public bool IsSupportedAlgorithm(string algorithm, params object[] args) => algorithm == EdDsaSecurityAlgorithmConstants.EdDsa;

		public void Release(object cryptoInstance)
		{
			if (cryptoInstance is IDisposable disposableObject)
				disposableObject.Dispose();
		}
	}
}
