sequenceDiagram
    participant U as "User"
    participant API as "API Gateway"
    participant Cart as "Cart Service"

    U->>API: POST /cart/{cart_id}/items (sku, quantity)
    API->>Cart: addToCart(cart_id, sku, quantity)
    Cart-->>API: OK / Error
    API-->>U: OK / Error