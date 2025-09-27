# Cache Eviction Strategy Comparison

| Strategy         | Best Use Case                       | Space Overhead       | Time Complexity       | Pros                                      | Cons                                      | Key Takeaway                              |
|------------------|-------------------------------------|----------------------|-----------------------|------------------------------------------|------------------------------------------|------------------------------------------|
| FIFO (First-In-First-Out) | - Sequential access patterns<br>- When age of entry matters<br>- Batch processing systems | Low<br>- Queue: O(n)<br>- Hash set for tracking | - Add: O(1)<br>- Remove: O(1)<br>- Access: O(1) | - Simple to implement<br>- Low overhead<br>- Predictable behavior<br>- Good scan-resistance | - No consideration for usage frequency<br>- Can evict commonly used items<br>- No adaptation to changing access patterns | Best for workloads with uniform access or where recency of data is more important than frequency |
| LIFO (Last-In-First-Out) | - Short-lived temporary data<br>- Stack-based computations<br>- Navigation history | Low<br>- Stack: O(n)<br>- Hash set for tracking | - Add: O(1)<br>- Remove: O(1)<br>- Access: O(1) | - Simple to implement<br>- Very low overhead<br>- Works well for temporary data | - Terrible locality properties<br>- Can quickly evict useful data<br>- No consideration for usage patterns | Only suitable for very specific use cases where newest items are least valuable |
| LRU (Least Recently Used) | - General-purpose caching<br>- Systems with temporal locality<br>- Database buffer pools | Medium<br>- Doubly linked list: O(n)<br>- HashMap for O(1) lookup | - Add: O(1)<br>- Remove: O(1)<br>- Access: O(1) | - Adapts to changing access patterns<br>- Respects temporal locality<br>- Well-balanced performance<br>- Intuitive behavior | - Does not consider access frequency<br>- Vulnerable to scan pollution<br>- Movement on each access | The most widely used strategy for general caching needs due to good balance of complexity and effectiveness |
| LFU (Least Frequently Used) | - Static content caching<br>- Read-heavy workloads<br>- Pattern recognition systems | High<br>- Frequency counter for each key<br>- Multiple data structures | - Add: O(1)<br>- Remove: O(1)<br>- Access: O(log n) or O(1) with optimization | - Excellent for stable access patterns<br>- Less vulnerable to cache pollution<br>- Maintains hot items longer | - Slow to adapt to changing patterns<br>- Complex implementation<br>- Historical frequency can dominate | Best for workloads where access frequency is more important than recency, like content delivery networks |
| Random           | - Large-scale distributed caches<br>- Systems where eviction speed matters most<br>- When simplicity is preferred over optimization | Very Low<br>- Just a hash set for keys | - Add: O(1)<br>- Remove: O(1)<br>- Access: O(1)<br>- Eviction: O(1) | - Extremely simple implementation<br>- No algorithm bias<br>- Good performance at scale | - No intelligence in eviction choices<br>- Unpredictable behavior<br>- May evict important items | Surprisingly effective in large-scale systems where the law of large numbers provides statistical fairness |
| Time-Based       | - Data with well-defined lifespans<br>- Credentials and authentication<br>- Tokens<br>- Regulatory compliance requirements | Medium<br>- Expiration timestamps<br>- Sorted time index | - Add: O(log n)<br>- Remove: O(log n)<br>- Access: O(1) + check | - Self-cleaning<br>- Clear data lifecycle<br>- Easy to reason about<br>- Compliance friendly | - Requires periodic cleanup<br>- No adaptation to usage patterns<br>- May evict useful data prematurely | Best when data has natural expiration requirements, independent of access patterns |
| Size-Based       | - Multimedia caching<br>- Variable-sized objects<br>- Memory-constrained environments | Variable<br>- Size tracking per item<br>- Sorted size index | - Add: O(log n)<br>- Remove: O(log n)<br>- Access: O(1) | - Efficient space utilization<br>- Prevents single large items from dominating<br>- Better memory management | - Items over few large ones<br>- Complex size calculation<br>- Overhead of tracking sizes | Ideal for caches storing variable-sized objects where memory footprint is a primary constraint |
| Priority-Based   | - Multi-tenant systems<br>- Quality-of-service requirements<br>- Business-critical applications | Medium<br>- High<br>- Priority value per item<br>- Sorted priority index | - Add: O(log n)<br>- Remove: O(log n)<br>- Access: O(1) | - Business logic can drive caching<br>- Flexible prioritization<br>- Can incorporate multiple factors | - Manual priority assignment complexity<br>- May require application-specific logic<br>- Difficult to automate | Best when different cache entries have clearly differentiated importance levels based on business requirements |
| TinyLFU / W-TinyLFU | - Modern, high-performance caches<br>- Systems needing both recency and frequency<br>- Scan-resistant applications | Medium<br>- Frequency sketches<br>- Window tracking | - Add: O(1)<br>- Remove: O(1)<br>- Access: O(1) | - Excellent scan resistance<br>- Balances recency and frequency<br>- Low overhead frequency counting<br>- Adaptive to changing patterns | - More complex implementation<br>- Requires tuning window sizes<br>- Probabilistic nature | State-of-the-art approach that combines the best aspects of LRU and LFU with overhead lower than advanced strategies |
| ARC (Adaptive Replacement Cache) | - Database systems<br>- Performance-critical applications | High<br>- Maintains four separate lists | - Add: O(1)<br>- Remove: O(1)<br>- Access: O(1) | - Self-tuning<br>- Balances recency and frequency<br>- Scan-resistant<br>- Adapts to workload changes | - Complex implementation<br>- Higher memory overhead<br>- Patent encumbered (historically) | Advanced strategy that dynamically adjusts between recency and frequency based on workload patterns |
| Clock / Second Chance | - Operating systems<br>- Virtual memory management<br>- Simplified LRU approximation | Low<br>- Single bit per entry<br>- Circular buffer structure | - Add: O(1)<br>- Remove: O(1)<br>- Access: O(1) | - Approximates LRU with lower overhead<br>- No list reorganization on access<br>- Simple and efficient | - Less precise than true LRU<br>- May require multiple passes to find candidate<br>- Limited adaptability | Excellent compromise between implementation simplicity and effectiveness when true LRU would be too expensive |

