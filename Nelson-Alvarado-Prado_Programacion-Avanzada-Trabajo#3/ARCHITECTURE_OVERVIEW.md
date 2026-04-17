# Car Park REST API - Architecture Overview

## 🏗️ System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                     CLIENT APPLICATIONS                          │
│  (Frontend, Mobile, 3rd Party Services, Testing Tools)          │
└─────────────────────────┬───────────────────────────────────────┘
                          │
                 HTTP/HTTPS Requests
                          │
                          ▼
┌─────────────────────────────────────────────────────────────────┐
│                    REST API SERVERS                              │
├──────────────────────────────────────────────────────────────────┤
│  Node.js + Express              │   Python + Flask               │
│  (Port 3000)                    │   (Port 5000)                  │
├─────────────────────────────────┼────────────────────────────────┤
│ Routes & Controllers            │ Routes & Handlers              │
│  - CRUD Operations              │  - CRUD Operations             │
│  - Query/Filters                │  - Query/Filters               │
│  - Error Handling               │  - Error Handling              │
│  - CORS Support                 │  - CORS Support                │
├─────────────────────────────────┼────────────────────────────────┤
│ Service Layer                   │ Service Layer                  │
│  - crudService.js               │  - crud_service.py             │
│  - Business Logic               │  - Business Logic              │
├─────────────────────────────────┼────────────────────────────────┤
│ Data Access Layer (Repository)  │ Data Access Layer (Repository) │
│  - repositories.js              │  - repositories.py             │
│  - Query Building               │  - Query Building              │
│  - Data Transformation          │  - Data Transformation         │
└──────────────────────┬──────────┴───────────────┬────────────────┘
                       │                         │
          ┌────────────┴─────────────────────────┘
          │
          ▼
┌─────────────────────────────────────────────────────────────────┐
│                    DATA LAYER                                    │
├──────────────────────────────────────────────────────────────────┤
│   JSON Files          │        Database (MySQL)                   │
│ (Development)         │       (Production)                        │
│                       │                                            │
│ - prq_cars.json       │   - PRQ_Cars table                        │
│ - prq_parking.json    │   - PRQ_Parking table                     │
│ - prq_car_entry.json  │   - PRQ_Car_Entry table                   │
│                       │   - Relationships & Constraints           │
└─────────────────────────────────────────────────────────────────┘
```

---

## 📊 API Endpoint Structure

```
BASE_URL/api
│
├── /cars (10 endpoints)
│   ├── POST      Create new car
│   ├── GET       All cars
│   ├── GET/:id   Car by ID
│   ├── /filter
│   │   ├── /color/:color           Filter by color
│   │   ├── /make/:make             Filter by make
│   │   ├── /type/:type             Filter by type
│   │   ├── /year-range?min=&max=   Filter by year range
│   │   └── /advanced?...           Multi-criteria filter
│   ├── PUT/:id   Update car
│   └── DELETE/:id Delete car
│
├── /parking (9 endpoints)
│   ├── POST      Create parking facility
│   ├── GET       All parking facilities
│   ├── GET/:id   Parking by ID
│   ├── /filter
│   │   ├── /province/:province           Filter by province
│   │   ├── /name/:name                   Filter by name
│   │   ├── /rate-range?min=&max=        Filter by rate
│   │   └── /advanced?...                 Multi-criteria filter
│   ├── PUT/:id   Update parking
│   └── DELETE/:id Delete parking
│
├── /car-entry (9 endpoints)
│   ├── POST      Create entry
│   ├── GET       All entries
│   ├── GET/:id   Entry by ID
│   ├── /filter
│   │   ├── /parking/:parkingId                Filter by parking
│   │   ├── /car-type/:type?start=&end=      Filter by car type
│   │   └── /province/:province?start=&end=  Filter by province
│   ├── /price/:parkingId                Get hourly price
│   ├── PUT/:id   Update entry
│   └── DELETE/:id Delete entry
│
└── /system (2 endpoints)
    ├── /health                  Health check
    └── /endpoints               List all endpoints
```

---

## 🔄 Request-Response Flow

```
1. CLIENT REQUEST
   │
   └─► HTTP Method: GET/POST/PUT/DELETE
       URL: http://localhost:PORT/api/resource
       Body (if POST/PUT): JSON data
       Headers: Content-Type: application/json

2. SERVER RECEIVES
   │
   ├─► CORS Middleware (verify origin)
   ├─► Body Parser (parse JSON)
   └─► Route Matching (find handler)

