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

		public override byte[] Sign(byte[] input, int offset, int count)
		{
			if (!WillCreateSignatures)
				throw new InvalidOperationException("Provider not configured for signing.");

			_signer.Init(true, _edDsaKey.PrivateKey);
			_signer.BlockUpdate(input, offset, count);
			return _signer.GenerateSignature();
		}

		public override bool Sign(ReadOnlySpan<byte> data, Span<byte> destination, out int bytesWritten)
		{
			var signature = Sign(data.ToArray());
			signature.CopyTo(destination);
			bytesWritten = signature.Length;
			return true;
		}

		public override bool Verify(byte[] input, byte[] signature)
		{
			_signer.Init(false, _edDsaKey.PublicKey);

			_signer.BlockUpdate(input, 0, input.Length);

			return _signer.VerifySignature(signature);
		}

		public override bool Verify(byte[] input, int inputOffset, int inputLength, byte[] signature, int signatureOffset, int signatureLength)
		{
			ArgumentNullException.ThrowIfNull(input);
			ArgumentNullException.ThrowIfNull(signature);
			if (inputLength <= 0) throw new ArgumentException($"{nameof(inputLength)} must be greater than 0");
			if (signatureLength <= 0) throw new ArgumentException($"{nameof(signatureLength)} must be greater than 0");

			return Verify(input.Skip(inputOffset).Take(inputLength).ToArray(), signature.Skip(signatureOffset).Take(signatureLength).ToArray());
		}

		protected override void Dispose(bool disposing)
		{
		}
	}
}
