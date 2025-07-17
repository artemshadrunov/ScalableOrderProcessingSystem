-- Платёж
CREATE TABLE payments (
    payment_id    UUID PRIMARY KEY,
    order_id      UUID NOT NULL REFERENCES orders(order_id),
    status        TEXT NOT NULL CHECK (status IN ('initiated', 'captured', 'failed', 'refunded')),
    provider      TEXT NOT NULL, -- 'stripe', 'paypal', etc.
    external_ref  TEXT,          -- id от платёжки
    created_at    TIMESTAMP NOT NULL DEFAULT now()
);