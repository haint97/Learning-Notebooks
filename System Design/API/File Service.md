# Definition
File or cloud storage services offer reliable and cheaper storage for various media like photos, videos, and documents, allowing users to upload and download data on demand. These services abstract complex backend systems into simple API calls for clients.
![](api-images/file-service-high-level.png)
# Non-functional requirements
- Upload: Securely upload files to remote storage.
- Download: Retrieve uploaded files from remote storage.
- Delete: Allow file owners to delete their uploaded files.
- List: Enable file owners to list their uploaded files.

# Non-functional Requirements
- Reliability: Ensure data is not lost or corrupted during transfer and is persistently stored.
- Security: Implement appropriate mechanisms for secure data access.
- Scalability: Handle an increasing number of users and files.
- Availability: Provide high availability for upload/download operations.
- Low latency: Achieve fast upload and download speeds.

# Design decision
![alt text](api-images/file-service-workflow.png)

## Components and services detail
| Component or Service | Details |
| :--- | :--- |
| File servers | • Accept and process file API requests forwarded by the API gateway<br>• Split requests into metadata (user and file information) and file content |
| Processing server | • Performs encoding/decoding of data into different encoding schemes<br>• Encrypt/decrypt data at-rest to prevent unauthorized access |
| User to file mapping system (UFMS) | • Maps users to files and where those files can be found in storage |
| Blob storage | • Stores binary data objects (file content) |
| Temporary storage | • Stores files and objects temporarily before processing |
| User server | • Handles user related requests forwarded by the API gateway |
| SQL database | • Stores information related to files and users (metadata) |
| CDN servers | • Geographically distributed network for serving data to the user from a nearby location instead of serving it from the blob storage |
| API gateway | • Forwards client requests to the appropriate application server<br>• Authenticates and authorizes client requests<br>• Rate-limits client requests to keep the server from overburdening |
| Client | • The consumer of the API service<br>• Can perform lossless compression before sending a file |

## Workflow
- Client to API gateway:

    Client authentication and authorization for read/write access. After verification, clients can upload, download, list, or delete files via HTTP requests.
- API gateway to downstream services:

    - Upload: Gateway forwards requests to the application server. Metadata (user ID, file name, size, location) is extracted and sent to UFMS, which stores it in a SQL database. File content is stored in temporary storage, processed (compressed, encrypted), and then moved to blob storage. Multiple copies are made for reliability and faster delivery (CDN, backup).
    - Download: Client requests reach the gateway, forwarded to the application server, which queries UFMS for information. The server retrieves file contents from blob storage (or CDN), compiles the response, and returns it.

## Consideration
### Architecture style
- REST: Chosen as the primary communication protocol (client-to-API Gateway and API Gateway-to-downstream services). It's well-suited for basic operations on a single resource (files) and provides a standardized approach, avoiding unnecessary complexity.
- gRPC: Less suitable due to its focus on communication-intensive systems and persistent connections, which may not be fully utilized for individual file uploads.
- GraphQL: Not ideal as the service does not involve managing different resources with a single request or addressing overfetching/underfetching from multiple endpoints.

The discussion above suggests we should use REST because it has better support for different operations in our file API and avoids unnecessary complexity. Therefore, we propose REST as a communication protocol from client to API gateway and from API gateway to downstream services.

### Data format
- JSON: Chosen for metadata (user and file information) and requests/responses due to its human readability, compactness, ease of implementation, and built-in JavaScript parser support.
- XML: Less suitable due to larger encoded sizes and lack of built-in parser support.
- Binary: Used for the actual file content transfer, storage, and retrieval due to its speed and compactness, while JSON handles the human-readable parts.

The actual file content is transmitted in binary format. Only the requests and responses are formatted in JSON for processing.

### HTTP version
![alt text](api-images/file-service-http.png)


-`Chunk and frame`

A chunk is a small portion of bytes in the entire data sequence (complete file). In HTTP/1.1, we can enable chunked data transfers by specifying Transfer-Encoding: chunked in the request header. On the other hand, in HTTP/2.0, all requests are sent in byte ranges (chunk in HTTP/1.1) by default, and these ranges are called frames.

From the explanation above, we conclude that HTTP/2.0 supports segmented data transfers (called frames) by default, whereas in HTTP/1.1, we have to enable segmented transmission of data, referred to as chunks.


# API model
## Endpoints
* **Operations**
    * Upload: `POST /files HTTP/1.1`
    * List: `GET /files HTTP/1.1`
    * Download: `GET /files/{fileId} HTTP/1.1`
    * Remove: `DELETE /files/{fileId} HTTP/1.1`

## Entity
```
type file
{
    fileId: string      // Unique identifier of a file on the host system
    ownerId: string     // Unique identifier of a user on the host system
    authToken: string   // Token verifying the authenticity and privileges of the user
    checksum: string    // Hash value verifying the integrity of data
    fileLink: string    // The URI of the stored the file contents
    userList: list      // A list of users with the specified permissions to access the resource
    timeStamp: date     // The data and time at which the particular file is uploaded
    fileTitle: string   // The name/title of the file along with its extension
    fileSize: long      // Total size of a file in bytes
    content: binary     // The actual content of a file being transferred
    mimeType: *         // file format or extension of the file such as png, jpeg, mp3, mkv, etc.
}
```

## Upload file

### Overview
File upload is a critical operation that transfers file content and metadata from client to server. The process involves:
1. **Authentication & Authorization**: Verify user identity and permissions
2. **Metadata Extraction**: Parse file information (name, size, type, owner)
3. **Content Processing**: Validate, scan, compress, and encrypt file data
4. **Storage**: Persist to temporary storage, then move to blob storage
5. **Confirmation**: Return fileId and success status to client

### Basic Upload Request

Request:
```http
POST /files HTTP/1.1
Host: api.fileservice.com
Content-Type: multipart/mixed;boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW
Authorization: Bearer <auth_token>

------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="metadata"
Content-Type: application/json; charset=utf-8

{
    "fileTitle": "example.png",
    "ownerId": "user-987",
    "mimeType": "image/png",
    "fileSize": 123456
}
------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="file"; filename="example.png"
Content-Type: image/png

<binary file content>
------WebKitFormBoundary7MA4YWxkTrZu0gW--
```

**Request Breakdown:**
- **Content-Type header**: `multipart/mixed` allows sending both structured metadata (JSON) and binary content in a single request
  - **Boundary token**: Separates different parts of the multipart body
  - Alternative: `multipart/form-data` (common for browser form submissions)

- **Authorization**: Bearer token for authentication/authorization
  - Validated by API gateway before downstream processing
  - Should include scope/permissions for file operations

- **Metadata part** (`name="metadata"`):
  - JSON structure with file information
  - Used to create database records, set permissions, determine storage location
  - Fields:
    - `fileTitle`: Original filename with extension
    - `ownerId`: User identifier (validated against auth token)
    - `mimeType`: File format for content-type validation
    - `fileSize`: Total bytes (used for quota checks and progress tracking)

- **File part** (`name="file"`):
  - Binary payload containing actual file content
  - `filename` attribute preserves original name
  - Should be **streamed** to avoid memory issues with large files
  - Server computes checksum during streaming for integrity verification

**Why multipart/mixed vs multipart/form-data?**
- `multipart/mixed`: Better for API design with explicit metadata + binary separation
- `multipart/form-data`: Traditional browser form submission; either works, but mixed is clearer

