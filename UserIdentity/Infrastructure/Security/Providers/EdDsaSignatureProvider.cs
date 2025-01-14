using Microsoft.IdentityModel.Tokens;

using Org.BouncyCastle.Crypto.Signers;

namespace UserIdentity.Infrastructure.Security.Providers
{
	public class EdDsaSignatureProvider : SignatureProvider
	{
		private readonly Ed25519Signer _signer;
		private readonly EdDsaSecurityKey _edDsaKey;
		public EdDsaSignatureProvider(EdDsaSecurityKey key, string algorithm, bool willCreateSignatures) : base(key, algorithm)
		{
			_edDsaKey = key;
			_signer = new Ed25519Signer();
			WillCreateSignatures = willCreateSignatures;
		}

		public override byte[] Sign(byte[] input)
		{
			if (!WillCreateSignatures)
				throw new InvalidOperationException("Provider not configured for signing.");

			_signer.Init(true, _edDsaKey.PrivateKey);
			_signer.BlockUpdate(input, 0, input.Length);
			return _signer.GenerateSignature();
		}

		public override bool Verify(byte[] input, byte[] signature)
		{
			_signer.Init(false, _edDsaKey.PublicKey);
			_signer.BlockUpdate(input, 0, input.Length);
			return _signer.VerifySignature(signature);
		}

		protected override void Dispose(bool disposing)
		{
		}
	}
}
