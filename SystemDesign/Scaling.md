# Стратегия масштабирования

## Производительность компонентов

### API Gateway + Lambda
**≈ 10,000 RPS** (при 1,000 concurrency и 100ms среднем ответе)
- **Источник**: [AWS API Gateway Quotas](https://docs.aws.amazon.com/apigateway/latest/developerguide/limits.html)
- **По умолчанию**: 10,000 RPS на регион, можно запросить увеличение до 50,000 RPS
- **Lambda Concurrency**: [AWS Lambda Limits](https://docs.aws.amazon.com/lambda/latest/dg/gettingstarted-limits.html) — по умолчанию 1,000 concurrent executions, up to tens of thousands

### DynamoDB
**≈ до 40,000 RPS** чтения и 40,000 RPS записи на таблицу
- **Источник**: [DynamoDB Performance](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/ProvisionedThroughput.html)
- **On-Demand**: Автоматически масштабируется
- **Hot Partition**: Один partition до 3,000 RCU и 1,000 WCU — требуется шардирование ключей ([Partition Management](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/HowItWorks.Partitions.html))

### Aurora
**~ 2,000–5,000 QPS** на r5.large/xlarge
- **Источник**: [Aurora Performance](https://docs.aws.amazon.com/AmazonRDS/latest/AuroraUserGuide/Aurora.Overview.Performance.html)
- **r5.large**: ~2,000-3,000 QPS
- **r5.xlarge**: ~4,000-5,000 QPS
- **Read Replicas**: Могут увеличить производительность чтения в 2-5 раз

## Стратегии масштабирования

### Горизонтальное масштабирование
- **Lambda**: Масштабирование до 10,000+ concurrent executions по запросу
- **API Gateway**: Масштабирование выше 10,000 RPS по запросу
- **DynamoDB**: On-Demand режим для автоматического масштабирования до 40,000 RPS

### Вертикальное масштабирование
- **Aurora**: Увеличение размера инстанса (r5.large → r5.xlarge → r5.2xlarge)
- **Read Replicas**: Добавление реплик для распределения нагрузки чтения

**Важно:** указанные цифры могут зависеть от региона размещения