### Success Response

```http
HTTP/1.1 201 Created
Content-Type: application/json
Location: https://api.fileservice.com/files/12345

{
    "fileId": "12345",
    "fileLink": "https://cdn.fileservice.com/files/12345/example.png",
    "checksum": "sha256:a3b2c1d4e5f6...",
    "message": "File uploaded successfully",
    "uploadedAt": "2024-01-15T10:30:00Z"
}
```

**Response Details:**
- **201 Created**: Indicates new resource created successfully
- **Location header**: URI of the newly created resource
- **fileId**: Unique identifier for future operations (download, delete, list)
- **fileLink**: CDN or direct URL for file access
- **checksum**: Hash for client-side integrity verification
- **uploadedAt**: Timestamp for auditing and version control

### Error Responses

#### 1. Unauthorized Request
```http
HTTP/1.1 401 Unauthorized
Content-Type: application/json

{
    "error": "Unauthorized",
    "message": "Invalid or expired authentication token",
    "errorCode": "AUTH_001"
}
```

#### 2. Invalid Metadata
```http
HTTP/1.1 400 Bad Request
Content-Type: application/json

{
    "error": "Bad Request",
    "message": "Invalid metadata: fileSize exceeds maximum allowed (100MB)",
    "errorCode": "VALIDATION_001",
    "details": {
        "field": "fileSize",
        "provided": 150000000,
        "maximum": 104857600
    }
}
```

#### 3. File Too Large
```http
HTTP/1.1 413 Payload Too Large
Content-Type: application/json

{
    "error": "Payload Too Large",
    "message": "File size exceeds maximum limit",
    "errorCode": "SIZE_001",
    "maxSize": 104857600
}
```

#### 4. Unsupported File Type
```http
HTTP/1.1 415 Unsupported Media Type
Content-Type: application/json

{
    "error": "Unsupported Media Type",
    "message": "File type 'application/x-executable' not allowed",
    "errorCode": "TYPE_001",
    "allowedTypes": ["image/*", "video/*", "application/pdf"]
}
```

#### 5. Storage Quota Exceeded
```http
HTTP/1.1 507 Insufficient Storage
Content-Type: application/json

{
    "error": "Insufficient Storage",
    "message": "User storage quota exceeded",
    "errorCode": "QUOTA_001",
    "details": {
        "used": 5368709120,
        "limit": 5368709120,
        "requested": 123456
    }
}
```

#### 6. Server Error
```http
HTTP/1.1 500 Internal Server Error
Content-Type: application/json

{
    "error": "Internal Server Error",
    "message": "Failed to process file upload",
    "errorCode": "SERVER_001",
    "requestId": "req-abc-123"
}
```

### Resumable Upload (for Large Files)

**Use Case**: Handle network interruptions, allow pause/resume for large files (>100MB)

**Initial Request** (Initiate Upload Session):
```http
POST /files/upload-session HTTP/1.1
Host: api.fileservice.com
Authorization: Bearer <auth_token>
Content-Type: application/json

{
    "fileTitle": "large-video.mp4",
    "fileSize": 524288000,
    "mimeType": "video/mp4",
    "chunkSize": 5242880
}
```

**Response**:
```http
HTTP/1.1 201 Created
Content-Type: application/json

{
    "sessionId": "session-xyz-789",
    "uploadUrl": "https://api.fileservice.com/files/upload-session/session-xyz-789",
    "expiresAt": "2024-01-15T12:30:00Z",
    "chunkSize": 5242880
}
```

**Upload Chunk**:
```http
PUT /files/upload-session/session-xyz-789 HTTP/1.1
Host: api.fileservice.com
Authorization: Bearer <auth_token>
Content-Type: application/octet-stream
Content-Range: bytes 0-5242879/524288000

<binary chunk data>
```

**Chunk Response**:
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
    "received": 5242880,
    "nextOffset": 5242880,
    "completed": false
}
```

**Final Chunk Response**:
```http
HTTP/1.1 201 Created
Content-Type: application/json

{
    "fileId": "file-abc-456",
    "message": "Upload completed",
    "totalSize": 524288000
}
```

### Server-Side Processing Flow

1. **API Gateway Stage**:
   - Validate Authorization header and extract user identity
   - Rate-limit check (prevent abuse)
   - Route request to file server

2. **File Server Stage**:
   - Parse multipart body (metadata + file content)
   - Validate metadata against business rules:
     - File size within limits
     - MIME type allowed
     - User has storage quota
   - Extract and validate ownerId matches authenticated user

3. **Processing Server Stage**:
   - Stream file content (avoid loading entire file in memory)
   - Compute checksum (SHA-256) for integrity
   - Virus/malware scan
   - Optional: Compress (reduce storage cost)
   - Encrypt at-rest (security requirement)

4. **Storage Stage**:
   - Write to temporary storage
   - Update UFMS database with metadata:
     - fileId, ownerId, fileTitle, fileSize, mimeType, checksum, timestamp
   - Move file from temporary to blob storage
   - Create CDN copies for faster delivery
   - Optional: Create backup replicas

5. **Response Stage**:
   - Return 201 Created with fileId and fileLink
   - Log upload event for auditing

### Security Considerations

| Concern | Mitigation |
|---------|-----------|
| **Malicious files** | Scan uploads with antivirus/malware detection before storage |
| **File type validation** | Verify MIME type matches file signature (magic numbers), not just extension |
| **File size bombs** | Enforce maximum file size limits; use streaming to detect size mismatches early |
| **Path traversal** | Sanitize filenames; reject paths with `..`, `/`, or special characters |
| **Token theft** | Use short-lived tokens; implement token rotation; validate scope |
| **Data tampering** | Compute and verify checksum on both client and server |
| **Encryption at-rest** | Encrypt files before storing in blob storage; manage keys securely |
| **Encryption in-transit** | Enforce HTTPS/TLS for all API communication |
| **Access control** | Validate user owns the file; implement fine-grained permissions |

### Best Practices

1. **Client-Side**:
   - Compress files before upload to reduce bandwidth
   - Compute checksum before sending for integrity verification
   - Implement retry logic with exponential backoff
   - Show upload progress to user
   - Use chunked uploads for files >10MB

2. **Server-Side**:
   - Stream file content (don't buffer entire file in memory)
   - Use asynchronous processing for CPU-intensive tasks (encoding, encryption)
   - Implement idempotency (same request shouldn't create duplicate files)
   - Set upload timeouts to prevent resource exhaustion
   - Use Content-MD5 header for additional integrity checking

3. **Scalability**:
   - Use object storage (S3, GCS, Azure Blob) for blob storage
   - Implement CDN for geographically distributed access
   - Use database sharding for metadata storage at scale
   - Queue-based processing for asynchronous tasks


# Design evaluation
## Non-functional Requirements
- Reliability: Achieved through shadow copies of service components (API gateways, app servers, UFMS), geographically distributed data backups, circuit breakers, and proactive monitoring.

- Security: Ensures authorized access for authenticated users (credentials, OAuth 2.0/OIDC with PKCE flow). Data is always encrypted at rest, and intrusion detection mechanisms are assumed.

- Scalability: UFMS decouples metadata from file content, allowing independent scaling. A flexible storage scheme handles diverse structures and schemas efficiently. Storage is horizontally scalable, and adherence to REST principles enhances long-term stability.

- Availability: Utilizes an API gateway to separate public/private endpoints. Rate limits and quota limits prevent DoS attacks and service abuse. Maximum file upload size limits are enforced, and critical endpoints avoid single points of failure.

- Low Latency: Employs temporary storage for fast uploads and CDN servers for quick downloads. Streaming encryption/decryption reduces processing delays. HTTP/2.0 connections are supported for compatible clients to further minimize latency.

| Non-Functional Requirements | Approaches |
| :--- | :--- |
| Reliability | • Replication to avoid SPOF<br>• Circuit breakers and service monitoring |
| Security | • OAuth 2.0 and OIDC for third-party interactions<br>• Store encrypted data |
| Scalability | • Separate metadata and blob storage<br>• Persistently storing the schema information (schema version, type of encryption, etc.) along with data used to read/write the file |
| Availability | • Rate limiting incoming requests<br>• Limiting the maximum upload size<br>• Avoiding SPOF for popular endpoints |
| Low latency | • Use of temporary storage for fast uploads<br>• Use of CDNs for fast downloads<br>• Stream encryption/decryption to improve response time |

## Latency Analysis

Understanding upload and download latency is crucial for designing performant file services. This section analyzes the time it takes to upload a 10 MB image file, breaking down each component that contributes to total latency.

### Upload Request

When a client uploads a file, the total response time is the sum of multiple latency components. Let's analyze each stage in detail.

#### Overview: Total Upload Time Formula

```
Total Upload Time = Network Transfer Time + Server Processing Time + Storage Write Time
```

More specifically:
```
Total Time = Request Preparation + Network RTT + Server Processing + Response Generation + Response Transfer
```

#### 1. Request Composition & Size Calculation

Before the file is sent, the client must prepare the HTTP request. This includes headers, metadata, and the actual file content.

**Components of Upload Request:**

| Component | Size | Description |
|-----------|------|-------------|
| HTTP headers | ~1-2 KB | POST method, Host, Content-Type, Authorization, User-Agent, Accept, etc. |
| JSON metadata | ~1 KB | fileTitle, ownerId, mimeType, fileSize, timestamp, etc. |
| Multipart boundaries | ~0.5 KB | Boundary markers separating metadata from file content |
| **Subtotal (overhead)** | **~2.5 KB** | Fixed overhead regardless of file size |
| File content | 10 MB | Actual binary data (10,240 KB) |
| **Total request size** | **~10,242.5 KB** | Overhead + file content |

**Key Insight:** For large files (>1 MB), the overhead is negligible (<0.1%). For small files (<10 KB), overhead can be 25-50% of total request size.

**Example Request Structure:**
```http
POST /files HTTP/1.1                          # ~0.05 KB
Host: api.fileservice.com                     # ~0.03 KB
Authorization: Bearer eyJhbGc...               # ~0.5 KB
Content-Type: multipart/mixed; boundary=...   # ~0.1 KB
Content-Length: 10485760                      # ~0.02 KB
User-Agent: Mozilla/5.0...                    # ~0.2 KB

