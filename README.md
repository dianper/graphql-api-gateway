# GraphQL API Gateway

![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?logo=dotnet&logoColor=white)
![HotChocolate](https://img.shields.io/badge/HotChocolate-12.14.x-E10098?logo=graphql&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-Schema%20Store-DC382D?logo=redis&logoColor=white)
![License](https://img.shields.io/badge/license-MIT-green)

A **federated GraphQL API Gateway** built with [HotChocolate](https://chillicream.com/docs/hotchocolate) and .NET 6. Multiple independent GraphQL services publish their schemas to Redis, and the Gateway automatically stitches them into a single, unified graph that clients query through one endpoint.

---

## Table of Contents

- [Project Overview](#project-overview)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Service Breakdown](#service-breakdown)
  - [API.Bag](#apibag)
  - [API.Catalog](#apicatalog)
  - [API.Common](#apicommon)
  - [API.Gateway](#apigateway)
  - [Framework.Diagnostics](#frameworkdiagnostics)
- [Running the Project](#running-the-project)
- [GraphQL Endpoints](#graphql-endpoints)
- [Example Federated Query](#example-federated-query)
- [Folder Structure](#folder-structure)
- [Development Notes](#development-notes)

---

## Project Overview

This repository demonstrates a **federated GraphQL architecture** where each domain service owns its part of the graph and independently publishes its schema definition to a shared Redis instance. The **API.Gateway** service subscribes to those schema definitions and composes them at runtime into a single unified schema using **HotChocolate Schema Stitching**.

Key characteristics:

- Each downstream service (`bag`, `catalog`, `common`) runs independently on its own port and exposes its own `/graphql` endpoint.
- On startup, each service serializes its SDL (Schema Definition Language) and pushes it to Redis under its service name.
- The Gateway reads all published schemas from Redis and stitches them together transparently.
- Cross-service field extensions are declared via `@delegate` directives in each service's `Stitching.graphql` file, enabling fields on one type to be resolved by a different service.

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                         Clients                             │
└────────────────────────────┬────────────────────────────────┘
                             │  HTTP  /graphql
                             ▼
┌─────────────────────────────────────────────────────────────┐
│                      API.Gateway  :5001                     │
│      AddRemoteSchemasFromRedis("gateway", redis)            │
│      Composes unified schema at runtime                     │
└───────┬──────────────────┬──────────────────┬──────────────┘
        │ HTTP             │ HTTP             │ HTTP
        ▼                  ▼                  ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐
│  API.Bag     │  │  API.Catalog │  │  API.Common          │
│  :5002       │  │  :5003       │  │  :5004               │
│              │  │              │  │                      │
│  Bag, BagItem│  │  Product,    │  │  User, Brand         │
│              │  │  Brand,      │  │                      │
│              │  │  Category    │  │                      │
└──────┬───────┘  └──────┬───────┘  └──────────┬───────────┘
       │                 │                      │
       │    PublishSchemaDefinition (SDL + type extensions)
       │                 │                      │
       └─────────────────┴──────────────────────┘
                                │
                                ▼
                    ┌───────────────────────┐
                    │         Redis         │
                    │  key: "gateway"       │
                    │  bag SDL, catalog SDL,│
                    │  common SDL stored    │
                    └───────────────────────┘
```

### How Federation Works

1. **Schema Publishing** — At startup, each service calls `PublishSchemaDefinition`, which serializes its SDL (including any `Stitching.graphql` type extensions) and writes it to the Redis key shared with the Gateway (`"gateway"`).
2. **Schema Stitching** — The Gateway calls `AddRemoteSchemasFromRedis("gateway", redis)`, which reads all published schemas from Redis and merges them into one unified schema. Incoming requests are automatically routed to the correct downstream service via HTTP.
3. **Type Extensions & Delegation** — Each service can extend types that belong to another service by declaring `@delegate` directives in its `Stitching.graphql`. For example, the `Bag` type is extended to include a `user` field that is resolved by the `common` service.

---

## Tech Stack

| Technology | Version | Purpose |
|---|---|---|
| .NET | 6.0 | Runtime and Web API host |
| C# | 10.0 | Language (nullable enabled, implicit usings) |
| HotChocolate.AspNetCore | 12.14.0 | GraphQL server per service |
| HotChocolate.Stitching.Redis | 12.14.0 | Schema publishing and stitching via Redis |
| HotChocolate.Data | 12.14.0 | Filtering support (`[UseFiltering]`) |
| HotChocolate.Types.Analyzers | 12.14.0 | Source-generator-based type registration |
| StackExchange.Redis | — | Redis client |
| Microsoft.Build.CentralPackageVersions | 2.1.3 | Centralized NuGet version management |

> **No Docker configuration** is currently present in this repository. All services run directly with `dotnet run`.

---

## Service Breakdown

### API.Bag

**Port:** `5002` | **Responsibility:** Shopping cart management

Manages user shopping bags. Each `Bag` belongs to a `User` and contains a list of `BagItem` entries. The service extends the `Bag` type to add a `user` field resolved by `API.Common`, and extends `BagItem` to add a `product` field resolved by `API.Catalog`.

**Schema Highlights**

```graphql
type Bag {
  id: Int!
  userId: Int!
  items: [BagItem!]!
  count: Int!
  user: User            # @delegate → common service
}

type BagItem {
  productId: Int!
  quantity: Int!
  product: Product      # @delegate → catalog service
}
```

**Example Query**

```graphql
query GetBag {
  bag(id: 1) {
    id
    count
    items {
      productId
      quantity
    }
  }
}
```

**Example Mutation**

```graphql
mutation AddItemToBag {
  addToBag(id: 1, productId: 3, quantity: 2) {
    bag {
      id
      count
      items {
        productId
        quantity
      }
    }
  }
}
```

> Mutations use [HotChocolate Mutation Conventions](https://chillicream.com/docs/hotchocolate/v12/defining-a-schema/mutations#mutation-conventions) (`AddMutationConventions(applyToAllMutations: true)`), so each mutation returns a payload wrapper.

---

### API.Catalog

**Port:** `5003` | **Responsibility:** Product and brand catalog

Manages the product and brand catalogue. Supports `[UseFiltering]` on brand queries. Extends the `User` type (owned by `API.Common`) with convenient `favoriteBrands*` fields that delegate back into the catalog service itself.

**Schema Highlights**

```graphql
type Product {
  id: Int!
  shortDescription: String!
  description: String!
  price: Float!
  hasStock: Boolean!
  brand: Brand!
  category: Category!
}

type Brand {
  id: Int!
  name: String!
  description: String!
}

type Category {
  id: Int!
  description: String!
}

extend type User {
  favoriteBrandsByFilter: [Brand]        # @delegate → catalog (with filtering)
  favoriteBrandsByBrandIds: [Brand]      # @delegate → catalog (by IDs array)
  favoriteBrandsByUserId: [Brand]        # @delegate → catalog (by user ID)
}

extend type BagItem {
  product: Product                       # @delegate → catalog
}
```

**Example Queries**

```graphql
query GetProducts {
  products {
    id
    shortDescription
    price
    brand { name }
    category { description }
  }
}

query GetBrands {
  brands(where: { name: { eq: "Nike" } }) {
    id
    name
    description
  }
}

query GetBrandsByUserId {
  brandsByUserId(id: 1) {
    id
    name
  }
}
```

---

### API.Common

**Port:** `5004` | **Responsibility:** Shared user and brand data

Provides `User` and `Brand` (common/shared) data used across the other services. Extends the `User` type with computed `favoriteBrands` fields resolved locally. Also extends the `Bag` type to add a `user` field delegated to this service.

**Schema Highlights**

```graphql
type User {
  id: Int!
  username: String!
  favoriteBrandIds: [Int!]
  favoriteCommonBrands: [CommonBrand!]!   # resolved locally
  favoriteBrands: [Brand!]!               # resolved locally
}

type Brand {
  id: Int!
  name: String!
}

type CommonBrand {
  id: Int!
  name: String!
}

extend type Bag {
  user: User   # @delegate → common service
}
```

**Example Queries**

```graphql
query GetUser {
  user(id: 1) {
    id
    username
    favoriteBrandIds
  }
}

query GetUsers {
  users {
    id
    username
  }
}
```

---

### API.Gateway

**Port:** `5001` | **Responsibility:** Schema stitching and unified graph

The Gateway is a thin orchestration layer. It does not define any types of its own. On startup it:

1. Reads the `GraphQL` configuration section to discover services and the Redis endpoint.
2. Registers an `HttpClient` for each downstream service (used by HotChocolate to forward requests).
3. Calls `AddRemoteSchemasFromRedis("gateway", redis)` to pull all published SDL schemas from Redis and compose them into a single schema.

Any query sent to the Gateway is automatically analyzed, split into sub-queries per service, dispatched in parallel, and merged before being returned to the client.

**Configuration (`appsettings.json`)**

```json
{
  "GraphQL": {
    "ServiceName": "gateway",
    "Services": [
      { "Name": "bag",     "Endpoint": "http://localhost:5002/graphql" },
      { "Name": "catalog", "Endpoint": "http://localhost:5003/graphql" },
      { "Name": "common",  "Endpoint": "http://localhost:5004/graphql" }
    ],
    "Redis": { "Endpoint": "localhost" }
  }
}
```

---

### Framework.Diagnostics

A shared class library referenced by all services. It provides a `CustomExecutionEventListener` that hooks into HotChocolate's diagnostic pipeline to log:

- Request start/end with operation name and elapsed time.
- Resolver field start/end with field name and elapsed time.

All log entries are emitted via `ILogger<T>` and integrate naturally with the .NET logging infrastructure.

---

## Running the Project

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- A running Redis instance on `localhost:6379` (default)

### Start Redis locally

Using the official Docker image (quickest approach):

```bash
docker run -d -p 6379:6379 redis:7-alpine
```

Or install and start Redis directly via your OS package manager.

### Run each service

Open a separate terminal for each service and run in the following order — downstream services must publish their schemas **before** the Gateway starts.

```bash
# Terminal 1 — Bag service
cd API.Bag
dotnet run

# Terminal 2 — Catalog service
cd API.Catalog
dotnet run

# Terminal 3 — Common service
cd API.Common
dotnet run

# Terminal 4 — Gateway (start last, after schemas are published)
cd API.Gateway
dotnet run
```

### Environment Variables / Configuration

All services read their configuration from `appsettings.json`. The following keys are relevant:

| Key | Description | Default |
|---|---|---|
| `GraphQL:ServiceName` | Name used to register this service's schema in Redis | per service |
| `GraphQL:GatewayName` | Redis key under which schemas are aggregated | `"gateway"` |
| `GraphQL:Redis:Endpoint` | Redis connection string | `"localhost"` |
| `GraphQL:Stitching:Enabled` | Whether to publish the schema to Redis on startup | `true` |
| `GraphQL:Services[*]:Endpoint` | *(Gateway only)* Downstream service HTTP endpoint | see config |

To override any value at runtime, use standard .NET configuration overrides:

```bash
# Example: point the bag service at a remote Redis
dotnet run --GraphQL:Redis:Endpoint=redis.example.com:6379
```

Or set environment variables (colons become double-underscores):

```bash
export GraphQL__Redis__Endpoint=redis.example.com:6379
dotnet run
```

---

## GraphQL Endpoints

| Service | URL | GraphQL Playground |
|---|---|---|
| API.Bag | `http://localhost:5002/graphql` | `http://localhost:5002/graphql` |
| API.Catalog | `http://localhost:5003/graphql` | `http://localhost:5003/graphql` |
| API.Common | `http://localhost:5004/graphql` | `http://localhost:5004/graphql` |
| **API.Gateway** | **`http://localhost:5001/graphql`** | **`http://localhost:5001/graphql`** |

> HotChocolate ships with [Banana Cake Pop](https://chillicream.com/docs/bananacakepop) embedded at the `/graphql` endpoint. Open any of the URLs above in a browser to access the interactive IDE.

---

## Example Federated Query

The following query is sent to the **Gateway** only. HotChocolate automatically fans it out to `bag`, `catalog`, and `common`, then merges the results:

```graphql
query GetBagWithDetails {
  bag(id: 1) {
    id
    count
    user {
      id
      username
      favoriteBrands {
        id
        name
      }
    }
    items {
      productId
      quantity
      product {
        id
        shortDescription
        price
        brand {
          name
        }
        category {
          description
        }
      }
    }
  }
}
```

**What happens behind the scenes:**

1. The Gateway receives the request and resolves `bag(id: 1)` against **API.Bag**.
2. The `user` field on `Bag` is decorated with `@delegate(path: "user(id: $fields:userId)", schema: "common")`, so the Gateway fetches the user from **API.Common**.
3. The `favoriteBrands` field on `User` is resolved by **API.Common** locally.
4. Each `BagItem.product` field is decorated with `@delegate(path: "productById(id: $fields:productId)", schema: "catalog")`, so products are fetched from **API.Catalog**.
5. All results are merged and returned as a single JSON response.

---

## Folder Structure

```
graphql-api-gateway/
│
├── GraphQL.API.sln                  # Solution file
├── Directory.Build.props            # Global MSBuild properties (LangVersion, Nullable)
├── Directory.Build.targets          # Central package versioning SDK import
├── Packages.props                   # Centralized NuGet package versions
│
├── API.Gateway/                     # Federation gateway — port 5001
│   ├── Configuration/               # GraphQLConfiguration model
│   ├── Extensions/                  # IServiceCollection extensions (HttpClient registration)
│   ├── Program.cs
│   └── appsettings.json
│
├── API.Bag/                         # Shopping cart service — port 5002
│   ├── Configuration/               # GraphQLConfiguration model
│   ├── Extensions/                  # Schema publish helper
│   ├── Models/                      # Bag, BagItem records
│   ├── Repositories/                # In-memory BagRepository
│   ├── Types/                       # Query and Mutation GraphQL types
│   ├── Stitching.graphql            # Type extensions: Bag.user → common
│   ├── Program.cs
│   └── appsettings.json
│
├── API.Catalog/                     # Product & brand service — port 5003
│   ├── Configuration/               # GraphQLConfiguration model
│   ├── Extensions/                  # Schema publish helper
│   ├── Models/                      # Product, Brand, Category, User records
│   ├── Repositories/                # In-memory Brand/Product/User repositories
│   ├── Services/                    # IBrandService / BrandService
│   ├── Types/                       # Query and QueryBrand GraphQL types
│   ├── Stitching.graphql            # Type extensions: User.favoriteBrands, BagItem.product
│   ├── Program.cs
│   └── appsettings.json
│
├── API.Common/                      # Shared user/brand service — port 5004
│   ├── Configuration/               # GraphQLConfiguration model
│   ├── Extensions/                  # Schema publish helper
│   ├── Models/                      # User, Brand, CommonBrand records
│   ├── Repositories/                # In-memory User/Brand repositories
│   ├── Types/                       # Query, UserExtensions GraphQL types
│   ├── Stitching.graphql            # Type extensions: Bag.user → common
│   ├── Program.cs
│   └── appsettings.json
│
└── Framework.Diagnostics/           # Shared diagnostics library
    ├── ExecutionEvents/             # CustomExecutionEventListener
    └── Framework.Diagnostics.csproj
```

---

## Development Notes

### HotChocolate Version

This project uses **HotChocolate 12.14.0**. All package versions are centrally managed in `Packages.props` via the `Microsoft.Build.CentralPackageVersions` SDK, making version upgrades straightforward.

### Potential Upgrade Path

| Current | Target | Notes |
|---|---|---|
| .NET 6 | .NET 8 | LTS release; minimal code changes expected |
| HotChocolate 12.14.x | HotChocolate 13.x / 14.x | Review breaking changes in Stitching and Fusion APIs; HC 13+ introduces [Fusion](https://chillicream.com/docs/hotchocolate/v13/distributed-schema/schema-federation) as the recommended approach for distributed schemas |

### In-Memory Data

All repositories use **in-memory, hard-coded seed data**. There is no database dependency, making it easy to run the project without any additional infrastructure beyond Redis.

### Centralized Package Management

Versions for all NuGet packages are declared once in `Packages.props`. Individual `.csproj` files reference packages **without** version numbers — the SDK resolves them centrally. To upgrade a package, change the version in `Packages.props` only.
