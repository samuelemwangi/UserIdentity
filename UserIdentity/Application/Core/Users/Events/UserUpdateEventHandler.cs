using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Core.Messages.Events;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Common.Enums;

using UserIdentity.Application.Enums;
using UserIdentity.Application.Interfaces;
using UserIdentity.Domain.Identity;

namespace UserIdentity.Application.Core.Users.Events;

public record UserEventContent
{
  public string? UserIdentityId { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? UserName { get; set; }
  public string? PhoneNumber { get; set; }
  public bool PhoneNumberConfirmed { get; set; }
  public string? UserEmail { get; set; }
  public bool EmailConfirmed { get; set; }
}

public record UserUpdateEvent : IBaseEvent
{
  public RequestSource RequestSource { get; set; }

  public CrudEvent EventType { get; set; }

  public UserEventContent UserContent { get; set; } = null!;

  public RegisteredAppEntity RegisteredApp { get; set; } = null!;
}

public class UserUpdateEventHandler(
  IAppCallbackService appCallbackService,
  IKafkaMessageProducer<string, MessageEvent> kafkaMessageProducer
  ) : IBaseEventHandler<UserUpdateEvent>
{
  private readonly IAppCallbackService _appCallbackService = appCallbackService;
  private readonly IKafkaMessageProducer<string, MessageEvent> _kafkaMessageProducer = kafkaMessageProducer;

  public async Task HandleEventAsync(UserUpdateEvent eventItem)
  {
    if (eventItem.RequestSource == RequestSource.UI && eventItem.RegisteredApp.CallbackUrl != null)
      await _appCallbackService.SendCallbackRequestAsync(eventItem);

    var topicKey = "UserRegistered";

    var messageEvent = new MessageEvent
    {
      CorrelationId = eventItem.UserContent.UserIdentityId!,
      Action = MessageAction.SEND,
      Attributes = new()
      {
        { MessageAttribute.RECIPIENT, eventItem.UserContent.UserEmail! }
      }
    };

    await _kafkaMessageProducer.ProduceAsync(messageEvent.CorrelationId, messageEvent, topicKey);
  }
}