------WebKitFormBoundary7MA4YWxkTrZu0gW      # ~0.05 KB
Content-Disposition: form-data; name="metadata"
Content-Type: application/json

{ "fileTitle": "photo.png", ... }            # ~1 KB

------WebKitFormBoundary7MA4YWxkTrZu0gW      # ~0.05 KB
Content-Disposition: form-data; name="file"
Content-Type: image/png

<10 MB binary data>                           # 10,240 KB
------WebKitFormBoundary7MA4YWxkTrZu0gW--    # ~0.05 KB
```

#### 2. Network Transfer Time

Network transfer time depends on three factors:
1. **Bandwidth**: How much data can be sent per second
2. **Round-Trip Time (RTT)**: Time for signal to travel from client → server → client
3. **Protocol Overhead**: TCP/IP, TLS handshakes, HTTP framing

**Formula Components:**

**a) Base Round-Trip Time (RTT):**
- **RTT_base** = 260 ms (assumed typical internet connection)
- This is the time for an empty packet to go client → server → client
- Includes: propagation delay, router processing, queuing delay

**b) Transfer Time (Data Size Effect):**
- Formula: `1.15 ms per KB` (empirically derived from TCP throughput modeling)
- For 10,242.5 KB: `1.15 × 10,242.5 = 11,778.875 ms`

**Why 1.15 ms/KB?**
```
Assumed bandwidth: ~870 KB/s (7 Mbps)
Transfer time per KB = 1,000 KB / 870 KB/s ≈ 1.15 ms/KB
```
This accounts for real-world TCP behavior: congestion control, packet loss recovery, ACK delays.

**c) Total Network Latency:**
```
Network_Latency = RTT_base + (Transfer_coefficient × Request_size)
                = 260 ms + (1.15 ms/KB × 10,242.5 KB)
                = 260 ms + 11,778.875 ms
                = 12,038.875 ms ≈ 12.04 seconds
```

**Network Time Breakdown:**

| Stage | Time | Description |
|-------|------|-------------|
| TCP handshake | ~260 ms | SYN, SYN-ACK, ACK (1 RTT) |
| TLS handshake | ~520 ms | ClientHello, ServerHello, certificates, keys (2 RTT) |
| HTTP request upload | ~11,778 ms | Sending 10 MB at ~870 KB/s |
| Server processing | (see next section) | File validation, storage, etc. |
| HTTP response download | ~2-3 ms | Small JSON response (~2 KB) |
| **Total network time** | **~12,560 ms** | Dominated by file upload time |

**Real-World Variations:**

| Connection Type | Bandwidth | RTT | 10 MB Upload Time |
|----------------|-----------|-----|-------------------|
| 4G Mobile | 5-12 Mbps | 50-100 ms | 7-16 seconds |
| Home Broadband | 10-100 Mbps | 20-50 ms | 1-8 seconds |
| Office Ethernet | 100-1000 Mbps | 5-20 ms | 0.1-1 second |
| Data Center | 10+ Gbps | 1-5 ms | <0.1 second |

#### 3. Server Processing Time

After the server receives the request, it must perform several operations before storing the file.

**Processing Pipeline:**

```
Request → Authentication → Validation → Streaming → Checksum →
Virus Scan → Compression → Encryption → Write to Temp Storage →
Move to Blob Storage → Update Database → Generate Response
```

**Time Breakdown for 10 MB File:**

| Operation | Time | Notes |
|-----------|------|-------|
| **Authentication/Authorization** | 2-5 ms | JWT validation, database lookup for user permissions |
| **Metadata validation** | 1-2 ms | Check fileSize, mimeType, quota limits |
| **Stream file content** | 5-10 ms | Read from network buffer, write to temp storage (disk I/O) |
| **Checksum calculation** | 15-30 ms | SHA-256 hashing: ~330-660 MB/s throughput → 10 MB / 660 MB/s ≈ 15 ms (hardware) |
| **Virus/malware scan** | 50-200 ms | Pattern matching; varies widely by scanner and file type |
| **Compression** | 20-100 ms | Optional; gzip: ~100-500 MB/s → 10 MB / 100 MB/s = 100 ms |
| **Encryption (AES-256)** | 10-30 ms | AES-NI hardware: ~1-3 GB/s → 10 MB / 1 GB/s = 10 ms |
| **Write to temp storage** | 5-20 ms | Local SSD: ~500 MB/s write → 10 MB / 500 MB/s = 20 ms |
| **Move to blob storage** | 50-200 ms | Network transfer to S3/GCS; depends on proximity |
| **Database update (metadata)** | 5-15 ms | INSERT into SQL database with indexes |
| **Response generation** | 1-2 ms | JSON serialization |
| **Total processing time** | **164-614 ms** | Highly variable; median ~300-400 ms |

**Detailed Processing Time Formula:**

Using typical values:
```
Processing_Time = Auth + Validation + Stream + Checksum + VirusScan +
                  Encryption + TempWrite + DBInsert + Response
                = 3 + 1.5 + 7 + 20 + 100 + 15 + 10 + 100 + 8 + 1.5
                = 266 ms
