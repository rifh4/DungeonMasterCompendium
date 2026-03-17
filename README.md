# Dungeon Master Compendium API Wrapper

I built this backend project to practice designing a middle-tier API with Redis caching.

Instead of having a client application call a third-party source directly, this API sits in the middle, wraps the public Open5e API, and returns normalized data through its own internal response contracts.

Open5e is an open-source 5e rules/content resource that exposes API access to monsters, spells, items, and other SRD/OGL data. This project only wraps a small part of that API.

Official Open5e links:
- [Open5e main site](https://open5e.com/)
- [Open5e API docs](https://open5e.com/api-docs)

## Screenshots

### Swagger UI overview
<img src="./docs/screenshots/swagger-overview.png" alt="Swagger UI overview" width="300">

### Monster detail example
<img src="./docs/screenshots/swagger-monster-detail.jpg" alt="GET /compendium/monsters/{externalId} response example" width="300">

### Redis cache keys
<img src="./docs/screenshots/redis-cache-keys.png" alt="Redis cache entries created by the API" width="300">

---

## What the API Covers

The API exposes normalized data for:

- Monsters
- Spells
- Items

Each resource supports:

- list queries
- optional name filtering
- detail lookup by `externalId`

You can view the Swagger interface locally at:

`http://localhost:5201/swagger`

after starting the project.

---

## Why Build a Wrapper?

When working with external APIs, the response schema can change or include more data than I actually want to expose to a client.

I built this layer to keep control over the public data shape.

Before returning data, the API does three things:

1. validates incoming request parameters
2. fetches raw data from Open5e through typed HTTP clients
3. maps the external DTOs into internal response models

That way, if the upstream schema changes, the mapping layer is the main place that needs to be updated.

---

## Caching Strategy

The most important part of this project is the Redis cache.

Calling an external provider repeatedly is slower than serving a cached response, and it also makes the API depend more heavily on the upstream service for repeated lookups. To reduce that, I used a cache-aside pattern.

When a request comes in, the service layer builds a deterministic cache key first.

For example, this request:

`GET /compendium/monsters?name=kobold&limit=10`

produces this cache key:

`dmcomp:monsters:list:name:kobold:limit:10`

The flow is:

- check Redis for the key
- on a cache miss, call Open5e
- map the response into internal contracts
- store the result in Redis
- return the response

Cached entries use a 10-minute absolute expiration.

---

## Main Endpoints

Base route:

`/compendium`

### Monsters
- `GET /compendium/monsters?limit=10`
- `GET /compendium/monsters?name=kobold&limit=10`
- `GET /compendium/monsters/{externalId}`

### Spells
- `GET /compendium/spells?limit=10`
- `GET /compendium/spells?name=magic&limit=10`
- `GET /compendium/spells/{externalId}`

### Items
- `GET /compendium/items?limit=10`
- `GET /compendium/items?name=sword&limit=10`
- `GET /compendium/items/{externalId}`

---

## Validation Rules

List endpoints enforce:

- `limit` must be between **1 and 100**
- `name` must be **50 characters or less**

Detail endpoints enforce:

- `externalId` must not be empty after normalization
- unknown `externalId` returns **404 Not Found**

Examples of invalid requests:

- `GET /compendium/monsters?limit=0`
- `GET /compendium/spells?limit=101`
- `GET /compendium/items?name=<51 characters>`

These return **400 Bad Request**.

---

## Tech Stack

- C# / .NET 8
- ASP.NET Core Web API
- Redis
- Open5e API
- xUnit

---

## How the Code is Organized

### Controllers
Responsibilities:

- HTTP endpoints
- request validation
- response mapping

### Services
Responsibilities:

- orchestration logic
- cache interaction
- calling Open5e clients
- mapping external DTOs into internal contracts

### Integrations
Responsibilities:

- typed HTTP clients for Open5e
- external DTOs
- upstream API communication

### Contracts
Responsibilities:

- internal response models
- keeping the public API separate from the external schema

---

## Running Locally

You will need the .NET 8 SDK and Docker Desktop installed.

### 1. Start Redis

```bash
docker run -d --name redis -p 6379:6379 redis
```

### 2. Restore packages

```bash
dotnet restore
```

### 3. Run the API

```bash
dotnet run --project .\DungeonMasterCompendium.Api\DungeonMasterCompendium.Api.csproj
```

### 4. Open Swagger

Open:

`http://localhost:5201/swagger`

---

## Testing

Run the tests with:

```bash
dotnet test
```

I wrote unit tests around the core service logic.

Because the services depend on external HTTP calls and a distributed cache, I used fake implementations of the Open5e clients and `IDistributedCache` so the cache-hit and cache-miss paths can be tested without relying on a live upstream service or a running Redis container.

---

## Scope

This project is intentionally smaller than my main SQL-backed backend project.

Included here:

- Open5e integration
- internal response contracts
- Redis caching
- validation behavior
- service-layer tests

Not included:

- authentication / authorization
- database persistence
- frontend client
- rate limiting
- production deployment infrastructure

---

## What I’d Improve Next

If I kept expanding this project, the next things I would look at are:

- adding rate-limit and retry handling around the upstream API
- adding logging or tracing around cache hits and misses
- expanding integration test coverage around the HTTP layer
