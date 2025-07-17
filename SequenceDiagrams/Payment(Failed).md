sequenceDiagram
    participant UI
    participant PaymentsAPI
    participant OrdersAPI
    participant EventBridge
    participant EventHandlerLambda

    UI->>PaymentsAPI: POST /payments (инициировать платёж)
    PaymentsAPI-->>UI: 201 Created (payment_id)
    UI->>PaymentsAPI: Webhook/payment.failed (от платёжного провайдера)
    PaymentsAPI->>OrdersAPI: PUT /orders/{order_id}/status (status: payment_failed)
    OrdersAPI-->>PaymentsAPI: 200 OK
    OrdersAPI->>EventBridge: OrderStatusChanged (payment_failed)
    EventBridge->>EventHandlerLambda: (OrderStatusChanged: payment_failed)
    EventBridge->>UI: Показывает ошибку/уведомление пользователю