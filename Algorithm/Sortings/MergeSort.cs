using System;

namespace Algorithm
{
    /*
        Algorithm overview: Merge Sort is a divide-and-conquer sorting algorithm that recursively divides the array into two halves,
        sorts each half, and then merges the sorted halves back together. It guarantees O(n log n) performance in all cases by
        systematically breaking down the problem into smaller subproblems until they are trivially sorted (single elements),
        then combining them in sorted order.
        Why is it called "Merge": The algorithm's key operation is "merging" two sorted subarrays into a single sorted array.

        Characteristics:
        - Comparison-based: Uses the IComparable<T> interface to compare elements during the merge step.
        - Stable: Preserves the relative order of equal elements, making it ideal for multi-key sorting.
        - Not in-place: Requires additional memory proportional to the array size (O(n) space complexity).
        - Recursive: Uses a divide-and-conquer approach, breaking the problem into smaller subproblems.
        - Deterministic: Produces the same output for the same input with predictable O(n log n) performance.
        - Non-adaptive: Performance is consistent regardless of initial array order (always O(n log n)).
        - Parallelizable: Recursive nature allows for parallel processing of independent subtrees.

        Time complexity:
        - Best case: O(n log n) – even for sorted arrays, still performs all divisions and merges.
        - Average case: O(n log n) – consistent performance across all input distributions.
        - Worst case: O(n log n) – guaranteed performance, making it reliable for large datasets.

        Space complexity: O(n) – requires additional memory for temporary arrays during the merge operation.
        - Can be optimized to O(log n) for in-place variants, but with increased complexity.

        When to use:
        - Large datasets where guaranteed O(n log n) performance is required.
        - Scenarios requiring stable sorting (e.g., sorting records by multiple keys).
        - External sorting (e.g., sorting data that doesn't fit in memory, like database operations).
        - Linked list sorting, where Merge Sort's sequential access pattern is efficient.
        - Parallel processing environments, as the divide-and-conquer approach is naturally parallelizable.
        - When worst-case performance matters (e.g., real-time systems, critical applications).

        Compare to QuickSort and Heap Sort:
        - Unlike QuickSort (unstable, O(n^2) worst case), Merge Sort is stable with guaranteed O(n log n) performance.
        - Compared to Heap Sort (unstable, in-place), Merge Sort is stable but requires O(n) extra space.
        - QuickSort is often faster in practice due to better cache locality, but Merge Sort is more predictable.
        - Merge Sort is preferred when stability is required and extra memory is available.

        Advantages:
        - Guaranteed O(n log n) time complexity in all cases, providing predictable performance.
        - Stable sorting: Preserves the relative order of equal elements.
        - Excellent for linked lists due to sequential access pattern (no random access needed).
        - Naturally parallelizable, making it suitable for multi-core processors.
        - Efficient for external sorting (sorting data larger than available memory).
        - Performs well on arrays of any size, unlike insertion sort which degrades on large datasets.

        Disadvantages:
        - Requires O(n) additional space for temporary arrays, which can be prohibitive for memory-constrained systems.
        - Slower than QuickSort in practice for in-memory sorting due to more data movement and worse cache performance.
        - Not adaptive: Doesn't take advantage of partially sorted arrays (always O(n log n)).
        - Recursive approach may cause stack overflow for very large arrays if not implemented iteratively.
        - Overhead from array copying during merge operations can impact performance.

        Use Case Examples:
        - Sorting large datasets in databases where stability and predictable performance are required.
        - External merge sort for sorting files larger than available RAM (e.g., log file processing).
        - Sorting linked lists efficiently due to sequential access pattern.
        - Multi-threaded applications where independent subarrays can be sorted in parallel.
        - Sorting records by multiple keys where stability is crucial (e.g., sorting employees by department, then by name).
        - Real-time systems requiring guaranteed worst-case performance.

        Common Pitfalls:
        - Not handling null or empty arrays, leading to NullReferenceException or stack overflow.
        - Incorrect calculation of midpoint, especially for very large arrays (use mid = left + (right - left) / 2 to avoid overflow).
        - Memory leaks or excessive allocations when creating temporary arrays in each merge operation.
        - Incorrect merge logic, leading to out-of-bounds errors or unsorted results.
        - Not considering stack depth for recursive implementation, which can cause stack overflow.
        - Forgetting to handle base cases (single element or empty subarrays).

        Interview Tips:
        - Emphasize its guaranteed O(n log n) performance and stability, especially compared to QuickSort.
        - Explain the divide-and-conquer approach: divide array in half, recursively sort, then merge.
        - Walk through an example array (e.g., [38, 27, 43, 3, 9, 82, 10]) showing the division and merge steps.
        - Discuss space complexity trade-offs: O(n) extra space vs. guaranteed performance.
        - Mention its use in external sorting and suitability for linked lists.
        - If asked to optimize, discuss bottom-up (iterative) Merge Sort to avoid recursion overhead.
        - Explain how to maintain stability during merge: when elements are equal, take from the left subarray first.
        - Be ready to compare with QuickSort (faster average case but unstable) and Heap Sort (in-place but unstable).
        - Mention parallelization opportunities in the divide phase for multi-core systems.
    */
    public class MergeSort<T> : SortingBase<T> where T : IComparable<T>
    {
        // Sorts the array in ascending order using the merge sort algorithm.
        public override void Sort(T[] array)
        {
            // Early exit for null, empty, or single-element arrays (no sorting needed).
            if (array == null || array.Length < 2)
            {
                return;
            }

            // Start the recursive merge sort process.
            MergeSortRecursive(array, 0, array.Length - 1);
        }