```

**Original Document's Simplified Formula:**
```
Processing_Time = Base_Processing + Encryption_Time
                = 4 ms + 6.191 ms
                = 10.191 ms
```

**Why the Discrepancy?**
- The original document uses a simplified model focusing only on base request processing + encryption
- Real-world processing includes virus scanning, compression, database operations, which dominate latency
- The original formula may assume asynchronous processing (some operations happen after response is sent)

**Asynchronous vs Synchronous Processing:**

| Model | Synchronous (wait for all) | Asynchronous (return early) |
|-------|---------------------------|----------------------------|
| **When to respond** | After blob storage write + DB update | After temp storage write |
| **Processing time** | 266-614 ms | 10-50 ms |
| **User experience** | Slower but guarantees file is stored | Faster but file may fail post-upload processing |
| **Trade-off** | Reliability > Speed | Speed > Immediate reliability |

**Best Practice:** Use asynchronous processing for non-critical operations:
- **Synchronous**: Authentication, validation, temp storage write, DB insert (minimal metadata)
- **Asynchronous**: Virus scan, compression, encryption, blob storage move, CDN replication

#### 4. Total Upload Latency Calculation

**Synchronous Processing Model:**
```
Total_Time = Network_Latency + Processing_Time + Response_Transfer

Network_Latency = RTT_base + (Transfer_coefficient × Request_size)
                = 260 ms + (1.15 ms/KB × 10,242.5 KB)
                = 12,038.875 ms

Processing_Time = 266 ms (typical from table above)

Response_Transfer = RTT_base + (1.15 ms/KB × Response_size)
                  = 260 ms + (1.15 ms/KB × 2 KB)  # Small JSON response
                  = 262.3 ms

Total_Time = 12,038.875 + 266 + 262.3
           = 12,567.175 ms
           ≈ 12.57 seconds
```

**Asynchronous Processing Model:**
```
Total_Time = Network_Latency + Fast_Processing + Response_Transfer

Fast_Processing = Auth + Validation + TempWrite + DBInsert
                = 3 + 1.5 + 10 + 8
                = 22.5 ms

Total_Time = 12,038.875 + 22.5 + 262.3
           = 12,323.675 ms
           ≈ 12.32 seconds
```

**Key Insight:** Network transfer dominates total time (~95-98%). Optimizing server processing from 266 ms to 22 ms only saves ~240 ms (~2% improvement).

**Original Document's Calculation:**
```
Time_latency_min = Base_min + RTT_POST + Response_overhead
                 = 120.5 + (260 + 1.15 × 10,242) + 0.4
                 = 120.5 + 12,038.3 + 0.4
                 = 12,159.2 ms

Time_latency_max = Base_max + RTT_POST + Response_overhead
                 = 201.5 + 12,038.3 + 0.4
                 = 12,240.2 ms

Processing_Time = 4 + 6.191 = 10.191 ms

Total_Response_min = 12,159.2 + 10.191 = 12,169.39 ms
Total_Response_max = 12,240.2 + 10.191 = 12,250.39 ms
```

**Where do 120.5 and 201.5 come from?**
- These are likely base latency values for minimal HTTP requests (empty POST)
- `120.5 ms`: Best-case scenario (no congestion, optimal routing)
- `201.5 ms`: Worst-case scenario (some congestion, suboptimal routing)
- The formula adds these to the data transfer time

#### 5. Latency Optimization Strategies

**A. Network-Level Optimizations:**

| Technique | Improvement | Implementation |
|-----------|-------------|----------------|
| **HTTP/2** | 10-30% faster | Multiplexing, header compression, server push |
| **HTTP/3 (QUIC)** | 20-50% faster | 0-RTT connection, multiplexing, no head-of-line blocking |
| **CDN/Edge Upload** | 40-60% faster | Upload to nearest geographic location |
| **Compression** | 50-80% faster | Client compresses before upload (gzip, brotli) |
| **Chunked parallel upload** | 2-4× faster | Split file into chunks, upload in parallel |

**Example: HTTP/2 Improvement**
```
HTTP/1.1: 2 RTT for handshake (TCP + TLS) = 520 ms
HTTP/2: 1 RTT (reuse connection) = 260 ms
Savings: 260 ms (not huge for 10 MB upload, but significant for many small files)
```

**Example: Compression Improvement**
```
Original file: 10 MB (uncompressed image)
Compressed (gzip): 3 MB (70% reduction)

Transfer time: 1.15 ms/KB × 3,072 KB = 3,532.8 ms
Savings: 12,038.875 - 3,532.8 = 8,506 ms (68% faster)
```

**B. Server-Level Optimizations:**

| Technique | Improvement | Implementation |
|-----------|-------------|----------------|
| **Async processing** | 50-90% faster response | Respond after temp storage, process in background |
| **Hardware acceleration** | 2-5× faster | Use AES-NI for encryption, GPU for video encoding |
| **Direct upload to blob** | 30-50% faster | Client gets signed URL, uploads directly to S3/GCS |
| **Skip virus scan for trusted users** | 100-200 ms saved | Risk-based authentication |
| **Database connection pooling** | 5-10 ms saved per request | Reuse connections instead of creating new ones |

**C. Protocol-Level Optimizations:**

| Technique | Improvement | Implementation |
|-----------|-------------|----------------|
| **Resumable uploads** | Handles interruptions | Use upload sessions with Content-Range headers |
| **Deduplication** | Instant for duplicates | Check checksum, return existing fileId if match |
| **Delta uploads** | 70-90% faster | Only upload changed portions (rsync-like) |

#### 6. Comparative Analysis: Upload Scenarios

**Scenario 1: Small File (100 KB image)**
```
Request size = 2 KB overhead + 100 KB = 102 KB
Network time = 260 + (1.15 × 102) = 377.3 ms
Processing = 22.5 ms (async)
Response = 262.3 ms
Total = 662.1 ms ≈ 0.66 seconds

Network: 96% | Processing: 3% | Response: 40%
```

**Scenario 2: Medium File (10 MB image - our example)**
```
Total = 12,323.675 ms ≈ 12.32 seconds
Network: 98% | Processing: 0.2% | Response: 2%
```

**Scenario 3: Large File (1 GB video)**
```
Request size = 2 KB + 1,048,576 KB = 1,048,578 KB
Network time = 260 + (1.15 × 1,048,578) = 1,205,124.7 ms ≈ 20 minutes
Processing = 266 ms (sync, with compression/encoding)
Response = 262.3 ms
Total ≈ 1,205,653 ms ≈ 20.1 minutes

