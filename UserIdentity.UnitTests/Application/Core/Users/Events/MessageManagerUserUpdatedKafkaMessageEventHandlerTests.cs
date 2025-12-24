using System.Threading.Tasks;

using FakeItEasy;

using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Core.Messages.Events;
using PolyzenKit.Common.Enums;

using UserIdentity.Application.Core.Users.Commands;
using UserIdentity.Application.Core.Users.Events;
using UserIdentity.Application.Core.Users.ViewModels;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Users.Events;

public class MessageManagerUserUpdatedKafkaMessageEventHandlerTests
{
  private readonly IUpdateItemCommandHandler<ConfirmUserCommand, ConfirmUserViewModel> _updateItemCommandHandler;

  public MessageManagerUserUpdatedKafkaMessageEventHandlerTests()
  {
    _updateItemCommandHandler = A.Fake<IUpdateItemCommandHandler<ConfirmUserCommand, ConfirmUserViewModel>>();
  }

  [Fact]
  public async Task HandleEventAsync_WhenActionIsWelcomeUser_CallsUpdateCommandHandler()
  {
    var eventItem = new MessageManagerUserUpdatedKafkaMessageEvent
    {
      MessageKey = "message-key",
      MessageValue = new MessageEvent
      {
        Action = MessageAction.WELCOME_USER,
        CorrelationId = "user-id",
        RegisteredApp = "TestApp",
        Attributes = []
      }
    };

    var handler = new MessageManagerUserUpdatedKafkaMessageEventHandler(_updateItemCommandHandler);

    await handler.HandleEventAsync(eventItem);

    A.CallTo(() => _updateItemCommandHandler.UpdateItemAsync(A<ConfirmUserCommand>.That.Matches(c => c.UserId == eventItem.MessageValue.CorrelationId), eventItem.MessageKey))
    .MustHaveHappenedOnceExactly();
  }

  [Fact]
  public async Task HandleEventAsync_WhenActionIsNotWelcomeUser_DoesNotCallUpdateCommandHandler()
  {
    var eventItem = new MessageManagerUserUpdatedKafkaMessageEvent
    {
      MessageKey = "message-key",
      MessageValue = new MessageEvent
      {
        Action = (MessageAction)999,
        CorrelationId = "user-id",
        RegisteredApp = "TestApp",
        Attributes = []
      }
    };

    var handler = new MessageManagerUserUpdatedKafkaMessageEventHandler(_updateItemCommandHandler);

    await handler.HandleEventAsync(eventItem);

    A.CallTo(() => _updateItemCommandHandler.UpdateItemAsync(A<ConfirmUserCommand>._, A<string>._))
      .MustNotHaveHappened();
  }
}
