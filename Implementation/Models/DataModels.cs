namespace Implementation.Models;

using System;
using System.Collections.Generic;

// Aurora (EF Core) Models
public class Order
{
    public string OrderId { get; set; } // UUID PRIMARY KEY
    public string UserId { get; set; } // UUID NOT NULL
    public string CartId { get; set; } // UUID REFERENCES carts(cart_id)
    public string Status { get; set; } // TEXT NOT NULL CHECK (status IN ('pending', 'confirmed', 'payment_failed', 'cancelled', 'shipped', 'delivered', 'pending_refund'))
    public int TotalCents { get; set; } // INTEGER NOT NULL
    public string Address { get; set; } // TEXT NOT NULL
    public DateTime CreatedAt { get; set; } // TIMESTAMP NOT NULL DEFAULT now()
    public DateTime? ExpiresAt { get; set; } // TIMESTAMP -- используется для TTL
    public List<CartItem> Items { get; set; }
}

public class OrderItem
{
    public string OrderId { get; set; } // UUID REFERENCES orders(order_id)
    public string Sku { get; set; } // TEXT NOT NULL
    public int Qty { get; set; } // INTEGER NOT NULL
    public int PriceCents { get; set; } // INTEGER NOT NULL
}

public class Cart
{
    public string CartId { get; set; } // UUID PRIMARY KEY
    public string UserId { get; set; } // UUID NOT NULL
    public DateTime CreatedAt { get; set; } // TIMESTAMP NOT NULL DEFAULT now()
    public List<CartItem> Items { get; set; }
}

public class CartItem
{
    public string CartId { get; set; } // UUID REFERENCES carts(cart_id)
    public string Sku { get; set; } // TEXT REFERENCES products(sku)
    public int Qty { get; set; } // INTEGER NOT NULL
    public int PriceCents { get; set; } // INTEGER NOT NULL
}

public class Product
{
    public string Sku { get; set; } // TEXT PRIMARY KEY
    public string Name { get; set; } // TEXT NOT NULL
    public string Description { get; set; } // TEXT
    public int PriceCents { get; set; } // INTEGER NOT NULL
    public DateTime CreatedAt { get; set; } // TIMESTAMP NOT NULL DEFAULT now()
}

public class Stock
{
    public string Sku { get; set; } // TEXT PRIMARY KEY REFERENCES products(sku)
    public int Quantity { get; set; } // INTEGER NOT NULL -- всего доступно
    public int Reserved { get; set; } // INTEGER NOT NULL DEFAULT 0 -- временно зарезервировано
}



public class Payment
{
    public string PaymentId { get; set; } // UUID PRIMARY KEY
    public string OrderId { get; set; } // UUID NOT NULL REFERENCES orders(order_id)
    public string Status { get; set; } // TEXT NOT NULL CHECK (status IN ('initiated', 'captured', 'failed', 'refunded'))
    public string Provider { get; set; } // TEXT NOT NULL -- 'stripe', 'paypal', etc.
    public string ExternalRef { get; set; } // TEXT -- id от платёжки
    public DateTime CreatedAt { get; set; } // TIMESTAMP NOT NULL DEFAULT now()
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
}

public class PaymentMethod
{
    public string Type { get; set; } // card, paypal
    public string Token { get; set; }
    public string Provider { get; set; } // stripe, paypal
}

// DynamoDB Models
public class UserProfile
{
    public string PK { get; set; } // "USER#42"
    public string SK { get; set; } // "PROFILE"
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

public class UserAddress
{
    public string PK { get; set; } // "USER#42"
    public string SK { get; set; } // "ADDRESS#abc"
    public string City { get; set; }
    public string Zip { get; set; }
}

public class UserPayment
{
    public string PK { get; set; } // "USER#42"
    public string SK { get; set; } // "PAYMENT#stripe"
    public string Provider { get; set; }
    public string Token { get; set; }
    public bool IsDefault { get; set; }
}

public class DeliveryShipment
{
    public string PK { get; set; } // "ORDER#abc123"
    public string SK { get; set; } // "SHIPMENT"
    public string Status { get; set; } // "created"
    public DeliveryAddress Address { get; set; }
}

public class DeliveryTracking
{
    public string PK { get; set; } // "ORDER#abc123"
    public string SK { get; set; } // "TRACKING#2025-07-13T15:00"
    public string Status { get; set; } // "picked_up"
    public DateTime Timestamp { get; set; }
}

public class DeliveryAddress
{
    public string City { get; set; }
    public string Zip { get; set; }
    public string Street { get; set; }
} 