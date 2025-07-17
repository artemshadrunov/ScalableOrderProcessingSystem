-- Список товаров
CREATE TABLE products (
    sku           TEXT PRIMARY KEY,
    name          TEXT NOT NULL,
    description   TEXT,
    price_cents   INTEGER NOT NULL,
    created_at    TIMESTAMP NOT NULL DEFAULT now()
);

-- Остатки и зарезервированное количество
CREATE TABLE stock (
    sku           TEXT PRIMARY KEY REFERENCES products(sku),
    quantity      INTEGER NOT NULL,         -- всего доступно
    reserved      INTEGER NOT NULL DEFAULT 0 -- временно зарезервировано
);

-- Резервы: контроль TTL и "кому чего"
CREATE TABLE stock_reserve (
    reserve_id    UUID PRIMARY KEY,
    sku           TEXT NOT NULL REFERENCES stock(sku),
    order_id      UUID NOT NULL REFERENCES orders(order_id),
    qty           INTEGER NOT NULL,
    expires_at    TIMESTAMP NOT NULL
);