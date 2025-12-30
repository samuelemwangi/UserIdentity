using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Core.Messages.Events;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Common.Enums;
using PolyzenKit.Infrastructure.Kafka;

using UserIdentity.Application.Core.Users.Events;
using UserIdentity.Infrastructure.Kafka;

using Xunit;

namespace UserIdentity.UnitTests.Infrastructure.Kafka;

public class MessageManagerUserUpdatedKafkaMessageConsumerTests
{
  private readonly IEventHandler<MessageManagerUserUpdatedKafkaMessageEvent> _eventHandler;
  private readonly ILogHelper<MessageManagerUserUpdatedKafkaMessageConsumer> _logHelper;
  private readonly IOptions<KafkaSettings> _kafkaSettings;

  private static readonly MessageEvent _messageEvent = new()
  {
    EventType = CrudEvent.CREATE,
    Action = MessageAction.WELCOME_USER,
    CorrelationId = "correlation-id",
    RegisteredApp = "app",
    Attributes = new Dictionary<MessageAttribute, string>()
  };

  public MessageManagerUserUpdatedKafkaMessageConsumerTests()
  {
    _eventHandler = A.Fake<IEventHandler<MessageManagerUserUpdatedKafkaMessageEvent>>();
    _logHelper = A.Fake<ILogHelper<MessageManagerUserUpdatedKafkaMessageConsumer>>();
    _kafkaSettings = Options.Create(new KafkaSettings
    {
      BootstrapServers = "localhost:9092",
      ConsumerTopics = new Dictionary<string, string>
      {
        { nameof(MessageManagerUserUpdatedKafkaMessageConsumer), "topic-1" }
      }
    });
  }

  [Fact]
  public async Task ConsumeAsync_InvokesEventHandler()
  {
    var consumer = new MessageManagerUserUpdatedKafkaMessageConsumer(_eventHandler, _kafkaSettings, _logHelper);

    await consumer.ConsumeAsync("key", _messageEvent);

    A.CallTo(() => _eventHandler.HandleEventAsync(A<MessageManagerUserUpdatedKafkaMessageEvent>.That.Matches(e => e.MessageKey == "key" && e.MessageValue == _messageEvent)))
      .MustHaveHappenedOnceExactly();
  }

  [Fact]
  public async Task ConsumeAsync_WhenHandlerThrows_LogsError()
  {
    A.CallTo(() => _eventHandler.HandleEventAsync(A<MessageManagerUserUpdatedKafkaMessageEvent>._))
      .ThrowsAsync(new InvalidOperationException("boom"));

    var consumer = new MessageManagerUserUpdatedKafkaMessageConsumer(_eventHandler, _kafkaSettings, _logHelper);

    await consumer.ConsumeAsync("key", _messageEvent);

    A.CallTo(() => _logHelper.LogEvent(
      A<string>.That.Matches(s => s.Contains("TOPIC-topic-1") && s.Contains("boom")),
      LogLevel.Error)).MustHaveHappenedOnceExactly();
  }
}