![](top-8-cache-eviction.gif)

# Additional Considerations for Strategy Selection

## Workload Characteristics
- **Read-heavy vs. Write-heavy:** LRU works better for read-heavy workloads, while FIFO might be better for write-heavy scenarios
- **Scan Resistance:** LFU, TinyLFU, and ARC provide better resistance to scanning operations that could pollute the cache
- **Temporal Locality:** LRU works best when recently accessed items are likely to be accessed again soon

## System Constraints
- **Memory Constraints:** Size-based strategies become critical when memory is limited
- **Processing Overhead:** Random and CLOCK strategies minimize CPU usage for eviction decisions
- **Implementation Complexity:** FIFO and Random are easiest to implement; ARC and TinyLFU are most complex

## Advanced Hybrid Strategies
- Many modern systems combine multiple strategies:
  - **Segmented LRU:** Divides cache into multiple segments with different policies
  - **Multi-Queue:** Uses multiple queues with different priorities and policies
  - **W-TinyLFU:** Combines a small window cache (for bursty access) with a frequency-based main cache

## Metrics for Evaluating Cache Effectiveness
- **Hit Ratio:** Percentage of requests served from cache
- **Latency:** Time to retrieve items from cache
- **Throughput:** Number of requests served per time unit
- **Space Amplification:** Extra memory used by the caching mechanism beyond the cached data itself
- **CPU Utilization:** Processing overhead of the caching algorithm

# Executive Summary
- **FIFO:** Ideal for simple, predictable systems where recency trumps frequency, but lacks adaptability.
- **LIFO:** Best for temporary stacks or navigation, though it ignores usage patterns entirely.
- **LRU:** Versatile for general caching with temporal locality, though vulnerable to scan pollution.
- **LFU:** Excels in stable, frequency-driven workloads but struggles with dynamic patterns.
- **Random:** Great for large-scale simplicity, yet lacks intelligence in evictions.
- **Time-Based:** Perfect for expiring data like tokens, requiring periodic cleanup.
- **Size-Based:** Optimizes memory-constrained caches but adds size-tracking overhead.
- **Priority-Based:** Suits business-critical needs with manual tuning challenges.
- **TinyLFU/W-TinyLFU:** Balances recency and frequency with low overhead, ideal for modern caches.
- **ARC:** Dynamically adapts to workloads but incurs higher complexity.
- **Clock/Second Chance:** Lightweight LRU approximation, less precise but efficient.

# Definitions & Terminology
- **Cache:** Temporary storage to speed up data access.
- **Hit/Miss:** Successful/failed retrieval from cache.
- **TTL:** Time-to-live, expiration duration for cache entries.
- **Cold/Warm Cache:** Initially empty vs. pre-warmed cache state.
- **Eviction:** Removing items from cache when full.
- **Recency vs Frequency:** Prioritizing recently used vs. often used items.

# When to Use Which Strategy
- **Read-heavy:** LRU, TinyLFU.
- **Write-heavy:** FIFO.
- **Scan-prone:** LFU, ARC, TinyLFU.
- **Temporal locality:** LRU, Clock.
- **Frequency-driven:** LFU, W-TinyLFU.
- **Expiring data:** Time-Based.
- **Memory-constrained:** Size-Based.
- **Business-critical:** Priority-Based.

