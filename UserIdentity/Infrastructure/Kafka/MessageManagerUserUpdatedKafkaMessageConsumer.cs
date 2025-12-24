using Microsoft.Extensions.Options;

using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Core.Messages.Events;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Infrastructure.Kafka;

using UserIdentity.Application.Core.Users.Events;

namespace UserIdentity.Infrastructure.Kafka;

public class MessageManagerUserUpdatedKafkaMessageConsumer(
  IEventHandler<MessageManagerUserUpdatedKafkaMessageEvent> eventHandler,
  IOptions<KafkaSettings> kafkaSettings,
  ILogHelper<MessageManagerUserUpdatedKafkaMessageConsumer> logHelper
  ) : IKafkaMessageConsumer
{

  private readonly IEventHandler<MessageManagerUserUpdatedKafkaMessageEvent> _eventHandler = eventHandler;
  private readonly Dictionary<string, string>? _consumerTopics = kafkaSettings.Value.ConsumerTopics;
  private readonly ILogHelper<MessageManagerUserUpdatedKafkaMessageConsumer> _logHelper = logHelper;

  public async Task ConsumeAsync(string key, MessageEvent message, CancellationToken cancellationToken = default)
  {
    var consumedEvent = new MessageManagerUserUpdatedKafkaMessageEvent
    {
      MessageKey = key,
      MessageValue = message
    };

    try
    {
      await _eventHandler.HandleEventAsync(consumedEvent);
    }
    catch (Exception exception)
    {
      var logMessage = $"TOPIC-{_consumerTopics?[nameof(MessageManagerUserUpdatedKafkaMessageConsumer)]} START >> | Exception Message :: {exception.Message ?? "NO-EXCEPTION-MESSAGE"}";
      logMessage += $"\n | Stack Trace :: {exception.StackTrace ?? "NO-STACK-TRACE"}";
      logMessage += $"\n | Inner Exception :: {exception.InnerException?.Message ?? "NO-INNER-EXCEPTION"} << END";

      _logHelper.LogEvent(logMessage, LogLevel.Error);
    }
  }
}

