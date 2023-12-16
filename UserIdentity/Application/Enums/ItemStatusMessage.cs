using System.ComponentModel;

namespace UserIdentity.Application.Enums
{
	public enum ItemStatusMessage
	{
		[Description("Item created successfully")]
		CREATE_ITEM_SUCCESSFUL,


		[Description("Item updated successfully")]
		UPDATE_ITEM_SUCCESSFUL,


		[Description("Item fetched successfully")]
		FETCH_ITEM_SUCCESSFUL,

		[Description("Items fetched successfully")]
		FETCH_ITEMS_SUCCESSFUL,

		[Description("No items found")]
		FETCH_ITEMS_SUCCESSFUL_NO_ITEMS,

		[Description("Item deleted successfully")]
		DELETE_ITEM_SUCCESSFUL,


		[Description("Fetching item failed")]
		FETCH_ITEM_FAILED,

		[Description("Fetching items failed")]
		FETCH_ITEMS_FAILED,

		[Description("The requested item could not be found")]
		ITEM_NOT_FOUND
	}
}