Network: 99.9% | Processing: 0.02% | Response: 0.02%
```

**Key Takeaway:** For files >1 MB, network transfer dominates. Server optimizations have minimal impact unless you reduce the transferred data size (compression) or reduce RTT (geographic proximity).

### Download request

#### Overview
File download is the process of retrieving stored files from the server to the client. Unlike upload, download is typically read-only and focuses on fast content delivery. The process involves:
1. **Authentication & Authorization**: Verify user has permission to access the file
2. **File Lookup**: Query UFMS for file metadata and storage location
3. **Content Retrieval**: Fetch file from blob storage or CDN
4. **Response Streaming**: Send file content to client with proper headers
5. **Integrity Verification**: Provide checksum for client-side validation

#### Basic Download Request

**Request:**
```http
GET /files/12345 HTTP/1.1
Host: api.fileservice.com
Authorization: Bearer <auth_token>
Accept: image/png, */*
Range: bytes=0-
If-None-Match: "sha256-a3b2c1d4e5f6..."
```

**Request Breakdown:**
- **GET method**: Standard HTTP method for retrieving resources
- **fileId in path**: `12345` identifies the specific file to download
- **Authorization**: Bearer token to verify user has read access
- **Accept header**: Indicates client's preferred content types
- **Range header**: Optional; enables partial content downloads (resumable downloads)
  - `bytes=0-`: Request entire file from start
  - `bytes=0-1023`: Request first 1 KB
  - `bytes=1024-`: Request from byte 1024 to end
- **If-None-Match**: Optional; provides cached ETag for conditional requests (304 Not Modified if unchanged)

#### Success Response (Full Download)

```http
HTTP/1.1 200 OK
Content-Type: image/png
Content-Length: 10485760
Content-Disposition: attachment; filename="example.png"
ETag: "sha256-a3b2c1d4e5f6..."
Digest: sha-256=o6LB1OX2...
Cache-Control: public, max-age=31536000
X-File-Id: 12345
X-Owner-Id: user-987
Last-Modified: Wed, 15 Jan 2024 10:30:00 GMT
Accept-Ranges: bytes

<10 MB binary file content>
```

**Response Details:**
- **200 OK**: Successful retrieval
- **Content-Type**: MIME type of the file (from metadata)
- **Content-Length**: Total file size in bytes (for progress tracking)
- **Content-Disposition**:
  - `attachment`: Forces download (not inline display)
  - `inline`: Display in browser (for images, PDFs)
  - `filename`: Suggested filename for saving
- **ETag**: Entity tag for caching; typically contains checksum
- **Digest**: SHA-256 checksum for integrity verification
- **Cache-Control**: Browser/CDN caching policy
  - `public`: Can be cached by CDN and browsers
  - `max-age=31536000`: Cache for 1 year (immutable files)
- **X-File-Id**: Custom header for tracking
- **Last-Modified**: Timestamp for conditional requests
- **Accept-Ranges**: Indicates server supports partial content requests

#### Partial Download Response (Range Request)

**Request:**
```http
GET /files/12345 HTTP/1.1
Host: api.fileservice.com
Authorization: Bearer <auth_token>
Range: bytes=5242880-10485759
```

**Response:**
```http
HTTP/1.1 206 Partial Content
Content-Type: image/png
Content-Length: 5242880
Content-Range: bytes 5242880-10485759/10485760
ETag: "sha256-a3b2c1d4e5f6..."
Accept-Ranges: bytes

<5 MB binary chunk>
```

**Response Details:**
- **206 Partial Content**: Indicates partial response
- **Content-Range**: Specifies which bytes are being sent
  - Format: `bytes start-end/total`
  - Example: `bytes 5242880-10485759/10485760` (second half of 10 MB file)
- **Content-Length**: Size of this chunk (not total file size)

**Use Cases:**
- **Resumable downloads**: Client downloads in chunks, can resume if interrupted
- **Video streaming**: Download file progressively as user watches
- **Large file optimization**: Download in parallel chunks for faster transfer

#### Error Responses

**1. Unauthorized Access**
```http
HTTP/1.1 401 Unauthorized
Content-Type: application/json

{
    "error": "Unauthorized",
    "message": "Invalid or expired authentication token",
    "errorCode": "AUTH_001"
}
```

**2. Forbidden (User doesn't own file)**
```http
HTTP/1.1 403 Forbidden
Content-Type: application/json

{
    "error": "Forbidden",
    "message": "You do not have permission to access this file",
    "errorCode": "PERMISSION_001",
    "fileId": "12345"
}
```

**3. File Not Found**
```http
HTTP/1.1 404 Not Found
Content-Type: application/json

{
    "error": "Not Found",
    "message": "File with ID '12345' does not exist",
    "errorCode": "FILE_001",
    "requestId": "req-xyz-456"
}
```

**4. Not Modified (Cached Version Valid)**
```http
HTTP/1.1 304 Not Modified
ETag: "sha256-a3b2c1d4e5f6..."
Cache-Control: public, max-age=31536000
```

**Use Case**: Client sends `If-None-Match: "sha256-a3b2c1d4e5f6..."`. Server checks if file changed. If not, returns 304 (no body) to save bandwidth.

**5. Range Not Satisfiable**
```http
HTTP/1.1 416 Range Not Satisfiable
Content-Range: bytes */10485760

{
    "error": "Range Not Satisfiable",
    "message": "Requested range exceeds file size",
    "errorCode": "RANGE_001",
    "fileSize": 10485760,
    "requestedRange": "bytes=20000000-30000000"
}
```

**6. File Corrupted or Missing in Storage**
```http
HTTP/1.1 500 Internal Server Error
Content-Type: application/json

{
    "error": "Internal Server Error",
    "message": "File exists in database but missing from storage",
    "errorCode": "STORAGE_001",
    "fileId": "12345",
    "requestId": "req-abc-789"
}
```

#### Server-Side Processing Flow

**1. API Gateway Stage:**
   - Validate Authorization header and extract user identity
   - Rate-limit check (prevent abuse, especially for large downloads)
   - Route request to file server

**2. File Server Stage:**
   - Parse fileId from request path
   - Query UFMS for file metadata (ownerId, fileTitle, fileSize, mimeType, storageLocation, checksum)
   - **Authorization check**: Verify requesting user has permission to access file
     - Owner: Full access
     - Shared users: Check `userList` field in metadata
     - Public files: Anyone can access
   - Return 403 Forbidden if user lacks permission
   - Return 404 Not Found if file doesn't exist in database

**3. Content Retrieval Stage:**
   - Check if file is available in CDN (nearest geographic location)
     - **CDN hit**: Serve directly from CDN (fastest path)
     - **CDN miss**: Fetch from blob storage
   - If blob storage request:
     - Connect to blob storage (S3, GCS, Azure Blob)
     - Stream file content (avoid loading entire file in memory)
     - Optionally cache in CDN for future requests

**4. Processing Stage (if needed):**
   - **Decryption**: If file is encrypted at-rest, decrypt while streaming
   - **Decompression**: If file was compressed server-side, decompress
   - **Format conversion**: Optional (e.g., resize image, transcode video)

**5. Response Generation Stage:**
   - Set appropriate headers:
     - Content-Type (from metadata)
     - Content-Length (file size)
     - Content-Disposition (attachment vs inline)
     - ETag (checksum for caching)
     - Cache-Control (caching policy)
   - Stream file content to client
   - Log download event for analytics and auditing

#### Download Request Latency Breakdown

Understanding download performance is critical for user experience. Let's analyze the time to download a 10 MB image.

**Total Download Time Formula:**
```
Total_Time = Request_Processing + Content_Retrieval + Network_Transfer + Decryption
```

**1. Request Processing Time**

| Operation | Time | Notes |
|-----------|------|-------|
| **Authentication/Authorization** | 2-5 ms | JWT validation, database lookup |
| **UFMS metadata query** | 3-8 ms | SQL SELECT with indexes on fileId |
| **Permission check** | 1-2 ms | Compare userId with ownerId or userList |
| **CDN lookup** | 5-15 ms | Check if file cached in nearest CDN node |
| **Total request processing** | **11-30 ms** | Fast for metadata-only operations |

**2. Content Retrieval Time**

**Scenario A: CDN Hit (Best Case)**
```
CDN retrieval = Network latency to CDN + Data transfer
              = 10-30 ms (nearby CDN node)

