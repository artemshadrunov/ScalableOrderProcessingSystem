sequenceDiagram
    participant U as "User"
    participant API as "API Gateway"
    participant Orders as "Order Service"
    participant AuroraDB as "Aurora DB"
    participant DynamoDB as "DynamoDB"
    participant Payments as "Payment Service"
    participant Stock as "Stock Service"
    participant EventBridge as "EventBridge"
    participant EventHandler as "Event Handler Lambda"
    participant SNS as "SNS"
    participant Stripe as "Stripe"
    participant Warehouse as "Warehouse Portal"

    U->>API: POST /orders/{order_id}/cancel (reason: user_cancelled)
    API->>Orders: cancelOrder(order_id, reason)
    Orders->>AuroraDB: getOrder(order_id)
    AuroraDB-->>Orders: order_data (status: paid)
    Orders->>AuroraDB: updateOrderStatus(order_id, status: cancelled)
    Orders->>DynamoDB: updateDeliveryStatus(order_id, status: cancelled)
    Orders->>EventBridge: OrderCancelled(order_id, reason)
    EventBridge->>EventHandler: OrderCancelled event

    Orders-->>API: 200 OK (cancelled)
    API-->>U: 200 OK (order cancelled) 
    
    par Refund Process
        EventHandler->>Payments: POST /payments/{payment_id}/refund
        Payments->>Stripe: processRefund(payment_id)
        Stripe-->>Payments: refund_confirmed
        Payments-->>EventHandler: 200 OK (refund_id)
    and Stock Notification
        EventHandler->>Stock: releaseReservation(order_id)
        Stock->>AuroraDB: updateStockQuantity(order_items)
        AuroraDB-->>Stock: updated_quantities
        Stock-->>EventHandler: 200 OK
        EventHandler->>SNS: OrderCancelled event
        SNS->>Warehouse: notifyOrderCancelled(order_id, items)
    end