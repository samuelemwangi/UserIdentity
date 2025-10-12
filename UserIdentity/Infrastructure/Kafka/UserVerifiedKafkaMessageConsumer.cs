using PolyzenKit.Application.Core.Messages.Events;
using PolyzenKit.Application.Interfaces;

namespace UserIdentity.Infrastructure.Kafka;

public class UserVerifiedKafkaMessageConsumer(
  ILogHelper<UserVerifiedKafkaMessageConsumer> logHelper
  ) : IKafkaMessageConsumer
{
  private readonly ILogHelper<UserVerifiedKafkaMessageConsumer> _logHelper = logHelper;

  public async Task ConsumeAsync(MessageEvent message, CancellationToken cancellationToken = default)
  {
    _logHelper.LogEvent($"Consume Message {message}", LogLevel.Information);
  }
}

