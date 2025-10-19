using PolyzenKit.Application.Core.Messages.Events;
using PolyzenKit.Application.Interfaces;

namespace UserIdentity.Infrastructure.Kafka;

public class UserVerifiedKafkaMessageConsumer(
  ILogHelper<UserVerifiedKafkaMessageConsumer> logHelper
  ) : IKafkaMessageConsumer
{
  private readonly ILogHelper<UserVerifiedKafkaMessageConsumer> _logHelper = logHelper;

  public async Task ConsumeAsync(string key, MessageEvent message, CancellationToken cancellationToken = default)
  {
    _logHelper.LogEvent($"Consume Message {message} with key {key}", LogLevel.Information);
  }
}

