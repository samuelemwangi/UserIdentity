using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using PolyzenKit.Application.Core.Messages.Events;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Infrastructure.Kafka;

namespace UserIdentity.IntegrationTests.TestUtils;

public sealed class KafkaProducerHelper(
  IConfiguration configuration,
  IKafkaMessageProducer<string, MessageEvent> kafkaMessageProducer
  )
{

  private readonly KafkaSettings _kafkaSettings = configuration.GetSetting<KafkaSettings>();
  private readonly IKafkaMessageProducer<string, MessageEvent> _kafkaMessageProducer = kafkaMessageProducer;

  public async Task ProduceMessageEventAsync(string key, MessageEvent messageEvent, string topic)
  {
    await _kafkaMessageProducer.ProduceAsync(key, messageEvent, topic);
  }

  public async Task ProduceUserMessageManagerUserUpdatedEventAsync(string key, MessageEvent messageEvent)
  {
    await ProduceMessageEventAsync(key, messageEvent, _kafkaSettings.ConsumerTopics!["MessageManagerUserUpdatedKafkaMessageConsumer"]);
  }
}
