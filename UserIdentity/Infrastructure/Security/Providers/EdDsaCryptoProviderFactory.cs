using Microsoft.IdentityModel.Tokens;

namespace UserIdentity.Infrastructure.Security.Providers
{
	public class EdDsaCryptoProviderFactory : CryptoProviderFactory
	{
		public override SignatureProvider CreateForSigning(SecurityKey key, string algorithm)
		{
			if (key is EdDsaSecurityKey edDsaKey && algorithm == EdDsaSecurityAlgorithmConstants.EdDsa)
			{
				return new EdDsaSignatureProvider(edDsaKey, algorithm, true);
			}

			return base.CreateForSigning(key, algorithm);
		}

		public override SignatureProvider CreateForVerifying(SecurityKey key, string algorithm)
		{
			if (key is EdDsaSecurityKey edDsaKey && algorithm == EdDsaSecurityAlgorithmConstants.EdDsa)
			{
				return new EdDsaSignatureProvider(edDsaKey, algorithm, false);
			}

			return base.CreateForVerifying(key, algorithm);
		}
	}
}
