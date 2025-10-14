# The Complete Guide to API Architecture Styles for Software Engineers

## Table of Contents
- [Introduction](#introduction)
- [HTTP Fundamentals](#http-fundamentals)
- [REST and RESTful APIs](#rest-and-restful-apis)
- [RPC (Remote Procedure Call)](#rpc-remote-procedure-call)
- [GraphQL](#graphql)
- [Architecture Comparison: REST vs RPC vs GraphQL](#architecture-comparison-rest-vs-rpc-vs-graphql)
- [Advanced Topics](#advanced-topics)
- [Best Practices](#best-practices)
- [Interview Preparation](#interview-preparation)
- [Conclusion](#conclusion)

---

## Introduction

This comprehensive guide explores the fundamental API architecture styles that power modern distributed systems: **HTTP-based APIs** (including REST/RESTful), **RPC (Remote Procedure Call)**, and **GraphQL**. Whether you're building microservices, designing public APIs, or preparing for technical interviews, this resource provides the depth and practical knowledge you need to master API design.

### What You'll Learn
- **HTTP Protocol**: From basics to HTTP/3, with deep dives into methods, status codes, headers, and performance optimization
- **REST Architecture**: Principles, constraints, and practical implementation of RESTful APIs
- **RPC Systems**: How remote procedure calls work, focusing on modern frameworks like gRPC
- **GraphQL**: Query language and runtime for APIs, with schema design and implementation
- **Architectural Trade-offs**: When to use REST vs RPC vs GraphQL, with real-world examples
- **Production-Ready Patterns**: Security, scalability, error handling, and monitoring
- **Interview Mastery**: Common questions, system design scenarios, and solution approaches

### Why This Matters
- **HTTP/REST**: Powers public APIs used by billions (GitHub, Twitter, Stripe), essential for web and mobile applications
- **RPC**: Enables high-performance communication in distributed systems (Google, Netflix, Uber)
- **GraphQL**: Solves over-fetching/under-fetching problems (Facebook, GitHub, Shopify)
- **Career Impact**: Understanding these architectures is critical for system design interviews and building scalable production systems

### How to Use This Guide
- **Beginners**: Start with HTTP Fundamentals and REST basics
- **Intermediate**: Focus on GraphQL and architectural comparisons
- **Interview Prep**: Jump to Interview Preparation and system design scenarios
- **Practitioners**: Reference Best Practices and production patterns

---

## HTTP Fundamentals

HTTP (Hypertext Transfer Protocol) is the foundation of data communication on the web. It's a stateless, request-response protocol that enables client-server interactions.

### HTTP Request-Response Model

#### Request Structure
An HTTP request consists of three parts:

```http
GET /api/users/123 HTTP/1.1
Host: example.com
Accept: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9
User-Agent: Mozilla/5.0

```

**Components**:
1. **Start Line**: Method (GET) + URL path (/api/users/123) + HTTP version (HTTP/1.1)
2. **Headers**: Key-value pairs providing metadata
   - `Host`: Target server
   - `Accept`: Desired response format
   - `Authorization`: Authentication credentials
   - `User-Agent`: Client information
3. **Body**: Optional payload (empty in GET requests)

#### Response Structure
An HTTP response mirrors the request structure:

```http
HTTP/1.1 200 OK
Content-Type: application/json
Content-Length: 87
Cache-Control: max-age=3600
ETag: "33a64df551425fcc55e4d42a148795d9"

{"id": 123, "name": "Alice Smith", "email": "alice@example.com", "role": "engineer"}
```

**Components**:
1. **Status Line**: HTTP version + status code (200) + reason phrase (OK)
2. **Headers**: Metadata about the response
   - `Content-Type`: Response format
   - `Content-Length`: Body size in bytes
   - `Cache-Control`: Caching directives
   - `ETag`: Resource version identifier
3. **Body**: The actual data (JSON object in this case)

#### Request-Response Flow

```mermaid
sequenceDiagram
    participant Client as Client<br/>(Browser/App)
    participant DNS as DNS Server
    participant Server as HTTP Server
    participant App as Application
    participant DB as Database

    Client->>DNS: Resolve example.com
    DNS-->>Client: IP: 93.184.216.34
    Client->>Server: TCP Handshake
    Server-->>Client: Connection Established
    Client->>Server: GET /api/users/123<br/>Headers: Accept, Auth
    Server->>App: Route request
    App->>DB: Query user data
    DB-->>App: User record
    App->>App: Serialize to JSON
    App-->>Server: JSON response
    Server-->>Client: 200 OK<br/>Headers + Body
    Client->>Client: Parse & render
```

**Key Points**:
- **Stateless**: Each request is independent; the server doesn't retain client state
- **Text-Based**: HTTP/1.x uses human-readable text (though HTTP/2+ uses binary)
- **Port**: Typically port 80 (HTTP) or 443 (HTTPS)

---

### HTTP Methods Deep Dive

HTTP methods (also called verbs) define the action to perform on a resource. Each method has specific semantics regarding **safety** (no side effects) and **idempotency** (same result when repeated).

#### GET
**Purpose**: Retrieve a resource representation
**Safe**: Yes (read-only)
**Idempotent**: Yes
**Body**: No (query params in URL)

```http
GET /api/products?category=electronics&sort=price&limit=20 HTTP/1.1
Host: store.com
Accept: application/json
```

**Use Cases**:
- Fetching user profiles, product listings, search results
- Retrieving paginated data
- Loading dashboard metrics

**Best Practices**:
- Use query parameters for filtering, sorting, pagination
- Implement caching with `Cache-Control` and `ETag` headers
- Return 404 if resource doesn't exist

#### POST
**Purpose**: Create a new resource or submit data
**Safe**: No (creates/modifies data)
**Idempotent**: No (repeated calls may create duplicates)
**Body**: Yes (typically JSON or form data)

```http
POST /api/orders HTTP/1.1
Host: store.com
Content-Type: application/json
Authorization: Bearer token123

{
  "userId": 456,
  "items": [
    {"productId": 789, "quantity": 2},
    {"productId": 101, "quantity": 1}
  ],
  "shippingAddress": "123 Main St"
}
```

**Response**:
```http
HTTP/1.1 201 Created
Location: /api/orders/9876
Content-Type: application/json

{
  "orderId": 9876,
  "status": "pending",
  "createdAt": "2025-10-13T10:30:00Z"
}
```

**Use Cases**:
- Creating new users, orders, posts
- Form submissions
- Uploading files

**Best Practices**:
- Return `201 Created` with `Location` header pointing to the new resource
- Include the created resource in the response body
- Validate input thoroughly; return `400 Bad Request` with error details if invalid

#### PUT
**Purpose**: Update/replace a resource completely
**Safe**: No
**Idempotent**: Yes (same call produces the same result)
**Body**: Yes (complete resource representation)

```http
PUT /api/users/123 HTTP/1.1
Host: example.com
Content-Type: application/json

{
  "id": 123,
  "name": "Alice Johnson",
  "email": "alice.j@example.com",
  "role": "senior-engineer",
  "department": "backend"
}
```

**Key Characteristic**: Client sends the **complete** resource. Missing fields are removed/set to defaults.

**Use Cases**:
- Replacing entire resource state
- Setting configuration values
- Updating resources with all fields

**Best Practices**:
- Return `200 OK` with updated resource or `204 No Content`
- If resource doesn't exist, optionally create it (return `201 Created`)
- Validate that all required fields are present

#### PATCH
**Purpose**: Partially update a resource
**Safe**: No
**Idempotent**: Usually yes (but depends on implementation)
**Body**: Yes (partial resource representation)

```http
PATCH /api/users/123 HTTP/1.1
Host: example.com
Content-Type: application/json

{
  "email": "alice.new@example.com",
  "department": "platform"
}
```

**Formats**:
- **JSON Merge Patch** (RFC 7396): Simple object merge
- **JSON Patch** (RFC 6902): Array of operations

```json
[
  { "op": "replace", "path": "/email", "value": "alice.new@example.com" },
  { "op": "add", "path": "/tags/-", "value": "senior" }
]
```

**Use Cases**:
- Updating specific fields (email, status)
- Incrementing counters
- Toggling flags

**Best Practices**:
- Use JSON Merge Patch for simplicity or JSON Patch for complex operations
- Return `200 OK` with updated resource
- Validate that patched fields are valid

#### DELETE
**Purpose**: Remove a resource
**Safe**: No
**Idempotent**: Yes (deleting twice has same effect as once)
**Body**: Rarely used

```http
DELETE /api/posts/456 HTTP/1.1
Host: blog.com
Authorization: Bearer token123
```

**Response**:
```http
HTTP/1.1 204 No Content
```

**Use Cases**:
- Removing users, posts, orders
- Cleaning up temporary resources
- Cascade deletions (with caution)

**Best Practices**:
- Return `204 No Content` on success (no body needed)
- Return `404 Not Found` if resource doesn't exist
- Consider soft deletes for audit trails
- Implement authorization checks

#### HEAD
**Purpose**: Like GET but returns only headers (no body)
**Safe**: Yes
**Idempotent**: Yes

```http
HEAD /api/files/large-dataset.csv HTTP/1.1
Host: storage.com
```

**Response**:
```http
HTTP/1.1 200 OK
Content-Type: text/csv
Content-Length: 52428800
Last-Modified: Wed, 12 Oct 2025 15:00:00 GMT
```

**Use Cases**:
- Checking if resource exists
- Getting metadata (size, modification time)
- Testing caching behavior

#### OPTIONS
**Purpose**: List allowed methods for a resource (CORS preflight)
**Safe**: Yes
**Idempotent**: Yes

```http
OPTIONS /api/users HTTP/1.1
Host: example.com
Origin: https://frontend.com
```

**Response**:
```http
HTTP/1.1 204 No Content
Allow: GET, POST, PUT, DELETE, OPTIONS
Access-Control-Allow-Origin: https://frontend.com
Access-Control-Allow-Methods: GET, POST, PUT, DELETE
Access-Control-Allow-Headers: Content-Type, Authorization
```

**Use Cases**:
- CORS preflight requests
- API capability discovery

---

### HTTP Status Codes

Status codes inform clients about request outcomes. They're grouped into five classes.

#### 1xx Informational (Rare in APIs)
- **100 Continue**: Server received headers; client can send body
- **101 Switching Protocols**: Upgrading to WebSocket

#### 2xx Success
- **200 OK**: Request succeeded; body contains result
  ```json
  {"message": "User retrieved", "data": {...}}
  ```

- **201 Created**: Resource created successfully (POST)
  ```http
  Location: /api/users/789
  ```

- **202 Accepted**: Request accepted for async processing
  ```json
  {"jobId": "abc-123", "status": "processing"}
  ```

- **204 No Content**: Success with no response body (DELETE, PATCH)

- **206 Partial Content**: Range request succeeded (video streaming)

#### 3xx Redirection
- **301 Moved Permanently**: Resource relocated; update bookmarks
  ```http
  Location: https://newdomain.com/api/users/123
  ```

- **302 Found**: Temporary redirect (use original URL for future requests)

- **304 Not Modified**: Resource unchanged since last request (caching)
  ```http
  ETag: "abc123"
  ```

- **307 Temporary Redirect**: Like 302 but preserves HTTP method

- **308 Permanent Redirect**: Like 301 but preserves HTTP method

#### 4xx Client Errors
- **400 Bad Request**: Malformed request (invalid JSON, missing fields)
  ```json
  {
    "error": "VALIDATION_ERROR",
    "message": "Invalid input",
    "details": [
      {"field": "email", "issue": "must be valid email format"}
    ]
  }
  ```

- **401 Unauthorized**: Authentication required or failed
  ```http
  WWW-Authenticate: Bearer realm="api"
  ```

- **403 Forbidden**: Authenticated but not authorized
  ```json
  {"error": "FORBIDDEN", "message": "Insufficient permissions"}
  ```

- **404 Not Found**: Resource doesn't exist
  ```json
  {"error": "NOT_FOUND", "message": "User 123 not found"}
  ```

- **405 Method Not Allowed**: HTTP method not supported for resource
  ```http
  Allow: GET, POST
  ```

- **409 Conflict**: Request conflicts with current state (duplicate email)
  ```json
  {"error": "CONFLICT", "message": "Email already registered"}
  ```

- **422 Unprocessable Entity**: Request understood but semantically incorrect
  ```json
  {"error": "INVALID_DATA", "message": "Age must be positive"}
  ```

- **429 Too Many Requests**: Rate limit exceeded
  ```http
  Retry-After: 3600
  X-RateLimit-Limit: 1000
  X-RateLimit-Remaining: 0
  ```

#### 5xx Server Errors
- **500 Internal Server Error**: Generic server failure
  ```json
  {"error": "INTERNAL_ERROR", "message": "An unexpected error occurred", "requestId": "req-xyz"}
  ```

- **502 Bad Gateway**: Upstream server returned invalid response

- **503 Service Unavailable**: Server overloaded or down for maintenance
  ```http
  Retry-After: 600
  ```

- **504 Gateway Timeout**: Upstream server didn't respond in time

**Deep Insight**: Proper status code usage enables:
- **Client-side handling**: Browsers automatically follow 3xx redirects
- **Caching**: 304 avoids re-transmitting unchanged resources
- **Debugging**: Clear error codes speed up troubleshooting
- **Automation**: Monitoring tools alert on 5xx increases

---

### HTTP Headers Reference

Headers provide metadata for fine-grained control over requests and responses.

#### Request Headers

**Authentication & Authorization**
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Cookie: session=abc123; user_pref=dark_mode
```

**Content Negotiation**
```http
Accept: application/json, application/xml;q=0.9, */*;q=0.8
Accept-Language: en-US,en;q=0.9,es;q=0.8
Accept-Encoding: br, gzip, deflate
```

**Caching & Validation**
```http
If-None-Match: "33a64df551425fcc55e4d42a148795d9"
If-Modified-Since: Wed, 12 Oct 2025 10:00:00 GMT
Cache-Control: no-cache
```

**Client Information**
```http
User-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7)
Referer: https://example.com/page
```

**CORS**
```http
Origin: https://frontend.com
Access-Control-Request-Method: POST
Access-Control-Request-Headers: Content-Type
```

#### Response Headers

**Content Metadata**
```http
Content-Type: application/json; charset=utf-8
Content-Length: 1024
Content-Encoding: gzip
Content-Language: en-US
```

**Caching**
```http
Cache-Control: public, max-age=3600, must-revalidate
ETag: "33a64df551425fcc55e4d42a148795d9"
Expires: Thu, 13 Oct 2025 10:00:00 GMT
Vary: Accept-Encoding, Accept-Language
```

**Security**
```http
Strict-Transport-Security: max-age=31536000; includeSubDomains; preload
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
Content-Security-Policy: default-src 'self'
```

**CORS**
```http
Access-Control-Allow-Origin: https://frontend.com
Access-Control-Allow-Methods: GET, POST, PUT, DELETE
Access-Control-Allow-Headers: Content-Type, Authorization
Access-Control-Max-Age: 86400
```

**Rate Limiting**
```http
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 742
X-RateLimit-Reset: 1697198400
Retry-After: 3600
```

**Location & Redirection**
```http
Location: /api/users/789
```


### HTTP Version Evolution

Understanding HTTP version evolution is crucial for making informed architectural decisions.

#### HTTP/0.9 (1991) - The One-Line Protocol
**Characteristics**:
- Single method: GET
- No headers
- Text-only responses (HTML)
- Connection closes after each request

```http
GET /index.html
```

**Limitations**: No metadata, no status codes, no content type specification.

#### HTTP/1.0 (1996) - Adding Structure
**New Features**:
- Request/response headers
- Status codes (200, 404, etc.)
- Multiple methods (GET, POST, HEAD)
- Content-Type support (HTML, images, etc.)
- Basic authentication

```http
GET /image.jpg HTTP/1.0
Host: example.com
User-Agent: Mozilla/1.0

HTTP/1.0 200 OK
Content-Type: image/jpeg
Content-Length: 5120

[binary data]
```

**Problem**: Each request required a new TCP connection (expensive handshake).

#### HTTP/1.1 (1997) - Persistent Connections
**Major Improvements**:
- **Persistent connections** (Keep-Alive): Reuse TCP connection for multiple requests
- **Pipelining**: Send multiple requests without waiting for responses
- **Chunked transfer encoding**: Stream data without knowing size upfront
- **Host header**: Virtual hosting support
- **Cache control**: `Cache-Control`, `ETag`
- **New methods**: PUT, PATCH, DELETE, OPTIONS

```http
GET /page1.html HTTP/1.1
Host: example.com
Connection: keep-alive

GET /page2.html HTTP/1.1
Host: example.com
Connection: keep-alive
```

**Limitations**:
- **Head-of-line blocking**: One slow request blocks others in the queue
- **Header overhead**: Text headers repeated on every request
- **No request prioritization**

#### HTTP/2 (2015) - Binary and Multiplexing
**Revolutionary Changes**:
- **Binary framing**: Binary protocol instead of text
- **Multiplexing**: Multiple requests/responses in parallel over single connection
- **Stream prioritization**: Assign priority to requests
- **Server push**: Proactively send resources
- **Header compression**: HPACK algorithm reduces header size

```mermaid
graph LR
    A[Client] -->|Single TCP Connection| B[Server]
    B -->|Stream 1: HTML| A
    B -->|Stream 3: CSS| A
    B -->|Stream 5: JS| A
    B -->|Stream 7: Image| A

    style A fill:#e1f5ff
    style B fill:#ffe1e1
```

**Example Multiplexed Streams**:
```
Stream 1: GET /index.html
Stream 3: GET /style.css
Stream 5: GET /script.js
Stream 7: GET /logo.png
```

**Benefits**:
- Eliminates head-of-line blocking at application layer
- Reduces latency (single connection, no handshakes)
- Better resource utilization

**Server Push Example**:
```
Client: GET /index.html
Server: 200 OK /index.html
Server: PUSH_PROMISE /style.css (proactive)
Server: 200 OK /style.css
```

**Trade-offs**:
- Still has TCP head-of-line blocking at transport layer
- Complex to debug (binary protocol)
- Requires TLS (encrypted connections)

#### HTTP/3 (2022) - QUIC Revolution
**Core Innovation**: Uses **QUIC** (UDP-based) instead of TCP

**Key Features**:
- **Zero RTT**: Resume connections instantly
- **No head-of-line blocking**: Independent streams at transport layer
- **Connection migration**: Survive network changes (WiFi to cellular)
- **Built-in encryption**: TLS 1.3 integrated

```mermaid
graph TD
    A[HTTP/1.1] -->|TCP + TLS| B[3 RTT Connection Setup]
    C[HTTP/2] -->|TCP + TLS| D[2-3 RTT Connection Setup]
    E[HTTP/3] -->|QUIC/UDP| F[0-1 RTT Connection Setup]

    style E fill:#90EE90
    style F fill:#90EE90
```

**Connection Establishment**:
```
HTTP/1.1 / HTTP/2 (TCP):
1. TCP SYN/ACK (1 RTT)
2. TLS handshake (1-2 RTT)
Total: 2-3 RTT

HTTP/3 (QUIC):
1. Combined handshake (0-1 RTT with session resumption)
```

**Benefits**:
- **Faster**: 0-RTT for resumed connections
- **Reliable on mobile**: Survives IP changes
- **Better multiplexing**: No TCP-level blocking

**Challenges**:
- **UDP firewalls**: Some networks block UDP
- **CPU overhead**: Encryption at application layer
- **Maturity**: Newer, less tooling support

**Real-World Adoption**:
- Google: 75% of traffic uses HTTP/3
- Cloudflare: Supports HTTP/3 by default
- Facebook: Deployed for mobile apps

**Version Comparison Table**:

| Feature | HTTP/1.1 | HTTP/2 | HTTP/3 |
|---------|----------|--------|--------|
| Protocol | Text | Binary | Binary |
| Transport | TCP | TCP | QUIC (UDP) |
| Multiplexing | No | Yes | Yes |
| Header Compression | No | Yes (HPACK) | Yes (QPACK) |
| Server Push | No | Yes | Yes |
| Connection Setup | 2-3 RTT | 2-3 RTT | 0-1 RTT |
| Head-of-line Blocking | Yes (request & TCP) | Partial (TCP only) | No |
| TLS Required | Optional | Mandatory | Built-in |
| Connection Migration | No | No | Yes |

---

## REST and RESTful APIs

REST (Representational State Transfer) is an architectural style for designing networked applications, introduced by Roy Fielding in his 2000 doctoral dissertation. It's not a protocol or standard but a set of principles for building scalable, maintainable systems.

### Understanding REST vs RESTful

**REST**:
- An architectural style (set of principles)
- Not tied to HTTP (though HTTP is most common)
- Defines constraints for building distributed systems

**RESTful API**:
- An API that adheres to REST principles
- Typically uses HTTP as transport
- May implement REST constraints fully or partially

**Common Confusion**:
- **Myth**: "Any HTTP API is RESTful"
- **Reality**: RESTful APIs must follow REST constraints (resource-based, stateless, etc.)
- **Example**:
  - ✅ RESTful: `GET /users/123`
  - ❌ Not RESTful: `GET /getUser?id=123` (action-based)

### The Six REST Constraints

Roy Fielding defined six architectural constraints that characterize REST:

#### 1. Client-Server Architecture
**Principle**: Separation of concerns between client and server

**Benefits**:
- Independent evolution (update UI without changing backend)
- Scalability (add more clients or servers)
- Portability (mobile, web, IoT clients use same API)

**Example**:
```mermaid
graph LR
    A[React Web App] -->|HTTP| D[REST API Server]
    B[iOS App] -->|HTTP| D
    C[Android App] -->|HTTP| D
    D -->|SQL| E[(Database)]

    style A fill:#61dafb
    style B fill:#000000
    style C fill:#3DDC84
    style D fill:#ff6b6b
```

#### 2. Stateless
**Principle**: Server doesn't store client context between requests

**Each request must contain**:
- Authentication credentials (JWT, API key)
- All parameters needed to process the request
- No server-side session state

**Example Request (Stateless)**:
```http
GET /api/cart HTTP/1.1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Accept: application/json
```

**Benefits**:
- **Scalability**: Any server can handle any request (no session affinity)
- **Reliability**: Server failures don't lose session state
- **Visibility**: Each request is self-contained (easier debugging)

**Trade-offs**:
- **Increased payload**: Tokens sent with every request
- **Complexity**: Client must manage state (tokens, refresh logic)

**Common Solutions**:
- **JWT (JSON Web Tokens)**: Self-contained auth tokens
- **API Keys**: Stateless authentication
- **OAuth2**: Token-based authorization

**Anti-pattern (Stateful)**:
```http
POST /api/login
{"username": "alice", "password": "secret"}

Response: Set-Cookie: session=abc123

GET /api/cart
Cookie: session=abc123  ← Server stores session state
```

#### 3. Cacheable
**Principle**: Responses must define if they can be cached

**Cache-Control Directives**:
```http
Cache-Control: public, max-age=3600
Cache-Control: private, no-store
Cache-Control: no-cache, must-revalidate
```

**Validation Mechanisms**:
- **ETag**: Content hash for validation
- **Last-Modified**: Timestamp-based validation

**Example Flow**:
```mermaid
sequenceDiagram
    participant Client
    participant Cache
    participant Server

    Client->>Server: GET /api/products
    Server-->>Client: 200 OK<br/>ETag: "abc123"<br/>Cache-Control: max-age=3600
    Client->>Cache: Store response

    Note over Client,Cache: 30 minutes later...

    Client->>Cache: GET /api/products
    Cache-->>Client: 200 OK (from cache)

    Note over Client,Cache: After 1 hour...

    Client->>Server: GET /api/products<br/>If-None-Match: "abc123"
    Server-->>Client: 304 Not Modified
```

**Benefits**:
- **Performance**: Reduced latency and server load
- **Cost**: Lower bandwidth usage
- **Scalability**: CDNs can cache responses

**Best Practices**:
- Use `ETag` for dynamic content
- Set appropriate `max-age` based on update frequency
- Use `private` for user-specific data
- Implement cache invalidation strategy

#### 4. Uniform Interface
**Principle**: Standardized way to interact with resources

**Four Sub-constraints**:

##### a) Resource Identification
Resources are identified by URIs:
```
/users                  ← Collection
/users/123              ← Specific user
/users/123/orders       ← Nested resource
/users/123/orders/456   ← Specific order
```

##### b) Resource Manipulation via Representations
Clients interact with JSON/XML representations:
```json
{
  "id": 123,
  "name": "Alice",
  "email": "alice@example.com"
}
```

##### c) Self-Descriptive Messages
Each message includes metadata:
```http
GET /api/users/123 HTTP/1.1
Accept: application/json

HTTP/1.1 200 OK
Content-Type: application/json
Content-Length: 87
Cache-Control: max-age=300

{"id": 123, "name": "Alice", "email": "alice@example.com"}
```

##### d) HATEOAS (Hypermedia as the Engine of Application State)
Responses include links to related resources:
```json
{
  "id": 123,
  "name": "Alice",
  "email": "alice@example.com",
  "links": {
    "self": "/api/users/123",
    "orders": "/api/users/123/orders",
    "avatar": "/api/users/123/avatar",
    "edit": "/api/users/123",
    "delete": "/api/users/123"
  }
}
```

**HATEOAS Benefits**:
- **Discoverability**: Clients explore API dynamically
- **Loose coupling**: Server can change URLs without breaking clients
- **Workflow guidance**: Links show available actions

**HATEOAS Trade-offs**:
- **Complexity**: More work for clients to parse links
- **Size**: Larger payloads
- **Adoption**: Rarely fully implemented in practice

**Real-World Status**: Most "RESTful" APIs skip HATEOAS for simplicity, but GitHub API implements it:
```json
{
  "url": "https://api.github.com/repos/octocat/Hello-World",
  "forks_url": "https://api.github.com/repos/octocat/Hello-World/forks",
  "issues_url": "https://api.github.com/repos/octocat/Hello-World/issues{/number}"
}
```

#### 5. Layered System
**Principle**: Intermediaries can exist between client and server

**Common Layers**:
```mermaid
graph LR
    A[Client] -->|HTTPS| B[CDN]
    B -->|HTTPS| C[Load Balancer]
    C -->|HTTP| D[API Gateway]
    D -->|HTTP| E[App Server 1]
    D -->|HTTP| F[App Server 2]
    E -->|SQL| G[(Database)]
    F -->|SQL| G

    style A fill:#e1f5ff
    style B fill:#ffd700
    style C fill:#90EE90
    style D fill:#ff6b6b
```

**Benefits**:
- **Security**: Firewalls, proxies filter malicious traffic
- **Scalability**: Load balancers distribute load
- **Performance**: CDNs cache static content
- **Encapsulation**: Clients don't know server topology

**Trade-offs**:
- **Latency**: Each layer adds overhead
- **Debugging**: Harder to trace issues across layers

#### 6. Code-on-Demand (Optional)
**Principle**: Servers can send executable code to clients

**Examples**:
- JavaScript sent to browsers
- Java applets (historical)
- WebAssembly modules

**Why Optional?**:
- Security risks (executing arbitrary code)
- Complexity (sandboxing, verification)
- Rarely used in modern RESTful APIs

---

### Designing RESTful APIs: Best Practices

#### Resource Naming Conventions

**Rules**:
1. Use **nouns**, not verbs
2. Use **plural** names for collections
3. Use **lowercase** and **hyphens** (kebab-case)
4. Keep URLs **hierarchical** and **logical**

**Examples**:

| ✅ Good | ❌ Bad |
|---------|--------|
| `/users` | `/getUsers` |
| `/users/123` | `/user/123` |
| `/users/123/orders` | `/getUserOrders?id=123` |
| `/blog-posts` | `/blog_posts` |
| `/products/123/reviews` | `/products/123/getReviews` |

**Complex Resources**:
```
/organizations/456/teams/789/members
/courses/101/lessons/5/assignments/12
/stores/nyc-broadway/inventory/products/789
```

#### HTTP Method Mapping

**Standard CRUD Operations**:

| Operation | HTTP Method | Endpoint | Success Code |
|-----------|-------------|----------|--------------|
| List all | GET | `/users` | 200 OK |
| Get one | GET | `/users/123` | 200 OK |
| Create | POST | `/users` | 201 Created |
| Update (full) | PUT | `/users/123` | 200 OK |
| Update (partial) | PATCH | `/users/123` | 200 OK |
| Delete | DELETE | `/users/123` | 204 No Content |

**Advanced Operations**:

```http
# Search/Filter
GET /products?category=electronics&price_max=1000&sort=rating

# Pagination
GET /posts?page=2&size=20

# Field selection
GET /users/123?fields=name,email

# Bulk operations
POST /users/bulk-delete
{"ids": [1, 2, 3]}

# Custom actions (when CRUD doesn't fit)
POST /orders/123/cancel
POST /users/123/reset-password
POST /documents/456/publish
```

#### Response Structure Standards

**Successful Response**:
```json
{
  "data": {
    "id": 123,
    "name": "Alice",
    "email": "alice@example.com"
  },
  "meta": {
    "timestamp": "2025-10-13T10:30:00Z",
    "version": "1.0"
  }
}
```

**Collection Response with Pagination**:
```json
{
  "data": [
    {"id": 1, "title": "Post 1"},
    {"id": 2, "title": "Post 2"}
  ],
  "pagination": {
    "page": 2,
    "size": 20,
    "total": 157,
    "totalPages": 8,
    "hasNext": true,
    "hasPrev": true
  },
  "links": {
    "self": "/posts?page=2",
    "first": "/posts?page=1",
    "prev": "/posts?page=1",
    "next": "/posts?page=3",
    "last": "/posts?page=8"
  }
}
```

**Error Response (RFC 7807 Problem Details)**:
```json
{
  "type": "https://api.blog.com/errors/validation-error",
  "title": "Validation Failed",
  "status": 400,
  "detail": "Request body contains invalid fields",
  "instance": "/api/users",
  "errors": [
    {
      "field": "email",
      "message": "must be a valid email address",
      "code": "INVALID_EMAIL"
    },
    {
      "field": "age",
      "message": "must be at least 18",
      "code": "AGE_TOO_LOW"
    }
  ],
  "timestamp": "2025-10-13T10:30:00Z",
  "requestId": "req-abc-123"
}
```

---

## GraphQL

GraphQL is a query language for APIs and a runtime for executing those queries, developed by Facebook in 2012 and open-sourced in 2015. It provides a more efficient, powerful, and flexible alternative to REST.

### What is GraphQL?

**Core Concept**: A single endpoint that allows clients to request exactly the data they need

```mermaid
graph LR
    A[Client] -->|Single Query| B[GraphQL Endpoint]
    B --> C[Resolver Functions]
    C --> D[(Database)]
    C --> E[User Service]
    C --> F[Order Service]
    C --> G[External API]

    style A fill:#e1f5ff
    style B fill:#ff6b6b
    style C fill:#90EE90
```

**Key Characteristics**:
- **Single endpoint**: Usually `/graphql`
- **Flexible queries**: Clients specify exactly what data they need
- **Strongly typed schema**: Defines available operations and data structure
- **Hierarchical**: Queries mirror the structure of returned data
- **Introspective**: Schema is self-documenting

### GraphQL vs REST: Core Differences

| Aspect | REST | GraphQL |
|--------|------|---------|
| **Endpoints** | Multiple (`/users`, `/posts`) | Single (`/graphql`) |
| **Data Fetching** | Fixed structure | Client-specified |
| **Over/Under-fetching** | Common problem | Solved |
| **Network Requests** | Multiple for related data | Single request |
| **Caching** | HTTP-level (URLs) | More complex |
| **Learning Curve** | Lower | Higher |
| **Versioning** | URL/Header versioning | Schema evolution |
| **File Uploads** | Native support | Requires specification |

**Example Comparison**:

**REST (Multiple requests)**:
```http
GET /users/123
GET /users/123/posts
GET /posts/456/comments
```

**GraphQL (Single request)**:
```graphql
query {
  user(id: 123) {
    name
    email
    posts {
      title
      comments {
        content
        author { name }
      }
    }
  }
}
```

### GraphQL Schema & Type System

The schema defines the contract between client and server, specifying available operations and data structure.

#### Basic Types

```graphql
# filepath: blog-graphql/schema.graphql
# Scalar types
scalar Date
scalar Upload

# Object types
type User {
  id: ID!           # Non-nullable ID
  name: String!     # Non-nullable string
  email: String!
  bio: String       # Nullable string
  avatar: String
  createdAt: Date!
  posts: [Post!]!   # Non-nullable array of non-nullable Posts
}

type Post {
  id: ID!
  title: String!
  content: String!
  published: Boolean!
  publishedAt: Date
  author: User!
  comments: [Comment!]!
  tags: [String!]!
}

type Comment {
  id: ID!
  content: String!
  createdAt: Date!
  author: User!
  post: Post!
}

# Input types (for mutations)
input CreateUserInput {
  name: String!
  email: String!
  bio: String
}

input CreatePostInput {
  title: String!
  content: String!
  tags: [String!]!
}

input UpdatePostInput {
  title: String
  content: String
  published: Boolean
  tags: [String!]
}
```

#### Root Operations

```graphql
# filepath: blog-graphql/schema.graphql
type Query {
  # Get single entities
  user(id: ID!): User
  post(id: ID!): Post
  comment(id: ID!): Comment

  # Get collections with filtering/pagination
  users(
    first: Int
    after: String
    filter: UserFilter
  ): UserConnection!

  posts(
    first: Int = 10
    after: String
    filter: PostFilter
    orderBy: PostOrderBy = CREATED_AT_DESC
  ): PostConnection!

  # Search
  searchPosts(query: String!): [Post!]!
}

type Mutation {
  # User operations
  createUser(input: CreateUserInput!): CreateUserPayload!
  updateUser(id: ID!, input: UpdateUserInput!): UpdateUserPayload!
  deleteUser(id: ID!): DeleteUserPayload!

  # Post operations
  createPost(input: CreatePostInput!): CreatePostPayload!
  updatePost(id: ID!, input: UpdatePostInput!): UpdatePostPayload!
  deletePost(id: ID!): DeletePostPayload!

  # Comment operations
  createComment(postId: ID!, content: String!): CreateCommentPayload!
  deleteComment(id: ID!): DeleteCommentPayload!
}

type Subscription {
  # Real-time updates
  postAdded: Post!
  commentAdded(postId: ID!): Comment!
}

# Connection types for pagination (Relay specification)
type UserConnection {
  edges: [UserEdge!]!
  pageInfo: PageInfo!
  totalCount: Int!
}

type UserEdge {
  node: User!
  cursor: String!
}

type PageInfo {
  hasNextPage: Boolean!
  hasPreviousPage: Boolean!
  startCursor: String
  endCursor: String
}

# Filter inputs
input UserFilter {
  name_contains: String
  email_contains: String
  hasPost: Boolean
}

input PostFilter {
  published: Boolean
  author: ID
  tags_contain: [String!]
  createdAfter: Date
  createdBefore: Date
}

enum PostOrderBy {
  CREATED_AT_ASC
  CREATED_AT_DESC
  TITLE_ASC
  TITLE_DESC
  AUTHOR_NAME_ASC
}

# Mutation payloads (best practice for error handling)
type CreateUserPayload {
  user: User
  errors: [UserError!]!
}

type UserError {
  field: String
  message: String!
  code: String!
}
```

### GraphQL Implementation: Node.js

#### Server Implementation with Apollo Server

```javascript
// filepath: blog-graphql/server.js
const { ApolloServer } = require('apollo-server-express');
const { gql } = require('apollo-server-express');
const express = require('express');

// In-memory storage (use database in production)
const users = new Map([
  [1, { id: 1, name: 'Alice', email: 'alice@example.com', bio: 'Software Engineer', createdAt: new Date() }],
  [2, { id: 2, name: 'Bob', email: 'bob@example.com', bio: 'Product Manager', createdAt: new Date() }]
]);

const posts = new Map([
  [1, { id: 1, title: 'GraphQL Introduction', content: 'GraphQL is amazing...', published: true, authorId: 1, publishedAt: new Date(), tags: ['graphql', 'api'] }],
  [2, { id: 2, title: 'REST vs GraphQL', content: 'Comparing architectures...', published: true, authorId: 2, publishedAt: new Date(), tags: ['rest', 'graphql'] }]
]);

const comments = new Map([
  [1, { id: 1, content: 'Great article!', postId: 1, authorId: 2, createdAt: new Date() }],
  [2, { id: 2, content: 'Very informative', postId: 1, authorId: 1, createdAt: new Date() }]
]);

let userIdCounter = 3;
let postIdCounter = 3;
let commentIdCounter = 3;

// Type definitions
const typeDefs = gql`
  scalar Date

  type User {
    id: ID!
    name: String!
    email: String!
    bio: String
    createdAt: Date!
    posts: [Post!]!
    comments: [Comment!]!
  }

  type Post {
    id: ID!
    title: String!
    content: String!
    published: Boolean!
    publishedAt: Date
    author: User!
    comments: [Comment!]!
    tags: [String!]!
  }

  type Comment {
    id: ID!
    content: String!
    createdAt: Date!
    author: User!
    post: Post!
  }

  type Query {
    user(id: ID!): User
    users: [User!]!
    post(id: ID!): Post
    posts(published: Boolean): [Post!]!
    searchPosts(query: String!): [Post!]!
  }

  type Mutation {
    createUser(name: String!, email: String!, bio: String): User!
    updateUser(id: ID!, name: String, email: String, bio: String): User!
    deleteUser(id: ID!): Boolean!

    createPost(title: String!, content: String!, tags: [String!]!): Post!
    updatePost(id: ID!, title: String, content: String, published: Boolean): Post!
    deletePost(id: ID!): Boolean!

    createComment(postId: ID!, content: String!): Comment!
    deleteComment(id: ID!): Boolean!
  }

  type Subscription {
    postAdded: Post!
    commentAdded(postId: ID!): Comment!
  }
`;

// Resolvers
const resolvers = {
  Date: {
    serialize: (date) => date.toISOString(),
    parseValue: (value) => new Date(value),
    parseLiteral: (ast) => new Date(ast.value)
  },

  Query: {
    user: (parent, { id }) => {
      return users.get(parseInt(id));
    },

    users: () => {
      return Array.from(users.values());
    },

    post: (parent, { id }) => {
      return posts.get(parseInt(id));
    },

    posts: (parent, { published }) => {
      const allPosts = Array.from(posts.values());
      if (published !== undefined) {
        return allPosts.filter(post => post.published === published);
      }
      return allPosts;
    },

    searchPosts: (parent, { query }) => {
      const allPosts = Array.from(posts.values());
      const lowerQuery = query.toLowerCase();
      return allPosts.filter(post =>
        post.title.toLowerCase().includes(lowerQuery) ||
        post.content.toLowerCase().includes(lowerQuery) ||
        post.tags.some(tag => tag.toLowerCase().includes(lowerQuery))
      );
    }
  },

  Mutation: {
    createUser: (parent, { name, email, bio }) => {
      // Validation
      if (!name || !email) {
        throw new Error('Name and email are required');
      }

      // Check for duplicate email
      const existingUser = Array.from(users.values()).find(u => u.email === email);
      if (existingUser) {
        throw new Error(`User with email ${email} already exists`);
      }

      const userId = userIdCounter++;
      const newUser = {
        id: userId,
        name,
        email,
        bio: bio || '',
        createdAt: new Date()
      };

      users.set(userId, newUser);
      return newUser;
    },

    updateUser: (parent, { id, name, email, bio }) => {
      const userId = parseInt(id);
      const user = users.get(userId);

      if (!user) {
        throw new Error(`User with ID ${id} not found`);
      }

      const updatedUser = {
        ...user,
        ...(name && { name }),
        ...(email && { email }),
        ...(bio !== undefined && { bio })
      };

      users.set(userId, updatedUser);
      return updatedUser;
    },

    deleteUser: (parent, { id }) => {
      const userId = parseInt(id);
      if (!users.has(userId)) {
        throw new Error(`User with ID ${id} not found`);
      }

      users.delete(userId);
      return true;
    },

    createPost: (parent, { title, content, tags }, { user }) => {
      // Authentication check (in real app)
      if (!user) {
        throw new Error('Authentication required');
      }

      const postId = postIdCounter++;
      const newPost = {
        id: postId,
        title,
        content,
        published: false,
        authorId: user.id,
        tags: tags || [],
        createdAt: new Date()
      };

      posts.set(postId, newPost);

      // Trigger subscription
      pubsub.publish('POST_ADDED', { postAdded: newPost });

      return newPost;
    },

    updatePost: (parent, { id, title, content, published }) => {
      const postId = parseInt(id);
      const post = posts.get(postId);

      if (!post) {
        throw new Error(`Post with ID ${id} not found`);
      }

      const updatedPost = {
        ...post,
        ...(title && { title }),
        ...(content && { content }),
        ...(published !== undefined && { published, publishedAt: published ? new Date() : null })
      };

      posts.set(postId, updatedPost);
      return updatedPost;
    },

    createComment: (parent, { postId, content }, { user }) => {
      if (!user) {
        throw new Error('Authentication required');
      }

      const post = posts.get(parseInt(postId));
      if (!post) {
        throw new Error(`Post with ID ${postId} not found`);
      }

      const commentId = commentIdCounter++;
      const newComment = {
        id: commentId,
        content,
        postId: parseInt(postId),
        authorId: user.id,
        createdAt: new Date()
      };

      comments.set(commentId, newComment);

      // Trigger subscription
      pubsub.publish('COMMENT_ADDED', { commentAdded: newComment, postId });

      return newComment;
    }
  },

  // Field resolvers (resolve relationships)
  User: {
    posts: (parent) => {
      return Array.from(posts.values()).filter(post => post.authorId === parent.id);
    },

    comments: (parent) => {
      return Array.from(comments.values()).filter(comment => comment.authorId === parent.id);
    }
  },

  Post: {
    author: (parent) => {
      return users.get(parent.authorId);
    },

    comments: (parent) => {
      return Array.from(comments.values()).filter(comment => comment.postId === parent.id);
    }
  },

  Comment: {
    author: (parent) => {
      return users.get(parent.authorId);
    },

    post: (parent) => {
      return posts.get(parent.postId);
    }
  },

  Subscription: {
    postAdded: {
      subscribe: () => pubsub.asyncIterator('POST_ADDED')
    },

    commentAdded: {
      subscribe: (parent, { postId }) => {
        return pubsub.asyncIterator('COMMENT_ADDED');
      },
      resolve: (payload, { postId }) => {
        // Only send comment if it belongs to the subscribed post
        if (payload.postId === parseInt(postId)) {
          return payload.commentAdded;
        }
        return null;
      }
    }
  }
};

// Create Apollo Server
const server = new ApolloServer({
  typeDefs,
  resolvers,
  context: ({ req }) => {
    // In real app: extract user from JWT token
    const token = req.headers.authorization;
    const user = token ? users.get(1) : null; // Mock user
    return { user };
  },
  subscriptions: {
    path: '/graphql',
    onConnect: (connectionParams) => {
      // Authentication for subscriptions
      console.log('Client connected to subscriptions');
    }
  }
});

// Create Express app
const app = express();
server.applyMiddleware({ app });

const httpServer = require('http').createServer(app);
server.installSubscriptionHandlers(httpServer);

const PORT = process.env.PORT || 4000;
httpServer.listen(PORT, () => {
  console.log(`🚀 Server ready at http://localhost:${PORT}${server.graphqlPath}`);
  console.log(`🚀 Subscriptions ready at ws://localhost:${PORT}${server.subscriptionsPath}`);
});
```

#### Client Implementation

```javascript
// filepath: blog-graphql/client.js
const { ApolloClient, InMemoryCache, gql, createHttpLink } = require('@apollo/client');
const { WebSocketLink } = require('@apollo/client/link/ws');
const { split } = require('@apollo/client/link/core');
const { getMainDefinition } = require('@apollo/client/utilities');
const ws = require('ws');

// Create HTTP link
const httpLink = createHttpLink({
  uri: 'http://localhost:4000/graphql',
  headers: {
    authorization: 'Bearer fake-jwt-token'
  }
});

// Create WebSocket link for subscriptions
const wsLink = new WebSocketLink({
  uri: 'ws://localhost:4000/graphql',
  options: {
    reconnect: true
  },
  webSocketImpl: ws
});

// Split based on operation type
const splitLink = split(
  ({ query }) => {
    const definition = getMainDefinition(query);
    return (
      definition.kind === 'OperationDefinition' &&
      definition.operation === 'subscription'
    );
  },
  wsLink,
  httpLink
);

// Create Apollo Client
const client = new ApolloClient({
  link: splitLink,
  cache: new InMemoryCache()
});

// Example queries
const GET_USERS = gql`
  query GetUsers {
    users {
      id
      name
      email
      posts {
        id
        title
        published
      }
    }
  }
`;

const GET_POST_WITH_COMMENTS = gql`
  query GetPost($id: ID!) {
    post(id: $id) {
      id
      title
      content
      author {
        name
        email
      }
      comments {
        id
        content
        author {
          name
        }
        createdAt
      }
    }
  }
`;

const CREATE_POST = gql`
  mutation CreatePost($title: String!, $content: String!, $tags: [String!]!) {
    createPost(title: $title, content: $content, tags: $tags) {
      id
      title
      published
      author {
        name
      }
    }
  }
`;

const SUBSCRIBE_TO_COMMENTS = gql`
  subscription CommentAdded($postId: ID!) {
    commentAdded(postId: $postId) {
      id
      content
      author {
        name
      }
      createdAt
    }
  }
`;

// Example usage
async function examples() {
  try {
    // Query: Get all users with their posts
    const { data: usersData } = await client.query({
      query: GET_USERS
    });
    console.log('Users:', JSON.stringify(usersData, null, 2));

    // Query: Get specific post with comments
    const { data: postData } = await client.query({
      query: GET_POST_WITH_COMMENTS,
      variables: { id: '1' }
    });
    console.log('Post with comments:', JSON.stringify(postData, null, 2));

    // Mutation: Create new post
    const { data: newPost } = await client.mutate({
      mutation: CREATE_POST,
      variables: {
        title: 'GraphQL Best Practices',
        content: 'Here are some GraphQL best practices...',
        tags: ['graphql', 'best-practices']
      }
    });
    console.log('Created post:', newPost);

    // Subscription: Listen for new comments
    const subscription = client.subscribe({
      query: SUBSCRIBE_TO_COMMENTS,
      variables: { postId: '1' }
    }).subscribe({
      next: ({ data }) => {
        console.log('New comment:', data.commentAdded);
      },
      error: (error) => {
        console.error('Subscription error:', error);
      }
    });

    // Unsubscribe after 30 seconds
    setTimeout(() => {
      subscription.unsubscribe();
      console.log('Unsubscribed from comments');
    }, 30000);

  } catch (error) {
    console.error('Error:', error);
  }
}

examples();
```

### Advanced GraphQL Concepts

#### DataLoader for N+1 Problem

The N+1 problem occurs when resolvers make separate database queries for each item in a list.

```javascript
// filepath: blog-graphql/dataloaders.js
const DataLoader = require('dataloader');

// Without DataLoader: N+1 problem
const resolversWithoutDataLoader = {
  Post: {
    author: (parent) => {
      // This will execute for each post - N+1 queries!
      return User.findById(parent.authorId);
    }
  }
};

// With DataLoader: Batched queries
function createUserLoader() {
  return new DataLoader(async (userIds) => {
    // Batch load all users in a single query
    const users = await User.findByIds(userIds);

    // Return users in the same order as requested IDs
    return userIds.map(id => users.find(user => user.id === id));
  });
}

const resolversWithDataLoader = {
  Post: {
    author: (parent, args, { dataloaders }) => {
      // This will batch all author requests
      return dataloaders.userLoader.load(parent.authorId);
    }
  }
};

// Usage in context
const server = new ApolloServer({
  typeDefs,
  resolvers: resolversWithDataLoader,
  context: () => ({
    dataloaders: {
      userLoader: createUserLoader(),
      postLoader: createPostLoader()
    }
  })
});
```

#### Schema Stitching & Federation

For microservices, you can combine multiple GraphQL schemas:

```javascript
// filepath: user-service/user-v1.proto
syntax = "proto3";
package user.v1;

service UserService {
  rpc GetUser(GetUserRequest) returns (User);
}

// filepath: user-service/user-v2.proto
syntax = "proto3";
package user.v2;

service UserService {
  rpc GetUser(GetUserRequest) returns (EnhancedUser);
}
```

#### Real-time Subscriptions

```graphql
# Real-time features
subscription {
  messageAdded(chatId: "123") {
    id
    content
    user {
      name
    }
    createdAt
  }
}

subscription {
  orderStatusChanged(orderId: "456") {
    id
    status
    updatedAt
  }
}
```

---

## Architecture Comparison: REST vs RPC vs GraphQL

### Comprehensive Decision Matrix

```mermaid
graph TD
    Start[API Design Decision] --> Q1{Public API?}
    Q1 -->|Yes| Q2{Complex data relationships?}
    Q2 -->|Yes| GraphQL[Choose GraphQL]
    Q2 -->|No| Q3{Simple CRUD?}
    Q3 -->|Yes| REST[Choose REST]
    Q3 -->|No| GraphQL

    Q1 -->|No| Q4{Internal microservices?}
    Q4 -->|Yes| Q5{Need streaming?}
    Q5 -->|Yes| RPC[Choose gRPC]
    Q5 -->|No| Q6{Performance critical?}
    Q6 -->|Yes| RPC
    Q6 -->|No| Q7{Complex queries?}
    Q7 -->|Yes| GraphQL
    Q7 -->|No| REST

    style REST fill:#90EE90
    style RPC fill:#FFB6C1
    style GraphQL fill:#87CEEB
```

### Detailed Comparison Table

| Feature | REST | gRPC | GraphQL |
|---------|------|------|---------|
| **Protocol** | HTTP | HTTP/2, TCP | HTTP |
| **Data Format** | JSON, XML | Protobuf (binary) | JSON |
| **Endpoints** | Multiple (`/users`, `/posts`) | Service methods | Single (`/graphql`) |
| **Query Flexibility** | Fixed structure | Fixed | Highly flexible |
| **Over/Under-fetching** | Common problem | N/A | Solved |
| **Type System** | Loose (JSON Schema) | Strong (Protobuf) | Strong (Schema) |
| **Caching** | Excellent (HTTP) | Custom | Complex (query-based) |
| **Real-time** | Server-Sent Events | Streaming | Subscriptions |
| **File Upload** | Native support | Custom | Requires spec |
| **Browser Support** | Native | Limited | Native |
| **Learning Curve** | Low | Medium | Medium-High |
| **Tooling** | Mature | Growing | Good |
| **Performance** | Good | Excellent | Good-Excellent |
| **Bandwidth** | High | Low | Medium |
| **Development Speed** | Fast | Medium | Fast (after setup) |

### Use Case Matrix

#### Choose REST When:
- ✅ Public-facing APIs for third-party integration
- ✅ Simple CRUD operations
- ✅ Need extensive HTTP caching
- ✅ Team familiar with HTTP semantics
- ✅ Mobile apps with simple data needs
- ✅ Need broad client support (IoT, legacy systems)

**Examples**: Payment APIs (Stripe), Social media APIs (Twitter), E-commerce catalogs

#### Choose gRPC When:
- ✅ Internal microservices communication
- ✅ Performance-critical applications (< 10ms latency)
- ✅ Streaming data requirements
- ✅ Polyglot microservices architecture
- ✅ Strong typing requirements
- ✅ Binary protocol benefits outweigh HTTP compatibility

**Examples**: Google's internal systems, Netflix microservices, Uber's real-time location tracking

#### Choose GraphQL When:
- ✅ Complex, interconnected data models
- ✅ Multiple client types with different data needs
- ✅ Over-fetching/under-fetching is a problem
- ✅ Need real-time updates (subscriptions)
- ✅ Rapid frontend development
- ✅ API aggregation layer (Backend for Frontend)

**Examples**: Facebook's mobile apps, GitHub API v4, Shopify Partner API

### Performance Benchmarks

#### Scenario: E-commerce Product Page

**Data Required**: Product details, reviews, related products, user recommendations

**REST (Multiple Requests)**:
```
GET /products/123           → 150ms
GET /products/123/reviews   → 120ms
GET /products/related/123   → 200ms
GET /users/456/recommendations → 180ms

Total: 650ms, 4 requests, 2.1KB transferred
```

**GraphQL (Single Request)**:
```graphql
query ProductPage($id: ID!, $userId: ID!) {
  product(id: $id) {
    name          # Only name field
    reviews(first: 5) { # Only first 5 reviews
      rating
      comment
    }
    related(first: 3) { name, price }
  }
  user(id: $userId) {
    recommendations(first: 5) { name, price }
  }
}

Total: 200ms, 1 request, 1.2KB transferred
```

**gRPC (Function Call)**:
```protobuf
rpc GetProductPage(ProductPageRequest) returns (ProductPageResponse);

Total: 80ms, 1 request, 0.8KB transferred (binary)
```

**Performance Summary**:
- **REST**: 650ms, multiple round trips, potential over-fetching
- **GraphQL**: 200ms, single request, exact data needed
- **gRPC**: 80ms, binary efficiency, but requires code generation

### Real-World Architecture Patterns

#### Pattern 1: Hybrid Public API

```mermaid
graph LR
    A[Mobile App] -->|GraphQL| B[BFF Layer]
    C[Web App] -->|GraphQL| B
    D[Partners] -->|REST| E[Public API]
    B -->|gRPC| F[User Service]
    B -->|gRPC| G[Product Service]
    E -->|gRPC| F
    E -->|gRPC| G

    style A fill:#e1f5ff
    style C fill:#e1f5ff
    style D fill:#ffd700
    style B fill:#87CEEB
    style E fill:#90EE90
```

- **External**: REST for partners, GraphQL for own clients
- **Internal**: gRPC for service communication
- **BFF**: GraphQL aggregation layer

#### Pattern 2: Event-Driven Architecture

```mermaid
graph TD
    A[Order Service] -->|gRPC| B[Payment Service]
    A -->|Event| C[Message Queue]
    C -->|Event| D[Inventory Service]
    C -->|Event| E[Notification Service]
    F[GraphQL API] -->|Query| A
    F -->|Subscription| G[WebSocket]
    G -->|Real-time| H[Client]

    style F fill:#87CEEB
    style A fill:#FFB6C1
    style B fill:#FFB6C1
```

- **Commands**: gRPC for synchronous operations
- **Events**: Message queue for async processing
- **Queries**: GraphQL for complex data fetching
- **Real-time**: GraphQL subscriptions for live updates

#### Scenario 3: Design a Social Media Feed API

**Requirements**:
- User profiles, posts, comments, likes
- News feed with personalized content
- Real-time notifications
- Mobile and web clients

**Solution: GraphQL + gRPC Hybrid**

```graphql
# GraphQL for flexible client queries
type Query {
  feed(first: Int, after: String): PostConnection!
  user(id: ID!): User
  post(id: ID!): Post
}

type Mutation {
  createPost(input: CreatePostInput!): Post!
  likePost(postId: ID!): Like!
  followUser(userId: ID!): Follow!
}

type Subscription {
  feedUpdated: Post!
  notificationReceived: Notification!
}

type Post {
  id: ID!
  content: String!
  author: User!
  likes: [Like!]!
  comments(first: Int): CommentConnection!
  createdAt: DateTime!
}
```

**Architecture**:
```mermaid
graph LR
    A[Mobile App] -->|GraphQL| B[GraphQL Gateway]
    C[Web App] -->|GraphQL| B
    B -->|gRPC| D[User Service]
    B -->|gRPC| E[Post Service]
    B -->|gRPC| F[Feed Service]
    B -->|gRPC| G[Notification Service]

    D -->|Events| H[Message Queue]
    E -->|Events| H
    H -->|Stream| I[Real-time Engine]
    I -->|WebSocket| B

    style A fill:#e1f5ff
    style C fill:#e1f5ff
    style B fill:#87CEEB
```

**Key Decisions**:
- **GraphQL**: Flexible queries for different client needs (mobile vs web)
- **gRPC**: High-performance internal service communication
- **Subscriptions**: Real-time feed updates and notifications
- **Event-driven**: Async processing for likes, follows, notifications

#### Scenario 4: Design an API Gateway

**Requirements**:
- Route requests to multiple microservices
- Authentication and rate limiting
- Support REST and GraphQL clients
- Transform protocols (REST ↔ gRPC)

**Solution**:
```mermaid
graph TD
    A[REST Clients] -->|HTTP| C[API Gateway]
    B[GraphQL Clients] -->|HTTP| C
    C -->|Auth/Rate Limit| D[Auth Service]
    C -->|gRPC| E[User Service]
    C -->|gRPC| F[Order Service]
    C -->|gRPC| G[Product Service]

    style C fill:#ffd700
    style A fill:#90EE90
    style B fill:#87CEEB
```

**Implementation Approach**:
1. **Protocol Translation**: Convert REST requests to gRPC calls
2. **Schema Stitching**: Combine multiple GraphQL schemas
3. **Caching Layer**: Cache at gateway level
4. **Circuit Breaker**: Handle service failures gracefully

---

## Best Practices

### REST API Design Checklist

#### URLs & Resources
- ✅ Use nouns, not verbs (`/users`, not `/getUsers`)
- ✅ Use plural names (`/users`, not `/user`)
- ✅ Use hyphens for multi-word resources (`/blog-posts`)
- ✅ Keep hierarchy logical (`/users/123/orders/456`)
- ✅ Avoid deep nesting (max 3 levels)

#### HTTP Methods
- ✅ Use correct methods: GET (read), POST (create), PUT/PATCH (update), DELETE (delete)
- ✅ Make GET, PUT, DELETE idempotent
- ✅ Return 201 Created for POST with Location header
- ✅ Return 204 No Content for DELETE

#### Status Codes
- ✅ Use appropriate status codes (200, 201, 204, 400, 401, 403, 404, 409, 500)
- ✅ Include error details in response body
- ✅ Use RFC 7807 Problem Details format

#### Data Formats
- ✅ Use JSON as default format
- ✅ Support content negotiation (Accept header)
- ✅ Use ISO 8601 for dates (`2025-10-13T10:30:00Z`)
- ✅ Use snake_case or camelCase consistently

#### Pagination
- ✅ Implement pagination for collections
- ✅ Include pagination metadata (page, size, total)
- ✅ Provide navigation links (first, prev, next, last)

#### Filtering & Sorting
- ✅ Use query parameters (`?category=tech&sort=-created_at`)
- ✅ Support multiple sort fields
- ✅ Document filter options

#### Caching
- ✅ Set appropriate Cache-Control headers
- ✅ Use ETags for validation
- ✅ Implement conditional requests (If-None-Match)

#### Security
- ✅ Use HTTPS everywhere
- ✅ Implement authentication (JWT, OAuth2)
- ✅ Validate all inputs
- ✅ Rate limit endpoints
- ✅ Use CORS headers for browser clients

#### Versioning
- ✅ Version your API from day one
- ✅ Support at least 2 versions simultaneously
- ✅ Communicate deprecation 6-12 months in advance
- ✅ Use sunset headers

#### Documentation
- ✅ Provide OpenAPI/Swagger spec
- ✅ Include examples for all endpoints
- ✅ Document error responses
- ✅ Maintain changelog

---

### gRPC Best Practices

#### Protocol Buffers
- ✅ Use semantic versioning for .proto files
- ✅ Never reuse field numbers
- ✅ Use `reserved` for deleted fields
- ✅ Add new fields at end
- ✅ Document messages and services

#### Performance
- ✅ Use streaming for large datasets
- ✅ Implement connection pooling
- ✅ Set appropriate deadlines
- ✅ Enable compression for large payloads

#### Error Handling
- ✅ Use standard gRPC status codes
- ✅ Include detailed error messages
- ✅ Implement retry logic with exponential backoff
- ✅ Set appropriate timeouts

#### Security
- ✅ Use TLS for production
- ✅ Implement authentication interceptors
- ✅ Validate inputs in service methods
- ✅ Use mTLS for service-to-service communication

---

## Interview Preparation

### Common Interview Questions

#### REST API Questions

**Q1: Explain the difference between PUT and PATCH.**

**Answer**:
- **PUT**: Replaces the entire resource. Client sends complete representation. Idempotent.
- **PATCH**: Updates specific fields. Client sends partial changes. May not be idempotent depending on implementation (JSON Merge Patch vs JSON Patch).

**Example**:
```http
# PUT - Full replacement
PUT /users/123
{"id": 123, "name": "Alice", "email": "alice@example.com", "bio": "Engineer"}

# PATCH - Partial update
PATCH /users/123
{"email": "alice.new@example.com"}
```

**Q2: How would you design pagination for a large dataset?**

**Answer**: Use **cursor-based pagination** for consistency:
```http
GET /posts?cursor=eyJpZCI6MTIzfQ&size=20

Response:
{
  "data": [...],
  "pagination": {
    "next_cursor": "eyJpZCI6MTQzfQ",
    "has_more": true
  }
}
```

**Why cursor over offset**: Offset-based (`?page=2`) breaks when data changes. Cursor-based maintains consistency.

**Q3: How do you handle versioning in REST APIs?**

**Answer**: Use URL path versioning (`/api/v1/users`) for clarity. Support N-1 versions. Communicate deprecation 6-12 months in advance. Use sunset headers:
```http
Sunset: Sat, 31 Dec 2025 23:59:59 GMT
```

#### gRPC Questions

**Q4: When would you choose gRPC over REST?**

**Answer**:
- **Internal microservices**: Strong typing, performance
- **Streaming required**: Real-time data, bidirectional communication
- **Performance critical**: Low latency (<10ms), high throughput
- **Polyglot environments**: Auto-generated clients in 10+ languages
- **Strong type safety**

**Don't use for**: Public APIs (browser incompatibility), simple CRUD (REST overhead not justified)

**Q5: Explain gRPC streaming types with examples.**

**Answer**:
1. **Unary**: Single request/response (like REST)
2. **Server streaming**: Client sends one request, server streams multiple responses (e.g., stock price updates)
3. **Client streaming**: Client streams multiple requests, server sends one response (e.g., uploading chunks)
4. **Bidirectional**: Both stream simultaneously (e.g., chat application)

### System Design Scenarios

#### Scenario 1: Design a RESTful API for Twitter

**Requirements**:
- User profiles
- Tweets (create, read, delete)
- Timeline (home, user)
- Follows
- Likes, retweets

**Solution**:
```
Users:
GET    /users/{id}
POST   /users
PATCH  /users/{id}

Tweets:
GET    /tweets/{id}
POST   /tweets
DELETE /tweets/{id}
GET    /users/{id}/tweets

Timeline:
GET    /timeline/home
GET    /users/{id}/timeline

Interactions:
POST   /tweets/{id}/like
DELETE /tweets/{id}/like
POST   /tweets/{id}/retweet
POST   /users/{id}/follow
DELETE /users/{id}/follow
```

**Key Considerations**:
- Pagination for timelines (cursor-based)
- Caching for popular tweets
- Rate limiting per user
- Real-time updates (consider WebSocket for notifications)

#### Scenario 2: Design Microservices Communication

**Requirements**:
- Order service, Inventory service, Payment service
- Low latency requirements (<50ms)
- High reliability
- Strong typing

**Solution**: Use **gRPC for internal services**

```protobuf
// Order Service
service OrderService {
  rpc CreateOrder(CreateOrderRequest) returns (Order);
  rpc GetOrder(GetOrderRequest) returns (Order);
}

// Inventory Service
service InventoryService {
  rpc CheckAvailability(ProductRequest) returns (AvailabilityResponse);
  rpc ReserveStock(ReserveRequest) returns (ReservationResponse);
}

// Payment Service
service PaymentService {
  rpc ProcessPayment(PaymentRequest) returns (PaymentResponse);
}
```

**Architecture**:
```mermaid
graph LR
    Client[REST API Gateway] -->|HTTP/JSON| Gateway
    Gateway -->|gRPC| Order[Order Service]
    Gateway -->|gRPC| Inventory[Inventory Service]
    Gateway -->|gRPC| Payment[Payment Service]

    style Client fill:#e1f5ff
    style Gateway fill:#ffd700
```

**Key Considerations**:
- API Gateway for REST → gRPC translation
- Circuit breakers for fault tolerance
- Distributed tracing (OpenTelemetry)
- Service mesh (Istio) for observability

#### Scenario 3: Design a Social Media Feed API

**Requirements**:
- User profiles, posts, comments, likes
- News feed with personalized content
- Real-time notifications
- Mobile and web clients

**Solution: GraphQL + gRPC Hybrid**

```graphql
# GraphQL for flexible client queries
type Query {
  feed(first: Int, after: String): PostConnection!
  user(id: ID!): User
  post(id: ID!): Post
}

type Mutation {
  createPost(input: CreatePostInput!): Post!
  likePost(postId: ID!): Like!
  followUser(userId: ID!): Follow!
}

type Subscription {
  feedUpdated: Post!
  notificationReceived: Notification!
}

type Post {
  id: ID!
  content: String!
  author: User!
  likes: [Like!]!
  comments(first: Int): CommentConnection!
  createdAt: DateTime!
}
```

**Architecture**:
```mermaid
graph LR
    A[Mobile App] -->|GraphQL| B[GraphQL Gateway]
    C[Web App] -->|GraphQL| B
    B -->|gRPC| D[User Service]
    B -->|gRPC| E[Post Service]
    B -->|gRPC| F[Feed Service]
    B -->|gRPC| G[Notification Service]

    D -->|Events| H[Message Queue]
    E -->|Events| H
    H -->|Stream| I[Real-time Engine]
    I -->|WebSocket| B

    style A fill:#e1f5ff
    style C fill:#e1f5ff
    style B fill:#87CEEB
```

**Key Decisions**:
- **GraphQL**: Flexible queries for different client needs (mobile vs web)
- **gRPC**: High-performance internal service communication
- **Subscriptions**: Real-time feed updates and notifications
- **Event-driven**: Async processing for likes, follows, notifications

#### Scenario 4: Design an API Gateway

**Requirements**:
- Route requests to multiple microservices
- Authentication and rate limiting
- Support REST and GraphQL clients
- Transform protocols (REST ↔ gRPC)

**Solution**:
```mermaid
graph TD
    A[REST Clients] -->|HTTP| C[API Gateway]
    B[GraphQL Clients] -->|HTTP| C
    C -->|Auth/Rate Limit| D[Auth Service]
    C -->|gRPC| E[User Service]
    C -->|gRPC| F[Order Service]
    C -->|gRPC| G[Product Service]

    style C fill:#ffd700
    style A fill:#90EE90
    style B fill:#87CEEB
```

**Implementation Approach**:
1. **Protocol Translation**: Convert REST requests to gRPC calls
2. **Schema Stitching**: Combine multiple GraphQL schemas
3. **Caching Layer**: Cache at gateway level
4. **Circuit Breaker**: Handle service failures gracefully

---

## Best Practices

### REST API Design Checklist

#### URLs & Resources
- ✅ Use nouns, not verbs (`/users`, not `/getUsers`)
- ✅ Use plural names (`/users`, not `/user`)
- ✅ Use hyphens for multi-word resources (`/blog-posts`)
- ✅ Keep hierarchy logical (`/users/123/orders/456`)
- ✅ Avoid deep nesting (max 3 levels)

#### HTTP Methods
- ✅ Use correct methods: GET (read), POST (create), PUT/PATCH (update), DELETE (delete)
- ✅ Make GET, PUT, DELETE idempotent
- ✅ Return 201 Created for POST with Location header
- ✅ Return 204 No Content for DELETE

#### Status Codes
- ✅ Use appropriate status codes (200, 201, 204, 400, 401, 403, 404, 409, 500)
- ✅ Include error details in response body
- ✅ Use RFC 7807 Problem Details format

#### Data Formats
- ✅ Use JSON as default format
- ✅ Support content negotiation (Accept header)
- ✅ Use ISO 8601 for dates (`2025-10-13T10:30:00Z`)
- ✅ Use snake_case or camelCase consistently

#### Pagination
- ✅ Implement pagination for collections
- ✅ Include pagination metadata (page, size, total)
- ✅ Provide navigation links (first, prev, next, last)

#### Filtering & Sorting
- ✅ Use query parameters (`?category=tech&sort=-created_at`)
- ✅ Support multiple sort fields
- ✅ Document filter options

#### Caching
- ✅ Set appropriate Cache-Control headers
- ✅ Use ETags for validation
- ✅ Implement conditional requests (If-None-Match)

#### Security
- ✅ Use HTTPS everywhere
- ✅ Implement authentication (JWT, OAuth2)
- ✅ Validate all inputs
- ✅ Rate limit endpoints
- ✅ Use CORS headers for browser clients

#### Versioning
- ✅ Version your API from day one
- ✅ Support at least 2 versions simultaneously
- ✅ Communicate deprecation 6-12 months in advance
- ✅ Use sunset headers

#### Documentation
- ✅ Provide OpenAPI/Swagger spec
- ✅ Include examples for all endpoints
- ✅ Document error responses
- ✅ Maintain changelog

---

### gRPC Best Practices

#### Protocol Buffers
- ✅ Use semantic versioning for .proto files
- ✅ Never reuse field numbers
- ✅ Use `reserved` for deleted fields
- ✅ Add new fields at end
- ✅ Document messages and services

#### Performance
- ✅ Use streaming for large datasets
- ✅ Implement connection pooling
- ✅ Set appropriate deadlines
- ✅ Enable compression for large payloads

#### Error Handling
- ✅ Use standard gRPC status codes
- ✅ Include detailed error messages
- ✅ Implement retry logic with exponential backoff
- ✅ Set appropriate timeouts

#### Security
- ✅ Use TLS for production
- ✅ Implement authentication interceptors
- ✅ Validate inputs in service methods
- ✅ Use mTLS for service-to-service communication

---

## Interview Preparation

### Common Interview Questions

#### REST API Questions

**Q1: Explain the difference between PUT and PATCH.**

**Answer**:
- **PUT**: Replaces the entire resource. Client sends complete representation. Idempotent.
- **PATCH**: Updates specific fields. Client sends partial changes. May not be idempotent depending on implementation (JSON Merge Patch vs JSON Patch).

**Example**:
```http
# PUT - Full replacement
PUT /users/123
{"id": 123, "name": "Alice", "email": "alice@example.com", "bio": "Engineer"}

# PATCH - Partial update
PATCH /users/123
{"email": "alice.new@example.com"}
```

**Q2: How would you design pagination for a large dataset?**

**Answer**: Use **cursor-based pagination** for consistency:
```http
GET /posts?cursor=eyJpZCI6MTIzfQ&size=20

Response:
{
  "data": [...],
  "pagination": {
    "next_cursor": "eyJpZCI6MTQzfQ",
    "has_more": true
  }
}
```

**Why cursor over offset**: Offset-based (`?page=2`) breaks when data changes. Cursor-based maintains consistency.

**Q3: How do you handle versioning in REST APIs?**

**Answer**: Use URL path versioning (`/api/v1/users`) for clarity. Support N-1 versions. Communicate deprecation 6-12 months in advance. Use sunset headers:
```http
Sunset: Sat, 31 Dec 2025 23:59:59 GMT
```

#### gRPC Questions

**Q4: When would you choose gRPC over REST?**

**Answer**:
- **Internal microservices**: Strong typing, performance
- **Streaming required**: Real-time data, bidirectional communication
- **Performance critical**: Low latency (<10ms), high throughput
- **Polyglot environments**: Auto-generated clients in 10+ languages
- **Strong type safety**

**Don't use for**: Public APIs (browser incompatibility), simple CRUD (REST overhead not justified)

**Q5: Explain gRPC streaming types with examples.**

**Answer**:
1. **Unary**: Single request/response (like REST)
2. **Server streaming**: Client sends one request, server streams multiple responses (e.g., stock price updates)
3. **Client streaming**: Client streams multiple requests, server sends one response (e.g., uploading chunks)
4. **Bidirectional**: Both stream simultaneously (e.g., chat application)

### System Design Scenarios

#### Scenario 1: Design a RESTful API for Twitter

**Requirements**:
- User profiles
- Tweets (create, read, delete)
- Timeline (home, user)
- Follows
- Likes, retweets

**Solution**:
```
Users:
GET    /users/{id}
POST   /users
PATCH  /users/{id}

Tweets:
GET    /tweets/{id}
POST   /tweets
DELETE /tweets/{id}
GET    /users/{id}/tweets

Timeline:
GET    /timeline/home
GET    /users/{id}/timeline

Interactions:
POST   /tweets/{id}/like
DELETE /tweets/{id}/like
POST   /tweets/{id}/retweet
POST   /users/{id}/follow
DELETE /users/{id}/follow
```

**Key Considerations**:
- Pagination for timelines (cursor-based)
- Caching for popular tweets
- Rate limiting per user
- Real-time updates (consider WebSocket for notifications)

#### Scenario 2: Design Microservices Communication

**Requirements**:
- Order service, Inventory service, Payment service
- Low latency requirements (<50ms)
- High reliability
- Strong typing

**Solution**: Use **gRPC for internal services**

```protobuf
// Order Service
service OrderService {
  rpc CreateOrder(CreateOrderRequest) returns (Order);
  rpc GetOrder(GetOrderRequest) returns (Order);
}

// Inventory Service
service InventoryService {
  rpc CheckAvailability(ProductRequest) returns (AvailabilityResponse);
  rpc ReserveStock(ReserveRequest) returns (ReservationResponse);
}

// Payment Service
service PaymentService {
  rpc ProcessPayment(PaymentRequest) returns (PaymentResponse);
}
```

**Architecture**:
```mermaid
graph LR
    Client[REST API Gateway] -->|HTTP/JSON| Gateway
    Gateway -->|gRPC| Order[Order Service]
    Gateway -->|gRPC| Inventory[Inventory Service]
    Gateway -->|gRPC| Payment[Payment Service]

    style Client fill:#e1f5ff
    style Gateway fill:#ffd700
```

**Key Considerations**:
- API Gateway for REST → gRPC translation
- Circuit breakers for fault tolerance
- Distributed tracing (OpenTelemetry)
- Service mesh (Istio) for observability

#### Scenario 3: Design a Social Media Feed API

**Requirements**:
- User profiles, posts, comments, likes
- News feed with personalized content
- Real-time notifications
- Mobile and web clients

**Solution: GraphQL + gRPC Hybrid**

```graphql
# GraphQL for flexible client queries
type Query {
  feed(first: Int, after: String): PostConnection!
  user(id: ID!): User
  post(id: ID!): Post
}

type Mutation {
  createPost(input: CreatePostInput!): Post!
  likePost(postId: ID!): Like!
  followUser(userId: ID!): Follow!
}

type Subscription {
  feedUpdated: Post!
  notificationReceived: Notification!
}

type Post {
  id: ID!
  content: String!
  author: User!
  likes: [Like!]!
  comments(first: Int): CommentConnection!
  createdAt: DateTime!
}
```

**Architecture**:
```mermaid
graph LR
    A[Mobile App] -->|GraphQL| B[GraphQL Gateway]
    C[Web App] -->|GraphQL| B
    B -->|gRPC| D[User Service]
    B -->|gRPC| E[Post Service]
    B -->|gRPC| F[Feed Service]
    B -->|gRPC| G[Notification Service]

    D -->|Events| H[Message Queue]
    E -->|Events| H
    H -->|Stream| I[Real-time Engine]
    I -->|WebSocket| B

    style A fill:#e1f5ff
    style C fill:#e1f5ff
    style B fill:#87CEEB
```

**Key Decisions**:
- **GraphQL**: Flexible queries for different client needs (mobile vs web)
- **gRPC**: High-performance internal service communication
- **Subscriptions**: Real-time feed updates and notifications
- **Event-driven**: Async processing for likes, follows, notifications

#### Scenario 4: Design an API Gateway

**Requirements**:
- Route requests to multiple microservices
- Authentication and rate limiting
- Support REST and GraphQL clients
- Transform protocols (REST ↔ gRPC)

**Solution**:
```mermaid
graph TD
    A[REST Clients] -->|HTTP| C[API Gateway]
    B[GraphQL Clients] -->|HTTP| C
    C -->|Auth/Rate Limit| D[Auth Service]
    C -->|gRPC| E[User Service]
    C -->|gRPC| F[Order Service]
    C -->|gRPC| G[Product Service]

    style C fill:#ffd700
    style A fill:#90EE90
    style B fill:#87CEEB
```

**Implementation Approach**:
1. **Protocol Translation**: Convert REST requests to gRPC calls
2. **Schema Stitching**: Combine multiple GraphQL schemas
3. **Caching Layer**: Cache at gateway level
4. **Circuit Breaker**: Handle service failures gracefully

---

## Best Practices

### REST API Design Checklist

#### URLs & Resources
- ✅ Use nouns, not verbs (`/users`, not `/getUsers`)
- ✅ Use plural names (`/users`, not `/user`)
- ✅ Use hyphens for multi-word resources (`/blog-posts`)
- ✅ Keep hierarchy logical (`/users/123/orders/456`)
- ✅ Avoid deep nesting (max 3 levels)

#### HTTP Methods
- ✅ Use correct methods: GET (read), POST (create), PUT/PATCH (update), DELETE (delete)
- ✅ Make GET, PUT, DELETE idempotent
- ✅ Return 201 Created for POST with Location header
- ✅ Return 204 No Content for DELETE

#### Status Codes
- ✅ Use appropriate status codes (200, 201, 204, 400, 401, 403, 404, 409, 500)
- ✅ Include error details in response body
- ✅ Use RFC 7807 Problem Details format

#### Data Formats
- ✅ Use JSON as default format
- ✅ Support content negotiation (Accept header)
- ✅ Use ISO 8601 for dates (`2025-10-13T10:30:00Z`)
- ✅ Use snake_case or camelCase consistently

#### Pagination
- ✅ Implement pagination for collections
- ✅ Include pagination metadata (page, size, total)
- ✅ Provide navigation links (first, prev, next, last)

#### Filtering & Sorting
- ✅ Use query parameters (`?category=tech&sort=-created_at`)
- ✅ Support multiple sort fields
- ✅ Document filter options

#### Caching
- ✅ Set appropriate Cache-Control headers
- ✅ Use ETags for validation
- ✅ Implement conditional requests (If-None-Match)

#### Security
- ✅ Use HTTPS everywhere
- ✅ Implement authentication (JWT, OAuth2)
- ✅ Validate all inputs
- ✅ Rate limit endpoints
- ✅ Use CORS headers for browser clients

#### Versioning
- ✅ Version your API from day one
- ✅ Support at least 2 versions simultaneously
- ✅ Communicate deprecation 6-12 months in advance
- ✅ Use sunset headers

#### Documentation
- ✅ Provide OpenAPI/Swagger spec
- ✅ Include examples for all endpoints
- ✅ Document error responses
- ✅ Maintain changelog

---

### gRPC Best Practices

#### Protocol Buffers
- ✅ Use semantic versioning for .proto files
- ✅ Never reuse field numbers
- ✅ Use `reserved` for deleted fields
- ✅ Add new fields at end
- ✅ Document messages and services

#### Performance
- ✅ Use streaming for large datasets
- ✅ Implement connection pooling
- ✅ Set appropriate deadlines
- ✅ Enable compression for large payloads

#### Error Handling
- ✅ Use standard gRPC status codes
- ✅ Include detailed error messages
- ✅ Implement retry logic with exponential backoff
- ✅ Set appropriate timeouts

#### Security
- ✅ Use TLS for production
- ✅ Implement authentication interceptors
- ✅ Validate inputs in service methods
- ✅ Use mTLS for service-to-service communication

---

## Interview Preparation

### Common Interview Questions

#### REST API Questions

**Q1: Explain the difference between PUT and PATCH.**

**Answer**:
- **PUT**: Replaces the entire resource. Client sends complete representation. Idempotent.
- **PATCH**: Updates specific fields. Client sends partial changes. May not be idempotent depending on implementation (JSON Merge Patch vs JSON Patch).

**Example**:
```http
# PUT - Full replacement
PUT /users/123
{"id": 123, "name": "Alice", "email": "alice@example.com", "bio": "Engineer"}

# PATCH - Partial update
PATCH /users/123
{"email": "alice.new@example.com"}
```

**Q2: How would you design pagination for a large dataset?**

**Answer**: Use **cursor-based pagination** for consistency:
```http
GET /posts?cursor=eyJpZCI6MTIzfQ&size=20

Response:
{
  "data": [...],
  "pagination": {
    "next_cursor": "eyJpZCI6MTQzfQ",
    "has_more": true
  }
}
```

**Why cursor over offset**: Offset-based (`?page=2`) breaks when data changes. Cursor-based maintains consistency.

**Q3: How do you handle versioning in REST APIs?**

**Answer**: Use URL path versioning (`/api/v1/users`) for clarity. Support N-1 versions. Communicate deprecation 6-12 months in advance. Use sunset headers:
```http
Sunset: Sat, 31 Dec 2025 23:59:59 GMT
```

#### gRPC Questions

**Q4: When would you choose gRPC over REST?**

**Answer**:
- **Internal microservices**: Strong typing, performance
- **Streaming required**: Real-time data, bidirectional communication
- **Performance critical**: Low latency (<10ms), high throughput
- **Polyglot environments**: Auto-generated clients in 10+ languages
- **Strong type safety**

**Don't use for**: Public APIs (browser incompatibility), simple CRUD (REST overhead not justified)

**Q5: Explain gRPC streaming types with examples.**

**Answer**:
1. **Unary**: Single request/response (like REST)
2. **Server streaming**: Client sends one request, server streams multiple responses (e.g., stock price updates)
3. **Client streaming**: Client streams multiple requests, server sends one response (e.g., uploading chunks)
4. **Bidirectional**: Both stream simultaneously (e.g., chat application)

### System Design Scenarios

#### Scenario 1: Design a RESTful API for Twitter

**Requirements**:
- User profiles
- Tweets (create, read, delete)
- Timeline (home, user)
- Follows
- Likes, retweets

**Solution**:
```
Users:
GET    /users/{id}
POST   /users
PATCH  /users/{id}

Tweets:
GET    /tweets/{id}
POST   /tweets
DELETE /tweets/{id}
GET    /users/{id}/tweets

Timeline:
GET    /timeline/home
GET    /users/{id}/timeline

Interactions:
POST   /tweets/{id}/like
DELETE /tweets/{id}/like
POST   /tweets/{id}/retweet
POST   /users/{id}/follow
DELETE /users/{id}/follow
```

**Key Considerations**:
- Pagination for timelines (cursor-based)
- Caching for popular tweets
- Rate limiting per user
- Real-time updates (consider WebSocket for notifications)

#### Scenario 2: Design Microservices Communication

**Requirements**:
- Order service, Inventory service, Payment service
- Low latency requirements (<50ms)
- High reliability
- Strong typing

**Solution**: Use **gRPC for internal services**

```protobuf
// Order Service
service OrderService {
  rpc CreateOrder(CreateOrderRequest) returns (Order);
  rpc GetOrder(GetOrderRequest) returns (Order);
}

// Inventory Service
service InventoryService {
  rpc CheckAvailability(ProductRequest) returns (AvailabilityResponse);
  rpc ReserveStock(ReserveRequest) returns (ReservationResponse);
}

// Payment Service
service PaymentService {
  rpc ProcessPayment(PaymentRequest) returns (PaymentResponse);
}
```

**Architecture**:
```mermaid
graph LR
    Client[REST API Gateway] -->|HTTP/JSON| Gateway
    Gateway -->|gRPC| Order[Order Service]
    Gateway -->|gRPC| Inventory[Inventory Service]
    Gateway -->|gRPC| Payment[Payment Service]

    style Client fill:#e1f5ff
    style Gateway fill:#ffd700
```

**Key Considerations**:
- API Gateway for REST → gRPC translation
- Circuit breakers for fault tolerance
- Distributed tracing (OpenTelemetry)
- Service mesh (Istio) for observability

#### Scenario 3: Design a Social Media Feed API

**Requirements**:
- User profiles, posts, comments, likes
- News feed with personalized content
- Real-time notifications
- Mobile and web clients

**Solution: GraphQL + gRPC Hybrid**

```graphql
# GraphQL for flexible client queries
type Query {
  feed(first: Int, after: String): PostConnection!
  user(id: ID!): User
  post(id: ID!): Post
}

type Mutation {
  createPost(input: CreatePostInput!): Post!
  likePost(postId: ID!): Like!
  followUser(userId: ID!): Follow!
}

type Subscription {
  feedUpdated: Post!
  notificationReceived: Notification!
}

type Post {
  id: ID!
  content: String!
  author: User!
  likes: [Like!]!
  comments(first: Int): CommentConnection!
  createdAt: DateTime!
}
```

**Architecture**:
```mermaid
graph LR
    A[Mobile App] -->|GraphQL| B[GraphQL Gateway]
    C[Web App] -->|GraphQL| B
    B -->|gRPC| D[User Service]
    B -->|gRPC| E[Post Service]
    B -->|gRPC| F[Feed Service]
    B -->|gRPC| G[Notification Service]

    D -->|Events| H[Message Queue]
    E -->|Events| H
    H -->|Stream| I[Real-time Engine]
    I -->|WebSocket| B

    style A fill:#e1f5ff
    style C fill:#e1f5ff
    style B fill:#87CEEB
```

**Key Decisions**:
- **GraphQL**: Flexible queries for different client needs (mobile vs web)
- **gRPC**: High-performance internal service communication
- **Subscriptions**: Real-time feed updates and notifications
- **Event-driven**: Async processing for likes, follows, notifications

#### Scenario 4: Design an API Gateway

**Requirements**:
- Route requests to multiple microservices
- Authentication and rate limiting
- Support REST and GraphQL clients
- Transform protocols (REST ↔ gRPC)

**Solution**:
```mermaid
graph TD
    A[REST Clients] -->|HTTP| C[API Gateway]
    B[GraphQL Clients] -->|HTTP| C
    C -->|Auth/Rate Limit| D[Auth Service]
    C -->|gRPC| E[User Service]
    C -->|gRPC| F[Order Service]
    C -->|gRPC| G[Product Service]

    style C fill:#ffd700
    style A fill:#90EE90
    style B fill:#87CEEB
```

**Implementation Approach**:
1. **Protocol Translation**: Convert REST requests to gRPC calls
2. **Schema Stitching**: Combine multiple GraphQL schemas
3. **Caching Layer**: Cache at gateway level
4. **Circuit Breaker**: Handle service failures gracefully

---

## Best Practices

### REST API Design Checklist

#### URLs & Resources
- ✅ Use nouns, not verbs (`/users`, not `/getUsers`)
- ✅ Use plural names (`/users`, not `/user`)
- ✅ Use hyphens for multi-word resources (`/blog-posts`)
- ✅ Keep hierarchy logical (`/users/123/orders/456`)
- ✅ Avoid deep nesting (max 3 levels)

#### HTTP Methods
- ✅ Use correct methods: GET (read), POST (create), PUT/PATCH (update), DELETE (delete)
- ✅ Make GET, PUT, DELETE idempotent
- ✅ Return 201 Created for POST with Location header
- ✅ Return 204 No Content for DELETE

#### Status Codes
- ✅ Use appropriate status codes (200, 201, 204, 400, 401, 403, 404, 409, 500)
- ✅ Include error details in response body
- ✅ Use RFC 7807 Problem Details format

#### Data Formats
- ✅ Use JSON as default format
- ✅ Support content negotiation (Accept header)
- ✅ Use ISO 8601 for dates (`2025-10-13T10:30:00Z`)
- ✅ Use snake_case or camelCase consistently

#### Pagination
- ✅ Implement pagination for collections
- ✅ Include pagination metadata (page, size, total)
- ✅ Provide navigation links (first, prev, next, last)

#### Filtering & Sorting
- ✅ Use query parameters (`?category=tech&sort=-created_at`)
- ✅ Support multiple sort fields
- ✅ Document filter options

#### Caching
- ✅ Set appropriate Cache-Control headers
- ✅ Use ETags for validation
- ✅ Implement conditional requests (If-None-Match)

#### Security
- ✅ Use HTTPS everywhere
- ✅ Implement authentication (JWT, OAuth2)
- ✅ Validate all inputs
- ✅ Rate limit endpoints
- ✅ Use CORS headers for browser clients

#### Versioning
- ✅ Version your API from day one
- ✅ Support at least 2 versions simultaneously
- ✅ Communicate deprecation 6-12 months in advance
- ✅ Use sunset headers

#### Documentation
- ✅ Provide OpenAPI/Swagger spec
- ✅ Include examples for all endpoints
- ✅ Document error responses
- ✅ Maintain changelog

---

### gRPC Best Practices

#### Protocol Buffers
- ✅ Use semantic versioning for .proto files
- ✅ Never reuse field numbers
- ✅ Use `reserved` for deleted fields
- ✅ Add new fields at end
- ✅ Document messages and services

#### Performance
- ✅ Use streaming for large datasets
- ✅ Implement connection pooling
- ✅ Set appropriate deadlines
- ✅ Enable compression for large payloads

#### Error Handling
- ✅ Use standard gRPC status codes
- ✅ Include detailed error messages
- ✅ Implement retry logic with exponential backoff
- ✅ Set appropriate timeouts

#### Security
- ✅ Use TLS for production
- ✅ Implement authentication interceptors
- ✅ Validate inputs in service methods
- ✅ Use mTLS for service-to-service communication

---

## Interview Preparation

### Common Interview Questions

#### REST API Questions

**Q1: Explain the difference between PUT and PATCH.**

**Answer**:
- **PUT**: Replaces the entire resource. Client sends complete representation. Idempotent.
- **PATCH**: Updates specific fields. Client sends partial changes. May not be idempotent depending on implementation (JSON Merge Patch vs JSON Patch).

**Example**:
```http
# PUT - Full replacement
PUT /users/123
{"id": 123, "name": "Alice", "email": "alice@example.com", "bio": "Engineer"}

# PATCH - Partial update
PATCH /users/123
{"email": "alice.new@example.com"}
```

**Q2: How would you design pagination for a large dataset?**

**Answer**: Use **cursor-based pagination** for consistency:
```http
GET /posts?cursor=eyJpZCI6MTIzfQ&size=20

Response:
{
  "data": [...],
  "pagination": {
    "next_cursor": "eyJpZCI6MTQzfQ",
    "has_more": true
  }
}
```

**Why cursor over offset**: Offset-based (`?page=2`) breaks when data changes. Cursor-based maintains consistency.

**Q3: How do you handle versioning in REST APIs?**

**Answer**: Use URL path versioning (`/api/v1/users`) for clarity. Support N-1 versions. Communicate deprecation 6-12 months in advance. Use sunset headers:
```http
Sunset: Sat, 31 Dec 2025 23:59:59 GMT
```

#### gRPC Questions

**Q4: When would you choose gRPC over REST?**

**Answer**:
- **Internal microservices**: Strong typing, performance
- **Streaming required**: Real-time data, bidirectional communication
- **Performance critical**: Low latency (<10ms), high throughput
- **Polyglot environments**: Auto-generated clients in 10+ languages
- **Strong type safety**

**Don't use for**: Public APIs (browser incompatibility), simple CRUD (REST overhead not justified)

**Q5: Explain gRPC streaming types with examples.**

**Answer**:
1. **Unary**: Single request/response (like REST)
2. **Server streaming**: Client sends one request, server streams multiple responses (e.g., stock price updates)
3. **Client streaming**: Client streams multiple requests, server sends one response (e.g., uploading chunks)
4. **Bidirectional**: Both stream simultaneously (e.g., chat application)

### System Design Scenarios

#### Scenario 1: Design a RESTful API for Twitter

**Requirements**:
- User profiles
- Tweets (create, read, delete)
- Timeline (home, user)
- Follows
- Likes, retweets

**Solution**:
```
Users:
GET    /users/{id}
POST   /users
PATCH  /users/{id}

Tweets:
GET    /tweets/{id}
POST   /tweets
DELETE /tweets/{id}
GET    /users/{id}/tweets

Timeline:
GET    /timeline/home
GET    /users/{id}/timeline

Interactions:
POST   /tweets/{id}/like
DELETE /tweets/{id}/like
POST   /tweets/{id}/retweet
POST   /users/{id}/follow
DELETE /users/{id}/follow
```

**Key Considerations**:
- Pagination for timelines (cursor-based)
- Caching for popular tweets
- Rate limiting per user
- Real-time updates (consider WebSocket for notifications)

#### Scenario 2: Design Microservices Communication

**Requirements**:
- Order service, Inventory service, Payment service
- Low latency requirements (<50ms)
- High reliability
- Strong typing

**Solution**: Use **gRPC for internal services**

```protobuf
// Order Service
service OrderService {
  rpc CreateOrder(CreateOrderRequest) returns (Order);
  rpc GetOrder(GetOrderRequest) returns (Order);
}

// Inventory Service
service InventoryService {
  rpc CheckAvailability(ProductRequest) returns (AvailabilityResponse);
  rpc ReserveStock(ReserveRequest) returns (ReservationResponse);
}

// Payment Service
service PaymentService {
  rpc ProcessPayment(PaymentRequest) returns (PaymentResponse);
}
```

**Architecture**:
```mermaid
graph LR
    Client[REST API Gateway] -->|HTTP/JSON| Gateway
    Gateway -->|gRPC| Order[Order Service]
    Gateway -->|gRPC| Inventory[Inventory Service]
    Gateway -->|gRPC| Payment[Payment Service]

    style Client fill:#e1f5ff
    style Gateway fill:#ffd700
```

**Key Considerations**:
- API Gateway for REST → gRPC translation
- Circuit breakers for fault tolerance
- Distributed tracing (OpenTelemetry)
- Service mesh (Istio) for observability

#### Scenario 3: Design a Social Media Feed API

**Requirements**:
- User profiles, posts, comments, likes
- News feed with personalized content
- Real-time notifications
- Mobile and web clients

**Solution: GraphQL + gRPC Hybrid**

```graphql
# GraphQL for flexible client queries
type Query {
  feed(first: Int, after: String): PostConnection!
  user(id: ID!): User
  post(id: ID!): Post
}

type Mutation {
  createPost(input: CreatePostInput!): Post!
  likePost(postId: ID!): Like!
  followUser(userId: ID!): Follow!
}

type Subscription {
  feedUpdated: Post!
  notificationReceived: Notification!
}

type Post {
  id: ID!
  content: String!
  author: User!
  likes: [Like!]!
  comments(first: Int): CommentConnection!
  createdAt: DateTime!
}
```

**Architecture**:
```mermaid
graph LR
    A[Mobile App] -->|GraphQL| B[GraphQL Gateway]
    C[Web App] -->|GraphQL| B
    B -->|gRPC| D[User Service]
    B -->|gRPC| E[Post Service]
    B -->|gRPC| F[Feed Service]
    B -->|gRPC| G[Notification Service]

    D -->|Events| H[Message Queue]
    E -->|Events| H
    H -->|Stream| I[Real-time Engine]
    I -->|WebSocket| B

    style A fill:#e1f5ff
    style C fill:#e1f5ff
    style B fill:#87CEEB
```

**Key Decisions**:
- **GraphQL**: Flexible queries for different client needs (mobile vs web)
- **gRPC**: High-performance internal service communication
- **Subscriptions**: Real-time feed updates and notifications
- **Event-driven**: Async processing for likes, follows, notifications

#### Scenario 4: Design an API Gateway

**Requirements**:
- Route requests to multiple microservices
- Authentication and rate limiting
- Support REST and GraphQL clients
- Transform protocols (REST ↔ gRPC)

**Solution**:
```mermaid
graph TD
    A[REST Clients] -->|HTTP| C[API Gateway]
    B[GraphQL Clients] -->|HTTP| C
    C -->|Auth/Rate Limit| D[Auth Service]
    C -->|gRPC| E[User Service]
    C -->|gRPC| F[Order Service]
    C -->|gRPC| G[Product Service]

    style C fill:#ffd700
    style A fill:#90EE90
    style B fill:#87CEEB
```

**Implementation Approach**:
1. **Protocol Translation**: Convert REST requests to gRPC calls
2. **Schema Stitching**: Combine multiple GraphQL schemas
3. **Caching Layer**: Cache at gateway level
4. **Circuit Breaker**: Handle service failures gracefully

---

## Best Practices

### REST API Design Checklist

#### URLs & Resources
- ✅ Use nouns, not verbs (`/users`, not `/getUsers`)
- ✅ Use plural names (`/users`, not `/user`)
- ✅ Use hyphens for multi-word resources (`/blog-posts`)
- ✅ Keep hierarchy logical (`/users/123/orders/456`)
- ✅ Avoid deep nesting (max 3 levels)

#### HTTP Methods
- ✅ Use correct methods: GET (read), POST (create), PUT/PATCH (update), DELETE (delete)
- ✅ Make GET, PUT, DELETE idempotent
- ✅ Return 201 Created for POST with Location header
- ✅ Return 204 No Content for DELETE

#### Status Codes
- ✅ Use appropriate status codes (200, 201, 204, 400, 401, 403, 404, 409, 500)
- ✅ Include error details in response body
- ✅ Use RFC 7807 Problem Details format

#### Data Formats
- ✅ Use JSON as default format
- ✅ Support content negotiation (Accept header)
- ✅ Use ISO 8601 for dates (`2025-10-13T10:30:00Z`)
- ✅ Use snake_case or camelCase consistently

#### Pagination
- ✅ Implement pagination for collections
- ✅ Include pagination metadata (page, size, total)
- ✅ Provide navigation links (first, prev, next, last)

#### Filtering & Sorting
- ✅ Use query parameters (`?category=tech&sort=-created_at`)
- ✅ Support multiple sort fields
- ✅ Document filter options

#### Caching
- ✅ Set appropriate Cache-Control headers
- ✅ Use ETags for validation
- ✅ Implement conditional requests (If-None-Match)

#### Security
- ✅ Use HTTPS everywhere
- ✅ Implement authentication (JWT, OAuth2)
- ✅ Validate all inputs
- ✅ Rate limit endpoints
- ✅ Use CORS headers for browser clients

#### Versioning
- ✅ Version your API from day one
- ✅ Support at least 2 versions simultaneously
- ✅ Communicate deprecation 6-12 months in advance
- ✅ Use sunset headers

#### Documentation
- ✅ Provide OpenAPI/Swagger spec
- ✅ Include examples for all endpoints
- ✅ Document error responses
- ✅ Maintain changelog

---

### gRPC Best Practices

#### Protocol Buffers
- ✅ Use semantic versioning for .proto files
- ✅ Never reuse field numbers
- ✅ Use `reserved` for deleted fields
- ✅ Add new fields at end
- ✅ Document messages and services

#### Performance
- ✅ Use streaming for large datasets
- ✅ Implement connection pooling
- ✅ Set appropriate deadlines
- ✅ Enable compression for large payloads

#### Error Handling
- ✅ Use standard gRPC status codes
- ✅ Include detailed error messages
- ✅ Implement retry logic with exponential backoff
- ✅ Set appropriate timeouts

#### Security
- ✅ Use TLS for production
- ✅ Implement authentication interceptors
- ✅ Validate inputs in service methods
- ✅ Use mTLS for service-to-service communication

---

## Interview Preparation

### Common Interview Questions

#### REST API Questions

**Q1: Explain the difference between PUT and PATCH.**

**Answer**:
- **PUT**: Replaces the entire resource. Client sends complete representation. Idempotent.
- **PATCH**: Updates specific fields. Client sends partial changes. May not be idempotent depending on implementation (JSON Merge Patch vs JSON Patch).

**Example**:
```http
# PUT - Full replacement
PUT /users/123
{"id": 123, "name": "Alice", "email": "alice@example.com", "bio": "Engineer"}

# PATCH - Partial update
PATCH /users/123
{"email": "alice.new@example.com"}
```

**Q2: How would you design pagination for a large dataset?**

**Answer**: Use **cursor-based pagination** for consistency:
```http
GET /posts?cursor=eyJpZCI6MTIzfQ&size=20

Response:
{
  "data": [...],
  "pagination": {
    "next_cursor": "eyJpZCI6MTQzfQ",
    "has_more": true
  }
}
```

**Why cursor over offset**: Offset-based (`?page=2`) breaks when data changes. Cursor-based maintains consistency.

**Q3: How do you handle versioning in REST APIs?**

**Answer**: Use URL path versioning (`/api/v1/users`) for clarity. Support N-1 versions. Communicate deprecation 6-12 months in advance. Use sunset headers:
```http
Sunset: Sat, 31 Dec 2025 23:59:59 GMT
```

#### gRPC Questions

**Q4: When would you choose gRPC over REST?**

**Answer**:
- **Internal microservices**: Strong typing, performance
- **Streaming required**: Real-time data, bidirectional communication
- **Performance critical**: Low latency (<10ms), high throughput
- **Polyglot environments**: Auto-generated clients in 10+ languages
- **Strong type safety**

**Don't use for**: Public APIs (browser incompatibility), simple CRUD (REST overhead not justified)

**Q5: Explain gRPC streaming types with examples.**

**Answer**:
1. **Unary**: Single request/response (like REST)
2. **Server streaming**: Client sends one request, server streams multiple responses (e.g., stock price updates)
3. **Client streaming**: Client streams multiple requests, server sends one response (e.g., uploading chunks)
4. **Bidirectional**: Both stream simultaneously (e.g., chat application)

### System Design Scenarios

#### Scenario 1: Design a RESTful API for Twitter

**Requirements**:
- User profiles
- Tweets (create, read, delete)
- Timeline (home, user)
- Follows
- Likes, retweets

**Solution**:
```
Users:
GET    /users/{id}
POST   /users
PATCH  /users/{id}

Tweets:
GET    /tweets/{id}
POST   /tweets
DELETE /tweets/{id}
GET    /users/{id}/tweets

Timeline:
GET    /timeline/home
GET    /users/{id}/timeline

Interactions:
POST   /tweets/{id}/like
DELETE /tweets/{id}/like
POST   /tweets/{id}/retweet
POST   /users/{id}/follow
DELETE /users/{id}/follow
```

**Key Considerations**:
- Pagination for timelines (cursor-based)
- Caching for popular tweets
- Rate limiting per user
- Real-time updates (consider WebSocket for notifications)

#### Scenario 2: Design Microservices Communication

**Requirements**:
- Order service, Inventory service, Payment service
- Low latency requirements (<50ms)
- High reliability
- Strong typing

**Solution**: Use **gRPC for internal services**

```protobuf
// Order Service
service OrderService {
  rpc CreateOrder(CreateOrderRequest) returns (Order);
  rpc GetOrder(GetOrderRequest) returns (Order);
}

// Inventory Service
service InventoryService {
  rpc CheckAvailability(ProductRequest) returns (AvailabilityResponse);
  rpc ReserveStock(ReserveRequest) returns (ReservationResponse);
}

// Payment Service
service PaymentService {
  rpc ProcessPayment(PaymentRequest) returns (PaymentResponse);
}
```

**Architecture**:
```mermaid
graph LR
    Client[REST API Gateway] -->|HTTP/JSON| Gateway
    Gateway -->|gRPC| Order[Order Service]
    Gateway -->|gRPC| Inventory[Inventory Service]
    Gateway -->|gRPC| Payment[Payment Service]

    style Client fill:#e1f5ff
    style Gateway fill:#ffd700
```

**Key Considerations**:
- API Gateway for REST → gRPC translation
- Circuit breakers for fault tolerance
- Distributed tracing (OpenTelemetry)
- Service mesh (Istio) for observability

#### Scenario 3: Design a Social Media Feed API

**Requirements**:
- User profiles, posts, comments, likes
- News feed with personalized content
- Real-time notifications
- Mobile and web clients

**Solution: GraphQL + gRPC Hybrid**

```graphql
# GraphQL for flexible client queries
type Query {
  feed(first: Int, after: String): PostConnection!
  user(id: ID!): User
  post(id: ID!): Post
}

type Mutation {
  createPost(input: CreatePostInput!): Post!
  likePost(postId: ID!): Like!
  followUser(userId: ID!): Follow!
}

type Subscription {
  feedUpdated: Post!
  notificationReceived: Notification!
}

type Post {
  id: ID!
  content: String!
  author: User!
  likes: [Like!]!
  comments(first: Int): CommentConnection!
  createdAt: DateTime!
}
```

**Architecture**:
```mermaid
graph LR
    A[Mobile App] -->|GraphQL| B[GraphQL Gateway]
    C[Web App] -->|GraphQL| B
    B -->|gRPC| D[User Service]
    B -->|gRPC| E[Post Service]
    B -->|gRPC| F[Feed Service]
    B -->|gRPC| G[Notification Service]

    D -->|Events| H[Message Queue]
    E -->|Events| H
    H -->|Stream| I[Real-time Engine]
    I -->|WebSocket| B

    style A fill:#e1f5ff
    style C fill:#e1f5ff
    style B fill:#87CEEB
```

**Key Decisions**:
- **GraphQL**: Flexible queries for different client needs (mobile vs web)
- **gRPC**: High-performance internal service communication
- **Subscriptions**: Real-time feed updates and notifications
- **Event-driven**: Async processing for likes, follows, notifications

#### Scenario 4: Design an API Gateway

**Requirements**:
- Route requests to multiple microservices
- Authentication and rate limiting
- Support REST and GraphQL clients
- Transform protocols (REST ↔ gRPC)

**Solution**:
```mermaid
graph TD
    A[REST Clients] -->|HTTP| C[API Gateway]
    B[GraphQL Clients] -->|HTTP| C
    C -->|Auth/Rate Limit| D[Auth Service]
    C -->|gRPC| E[User Service]
    C -->|gRPC| F[Order Service]
    C -->|gRPC| G[Product Service]

    style C fill:#ffd700
    style A fill:#90EE90
    style B fill:#87CEEB
```

**Implementation Approach**:
1. **Protocol Translation**: Convert REST requests to gRPC calls
2. **Schema Stitching**: Combine multiple GraphQL schemas
3. **Caching Layer**: Cache at gateway level
4. **Circuit Breaker**: Handle service failures gracefully

---