File is already cached at geographically close CDN node.
```

**Scenario B: CDN Miss → Blob Storage (Common Case)**
```
Blob storage retrieval = Network latency to storage + Data transfer
                       = 50-200 ms (depends on region)

For 10 MB at 1 Gbps internal network:
Transfer time = (10 MB × 8 bits/byte) / 1 Gbps = 80 ms
Total = 50 ms latency + 80 ms transfer = 130 ms
```

**Scenario C: Blob Storage Failure → Backup Replica (Rare)**
```
Fallback retrieval = Failed attempt + Retry to backup
                   = 200-500 ms (includes timeout and retry)
```

**3. Network Transfer Time (Dominant Factor)**

Similar to upload, network transfer dominates for large files.

```
Network_Transfer = RTT_base + (Transfer_coefficient × File_size)
```

**Download is typically faster than upload due to:**
- **Asymmetric bandwidth**: Most ISPs provide faster download (10-100 Mbps) than upload (1-10 Mbps)
- **CDN acceleration**: Content served from geographically nearby servers
- **Optimized routing**: CDN providers use better network paths

**Example: 10 MB Download on Home Broadband**
```
Assumed download bandwidth: 50 Mbps (6.25 MB/s)
Transfer time = 10 MB / 6.25 MB/s = 1.6 seconds

