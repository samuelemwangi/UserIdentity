using System.ComponentModel;

namespace UserIdentity.Application.Enums
{
	public static class EnumExtensions
	{
		public static String? GetDisplayName(this Enum val)
		{
			if (val == null) return null;


#pragma warning disable CS8602 // Dereference of a possibly null reference.
			DescriptionAttribute[] attributes = (DescriptionAttribute[])val
					 .GetType()
					 .GetField(val.ToString())
					 .GetCustomAttributes(typeof(DescriptionAttribute), false);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

			return attributes.Length > 0 ? attributes[0].Description : String.Empty;

		}
	}
}
