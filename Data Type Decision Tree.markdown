## Decision Tree: Selecting SQL Server Data Types

Follow this structured process to choose the optimal data type based on data characteristics, query patterns, and application needs.

### Step 1: Identify Data Category
- **Is the data numeric?** → Go to Numeric Data Types.
- **Is it text-based?** → Go to Text Data Types.
- **Is it date or time?** → Go to Date/Time Data Types.
- **Is it binary or boolean?** → Go to Binary Data Types.
- **Is it specialized (e.g., UUID, XML, geospatial)?** → Go to Other Data Types.

### Step 2: Additional Considerations
- **Expected data volume?** (e.g., small dataset vs. millions of rows)
- **Query patterns?** (e.g., frequent filtering, sorting, joins)
- **Collation needs?** (e.g., case-sensitive, accent-sensitive)
- **Security requirements?** (e.g., encryption for PII)
- **Regulatory compliance?** (e.g., GDPR, HIPAA)

### Decision Flowchart (Text-Based)
```
Start
  |
  v
Is it numeric?
  | Yes: Whole number? → Integer (tinyint, int, bigint)
  |           Decimal? → Decimal/float (decimal, float, money)
  | No: Text-based? → Fixed length? → char/nchar
  |                  Variable length? → varchar/nvarchar
  |      Date/Time? → Date only? → date
  |                  Time only? → time
  |                  Both? → datetime2/datetimeoffset
  |      Binary? → Boolean? → bit
  |              Fixed? → binary
  |              Variable? → varbinary
  |      Specialized? → UUID? → uniqueidentifier
  |                    XML? → xml
  |                    Geospatial? → geometry/geography
  v
Check: Volume, Queries, Compliance
  v
Apply: Constraints, Indexes, Defaults
```

## Numeric Data Types

### Decision Path
- **Is it a whole number?** → Choose `tinyint`, `smallint`, `int`, or `bigint` based on range.
- **Does it need decimal precision?** → Choose `decimal`, `float`, or `money`.
- **Will it be used in calculations?** (e.g., aggregations)
- **Is it for large datasets?** → Consider partitioning or compression.

### Integer Types
- **tinyint**
  - **C# Mapping**: `byte`
  - **Use Cases**: Small counts (e.g., retry attempts, 0-255).
  - **Advantages**: Minimal storage, fast for small indexes.
  - **Disadvantages**: Limited range, no negative values.
  - **Byte Size**: 1 byte.
  - **Indexing**: Ideal for clustered indexes on small tables.
- **smallint**
  - **C# Mapping**: `short`
  - **Use Cases**: Medium counts (e.g., department IDs, -32,768 to 32,767).
  - **Advantages**: Compact, efficient for medium ranges.
  - **Disadvantages**: Not suitable for large IDs.
  - **Byte Size**: 2 bytes.
  - **Indexing**: Good for non-clustered indexes.
- **int**
  - **C# Mapping**: `int`
  - **Use Cases**: Primary keys, counters (e.g., order IDs, -2.1B to 2.1B).
  - **Advantages**: Balanced storage, optimal for most scenarios.
  - **Disadvantages**: Overkill for tiny ranges.
  - **Byte Size**: 4 bytes.
  - **Indexing**: Preferred for primary keys.
- **bigint**
  - **C# Mapping**: `long`
  - **Use Cases**: Large IDs, timestamps (e.g., -9 quintillion to 9 quintillion).
  - **Advantages**: Scalable for large datasets.
  - **Disadvantages**: Larger storage, slower indexing.
  - **Byte Size**: 8 bytes.
  - **Indexing**: Use for large tables with partitioning.

### Decimal/Floating-Point Types
- **decimal/numeric** (synonymous)
  - **C# Mapping**: `decimal`
  - **Use Cases**: Financial data (e.g., 12345.67).
  - **Advantages**: Exact precision for calculations.
  - **Disadvantages**: Storage varies (5-17 bytes).
  - **Byte Size**: 5-17 bytes (based on precision ≤ 38).
  - **Indexing**: Avoid for high-precision in large tables.
- **float**
  - **C# Mapping**: `double`
  - **Use Cases**: Scientific data (e.g., sensor readings).
  - **Advantages**: Wide range, efficient for approximations.
  - **Disadvantages**: Imprecise for financial data.
  - **Byte Size**: 4 or 8 bytes (float(24) or float(53)).
  - **Indexing**: Suitable for non-critical columns.
- **real**
  - **C# Mapping**: `float`
  - **Use Cases**: Low-precision data (e.g., statistics).
  - **Advantages**: Smaller than `float(53)`.
  - **Disadvantages**: Limited precision.
  - **Byte Size**: 4 bytes.
  - **Indexing**: Rarely indexed.
