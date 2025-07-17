sequenceDiagram
    participant EventBridge
    participant LambdaTTL
    participant Aurora
    participant StockAPI

    EventBridge->>LambdaTTL: Scheduled Event (rate: N sec)
    LambdaTTL->>Aurora: SELECT * FROM stock_reserve WHERE expires_at < NOW() OR status = cancelled
    LambdaTTL->>StockAPI: POST /orders/bulk-cancel (order_id1, order_id2,...)
    loop по каждому sku
        LambdaTTL->>StockAPI: POST /stock/{sku}/adjust (delta: +qty)
    end
    