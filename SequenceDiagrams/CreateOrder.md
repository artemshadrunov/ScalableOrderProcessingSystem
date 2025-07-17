sequenceDiagram
    participant U as "User"
    participant API as "API Gateway"
    participant Cart as "Cart Service"
    participant Stock as "Stock Service"
    participant Orders as "Order Service"

    U->>API: POST /cart/{cart_id}/checkout
    API->>Cart: getCartItems(cart_id)
    Cart-->>API: items
    API->>Stock: checkAvailability(items)
    Stock-->>API: OK / Error
    API-->>U: OK / Error

    U->>API: POST /orders (cart_id, user_id, shipping_address, payment_method)
    API->>Cart: getCartItems(cart_id)
    Cart-->>API: items
    API->>Stock: checkAndReserve(items)
    Stock-->>API: OK / Error
    API->>Orders: createOrder(user_id, cart_id, items, shipping_address, payment_method)
    Orders-->>API: order_id
    API-->>U: order_id / Error