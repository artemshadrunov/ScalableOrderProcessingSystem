sequenceDiagram
    participant U as "User"
    participant API as "API Gateway"
    participant Cart as "Cart Service"

    U->>API: DELETE /cart/{cart_id}/items/{sku}
    API->>Cart: removeFromCart(cart_id, sku)
    Cart-->>API: OK
    API-->>U: OK