3. ROUTE HANDLER
   │
   ├─► Validate Request Body
   ├─► Call Service Layer
   └─► Get Response

4. SERVICE LAYER
   │
   ├─► Business Logic (validation)
   ├─► Call Data Access Layer
   └─► Format Response

5. DATA ACCESS LAYER
   │
   ├─► Query Builder (SQL/filters)
   ├─► Execute Query (JSON/Database)
   └─► Return Data

6. RESPONSE BUILDER
   │
   ├─► Success Response: 200/201
   │   {success: true, data: {...}, timestamp: ...}
   │
   └─► Error Response: 4xx/5xx
       {success: false, error: "...", timestamp: ...}

7. CLIENT RECEIVES
   │
   └─► JSON Response with Status Code
```

---

## 📋 Data Model Relationships

```
┌─────────────────────────────────────────────────────────────────┐
│                    PRQ_CARS                                      │
├─────────────────────────────────────────────────────────────────┤
│ ID (PK)              ◄──┐                                        │
│ color                   │ Referenced by                          │
│ year                    │ PRQ_Car_Entry.car_id                   │
│ make                    │                                        │
│ type                    │                                        │
│ created_at              │                                        │
└─────────────────────────────────────────────────────────────────┘
                          │
                          │ 1:N relationship
                          │
┌─────────────────────────────────────────────────────────────────┐
│                 PRQ_CAR_ENTRY                                    │
├─────────────────────────────────────────────────────────────────┤
│ sequential_number (PK)                                           │
│ car_id (FK) ─────────► References PRQ_Cars.ID                   │
│ parking_id (FK) ──────► References PRQ_Parking.ID               │
│ entry_date_time                                                  │
│ exit_date_time                                                   │
│ created_at                                                       │
└─────────────────────────────────────────────────────────────────┘
                          │
                          │ N:1 relationship
                          │
┌─────────────────────────────────────────────────────────────────┐
│                  PRQ_PARKING                                     │
├─────────────────────────────────────────────────────────────────┤
│ ID (PK)              ◄──┐                                        │
│ province_name           │ Referenced by                          │
│ name                    │ PRQ_Car_Entry.parking_id               │
│ hourly_rate             │                                        │
│ created_at              │                                        │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🔍 Query Examples by Use Case

### Use Case 1: Find All Parking Lots in Madrid Under €4/hour

```
GET /api/parking/filter/advanced?province=Madrid&minRate=0&maxRate=4
```

Response contains all parking facilities in Madrid with hourly rates between €0 and €4.

### Use Case 2: Find All Sedan Cars Parked in 2026-04-17

```
GET /api/car-entry/filter/car-type/sedan?start=2026-04-15&end=2026-04-20
```

Response shows all sedan car entries during the specified date range.

### Use Case 3: Get Hourly Rate for Specific Parking Facility

```
GET /api/car-entry/price/1
```

Response returns the hourly rate for parking facility with ID 1.

### Use Case 4: Update Exit Time for Active Car

```
PUT /api/car-entry/1
{
  "parking_id": 1,
  "car_id": 1,
  "entry_date_time": "2026-04-17T10:00:00",
  "exit_date_time": "2026-04-17T14:30:00"
}
```

Response confirms the car entry has been updated with exit time.

---

## 🛡️ Error Handling Flow

```
Request Received
      │
      ▼
┌─────────────────────┐
│ Validate Request    │
└──────┬──────────────┘
       │
       ├─ Invalid ─┐
       │           │
       │ Valid     ▼
       │      ┌──────────────────┐
       │      │ 400 Bad Request  │
       │      │ { error: "..." } │
       │      └──────────────────┘
       │
       ▼
┌─────────────────────┐
│ Process Business    │
│ Logic              │
└──────┬──────────────┘
       │
       ├─ Error ──┐
       │          │
       │ Success  ▼
       │     ┌────────────────────────┐
       │     │ 400/404/500 Error      │
       │     │ { error: "...", ... }  │
       │     └────────────────────────┘
       │
       ▼
┌─────────────────────────┐
│ Query Data Layer        │
└──────┬──────────────────┘
       │
       ├─ Not Found ─┐
       │             │
       │ Found       ▼
       │        ┌──────────────────┐
       │        │ 404 Not Found    │
       │        │ { error: "..." } │
       │        └──────────────────┘
       │
       ▼
┌─────────────────────────┐
│ Return Success          │
│ Response               │
└──────┬──────────────────┘
       │
       ▼
┌─────────────────────────────────────┐
│ 200/201 Success                     │
│ { success: true, data: {...} }      │
└─────────────────────────────────────┘
```

