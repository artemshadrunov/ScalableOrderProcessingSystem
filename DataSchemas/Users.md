DynamoDB

{ "PK": "USER#42", "SK": "PROFILE", "name": "Jon", "email": "a@b.c", "phone": "+123456789" }
{ "PK": "USER#42", "SK": "ADDRESS#abc", "city": "Berlin", "zip": "12345" }
{ "PK": "USER#42", "SK": "PAYMENT#stripe", "provider": "stripe", "token": "tok_xxx" }

Access Patterns

| Запрос                                   | Как читается                                                 |
| ---------------------------------------- | ------------------------------------------------------------ |
| Получить весь профиль                    | `PK = USER#<user_id>`                                        |
| Получить адрес                           | `PK = USER#<user_id> AND SK = ADDRESS#<addr_id>`             |
| Получить все платёжки                    | `PK = USER#<user_id> AND begins_with(SK, 'PAYMENT#')`        |
| Получить конкретную платёжку             | `PK = USER#<user_id> AND SK = PAYMENT#<provider>#<method>`   |
| Получить дефолтную платёжку              | `FilterExpression is_default = true`                         |
