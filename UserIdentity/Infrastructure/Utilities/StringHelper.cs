﻿using System.Text;

using UserIdentity.Application.Interfaces.Utilities;

namespace UserIdentity.Infrastructure.Utilities
{
	public class StringHelper : IStringHelper
	{

		public string AddSpacesToSentence(string text, bool preserveAcronyms)
		{
			if (string.IsNullOrWhiteSpace(text))
				return string.Empty;

			var newText = new StringBuilder(text.Length * 2);

			newText.Append(text[0]);
			for (int i = 1; i < text.Length; i++)
			{
				if (char.IsUpper(text[i]))
					if (text[i - 1] != ' ' && !char.IsUpper(text[i - 1]) ||
							preserveAcronyms && char.IsUpper(text[i - 1]) &&
							 i < text.Length - 1 && !char.IsUpper(text[i + 1]))
						newText.Append(' ');
				newText.Append(text[i]);
			}

			return newText.ToString();
		}
	}
}
