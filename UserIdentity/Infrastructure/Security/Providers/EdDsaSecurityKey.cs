using Microsoft.IdentityModel.Tokens;

using Org.BouncyCastle.Crypto.Parameters;

namespace UserIdentity.Infrastructure.Security.Providers
{
	public class EdDsaSecurityKey : AsymmetricSecurityKey
	{
		private readonly Ed25519PrivateKeyParameters _privateKey;
		private readonly Ed25519PublicKeyParameters _publicKey;

		public EdDsaSecurityKey(Ed25519PrivateKeyParameters privateKey)
		{
			_privateKey = privateKey;
			_publicKey = privateKey.GeneratePublicKey();
		}

		public Ed25519PrivateKeyParameters PrivateKey => _privateKey;
		public Ed25519PublicKeyParameters PublicKey => _publicKey;

		public EdDsaSecurityKey(Ed25519PublicKeyParameters publicKey)
		{
			_publicKey = publicKey;
		}

		[Obsolete]
		public override bool HasPrivateKey => _privateKey != null;

		public override PrivateKeyStatus PrivateKeyStatus => _privateKey != null ? PrivateKeyStatus.Exists : PrivateKeyStatus.DoesNotExist;

		public override int KeySize => 256;
		public override bool IsSupportedAlgorithm(string algorithm) => algorithm == EdDsaSecurityAlgorithmConstants.EdDsa;
	}
}
