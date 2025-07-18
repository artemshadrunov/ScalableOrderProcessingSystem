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