sequenceDiagram
    participant U as "User"
    participant API as "API Gateway"
    participant Users as "User Service"
    participant DB as "DynamoDB"

    U->>API: POST /users (name, email, password, phone)
    API->>Users: createUser(name, email, password, phone)
    Users->>DB: checkUserExists(email)
    DB-->>Users: false
    Users->>DB: createUser(userData)
    DB-->>Users: user_id
    Users-->>API: 201 Created (user_id)
    API-->>U: 201 Created (user_id) 