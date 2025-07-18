-- Заказ
CREATE TABLE orders (
    order_id      UUID PRIMARY KEY,
    user_id       UUID NOT NULL,
    cart_id       UUID REFERENCES carts(cart_id),
    status        TEXT NOT NULL CHECK (status IN ('pending', 'confirmed', 'payment_failed', 'cancelled', 'shipped', 'delivered', 'pending_refund')),
    total_cents   INTEGER NOT NULL,
    address       TEXT NOT NULL,
    created_at    TIMESTAMP NOT NULL DEFAULT now(),
    expires_at    TIMESTAMP -- используется для TTL
);

-- Корзина пользователя
CREATE TABLE carts (
    cart_id       UUID PRIMARY KEY,
    user_id       UUID NOT NULL,
    created_at    TIMESTAMP NOT NULL DEFAULT now()
);

-- Позиции в корзине
CREATE TABLE cart_items (
    cart_id       UUID REFERENCES carts(cart_id),
    sku           TEXT REFERENCES products(sku),
    qty           INTEGER NOT NULL,
    price_cents   INTEGER NOT NULL,
    PRIMARY KEY (cart_id, sku)
);