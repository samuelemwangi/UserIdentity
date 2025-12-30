using System;
using System.Threading.Tasks;

using Confluent.Kafka;

using Microsoft.Extensions.Configuration;

using PolyzenKit.Infrastructure.Kafka;

namespace UserIdentity.IntegrationTests.TestUtils;

public sealed class KafkaConsumerHelper
{

  private readonly static string _kafkaConsumerGroupId = $"Integration-Tests-Consumer-Group-{Guid.NewGuid()}";

  private readonly KafkaSettings _kafkaSettings;
  private readonly KafkaJustInTimeConsumer _kafkaJustInTimeConsumer;

  public KafkaConsumerHelper(IConfiguration configuration)
  {
    _kafkaSettings = configuration.GetSetting<KafkaSettings>();
    _kafkaJustInTimeConsumer = new KafkaJustInTimeConsumer(_kafkaSettings.BootstrapServers, _kafkaConsumerGroupId, AutoOffsetReset.Earliest);
  }


  public async Task<T?> WaitAndConsumeMessageAsync<T>(string targetTopic)
  {
    return await _kafkaJustInTimeConsumer.WaitAndConsumeMessageAsync<T>(targetTopic, 2);
  }

  public async Task<T?> WaitAndConsumeUserUpdatedMessageAsync<T>()
  {
    return await WaitAndConsumeMessageAsync<T>(_kafkaSettings.ProducerTopics!["UserUpdated"]);
  }
}
