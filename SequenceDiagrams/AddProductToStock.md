sequenceDiagram
    participant M as "Manager"
    participant API as "API Gateway"
    participant Stock as "Stock Service"
    participant DB as "Aurora DB"
    participant S3 as "S3"

    M->>API: POST /products (sku, name, description, price_cents, stock_quantity, images)
    API->>Stock: createProduct(productData)
    Stock->>DB: checkProductExists(sku)
    DB-->>Stock: false
    Stock->>S3: uploadImages(images)
    S3-->>Stock: image_urls
    Stock->>DB: createProduct(sku, name, description, price_cents, stock_quantity, image_urls)
    DB-->>Stock: product_id
    Stock-->>API: 201 Created (product_id)
    API-->>M: 201 Created (product_id)