- **money**
  - **C# Mapping**: `decimal`
  - **Use Cases**: Currency (e.g., $1234.5678).
  - **Advantages**: Readable, fixed 4-decimal precision.
  - **Disadvantages**: Larger than `decimal` for similar needs.
  - **Byte Size**: 8 bytes.
  - **Indexing**: Use `decimal` for better flexibility.
- **smallmoney**
  - **C# Mapping**: `decimal`
  - **Use Cases**: Small currency (e.g., -$214,748.3648 to $214,748.3647).
  - **Advantages**: Compact for small amounts.
  - **Disadvantages**: Limited range.
  - **Byte Size**: 4 bytes.
  - **Indexing**: Suitable for small tables.

## Text Data Types

### Decision Path
- **Fixed or variable length?**
- **Unicode support needed?** (e.g., multilingual data)
- **Expected length?** (e.g., codes vs. articles)
- **Full-text search required?** → Consider `varchar(max)` or `nvarchar(max)`.

### Text Types
- **char(n)**
  - **C# Mapping**: `string`
  - **Use Cases**: Fixed-length codes (e.g., "US").
  - **Advantages**: Fast for fixed-length searches.
  - **Disadvantages**: Wastes space for shorter strings.
  - **Byte Size**: `n` bytes.
  - **Indexing**: Excellent for clustered indexes.
- **varchar(n)**
  - **C# Mapping**: `string`
  - **Use Cases**: Emails, usernames.
  - **Advantages**: Efficient for varying lengths.
  - **Disadvantages**: 2-byte overhead.
  - **Byte Size**: `n` bytes + 2 bytes.
  - **Indexing**: Good for non-clustered indexes.
- **nchar(n)**
  - **C# Mapping**: `string`
  - **Use Cases**: Multilingual codes (e.g., "日本").
  - **Advantages**: Unicode support, predictable storage.
  - **Disadvantages**: Doubles storage for ASCII.
  - **Byte Size**: `2n` bytes.
  - **Indexing**: Suitable for fixed-length columns.
- **nvarchar(n)**
  - **C# Mapping**: `string`
  - **Use Cases**: Multilingual names, descriptions.
  - **Advantages**: Flexible, supports internationalization.
  - **Disadvantages**: Higher storage.
  - **Byte Size**: `2n` bytes + 2 bytes.
  - **Indexing**: Use for smaller `n` values.
- **varchar(max)**
  - **C# Mapping**: `string`
  - **Use Cases**: Large text (e.g., articles).
  - **Advantages**: Replaces `text`, supports full-text search.
  - **Disadvantages**: Cannot be indexed directly.
  - **Byte Size**: Up to 2^31-1 bytes + 2 bytes.
  - **Indexing**: Use computed columns for indexing.
- **nvarchar(max)**
  - **C# Mapping**: `string`
  - **Use Cases**: Large Unicode text (e.g., multilingual documents).
  - **Advantages**: Supports large Unicode data.
  - **Disadvantages**: High storage, no direct indexing.
  - **Byte Size**: Up to 2^30-1 bytes + 2 bytes.
  - **Indexing**: Avoid for large datasets.
- **text** (Deprecated)
  - **C# Mapping**: `string`
  - **Use Cases**: Legacy large text.
  - **Advantages**: Large capacity (2GB).
  - **Disadvantages**: Deprecated, use `varchar(max)`.
  - **Byte Size**: Up to 2^31-1 bytes.
  - **Indexing**: Not indexable.
- **ntext** (Deprecated)
  - **C# Mapping**: `string`
  - **Use Cases**: Legacy large Unicode text.
  - **Advantages**: Large Unicode capacity.
  - **Disadvantages**: Deprecated, use `nvarchar(max)`.
  - **Byte Size**: Up to 2^30-1 bytes.
  - **Indexing**: Not indexable.

## Date/Time Data Types

### Decision Path
- **Date only, time only, or both?**
- **Timezone support needed?**
- **Precision required?** (e.g., nanoseconds)
- **Temporal tables?** → Use `datetime2` for system versioning.

### Date/Time Types
- **date**
  - **C# Mapping**: `DateOnly`
  - **Use Cases**: Birthdates, event dates.
  - **Advantages**: Compact, clear semantics.
  - **Disadvantages**: No time component.
  - **Byte Size**: 3 bytes.
  - **Indexing**: Ideal for range queries.
- **time**
  - **C# Mapping**: `TimeOnly`
  - **Use Cases**: Schedules (e.g., 09:44:00).
  - **Advantages**: Precise time storage.
  - **Disadvantages**: No date component.
  - **Byte Size**: 3-5 bytes (based on precision).
  - **Indexing**: Suitable for filtering.
