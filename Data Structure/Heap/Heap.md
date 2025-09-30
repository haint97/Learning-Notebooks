# Theoretical Foundation

## Core Concepts
A heap is a specialized tree-based data structure that satisfies the heap property:
- **Min Heap:** Parent node ≤ all children (smallest element at root)
- **Max Heap:** Parent node ≥ all children (largest element at root)

### Key Properties:
- **Complete Binary Tree:** All levels filled except possibly the last, which fills left to right
- **Heap Property:** Maintained throughout all operations
- **Array Representation:** Most efficient implementation using a single array

## Mathematical Foundations
For a node at index `i` in a 0-based array:
- **Parent:** `(i - 1) / 2`
- **Left Child:** `2 * i + 1`
- **Right Child:** `2 * i + 2`
- **Non-Leaf:** `(n / 2) - 1` where `n` is array length

## Visual Representation
### Min Heap Example:
1

/\

3   6

/   \   /     \

5  9 8  10

**Array:** [1, 3, 6, 5, 9, 8, 10]

**Index:** [0, 1, 2, 3, 4, 5, 6]

## Memory Layout:
- **Contiguous array storage (cache-friendly)**
- **No pointer overhead like linked structures**
- **Implicit parent-child relationships via indices**

## Problem Context & Real-World Applications
### What Problems Do Heaps Solve?
1. **Priority-based processing** - OS task scheduling, event handling
2. **Top-K problems** - Finding largest/smallest K elements
3. **Graph algorithms** - Dijkstra's shortest path, Prim's MST
4. **Streaming** - Median maintenance, running statistics
5. **Huffman coding** - Data compression algorithms

### When to Use Heaps:
- Need quick access to min/max element
- Frequently inserting and removing priority-based items
- Implementing priority queues
- K-way merge operations
- Real-time systems requiring predictable performance

## Trade-offs Analysis

| Operation         | Time Complexity | Space Complexity |
|-------------------|-----------------|------------------|
| Insert            | O(log n)        | O(1)             |
| Extract Min/Max   | O(log n)        | O(1)             |
| Peek Min/Max      | O(1)            | O(1)             |
| Delete            | O(log n)        | O(1)             |
| Build Heap        | O(n)            | O(1)             |
| Heapify           | O(log n)        | O(1)             |

## Advantages:
- Guaranteed O(log n) for insertion/deletion
- Fast access to min/max element
- Space efficient (no pointer overhead)
- Cache-friendly due to array representation
- Can be built in O(n) time from unordered array

## Limitations:
- **No efficient searching for arbitrary elements**
- **Not suitable for frequent arbitrary deletions**
- **Requires resizing for dynamic growth**

## Comparison with Alternatives:

| Data Structure | Insert       | Delete       | Find Min/Max | Search       |
|----------------|--------------|--------------|--------------|--------------|
| Heap           | O(log n)     | O(log n)     | O(1)         | O(n)         |
| Sorted Array   | O(n)         | O(n)         | O(1)         | O(log n)     |
| BST            | O(log n)     | O(log n)     | O(log n)     | O(log n)     |
| Hash Table     | O(1)         | O(1)         | O(n)         | O(1)         |
