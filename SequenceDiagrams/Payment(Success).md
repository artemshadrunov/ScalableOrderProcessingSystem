sequenceDiagram
    participant UI
    participant PaymentsAPI
    participant OrdersAPI
    participant EventBridge
    participant EventHandlerLambda
    participant DeliveryAPI

    UI->>PaymentsAPI: POST /payments (инициировать платёж)
    PaymentsAPI-->>UI: 201 Created (payment_id)
    UI->>PaymentsAPI: Webhook/payment.succeeded (от платёжного провайдера)
    PaymentsAPI->>OrdersAPI: PUT /orders/{order_id}/status (status: confirmed)
    OrdersAPI-->>PaymentsAPI: 200 OK
    OrdersAPI->>EventBridge: OrderStatusChanged (confirmed)
    EventBridge->>EventHandlerLambda: (OrderStatusChanged: confirmed)
    EventHandlerLambda->>DeliveryAPI: POST /orders/{order_id}/delivery (status: created)