        // Recursively divides the array and sorts the subarrays.
        private void MergeSortRecursive(T[] array, int left, int right)
        {
            // Base case: If left >= right, the subarray has 0 or 1 element and is already sorted.
            if (left >= right)
            {
                return;
            }

            // STEP 1: Divide - Find the middle point to divide the array into two halves.
            // Use left + (right - left) / 2 to avoid potential integer overflow with (left + right) / 2.
            int mid = left + (right - left) / 2;

            // STEP 2: Conquer - Recursively sort the left half [left...mid].
            MergeSortRecursive(array, left, mid);

            // STEP 3: Conquer - Recursively sort the right half [mid+1...right].
            MergeSortRecursive(array, mid + 1, right);

            // STEP 4: Combine - Merge the two sorted halves.
            Merge(array, left, mid, right);

            // Visual representation for array [38, 27, 43, 3, 9, 82, 10]
            // Division phase (top-down):
            // [38, 27, 43, 3, 9, 82, 10]
            //        /            \
            // [38, 27, 43, 3]  [9, 82, 10]
            //    /      \         /     \
            // [38, 27] [43, 3] [9, 82] [10]
            //   /  \    /  \    /  \     |
            // [38][27][43][3] [9][82] [10]
            //
            // Merge phase (bottom-up):
            // [27, 38] [3, 43] [9, 82] [10]
            //     \      /        \     /
            //  [3, 27, 38, 43]  [9, 10, 82]
            //         \              /
            //    [3, 9, 10, 27, 38, 43, 82]
        }

        // Merges two sorted subarrays: array[left...mid] and array[mid+1...right].
        private void Merge(T[] array, int left, int mid, int right)
        {
            // Calculate sizes of the two subarrays to be merged.
            int leftSize = mid - left + 1;
            int rightSize = right - mid;

            // Create temporary arrays to hold the left and right subarrays.
            T[] leftArray = new T[leftSize];
            T[] rightArray = new T[rightSize];

            // Copy data to temporary arrays.
            // Left subarray: array[left...mid]
            for (int i = 0; i < leftSize; i++)
            {
                leftArray[i] = array[left + i];
            }

            // Right subarray: array[mid+1...right]
            for (int j = 0; j < rightSize; j++)
            {
                rightArray[j] = array[mid + 1 + j];
            }

            // MERGE PROCESS: Combine the two sorted subarrays back into the original array.
            int leftIndex = 0;   // Index for leftArray
            int rightIndex = 0;  // Index for rightArray
            int mergedIndex = left; // Index for merged array (starts at left)

            // Compare elements from leftArray and rightArray, placing the smaller one into the original array.
            while (leftIndex < leftSize && rightIndex < rightSize)
            {
                // STABILITY NOTE: Using <= ensures that when elements are equal,
                // we take from the left subarray first, preserving the original relative order.
                if (leftArray[leftIndex].CompareTo(rightArray[rightIndex]) <= 0)
                {
                    array[mergedIndex] = leftArray[leftIndex];
                    leftIndex++;
                }
                else
                {
                    array[mergedIndex] = rightArray[rightIndex];
                    rightIndex++;
                }
                mergedIndex++;
            }

            // Copy any remaining elements from leftArray (if any).
            // This happens when the right subarray is exhausted first.
            while (leftIndex < leftSize)
            {
                array[mergedIndex] = leftArray[leftIndex];
                leftIndex++;
                mergedIndex++;
            }

            // Copy any remaining elements from rightArray (if any).
            // This happens when the left subarray is exhausted first.
            while (rightIndex < rightSize)
            {
                array[mergedIndex] = rightArray[rightIndex];
                rightIndex++;
                mergedIndex++;
            }

            // Example merge: leftArray = [27, 38], rightArray = [3, 43]
            // Step 1: Compare 27 and 3  → Take 3  → [3, ?, ?, ?]
            // Step 2: Compare 27 and 43 → Take 27 → [3, 27, ?, ?]
            // Step 3: Compare 38 and 43 → Take 38 → [3, 27, 38, ?]
            // Step 4: rightArray exhausted, copy 43 → [3, 27, 38, 43]
        }
    }
}
