using System.Net.Http;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using PolyzenKit.Common.Enums;
using PolyzenKit.Domain.RegisteredApps;
using PolyzenKit.Infrastructure.ExternalServices;
using PolyzenKit.Infrastructure.Security.Jwt;
using PolyzenKit.Presentation.Settings;

using UserIdentity.Application.Core.Users.Events;
using UserIdentity.Application.Enums;
using UserIdentity.Infrastructure.ExternalServices;

using Xunit;

namespace UserIdentity.UnitTests.Infrastructure.ExternalServices;

public class AppCallbackServiceTests
{
  private readonly IHttpClientHelper _httpClientHelper;
  private readonly IJwtTokenHandler _jwtTokenHandler;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly IOptions<RoleSettings> _roleSettingsOptions;

  public AppCallbackServiceTests()
  {
    _httpClientHelper = A.Fake<IHttpClientHelper>();
    _jwtTokenHandler = A.Fake<IJwtTokenHandler>();
    _httpContextAccessor = A.Fake<IHttpContextAccessor>();
    _httpContextAccessor.HttpContext = new DefaultHttpContext();
    _roleSettingsOptions = Options.Create(new RoleSettings
    {
      ServiceName = "UserIdentity",
      AdminRoles = string.Empty,
      DefaultRole = "user"
    });
  }

  [Fact]
  public async Task SendCallbackRequestAsync_ForwardsRequestToHttpClientHelper()
  {
    var registeredApp = new RegisteredAppEntity
    {
      AppName = "TestApp",
      CallbackUrl = "https://callback.test/cb"
    };

    var userUpdateEvent = new UserUpdateEvent
    {
      RegisteredApp = registeredApp,
      RequestSource = RequestSource.UI,
      EventType = CrudEvent.CREATE,
      UserContent = new UserEventContent { UserIdentityId = "user-id" }
    };

    var requestMessage = new HttpRequestMessage();
    A.CallTo(_httpClientHelper)
      .Where(call => call.Method.Name == nameof(IHttpClientHelper.CreateHttpRequestMessage))
      .WithReturnType<HttpRequestMessage>()
      .Returns(requestMessage);

    A.CallTo(_httpClientHelper)
      .Where(call => call.Method.Name == nameof(IHttpClientHelper.SendRequestAsync))
      .WithReturnType<Task>()
      .Returns(Task.CompletedTask);

    var service = new AppCallbackService(_httpClientHelper, _jwtTokenHandler, _httpContextAccessor, _roleSettingsOptions);

    await service.SendCallbackRequestAsync(userUpdateEvent);

    A.CallTo(_httpClientHelper)
      .Where(call => call.Method.Name == nameof(IHttpClientHelper.CreateHttpRequestMessage))
      .MustHaveHappenedOnceExactly();
    A.CallTo(_httpClientHelper)
      .Where(call => call.Method.Name == nameof(IHttpClientHelper.SendRequestAsync)
        && call.GetArgument<string>(0) == "AppCallback")
      .MustHaveHappenedOnceExactly();
    Assert.NotNull(requestMessage.Content);
  }

  [Fact]
  public async Task SendCallbackRequestAsync_WithCustomHeaders_AddsHeadersToRequest()
  {
    var registeredApp = new RegisteredAppEntity
    {
      AppName = "TestApp",
      CallbackUrl = "https://callback.test/cb",
      CallbackHeaders = new() { { "X-Test-Header", "value" } }
    };

    var userUpdateEvent = new UserUpdateEvent
    {
      RegisteredApp = registeredApp,
      RequestSource = RequestSource.UI,
      EventType = CrudEvent.CREATE,
      UserContent = new UserEventContent { UserIdentityId = "user-id" }
    };

    var requestMessage = new HttpRequestMessage();
    A.CallTo(_httpClientHelper)
      .Where(call => call.Method.Name == nameof(IHttpClientHelper.CreateHttpRequestMessage))
      .WithReturnType<HttpRequestMessage>()
      .Returns(requestMessage);

    A.CallTo(_httpClientHelper)
      .Where(call => call.Method.Name == nameof(IHttpClientHelper.SendRequestAsync))
      .WithReturnType<Task>()
      .Returns(Task.CompletedTask);

    var service = new AppCallbackService(_httpClientHelper, _jwtTokenHandler, _httpContextAccessor, _roleSettingsOptions);

    await service.SendCallbackRequestAsync(userUpdateEvent);

    Assert.True(requestMessage.Headers.Contains("X-Test-Header"));
    A.CallTo(_httpClientHelper)
      .Where(call => call.Method.Name == nameof(IHttpClientHelper.SendRequestAsync)
        && call.GetArgument<string>(0) == "AppCallback")
      .MustHaveHappenedOnceExactly();
  }
}
