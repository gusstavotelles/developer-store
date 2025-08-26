# Sales API

This document describes the Sales API endpoints, request/response examples, and business rules.

## Endpoints

### POST /api/sales
Create a sale.

Request:
```json
{
  "saleNumber": "string (optional)",
  "date": "2025-08-26T00:00:00Z",
  "customerId": "string",
  "branchId": "string",
  "items": [
    {
      "productId": "string",
      "productName": "string",
      "unitPrice": 10.0,
      "quantity": 4
    }
  ]
}
```

Response: 201 Created
```json
{
  "id": "guid"
}
```

Business rules:
- quantity < 4 => 0% discount
- 4 <= quantity < 10 => 10% discount
- 10 <= quantity <= 20 => 20% discount
- quantity > 20 => rejected (DomainException)

### PUT /api/sales/{id}
Update sale items and/or date.

Request body:
```json
{
  "date": "2025-08-26T00:00:00Z",
  "items": [
    {
      "productId": "string",
      "productName": "string",
      "unitPrice": 10.0,
      "quantity": 2
    }
  ]
}
```

Response:
200 OK
```json
{ "id": "guid" }
```

### POST /api/sales/{id}/cancel
Cancel entire sale.

Response:
200 OK
```json
{ "id": "guid" }
```

### GET /api/sales/{id}
Get sale details.

Response:
200 OK
```json
{
  "id": "guid",
  "saleNumber": "string",
  "date": "2025-08-26T00:00:00Z",
  "customerId": "string",
  "branchId": "string",
  "total": 36.00,
  "items": [
    {
      "id": "guid",
      "productId": "string",
      "productName": "string",
      "unitPrice": 10.0,
      "quantity": 4,
      "discountPercent": 0.10,
      "total": 36.00,
      "isCancelled": false
    }
  ]
}
```

### GET /api/sales?_page=1&_size=10
List paginated sales.

Response:
200 OK
```json
{
  "data": [ ... ],
  "totalItems": 123,
  "currentPage": 1,
  "totalPages": 13
}
```

## Notes
- Events: SaleCreated, SaleModified, SaleCancelled can be logged via IEventPublisher implementation (logger).
- Persistence: EF Core mapping uses Sales and SaleItems tables; migration `AddSales` created.
- Tests: Unit tests added covering discount tiers, quantity limits, replace items and cancel item logic.
