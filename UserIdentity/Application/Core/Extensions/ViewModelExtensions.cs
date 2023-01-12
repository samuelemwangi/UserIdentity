using UserIdentity.Application.Enums;

namespace UserIdentity.Application.Core.Extensions
{
	public static class ViewModelExtensions
	{
		public static void ResolveRequestStatus(this BaseViewModel viewModel, RequestStatus requestStatus, ItemStatusMessage itemStatusMessage, String cusomStatusMessage = "")
		{
			viewModel.RequestStatus = requestStatus.GetDisplayName();
			viewModel.StatusMessage = cusomStatusMessage != "" ? cusomStatusMessage : itemStatusMessage.GetDisplayName();
		}

		public static void ResolveEditDeleteRights(this ItemDetailBaseViewModel viewModel, String? userRoleClaims, String entity, Boolean isLoggedInUser =  false)
		{
			// check if edit is enabled
			if (isLoggedInUser || (userRoleClaims != null && userRoleClaims.ToLower().Contains(entity.ToLower() + ":edit")))
				viewModel.EditEnabled = true;
			else
				viewModel.EditEnabled = false;

			// check if delete is enabled
			if (isLoggedInUser ||  (userRoleClaims != null && userRoleClaims.ToLower().Contains(entity.ToLower() + ":delete")))
				viewModel.DeleteEnabled = true;
			else
				viewModel.DeleteEnabled = false;

		}

		public static void ResolveCreateDownloadRights(this ItemsBaseViewModel viewModel, String? userRoleClaims, String entity, Boolean isLoggedInUser = false)
		{
			// check if create is enabled
			if (isLoggedInUser || (userRoleClaims != null && userRoleClaims.ToLower().Contains(entity.ToLower() + ":create")))
				viewModel.CreateEnabled = true;
			else
				viewModel.CreateEnabled = false;

			// check if download is enabled
			if (isLoggedInUser || (userRoleClaims != null && userRoleClaims.ToLower().Contains(entity.ToLower() + ":download")))
				viewModel.DownloadEnabled = true;
			else
				viewModel.DownloadEnabled = false;

		}
	}
}
