using System.Threading.Tasks;

using PolyzenKit.Application.Core.Messages.Events;
using PolyzenKit.Common.Enums;

using Xunit;
using Xunit.Abstractions;

namespace UserIdentity.IntegrationTests.Infrastructure.Kafka;

public class MessageManagerUserUpdatedKafkaMessageConsumerTests(
    TestingWebAppFactory testingWebAppFactory,
    ITestOutputHelper outputHelper
  ) : BaseIntegrationTests(testingWebAppFactory, outputHelper)
{

  [Fact]
  public async Task Consumer_MessageManagerUserUpdatedKafkaMessageEvent_Consumes_Produced_Message_Successfully()
  {
    // Arrange
    var user = _testDbHelper.CreateIdentityUser();
    user.EmailConfirmed = false;
    _testDbHelper.UpdateIdentityUser(user);
    var existingUser = _testDbHelper.GetIdentityUserWithNoTracking(user.Id);

    // Confirm user email is not confirmed
    Assert.NotNull(existingUser);
    Assert.False(existingUser.EmailConfirmed);

    var messageEvent = new MessageEvent
    {
      CorrelationId = user.Id,
      EventType = CrudEvent.UPDATE,
      RegisteredApp = _registeredApp.AppName,
      Action = MessageAction.WELCOME_USER,
      Attributes = new()
      {
        { MessageAttribute.USER_ID, user.Id }
      }
    };

    // Act 
    await _kafkaProducerHelper.ProduceUserMessageManagerUserUpdatedEventAsync(user.Id, messageEvent);


    // Assert
    await _pollyResiliencePipeline.ExecuteAsync(async token =>
    {
      var updatedUser = _testDbHelper.GetIdentityUserWithNoTracking(user.Id);
      Assert.NotNull(updatedUser);
      Assert.True(updatedUser.EmailConfirmed);
    });
  }
}