- **datetime**
  - **C# Mapping**: `DateTime`
  - **Use Cases**: Legacy timestamps.
  - **Advantages**: Widely supported.
  - **Disadvantages**: Low precision, use `datetime2`.
  - **Byte Size**: 8 bytes.
  - **Indexing**: Less efficient than `datetime2`.
- **datetime2**
  - **C# Mapping**: `DateTime`
  - **Use Cases**: Timestamps, temporal tables.
  - **Advantages**: High precision (100ns), flexible.
  - **Disadvantages**: Slightly larger storage.
  - **Byte Size**: 6-8 bytes (based on precision).
  - **Indexing**: Preferred for modern apps.
- **smalldatetime**
  - **C# Mapping**: `DateTime`
  - **Use Cases**: Low-precision timestamps (1900-2079).
  - **Advantages**: Smaller than `datetime`.
  - **Disadvantages**: Limited range, low precision.
  - **Byte Size**: 4 bytes.
  - **Indexing**: Use for small datasets.
- **datetimeoffset**
  - **C# Mapping**: `DateTimeOffset`
  - **Use Cases**: Global apps with timezone (e.g., UTC+8).
  - **Advantages**: Timezone support.
  - **Disadvantages**: Largest storage.
  - **Byte Size**: 10 bytes.
  - **Indexing**: Use for global applications.

## Binary Data Types

### Decision Path
- **Boolean value?** → Use `bit`.
- **Fixed or variable-length binary?**
- **Large binary data?** → Use `varbinary(max)` or file storage.
- **Encrypted data?** → Use `varbinary` with Always Encrypted.

### Binary Types
- **bit**
  - **C# Mapping**: `bool`
  - **Use Cases**: Flags (e.g., is_active).
  - **Advantages**: Minimal storage, fast.
  - **Disadvantages**: Single-bit only.
  - **Byte Size**: 1 bit (up to 8 bits per byte).
  - **Indexing**: Excellent for filtering.
- **binary(n)**
  - **C# Mapping**: `byte[]`
  - **Use Cases**: Fixed-length hashes.
  - **Advantages**: Predictable storage.
  - **Disadvantages**: Wastes space for shorter data.
  - **Byte Size**: `n` bytes.
  - **Indexing**: Suitable for fixed-length.
- **varbinary(n)**
  - **C# Mapping**: `byte[]`
  - **Use Cases**: Encrypted data, small binaries.
  - **Advantages**: Flexible, efficient.
  - **Disadvantages**: 2-byte overhead.
  - **Byte Size**: `n` bytes + 2 bytes.
  - **Indexing**: Good for smaller `n`.
- **varbinary(max)**
  - **C# Mapping**: `byte[]`
  - **Use Cases**: Large binaries (e.g., PDFs).
  - **Advantages**: Replaces `image`, large capacity.
  - **Disadvantages**: No direct indexing.
  - **Byte Size**: Up to 2^31-1 bytes + 2 bytes.
  - **Indexing**: Use file storage for large data.
- **image** (Deprecated)
  - **C# Mapping**: `byte[]`
  - **Use Cases**: Legacy binary data.
  - **Advantages**: Large capacity (2GB).
  - **Disadvantages**: Deprecated, use `varbinary(max)`.
  - **Byte Size**: Up to 2^31-1 bytes.
  - **Indexing**: Not indexable.

## Other Data Types

### Decision Path
- **Unique identifier?** → Use `uniqueidentifier`.
- **Structured data?** → Use `xml` or JSON (via `nvarchar`).
- **Geospatial data?** → Use `geometry` or `geography`.
- **Hierarchical data?** → Use `hierarchyid`.

### Other Types
- **uniqueidentifier**
  - **C# Mapping**: `Guid`
  - **Use Cases**: Distributed system IDs.
  - **Advantages**: Globally unique.
  - **Disadvantages**: Larger storage, slower indexing.
  - **Byte Size**: 16 bytes.
  - **Indexing**: Use `int` for better performance.
- **xml**
  - **C# Mapping**: `string` or `XmlDocument`
  - **Use Cases**: Structured XML data.
  - **Advantages**: Native XML queries.
  - **Disadvantages**: Storage overhead.
  - **Byte Size**: Up to 2GB.
  - **Indexing**: Use XML indexes.
- **sql_variant**
  - **C# Mapping**: `object`
  - **Use Cases**: Mixed data types (rare).
  - **Advantages**: Flexible for dynamic schemas.
  - **Disadvantages**: Poor performance.
  - **Byte Size**: Up to 8016 bytes.
  - **Indexing**: Avoid indexing.
- **geometry**
  - **C# Mapping**: Custom (e.g., `Microsoft.SqlServer.Types.SqlGeometry`)
  - **Use Cases**: Planar spatial data (e.g., shapes).
  - **Advantages**: Supports spatial queries.
  - **Disadvantages**: Complex to manage.
  - **Byte Size**: Variable.
  - **Indexing**: Use spatial indexes.
