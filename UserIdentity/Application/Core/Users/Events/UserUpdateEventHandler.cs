using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Core.Messages.Events;
using PolyzenKit.Application.Enums;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Common.Enums;
using PolyzenKit.Domain.RegisteredApps;
using UserIdentity.Application.Enums;
using UserIdentity.Application.Interfaces;

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
  ) : IEventHandler<UserUpdateEvent>
{
  private readonly IAppCallbackService _appCallbackService = appCallbackService;
  private readonly IKafkaMessageProducer<string, MessageEvent> _kafkaMessageProducer = kafkaMessageProducer;

  private const string _userUpdateEventTopicKey = "UserUpdated";

  public async Task HandleEventAsync(UserUpdateEvent eventItem)
  {
    if (eventItem.RequestSource == RequestSource.UI && eventItem.RegisteredApp.CallbackUrl != null)
      await _appCallbackService.SendCallbackRequestAsync(eventItem);

    if (eventItem.EventType != CrudEvent.CREATE)
      return;

    var messageEvent = new MessageEvent
    {
      EventType = eventItem.EventType,
      CorrelationId = eventItem.UserContent.UserIdentityId!,
      Action = MessageAction.WELCOME_USER,
      RegisteredApp = eventItem.RegisteredApp.AppName,
      Attributes = new()
        {
          { MessageAttribute.RECIPIENT_EMAIL, eventItem.UserContent.UserEmail! },
          { MessageAttribute.RECIPIENT_PHONE, eventItem.UserContent.PhoneNumber ?? "" },
          { MessageAttribute.FIRST_NAME, eventItem.UserContent.FirstName! },
          { MessageAttribute.LAST_NAME, eventItem.UserContent.LastName! },
          { MessageAttribute.USER_NAME, eventItem.UserContent.UserName! },
          { MessageAttribute.USER_ID, eventItem.UserContent.UserIdentityId! }
        }
    };

    await _kafkaMessageProducer.ProduceAsync(messageEvent.CorrelationId, messageEvent, _userUpdateEventTopicKey);

  }
}
