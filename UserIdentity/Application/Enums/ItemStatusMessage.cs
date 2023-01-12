using System.ComponentModel;

namespace UserIdentity.Application.Enums
{
	public enum ItemStatusMessage
	{
		[Description("Item(s) fetched successfully")]
		SUCCESS,

		[Description("Fetching(s) item failed")]
		FAILED,

		[Description("The requested item(s) could not be found")]
		NOTFOUND
	}
}
