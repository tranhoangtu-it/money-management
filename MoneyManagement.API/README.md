# MoneyManagement API Documentation

## Overview
The MoneyManagement API is a .NET 9.0 Web API that implements a jar-based money management system. This API allows users to manage different financial "jars" (categories) and track money transfers between them.

## Architecture
- **Framework**: .NET 9.0 with ASP.NET Core
- **Database**: SQLite with Entity Framework Core
- **Pattern**: Repository pattern with service layer
- **Documentation**: Swagger/OpenAPI integration

## Data Models

### Jar Model
Represents a financial jar/category for money management.

```csharp
public class Jar
{
    public int Id { get; set; }                    // Primary key
    public string Name { get; set; }               // Jar name (max 50 chars)
    public decimal Percentage { get; set; }        // Allocation percentage (0-100)
    public string Description { get; set; }        // Description (max 500 chars)
    public decimal CurrentBalance { get; set; }    // Current balance
    public DateTime CreatedAt { get; set; }        // Creation timestamp
    public DateTime? UpdatedAt { get; set; }       // Last update timestamp
}
```

### Transaction Model
Represents a money transfer between jars.

```csharp
public class Transaction
{
    public int Id { get; set; }                    // Primary key
    public int SourceJarId { get; set; }           // Source jar ID
    public int DestinationJarId { get; set; }      // Destination jar ID
    public decimal Amount { get; set; }            // Transfer amount (min 0.01)
    public string Description { get; set; }        // Description (max 200 chars)
    public DateTime TransactionDate { get; set; }  // Transaction date
    public DateTime CreatedAt { get; set; }        // Creation timestamp
    public DateTime? UpdatedAt { get; set; }       // Last update timestamp
}
```

## API Endpoints

### Jars Controller (`/api/jars`)

#### GET /api/jars
Get all jars.

**Response**: Array of Jar objects
```json
[
  {
    "id": 1,
    "name": "Necessities",
    "percentage": 50,
    "description": "Essential expenses",
    "currentBalance": 1000.00,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": null
  }
]
```

#### GET /api/jars/{id}
Get a specific jar by ID.

**Parameters**:
- `id` (path): Jar ID

**Response**: Jar object or 404 if not found

#### POST /api/jars
Create a new jar.

**Request Body**:
```json
{
  "name": "Emergency Fund",
  "percentage": 20,
  "description": "Emergency savings",
  "currentBalance": 0
}
```

**Response**: Created jar object with assigned ID

#### PUT /api/jars/{id}
Update an existing jar.

**Parameters**:
- `id` (path): Jar ID

**Request Body**: Jar object with updated fields

**Response**: Updated jar object or 404 if not found

#### DELETE /api/jars/{id}
Delete a jar.

**Parameters**:
- `id` (path): Jar ID

**Response**: 204 No Content or 404 if not found

#### GET /api/jars/{id}/balance
Get the current balance of a jar.

**Parameters**:
- `id` (path): Jar ID

**Response**: Decimal balance value

#### POST /api/jars/{id}/add
Add money to a jar.

**Parameters**:
- `id` (path): Jar ID

**Request Body**: Decimal amount
```json
100.50
```

**Response**: Updated jar object

#### POST /api/jars/{id}/remove
Remove money from a jar.

**Parameters**:
- `id` (path): Jar ID

**Request Body**: Decimal amount
```json
50.25
```

**Response**: Updated jar object or 400 if insufficient funds

### Transactions Controller (`/api/transactions`)

#### GET /api/transactions
Get all transactions with related jar information.

**Response**: Array of Transaction objects with navigation properties

#### GET /api/transactions/{id}
Get a specific transaction by ID.

**Parameters**:
- `id` (path): Transaction ID

**Response**: Transaction object with navigation properties or 404 if not found

#### POST /api/transactions
Create a new transaction record.

**Request Body**:
```json
{
  "sourceJarId": 1,
  "destinationJarId": 2,
  "amount": 100.00,
  "description": "Transfer to savings",
  "transactionDate": "2024-01-15T10:30:00Z"
}
```

**Response**: Created transaction object

#### POST /api/transactions/transfer
Transfer money between jars (recommended method).

**Query Parameters**:
- `sourceJarId`: Source jar ID
- `destinationJarId`: Destination jar ID
- `amount`: Transfer amount
- `description`: Transfer description

**Response**: Transaction object representing the transfer

#### GET /api/transactions/jar/{jarId}
Get all transactions for a specific jar.

**Parameters**:
- `jarId` (path): Jar ID

**Response**: Array of transactions involving the jar

#### GET /api/transactions/daterange
Get transactions within a date range.

**Query Parameters**:
- `startDate`: Start date (ISO format)
- `endDate`: End date (ISO format)

**Response**: Array of transactions within the date range

## Usage Examples

### Creating a New Jar
```bash
curl -X POST "https://localhost:5001/api/jars" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Vacation Fund",
    "percentage": 5,
    "description": "Saving for vacation",
    "currentBalance": 0
  }'
```

### Adding Money to a Jar
```bash
curl -X POST "https://localhost:5001/api/jars/1/add" \
  -H "Content-Type: application/json" \
  -d '500.00'
```

### Transferring Money Between Jars
```bash
curl -X POST "https://localhost:5001/api/transactions/transfer?sourceJarId=1&destinationJarId=2&amount=100.00&description=Monthly%20savings"
```

### Getting Transaction History
```bash
curl "https://localhost:5001/api/transactions/daterange?startDate=2024-01-01&endDate=2024-01-31"
```

## Error Handling

The API returns standard HTTP status codes:
- `200 OK`: Successful request
- `201 Created`: Resource created successfully
- `204 No Content`: Successful deletion
- `400 Bad Request`: Invalid request data
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

Error responses include descriptive messages:
```json
{
  "error": "Insufficient funds in jar Emergency Fund",
  "statusCode": 400
}
```

## Database Configuration

The application uses SQLite with the following connection string:
```
Data Source=MoneyManagement.db
```

### Default Jars
The system includes six pre-configured jars based on the jar money management system:
1. **Necessities** (50%): Essential expenses
2. **Financial Freedom** (10%): Long-term investments
3. **Education** (10%): Personal development
4. **Long-term Savings** (10%): Emergency fund
5. **Play** (10%): Entertainment
6. **Give** (10%): Charitable donations

## Development Setup

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or VS Code

### Running the Application
1. Clone the repository
2. Navigate to the project directory
3. Run the application:
   ```bash
   dotnet run
   ```
4. Access Swagger UI at `https://localhost:5001/swagger`

### Database Migrations
To update the database schema:
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Security Considerations

⚠️ **Important**: This API is configured for development use only. For production deployment:
- Configure CORS policies appropriately
- Implement authentication and authorization
- Use HTTPS in production
- Implement rate limiting
- Add input validation middleware
- Configure proper logging

## Performance Considerations

- All database operations are asynchronous
- Navigation properties are loaded using Include() for optimal queries
- Transactions are ordered by date for better user experience
- Consider implementing pagination for large datasets

## Contributing

1. Follow the established patterns for controllers and services
2. Add appropriate validation attributes to models
3. Include unit tests for new functionality
4. Update documentation for API changes