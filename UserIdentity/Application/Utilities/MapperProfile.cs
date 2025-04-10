using AutoMapper;

using UserIdentity.Application.Core.RegisteredApps.Commands;
using UserIdentity.Application.Core.RegisteredApps.ViewModels;
using UserIdentity.Domain.Identity;


namespace UserIdentity.Application.Utilities;

public class MapperProfile : Profile
{
	public MapperProfile()
	{
		#region Command to Entity
		CreateMap<CreateRegisteredAppCommand, RegisteredAppEntity>();
		#endregion

		#region Entity to DTO 
		CreateMap<RegisteredAppEntity, RegisteredAppDTO>();
		#endregion
	}
}