- **geography**
  - **C# Mapping**: Custom (e.g., `Microsoft.SqlServer.Types.SqlGeography`)
  - **Use Cases**: Geodetic data (e.g., coordinates).
  - **Advantages**: Supports geospatial calculations.
  - **Disadvantages**: Higher storage.
  - **Byte Size**: Variable.
  - **Indexing**: Use spatial indexes.
- **hierarchyid**
  - **C# Mapping**: `Microsoft.SqlServer.Types.SqlHierarchyId`
  - **Use Cases**: Organizational charts, trees.
  - **Advantages**: Efficient for hierarchies.
  - **Disadvantages**: Specialized, complex.
  - **Byte Size**: Variable (typically < 892 bytes).
  - **Indexing**: Use for hierarchical queries.

## Comparison of Text Data Types

| **Property**        | **char**                   | **varchar**                 | **nchar**                         | **nvarchar**            | **varchar(max)**             | **nvarchar(max)**            | **text** (Deprecated)    |
| ------------------- | -------------------------- | --------------------------- | --------------------------------- | ----------------------- | ---------------------------- | ---------------------------- | ------------------------ |
| **Definition**      | Fixed-length non-Unicode   | Variable-length non-Unicode | Fixed-length Unicode              | Variable-length Unicode | Large non-Unicode            | Large Unicode                | Legacy large non-Unicode |
| **Storage**         | `n` bytes                  | `n` bytes + 2 bytes         | `2n` bytes                        | `2n` bytes + 2 bytes    | Up to 2^31-1 bytes + 2 bytes | Up to 2^30-1 bytes + 2 bytes | Up to 2^31-1 bytes       |
| **Unicode Support** | No                         | No                          | Yes                               | Yes                     | No                           | Yes                          | No                       |
| **Use Cases**       | Country codes (e.g., "US") | Emails, usernames           | Multilingual codes (e.g., "日本") | Multilingual names      | Articles, logs               | Multilingual documents       | Legacy text              |
| **Performance**     | Fast searches              | Efficient storage           | Fast, high storage                | Flexible, high storage  | Slow, no direct indexing     | Slow, no direct indexing     | Poor, deprecated         |
| **C# Mapping**      | `string`                   | `string`                    | `string`                          | `string`                | `string`                     | `string`                     | `string`                 |

**Guidance**:
- **char/nchar**: Use for fixed-length data (e.g., codes); `nchar` for Unicode.
- **varchar/nvarchar**: Use for variable-length text; `nvarchar` for multilingual.
- **varchar(max)/nvarchar(max)**: Use for large text; prefer file storage for massive data.
- **text/ntext**: Avoid; migrate to `varchar(max)` or `nvarchar(max)`.

## Best Practices and Advanced Considerations
- **Primary Keys**: Use `int` or `bigint` for performance; `uniqueidentifier` for distributed systems.
- **Date/Time**: Use `datetime2` for modern apps; `datetimeoffset` for global apps.
- **Deprecated Types**: Replace `text`, `ntext`, `image` with `varchar(max)`, `nvarchar(max)`, `varbinary(max)`.
- **Storage Optimization**: Use smallest type (e.g., `tinyint` vs. `int`); avoid oversized `nvarchar(n)`.
- **Nullability**: Define `NOT NULL` where possible; use defaults (e.g., `GETDATE()` for `datetime2`).
- **Indexing**: Use smaller types for indexes; avoid `varchar(max)` or `sql_variant`.
- **Collation**: Choose collation early (e.g., `Latin1_General_CI_AS`); avoid mismatches.
- **Security**: Use `varbinary` with Always Encrypted for PII; consider `nvarchar` for hashed passwords.
- **Scalability**: Enable compression for large tables; use partitioning for billions of rows.
- **Temporal Tables**: Use `datetime2` for `SysStartTime` and `SysEndTime`.
- **Full-Text Search**: Use `varchar(max)` or `nvarchar(max)` with full-text indexes.
- **Tools**: Use SQL Server Data Tools (SSDT) for schema validation.

## Common Pitfalls and Troubleshooting
- **Implicit Conversions**: Avoid mixing `varchar` and `nvarchar` in joins (e.g., causes index scans).
- **Collation Mismatches**: Ensure database and column collations align.
- **Overusing `max` Types**: `varchar(max)`/`nvarchar(max)` reduce performance; use `varchar(255)` for short text.
- **GUID Performance**: `uniqueidentifier` is slower than `int`; use `NEWSEQUENTIALID()` for better clustering.
- **Precision Loss**: Avoid `float` for financial data; use `decimal`.
