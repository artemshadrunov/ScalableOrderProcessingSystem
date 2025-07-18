using Implementation.Services;
using Implementation.Models;

namespace Implementation.Lambda
{
    public class CleanupExpiredOrdersV2
    {
        private readonly OrderService _orderService;
        private readonly StockService _stockService;
        private readonly CartService _cartService;

        // Конфигурация для обработки больших объемов
        private const int BATCH_SIZE = 100; // Размер пачки для обработки
        private const int MAX_ORDERS_PER_EXECUTION = 500; // Максимум заказов за один запуск

        public CleanupExpiredOrdersV2(OrderService orderService, StockService stockService, CartService cartService)
        {
            _orderService = orderService;
            _stockService = stockService;
            _cartService = cartService;
        }

        public async Task<CleanupResult> FunctionHandler()
        {
            var executionId = Guid.NewGuid().ToString();
            
            try
            {
                var result = await ProcessExpiredOrdersWithRetry(executionId);
                return result;
            }
            catch (Exception ex)
            {
                return new CleanupResult
                {
                    ExecutionId = executionId,
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<CleanupResult> ProcessExpiredOrdersWithRetry(string executionId)
        {
            const int maxRetries = 3;
            var attempt = 0;
            
            while (attempt < maxRetries)
            {
                attempt++;
                try
                {
                    return await ProcessExpiredOrders(executionId);
                }
                catch (Exception) when (attempt < maxRetries)
                {
                    await Task.Delay(1000 * attempt); // Exponential backoff
                }
            }
            
            throw new Exception($"All {maxRetries} attempts failed for execution {executionId}");
        }

        private async Task<CleanupResult> ProcessExpiredOrders(string executionId)
        {
            var result = new CleanupResult { ExecutionId = executionId };
            var totalProcessedOrders = 0;
            var totalReleasedStockItems = 0;
            var cutoffTime = DateTime.UtcNow;
            

            var offset = 0;
            var hasMoreOrders = true;
            
            while (hasMoreOrders && totalProcessedOrders < MAX_ORDERS_PER_EXECUTION)
            {
                // Получаем следующую пачку просроченных заказов через сервис
                var expiredOrders = await _orderService.GetExpiredOrdersPaginated(cutoffTime, BATCH_SIZE, offset);
                if (!expiredOrders.Any())
                {
                    hasMoreOrders = false;
                    break;
                }

                // Группируем товары для освобождения stock
                var stockToRelease = GroupItemsForStockRelease(expiredOrders);
                
                // Обрабатываем пачку
                var batchResult = await ProcessBatch(expiredOrders, stockToRelease);
                
                totalProcessedOrders += batchResult.ProcessedOrders;
                totalReleasedStockItems += batchResult.ReleasedStockItems;
                
                offset += BATCH_SIZE;
                
                // Небольшая пауза между пачками для снижения нагрузки на БД
                if (hasMoreOrders)
                {
                    await Task.Delay(100);
                }
            }
            
            result.ProcessedOrders = totalProcessedOrders;
            result.ReleasedStockItems = totalReleasedStockItems;
            result.Success = true;
            result.HasMoreOrders = hasMoreOrders; // Флаг для индикации что есть еще заказы

            return result;
        }

        private Dictionary<string, int> GroupItemsForStockRelease(List<Order> orders)
        {
            var stockToRelease = new Dictionary<string, int>();
            
            foreach (var order in orders)
            {
                if (order.Items == null)
                    continue;
                
                foreach (var item in order.Items)
                {
                    if (string.IsNullOrEmpty(item.Sku) || item.Qty <= 0)
                        continue;
                    
                    if (stockToRelease.ContainsKey(item.Sku))
                        stockToRelease[item.Sku] += item.Qty;
                    else
                        stockToRelease[item.Sku] = item.Qty;
                }
            }
            
            return stockToRelease;
        }

        private async Task<BatchResult> ProcessBatch(List<Order> orders, Dictionary<string, int> stockToRelease)
        {
            var result = new BatchResult();
            
            try
            {
                // Освобождаем stock через сервис
                await _stockService.ReleaseStockBulk(stockToRelease);
                // Удаляем заказы через сервис
                await _orderService.DeleteOrdersBulk(orders.Select(o => o.OrderId).ToList());

                result.ProcessedOrders = orders.Count;
                result.ReleasedStockItems = stockToRelease.Count;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                throw;
            }
            
            return result;
        }
    }

    public class CleanupResult
    {
        public string ExecutionId { get; set; }
        public int ProcessedOrders { get; set; }
        public int ReleasedStockItems { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasMoreOrders { get; set; } // Флаг для индикации что есть еще заказы
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }

    public class BatchResult
    {
        public int ProcessedOrders { get; set; }
        public int ReleasedStockItems { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
} 