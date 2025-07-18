# Мониторинг, алертинг и трейсинг

## Мониторинг

### CloudWatch Metrics

#### API Gateway
- **Count**: Количество запросов
- **Latency**: Время ответа (p50, p95, p99)
- **4XXError**: Ошибки клиента
- **5XXError**: Ошибки сервера

#### Lambda
- **Invocations**: Количество вызовов
- **Duration**: Время выполнения
- **Errors**: Количество ошибок
- **ConcurrentExecutions**: Одновременные выполнения

#### DynamoDB
- **ConsumedReadCapacityUnits**: Потребленные RCU
- **ConsumedWriteCapacityUnits**: Потребленные WCU
- **ThrottledRequests**: Отклоненные запросы

#### Aurora
- **CPUUtilization**: Загрузка CPU
- **DatabaseConnections**: Количество подключений
- **FreeableMemory**: Свободная память
- **ReadIOPS**: Операции чтения
- **WriteIOPS**: Операции записи

## Алертинг

### Критические алерты (P0)
- **API Gateway 5XX Error Rate > 1%** (5 минут)
- **Lambda Error Rate > 5%** (5 минут)
- **Database Connection Failure** (1 минута)
- **API Gateway Latency > 2s** (p95, 5 минут)
- **Lambda Duration > 20s** (p95, 5 минут)
- **Database CPU > 90%** (5 минут)

## Трейсинг

### Корреляция логов
```csharp
public class CorrelationMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var traceId = context.Request.Headers["X-Trace-Id"].FirstOrDefault() 
                     ?? GenerateTraceId();
        
        context.Response.Headers["X-Trace-Id"] = traceId;
        
        using (LogContext.PushProperty("TraceId", traceId))
        {
            await next(context);
        }
    }
}
```

## Дашборды

### Операционный дашборд
- **Real-time метрики**: RPS, Latency, Error Rate
- **Активные алерты**: Список текущих проблем
- **Топ ошибок**: Самые частые ошибки

### Бизнес-дашборд
- **Заказы**: Созданные, отмененные, выполненные
- **Платежи**: Успешные, неудачные, возвраты
- **Товары**: Популярные, с низким остатком

### Дашборд производительности
- **Database**: CPU, Memory, Connections
- **Cache**: Hit Rate, Miss Rate
- **Lambda**: Cold Starts, Duration, Errors
- **External APIs**: Response Time, Error Rate

## SLO

**Доступность**: 99.9% uptime (8.76 часов downtime в год)
**Латентность**: 95% запросов < 500ms
**Throughput**: Поддержка 10,000 RPS
**Error Rate**: < 0.1% ошибок