RTT to CDN = 20-50 ms (nearby CDN node vs origin server's 260 ms RTT)

Total network time = 50 ms RTT + 1,600 ms transfer = 1,650 ms ≈ 1.65 seconds
```

**Comparison: Upload vs Download (10 MB file)**

| Metric | Upload (7 Mbps) | Download (50 Mbps) | Speedup |
|--------|----------------|-------------------|---------|
| Bandwidth | 870 KB/s | 6,250 KB/s | 7.2× faster |
| Transfer time | 11,778 ms | 1,638 ms | 7.2× faster |
| RTT | 260 ms (origin) | 50 ms (CDN) | 5.2× faster |
| **Total time** | **12,038 ms** | **1,688 ms** | **7.1× faster** |

**4. Optional Processing Time**

| Operation | Time | Notes |
|-----------|------|-------|
| **Decryption (AES-256)** | 10-30 ms | AES-NI hardware: ~1-3 GB/s throughput |
| **Decompression** | 5-20 ms | If server-side compression was used |
| **Format conversion** | 50-500 ms | Optional: resize image, transcode video |
| **Total processing** | **15-50 ms** | Usually done asynchronously or skipped |

**5. Total Download Latency (CDN Hit)**

```
Total_Time = Request_Processing + CDN_Retrieval + Network_Transfer + Decryption

Request_Processing = 20 ms (avg)
CDN_Retrieval = 20 ms (nearby node)
Network_Transfer = 1,638 ms (50 Mbps download)
Decryption = 20 ms (if encrypted at-rest)

Total = 20 + 20 + 1,638 + 20 = 1,698 ms ≈ 1.7 seconds
```

**Breakdown:**
- Network transfer: 96%
- CDN retrieval: 1.2%
- Decryption: 1.2%
- Request processing: 1.2%

**6. Total Download Latency (CDN Miss)**

```
Total_Time = Request_Processing + Blob_Storage_Retrieval + Network_Transfer + Decryption

Request_Processing = 20 ms
Blob_Storage_Retrieval = 130 ms (internal network fetch)
Network_Transfer = 1,638 ms
Decryption = 20 ms

Total = 20 + 130 + 1,638 + 20 = 1,808 ms ≈ 1.81 seconds
```

**Impact of CDN miss:** +110 ms (6% slower)

**Key Insight:** CDN caching provides marginal improvement (~6%) for large files where network transfer dominates. For small files (<1 MB), CDN impact is more significant (30-50% faster).

#### Download Optimization Strategies

**A. CDN-Level Optimizations (Biggest Impact)**

| Technique | Improvement | Implementation |
|-----------|-------------|----------------|
| **Geographic CDN distribution** | 50-80% faster | Serve from nearest PoP (Point of Presence) |
| **HTTP/2 Server Push** | 20-40% faster | Push related resources (thumbnails, metadata) |
| **Smart caching policies** | 90%+ faster (cache hits) | Cache immutable files aggressively; purge on update |
| **Pre-warming cache** | Instant for popular files | Proactively cache trending content |
| **Adaptive bitrate streaming** | Better UX | Serve different quality based on bandwidth |

**Example: CDN Geographic Distribution**
```
Without CDN (origin server in US West):
- User in Asia: 250 ms RTT + 1,638 ms transfer = 1,888 ms

With CDN (edge node in Asia):
- User in Asia: 30 ms RTT + 1,638 ms transfer = 1,668 ms
- Improvement: 220 ms (12% faster)

For small files (100 KB):
- Without CDN: 250 ms RTT + 16 ms transfer = 266 ms
- With CDN: 30 ms RTT + 16 ms transfer = 46 ms
- Improvement: 220 ms (83% faster)
```

**B. Protocol-Level Optimizations**

| Technique | Improvement | Implementation |
|-----------|-------------|----------------|
| **HTTP/3 (QUIC)** | 20-50% faster | 0-RTT reconnection, multiplexing, no head-of-line blocking |
| **Compression** | 50-80% smaller | gzip, brotli for text; AVIF, WebP for images |
| **Range requests** | Enables streaming | Download file in chunks for progressive rendering |
| **ETag caching** | Instant (304 Not Modified) | Return 304 if client's cached version is current |

**C. Server-Level Optimizations**

| Technique | Improvement | Implementation |
|-----------|-------------|----------------|
| **Direct CDN serving** | 30-50% faster | Client gets pre-signed CDN URL, downloads directly (skip app server) |
| **Lazy decryption** | 10-30 ms saved | Decrypt on-the-fly while streaming (avoid pre-processing) |
| **Connection pooling** | 5-10 ms saved | Reuse blob storage connections |
| **Read-through cache** | 90%+ faster | Cache frequently accessed files in Redis/Memcached |

**D. Client-Level Optimizations**

| Technique | Improvement | Implementation |
|-----------|-------------|----------------|
| **Parallel chunk downloads** | 2-4× faster | Download multiple ranges simultaneously |
| **Browser caching** | Instant (no request) | Set long Cache-Control for immutable files |
| **Service Workers** | Offline access | Cache files in browser for offline viewing |
| **Progressive rendering** | Better UX | Display image/video as chunks arrive (not after full download) |

#### Comparative Analysis: Download Scenarios

**Scenario 1: Small File (100 KB thumbnail) - CDN Hit**
```
Request processing = 20 ms
CDN retrieval = 20 ms
Network transfer (50 Mbps) = 50 ms RTT + 16 ms = 66 ms
Decryption = 2 ms (small file)
Total = 108 ms ≈ 0.11 seconds

CDN impact: 20 ms (18% of total) — significant for small files
```

**Scenario 2: Medium File (10 MB image) - CDN Hit**
```
Total = 1,698 ms ≈ 1.7 seconds (as calculated above)
CDN impact: 20 ms (1.2% of total) — minimal for large files
```

**Scenario 3: Large File (1 GB video) - CDN Hit, 100 Mbps Connection**
```
Request processing = 20 ms
CDN retrieval = 30 ms
Network transfer = 50 ms RTT + 81,920 ms (1 GB at 100 Mbps) = 81,970 ms
Decryption = 300 ms (large file)
Total = 82,320 ms ≈ 82.3 seconds ≈ 1.37 minutes

Network transfer: 99.6% (completely dominates)
```

**Scenario 4: Video Streaming (Range Requests)**
```
Instead of downloading entire 1 GB video:
- Download first 10 MB chunk: 1.7 seconds
- Start playback while downloading remaining chunks
- User sees video in 1.7 seconds instead of 82 seconds
- Improvement: 48× better perceived performance
```

#### Security Considerations

| Concern | Mitigation |
|---------|-----------|
| **Unauthorized access** | Verify user permissions before serving file; check ownership or shared access |
| **Token replay attacks** | Use short-lived tokens; include request timestamp validation |
| **Hotlinking** | Use signed URLs with expiration; check Referer header (weak but helpful) |
| **Content tampering** | Return ETag/Digest header so client can verify integrity |
| **Information disclosure** | Return 404 (not 403) for non-existent files to avoid revealing file existence |
| **CDN cache poisoning** | Validate cache keys; purge cache on file update |
| **Bandwidth theft** | Rate-limit downloads per user; implement quota limits |
| **DDoS via large downloads** | Rate-limit at API Gateway; use CDN to absorb traffic |

#### Best Practices

**1. Client-Side:**
   - Implement retry logic with exponential backoff for transient failures
   - Verify checksum (ETag/Digest) after download to detect corruption
   - Use Range requests for large files (enable pause/resume)
   - Cache aggressively for immutable files (use ETag for validation)
   - Show download progress to user

**2. Server-Side:**
   - Always serve downloads through CDN (not directly from blob storage)
   - Use streaming responses (don't load entire file in memory)
   - Set appropriate Cache-Control headers based on file mutability
   - Return ETag/Digest for integrity verification
   - Log download events for analytics (popular files, bandwidth usage)
   - Implement conditional requests (If-None-Match, If-Modified-Since)

**3. CDN Configuration:**
   - Cache immutable files for 1 year: `Cache-Control: public, max-age=31536000, immutable`
   - Cache mutable files for shorter periods: `Cache-Control: public, max-age=3600`, purge cache on update
   - Purge CDN cache when file is updated or deleted
   - Use geo-routing to serve from nearest PoP
   - Enable compression at CDN level (gzip, brotli)

**4. Performance Monitoring:**
   - Track CDN hit rate (target: >90% for popular files)
   - Monitor download latency by region
   - Alert on high error rates (404, 500, 503)
   - Analyze bandwidth usage per user/file
   - Measure Time-To-First-Byte (TTFB) for performance

#### Interview-Ready Summary

**Q: Walk me through the download flow from client request to file delivery.**

**A:** Download flow has five stages:

1. **Authentication & Authorization** (20 ms):
   - API Gateway validates Bearer token
   - File server queries UFMS for file metadata
   - Verify user has read permission (owner or in shared userList)

2. **Content Location** (20-130 ms):
   - Check CDN for cached copy (20 ms if hit)
   - If miss, fetch from blob storage (130 ms)
   - CDN hit rate typically 80-95% for popular files

3. **Processing** (15-50 ms, optional):
   - Decrypt if file is encrypted at-rest
   - Decompress if server-side compression was used
   - Format conversion if needed (resize image, transcode video)

4. **Network Transfer** (1-80 seconds, dominant factor):
   - Stream file content to client
   - Time depends on file size and client's download bandwidth
   - 10 MB at 50 Mbps = 1.6 seconds

5. **Client Verification**:
   - Client checks ETag/Digest to verify integrity
   - Optionally cache for future access

**Q: Why is download typically faster than upload?**

**A:** Three main reasons:

1. **Asymmetric bandwidth**: ISPs provide 5-10× faster download than upload
   - Example: 10 Mbps upload, 50-100 Mbps download

2. **CDN acceleration**: Downloads served from nearby CDN edge nodes (10-50 ms RTT) vs uploads to origin server (100-300 ms RTT)

3. **Optimized infrastructure**: CDN providers use premium network paths, peering, and caching

Result: 10 MB file takes ~12 seconds to upload, ~1.7 seconds to download (7× faster)

**Q: How do you optimize download performance?**

**A:** Prioritize by impact:

1. **CDN (70-90% improvement for small files)**:
   - Serve from geographically distributed edge nodes
   - Aggressive caching for immutable files
   - Pre-warm cache for popular content

2. **Protocol (20-50% improvement)**:
   - HTTP/3 (QUIC) for 0-RTT and better congestion control
   - Compression (gzip, brotli) for 50-80% size reduction
   - Range requests for progressive rendering and resumable downloads

3. **Caching (instant for cache hits)**:
   - Long Cache-Control for immutable files
   - ETag/If-None-Match for conditional requests (304 Not Modified)
   - Browser/Service Worker caching for offline access

4. **Smart strategies**:
   - Direct CDN URLs (skip app server entirely)
   - Parallel chunk downloads for large files
   - Adaptive bitrate streaming for videos

**Q: How do Range requests work and when would you use them?**

**A:** Range requests allow downloading file in chunks:

**Mechanism:**
- Client sends `Range: bytes=0-1023` header
- Server responds with `206 Partial Content` and `Content-Range: bytes 0-1023/10485760`
- Client can request multiple ranges or continue from where it left off

**Use cases:**
1. **Video streaming**: Download first few seconds, start playback immediately
2. **Resumable downloads**: If connection drops, resume from last byte received
3. **Parallel downloads**: Request multiple ranges simultaneously (faster)
4. **Large file optimization**: Download in chunks to avoid memory issues

**Example:**
```http
GET /files/12345 HTTP/1.1
Range: bytes=0-5242879

Response: 206 Partial Content
Content-Range: bytes 0-5242879/10485760
<first 5 MB>

GET /files/12345 HTTP/1.1
Range: bytes=5242880-10485759

Response: 206 Partial Content
Content-Range: bytes 5242880-10485759/10485760
<second 5 MB>
```

**Q: What's the trade-off between CDN caching and data freshness?**

**A:**
- **Aggressive caching** (long TTL): Faster downloads, lower origin load, but stale content risk
- **Short caching** (short TTL): Always fresh, but slower, higher origin load

**Best practice:**
- **Immutable files** (never change): Cache for 1 year with `Cache-Control: public, max-age=31536000, immutable`
- **Mutable files** (can change): Cache for 1 hour with `Cache-Control: public, max-age=3600`, purge cache on update
- **Use versioned URLs**: `/files/12345?v=2024-01-15` — change URL when file changes, cache aggressively

**Q: How do you handle file corruption detection during download?**

**A:** Multi-layer approach:

1. **Transport layer**: HTTPS/TLS provides MAC/HMAC for in-transit integrity
2. **Application layer**: Server returns `ETag: "sha256-abc123"` or `Digest: sha-256=<base64>`
3. **Client verification**: After download, client computes SHA-256 and compares with ETag
4. **Retry on mismatch**: If checksums don't match, retry download (transient network error)
5. **Report corruption**: If repeated failures, alert user and log for investigation

**Trade-off**: Computing SHA-256 on client adds CPU cost (~20-50 ms for 10 MB), but critical for data integrity






# Interview Questions & Answers

**Q1: How would you handle uploading very large files (>1GB)?**

**A:** Implement resumable uploads using chunked transfer:
- Client splits file into chunks (5-10MB each)
- Each chunk uploaded separately with Content-Range header
- Server tracks progress via upload session
- If network fails, client resumes from last successful chunk
- Benefits: Better reliability, ability to pause/resume, lower memory usage
- Use HTTP/2 for parallel chunk uploads (faster overall transfer)

**Q2: How do you ensure file integrity during upload?**

**A:** Multiple mechanisms:
1. **Client-side checksum**: Compute SHA-256 hash before upload
2. **Server-side verification**: Recompute hash after receiving file, compare
3. **Content-MD5 header**: Include in request for intermediate validation
4. **Chunked verification**: For resumable uploads, verify each chunk's hash
5. **Post-storage validation**: Re-read from storage and verify hash
6. Return checksum in response so client can verify downloaded file later

**Q3: What's the difference between multipart/form-data and multipart/mixed?**

**A:**
- **multipart/form-data**: Designed for HTML form submissions; each part represents a form field; commonly used by browsers
- **multipart/mixed**: General-purpose; parts are independent entities; better for API design with explicit metadata + binary separation
- Both work technically, but multipart/mixed is clearer for APIs expecting JSON metadata + binary content

**Q4: How would you implement upload quotas and rate limiting?**

**A:**
- **Storage Quota**:
  - Track user's total storage usage in database
  - Before upload, check: `currentUsage + fileSize <= userQuota`
  - Update usage atomically after successful upload
  - Return 507 Insufficient Storage if quota exceeded

- **Rate Limiting**:
  - Use token bucket or sliding window algorithm at API Gateway
  - Limit: uploads per minute/hour per user
  - Return 429 Too Many Requests with Retry-After header
  - Consider different limits for different user tiers

**Q5: How do you handle concurrent uploads of the same file?**

**A:**
- **Idempotency**: Use client-generated request ID or file content hash
- **Deduplication**: Compute checksum; if file with same hash exists, return existing fileId
- **Optimistic locking**: Use version numbers or timestamps in metadata
- **Distributed locks**: For resumable uploads, lock upload session to prevent conflicts
- Trade-off: Deduplication saves storage but may have privacy implications (users can detect if others uploaded same file)

**Q6: Why separate metadata storage from file content storage?**

**A:**
1. **Query performance**: Metadata needs frequent queries (list, search, filter); SQL databases optimized for this
2. **Scalability**: File content is immutable, large, and rarely accessed; blob storage is cheaper and scales better
3. **Sharing**: Metadata shared with multiple services (auth, user service); keeping it in SQL allows efficient joins
4. **Failure isolation**: Metadata service failure doesn't affect file storage, and vice versa
5. **Encryption**: File content encrypted at-rest; metadata needs to be searchable (can't easily search encrypted data)

**Q7: How would you optimize upload performance?**

**A:**
1. **Client-side**:
   - Compress before upload (gzip, deflate)
   - Use HTTP/2 for multiplexing
   - Parallel chunk uploads for large files

2. **Server-side**:
   - Geographic distribution (upload to nearest server)
   - Direct upload to blob storage (client gets signed URL, uploads directly)
   - Asynchronous processing (virus scan, encoding after upload)
   - CDN for upload endpoints

3. **Network**:
   - Use TCP BBR congestion control
   - Enable HTTP/2 or HTTP/3 (QUIC)
   - Optimize TLS handshake (session resumption, 0-RTT)

**Q8: How do we ensure that uploaded data is not corrupted enroute to the service and also when downloading?**
**A:**

- Transport-layer integrity
    - Enforce HTTPS/TLS for all client↔server traffic — TLS provides encryption and message integrity (MAC/HMAC).
- End-to-end, application-level checksums
    - Client computes a strong checksum (SHA-256) before upload and sends it with the request (e.g., Digest: sha-256=<base64> or a custom X-Checksum-Sha256 header).
    - Server recomputes checksum while streaming the upload (no full buffering) and compares against the client-provided value before accepting the file.
    - Server returns the verified checksum (and fileId) in the success response so the client can validate stored data.
- Chunked/resumable uploads
    - For resumable uploads, require a checksum per chunk (e.g., Digest header on each PUT) and a final aggregate checksum at completion. Verify each chunk on receipt and the final assembled file before marking the session complete.
- Download validation
    - Include a checksum (Digest or ETag containing sha256) in download responses so clients can verify integrity after download.
- Storage-level verification
    - Perform server-side post-write validation (re-read + verify checksum) and periodic data integrity scans (scrubbing, checksumming replicas).
    - Use object-store features (versioning, checksums, and integrity metadata) where available.
- Use authenticated encryption for at-rest integrity
    - AEAD (e.g., AES-GCM) provides both confidentiality and integrity for stored objects; combined with checksums this defends against tampering.
- Practical notes / best practices
    - Prefer Digest (RFC 3230) or a SHA-256-based header over MD5; MD5 is weaker and sometimes deprecated.
    - Stream hashing to avoid memory pressure for large files.
    - Expect CPU cost for hashing; consider hardware acceleration or offloading for high throughput.
    - Log request IDs and checksum mismatches for debugging and auditing.

Example headers:
- Upload: Authorization, Content-Type, Digest: sha-256=<base64>
- Chunk PUT: Content-Range, Digest: sha-256=<base64>
- Download response: ETag: "sha256-<hex>" or Digest: sha-256=<base64>


**Q9: Will file encryption and decryption affect the response time of the API?**

**A:**
Yes, encrypting and decrypting files before storing and sending them back to the user, respectively, results in increased processing time. The extent of the impact depends on several factors:
1. **Algorithm Complexity**: Stronger encryption algorithms (e.g., AES-256) require more computational resources and time compared to lighter algorithms (e.g., AES-128).
2. **File Size**: Larger files take longer to encrypt and decrypt, increasing overall processing time.
3. **Hardware Acceleration**: Utilizing hardware-based encryption (e.g., AES-NI) can significantly reduce latency compared to software-only implementations.
4. **Concurrency**: High volumes of simultaneous encryption/decryption requests can lead to resource contention, increasing response times.
5. **Implementation Efficiency**: Efficient coding practices (e.g., streaming encryption) can minimize latency compared to naive implementations that load entire files into memory.
6. **Network Overhead**: If encryption/decryption is performed on the client side, it may add to the overall time taken for upload/download due to additional processing before transmission.
To mitigate the impact on response time:
- Use efficient, well-optimized libraries for cryptographic operations.
- Implement streaming encryption/decryption to handle large files without excessive memory usage.- Leverage hardware acceleration where available.
- Monitor and scale resources to handle peak loads effectively.

This is a tradeoff between the response time and the sensitivity of the stored information. If the information is critical, we need to see how much latency we can tolerate to keep the data safe. Also, the major delay in response time is due to network transfers. Usually, servers process information much faster than the delays caused by network transfers. So, encrypting data with a faster algorithm might not be a big deal for the overall response time of an API.