# Metrics & Evaluation
- **Hit Ratio:** % of cache hits.
- **Miss Penalty:** Delay on cache miss.
- **Latency:** Retrieval time.
- **Throughput:** Requests per unit time.
- **Memory Overhead:** Extra space used.
- **CPU Cost:** Processing overhead.

# Complexity & Resource Table
| Strategy         | Space Overhead       | Time Complexity       | Metadata Overhead     |
|------------------|----------------------|-----------------------|-----------------------|
| FIFO             | Low (Queue: O(n))    | O(1) all             | Hash set              |
| LIFO             | Low (Stack: O(n))    | O(1) all             | Hash set              |
| LRU              | Medium (Doubly list) | O(1) all             | HashMap, list         |
| LFU              | High (Counters)      | O(log n) or O(1)     | Frequency counters    |
| Random           | Very Low (Hash)      | O(1) all             | Hash set              |
| Time-Based       | Medium (Timestamps)  | O(log n) add/remove  | Sorted index          |
| Size-Based       | Variable (Size)      | O(log n) add/remove  | Sorted size index     |
| Priority-Based   | Medium-High (Priority) | O(log n) add/remove | Sorted priority index |
| TinyLFU          | Medium (Sketches)    | O(1) all             | Window, sketches      |
| ARC              | High (Four lists)    | O(1) all             | Multiple lists        |
| Clock            | Low (Bit per entry)  | O(1) all             | Circular buffer       |

# Worked Examples
- **LRU (Cache size 3):** [1, 2, 3, 2, 4] → Evict 1, cache [2, 3, 4].
- **LFU (Cache size 3):** [1, 2, 3, 2, 1] → Evict 3 (freq 1), cache [1(2), 2(2)].
- **TinyLFU (Cache size 3):** [1, 2, 3, 2, 1] → Window tracks recency, evict least frequent.
- **ARC (Cache size 3):** [1, 2, 3, 2, 4] → Adapts, evicts based on recency/frequency balance.
- **Clock (Cache size 3):** [1, 2, 3, 2, 4] → Second chance bit, evict unmarked 1.

# Implementation Notes & Pitfalls
- **Common Bugs:** Off-by-one errors, incorrect eviction logic.
- **Concurrency:** Use locks or atomic operations.
- **Cost of Updates:** LRU moves on access, LFU updates counters.
- **Scan Pollution:** Impacts LRU, mitigated by TinyLFU/ARC.
- **Starvation:** Priority-Based may neglect low-priority items.

# Variants & Hybrids
- **Segmented LRU:** Multiple LRU segments for priorities.
- **W-TinyLFU:** Window + frequency cache.
- **CLOCK-Pro:** Enhanced Clock with frequency.
- **Priority+LRU:** Combines business logic with recency.

# Tuning Guidelines
- **Parameters:** Cache size, window size (TinyLFU), TTL (Time-Based).
- **Measurement:** Monitor hit ratio, latency post-tune.

# Benchmarks & Testing
- **Workloads:** Read-heavy, scan-heavy traces.
- **Tools:** Cachegrind, synthetic datasets.
- **Metrics:** Hit ratio, throughput.

# Interview Questions
1. What’s the difference between LRU and LFU? (LRU: recency, LFU: frequency)
2. How does FIFO handle cache pollution? (Poorly, no adaptation)
3. Explain ARC’s adaptive nature. (Balances recency/frequency dynamically)
4. When would you use Time-Based eviction? (Expiring data like tokens)
5. What’s a drawback of Random eviction? (Unpredictable evictions)
6. How does Clock approximate LRU? (Second chance bit)
7. What’s TinyLFU’s advantage? (Scan resistance, low overhead)
8. How do you handle concurrency in LRU? (Locks or CAS)
9. What metrics evaluate cache performance? (Hit ratio, latency)
10. Why might Priority-Based be complex? (Manual tuning)

# Coding Exercises
1. **Implement LRU (Easy):** Use HashMap + DoublyLinkedList, hint: track head/tail.
2. **LFU Sketch (Medium):** Use frequency counter, hint: heap or map.
3. **CLOCK (Hard):** Circular buffer with bit, hint: rotate pointer.

# Real-world Case Studies
- **CDN:** LFU for static content.
- **DB Buffer:** LRU for temporal locality.
- **Session Store:** Time-Based for expiration.
- **Auth Tokens:** Time-Based for compliance.

# Migration & Rollout Considerations
- **Carrying:** Test on subset of traffic.
- **Metrics:** Hit ratio, latency drops.
- **Fallback:** Revert to old strategy if issues.

# Reference Implementations & Libraries
- **Caffeine:** Java, modern LRU/LFU.
- **Guava:** Java caching utilities.
- **Redis:** Configurable eviction policies.

# Glossary & Further Reading
- **Papers:** LRU paper (1976), ARC (2003).
- **Blogs:** HighScalability on caching.
- **Books:** "Cache and Memory Hierarchy Design".
