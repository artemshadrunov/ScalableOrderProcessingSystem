DynamoDB

{ "PK": "ORDER#abc123", "SK": "SHIPMENT", "status": "created", "address": { "city": "Paris" } }
{ "PK": "ORDER#abc123", "SK": "TRACKING#2025-07-13T15:00", "status": "picked_up" }
{ "PK": "ORDER#abc123", "SK": "TRACKING#2025-07-14T10:00", "status": "delivered" }


Access Patterns

Получить всю информацию по доставке заказа:
→ Query PK = ORDER#<order_id>

Получить основную запись о доставке:
→ Query PK = ORDER#<order_id> AND SK = SHIPMENT

Получить всю историю трекинга:
→ Query PK = ORDER#<order_id> AND begins_with(SK, 'TRACKING#')

Получить последний статус (по дате):
→ Query PK = ORDER#<order_id> AND begins_with(SK, 'TRACKING#')
→ Limit = 1, ScanIndexForward = false