---

## 📈 Performance Considerations

### Request Routing
```
Express Router
    ├─ Route: /api/cars
    ├─ Handler: carController
    ├─ Validation: validate()
    ├─ Service: crudService.getCar()
    ├─ Repository: carRepository.getById()
    └─ Response: JSON
```

### Data Access Optimization
```
Query Execution:
  1. Filter criteria built in Repository
  2. JSON: Array filtering (in-memory)
  3. Database: SQL query (server-side)
  4. Results cached during request lifecycle
  5. Response formatted and sent
```

### Indexing Strategy (Database)
```
PRQ_Cars
  ├─ Primary Key: ID
  ├─ Index on color (for filtering)
  ├─ Index on make (for filtering)
  └─ Index on type (for filtering)

PRQ_Parking
  ├─ Primary Key: ID
  ├─ Index on province_name
  ├─ Index on name
  └─ Index on hourly_rate

PRQ_Car_Entry
  ├─ Primary Key: sequential_number
  ├─ Foreign Key: car_id (indexed)
  ├─ Foreign Key: parking_id (indexed)
  ├─ Index on entry_date_time
  └─ Index on exit_date_time
```

---

## 🔐 Authentication & Authorization (Future Enhancement)

```
Current Implementation: No authentication
Recommended Enhancement:

1. JWT Token-based Auth
   ├─ POST /api/auth/login
   ├─ Returns: { token: "...", expiresIn: 3600 }
   └─ Include in requests: Authorization: Bearer <token>

2. Role-based Access Control
   ├─ Admin: Full CRUD
   ├─ Operator: Read + Create entries
   ├─ Viewer: Read-only
   └─ Anonymous: Health check only

3. Middleware
   ├─ JWT Verification
   ├─ Role Checking
   ├─ Request Logging
   └─ Rate Limiting
```

---

## 📦 Deployment Architecture

### Development (JSON Storage)
```
┌─────────────────┐
│  Express/Flask  │
│  (Port 3000/5000)
│ ─────────────── │
│  In-Memory Repo │
│  JSON Files     │
└─────────────────┘
```

### Production (Database Storage)
```
┌─────────────────┐
│  Load Balancer  │
│  (Port 80/443)  │
└────────┬────────┘
         │
    ┌────┴────┐
    ▼         ▼
┌────────┐ ┌────────┐
│Instance│ │Instance│ ... (Multiple Instances)
│  1     │ │  2     │
└────┬───┘ └───┬────┘
     │         │
     └────┬────┘
          │
          ▼
    ┌──────────────┐
    │ Azure MySQL  │
    │  Database    │
    │ (PRQ_Cars,   │
    │  PRQ_Parking,│
    │  PRQ_Car_    │
    │  Entry)      │
    └──────────────┘
```

---

## 🚀 Response Time Expectations

### JSON Storage (Development)
- Simple GET (all records): ~5-10ms
- Filtered GET: ~10-20ms
- POST/PUT/DELETE: ~15-25ms
- Complex filters: ~20-30ms

### Database Storage (Production)
- Simple SELECT: ~20-50ms
- Indexed query: ~30-80ms
- JOIN query: ~50-150ms
- Complex query: ~100-500ms

---

## 📊 Scalability Plan

### Current Architecture
- Single instance server
- JSON file storage
- Suitable for: Development, testing, small deployments

### Scale to 100 QPS
- Multiple server instances
- Database with connection pooling
- CDN for static content
- Redis for caching

### Scale to 1000+ QPS
- Distributed database (sharding)
- Message queue (RabbitMQ, Kafka)
- Microservices architecture
- Global load balancing

---

## ✨ Summary

| Aspect | Details |
|--------|---------|
| **API Type** | RESTful |
| **Data Format** | JSON |
| **Transport** | HTTP/HTTPS |
| **Endpoints** | 32 total |
| **Operations** | CRUD + Advanced Queries |
| **Authentication** | None (future enhancement) |
| **Authorization** | None (future enhancement) |
| **Rate Limiting** | Not implemented (future) |
| **Caching** | Not implemented (future) |
| **Documentation** | Complete API Reference |
| **Testing** | Example scripts included |
| **Deployment** | Ready for cloud (Heroku, Azure, AWS) |

