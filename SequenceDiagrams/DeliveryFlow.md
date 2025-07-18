sequenceDiagram
    participant Stripe as "Stripe"
    participant API as "API Gateway"
    participant Payments as "Payment Service"
    participant Orders as "Order Service"
    participant EventBridge as "EventBridge"
    participant EventHandler as "Event Handler Lambda"
    participant Delivery as "Delivery Service"
    participant DB as "DynamoDB"
    participant SNS as "SNS"
    participant Warehouse as "Warehouse Client"

    Stripe->>API: Webhook /webhooks/payment (payment.succeeded)
    API->>Payments: processPaymentWebhook(webhookData)
    Payments->>Orders: PUT /orders/{order_id}/status (status: paid)
    Orders-->>Payments: 200 OK
    Orders->>EventBridge: OrderStatusChanged(order_id, status: paid)
    EventBridge->>EventHandler: OrderStatusChanged event
    EventHandler->>Delivery: CreateDeliveryOrder(order_id)
    Delivery->>DB: createDeliveryRecord(order_id, status: created)
    DB-->>Delivery: delivery_id
    Delivery-->>EventHandler: 201 Created (delivery_id)
    EventHandler->>SNS: OrderReadyForDelivery(order_id, delivery_id)
    SNS->>Warehouse: notifyWarehouse(order_id, delivery_id) 