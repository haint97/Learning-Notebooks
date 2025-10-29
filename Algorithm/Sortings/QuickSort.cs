using System;

namespace Algorithm
{
    /*
        Algorithm overview: Quick Sort is a divide-and-conquer sorting algorithm that selects a 'pivot' element and partitions
        the array into two subarrays: elements less than the pivot and elements greater than the pivot. It then recursively
        sorts the subarrays. The algorithm's efficiency comes from its in-place partitioning and average-case O(n log n) performance.
        Why is it called "Quick": Despite its O(n^2) worst case, it's typically faster than other O(n log n) algorithms in practice
        due to excellent cache locality and fewer data movements.

        Characteristics:
        - Comparison-based: Uses the IComparable<T> interface to compare elements during partitioning.
        - Unstable: Does not preserve the relative order of equal elements (can be made stable with additional complexity).
        - In-place: Requires only O(log n) extra space for recursion stack, no additional arrays needed.
        - Recursive: Uses divide-and-conquer approach with recursive partitioning.
        - Deterministic (with fixed pivot): Produces the same output for the same input when pivot selection is deterministic.
        - Adaptive (with good pivot): Can perform well on partially sorted data if pivot selection is optimized.
        - Parallelizable: Independent partitions can be sorted in parallel.

        Time complexity:
        - Best case: O(n log n) – occurs when the pivot consistently divides the array into two equal halves.
        - Average case: O(n log n) – for randomly ordered arrays with reasonably balanced partitions.
        - Worst case: O(n^2) – occurs when the pivot is always the smallest or largest element (e.g., already sorted array with poor pivot selection).

        Space complexity: O(log n) – for the recursion stack in the best/average case, O(n) in the worst case (unbalanced partitions).
        - In-place partitioning requires no additional arrays, only stack space for recursive calls.

        When to use:
        - Large datasets where average-case performance is more important than worst-case guarantees.
        - In-memory sorting where cache locality matters (QuickSort has excellent cache performance).
        - General-purpose sorting where stability is not required.
        - Systems with limited memory where in-place sorting is essential.
        - When combined with optimizations (median-of-three pivot, hybrid with Insertion Sort for small subarrays).
        - Parallel computing environments where independent partitions can be processed simultaneously.

        Compare to Merge Sort and Heap Sort:
        - Unlike Merge Sort (stable, O(n) extra space), QuickSort is unstable but uses O(log n) space and is often faster in practice.
        - Compared to Heap Sort (unstable, in-place, O(n log n) guaranteed), QuickSort has better average-case performance and cache locality.
        - QuickSort's main disadvantage is its O(n^2) worst case, which can be mitigated with randomized pivot selection or median-of-three.
        - QuickSort is preferred for general-purpose sorting when stability is not required and average performance matters.

        Advantages:
        - Excellent average-case performance: O(n log n) with good constant factors.
        - In-place sorting: Requires only O(log n) extra space for recursion, no temporary arrays.
        - Superior cache locality: Sequential access patterns lead to fewer cache misses compared to Merge Sort.
        - Naturally parallelizable: Independent partitions can be sorted concurrently.
        - Efficient for large datasets in practice, often outperforming Merge Sort and Heap Sort.
        - Can be optimized with various techniques (randomized pivot, median-of-three, hybrid with Insertion Sort).

        Disadvantages:
        - Unstable: Does not preserve the relative order of equal elements.
        - Worst-case O(n^2): Occurs with poor pivot selection (e.g., already sorted arrays with fixed pivot).
        - Recursion overhead: Deep recursion can cause stack overflow for very large arrays or worst-case scenarios.
        - Not adaptive by default: Doesn't inherently take advantage of partially sorted data.
        - Performance depends heavily on pivot selection strategy.

        Use Case Examples:
        - Sorting large datasets in applications where stability is not required (e.g., numerical data, unique IDs).
        - General-purpose sorting in standard libraries (e.g., .NET's Array.Sort uses a variant of QuickSort).
        - In-memory sorting where cache performance is critical (e.g., game engines, real-time systems).
        - Parallel sorting tasks where independent partitions can be distributed across multiple cores.
        - Hybrid algorithms combining QuickSort for large partitions and Insertion Sort for small ones (e.g., Introsort).
        - Scenarios where O(log n) space complexity is acceptable but O(n) is not.

        Common Pitfalls:
        - Poor pivot selection leading to O(n^2) worst-case performance (always choosing first/last element on sorted data).
        - Not handling duplicate elements efficiently, causing unbalanced partitions.
        - Stack overflow due to deep recursion in worst-case scenarios (can be mitigated with iterative implementation or tail recursion).
        - Incorrect partition logic leading to infinite recursion or unsorted results.
        - Not optimizing for small subarrays (switching to Insertion Sort for small partitions improves performance).
        - Assuming stability, which QuickSort does not provide by default.

        Interview Tips:
        - Emphasize its average-case O(n log n) performance and practical efficiency due to cache locality.
        - Explain the partitioning process: choose a pivot, rearrange elements so smaller are on the left, larger on the right.
        - Walk through an example array (e.g., [10, 7, 8, 9, 1, 5]) showing pivot selection and partitioning steps.
        - Discuss pivot selection strategies: fixed (first/last), randomized, median-of-three to avoid worst case.
        - Compare with Merge Sort (stable, O(n) space) and Heap Sort (guaranteed O(n log n) but slower in practice).
        - If asked to optimize, mention: switching to Insertion Sort for small subarrays, randomized pivot, iterative implementation.
        - Explain why it's unstable: equal elements can be swapped during partitioning, changing their relative order.
        - Mention Introsort (QuickSort + Heap Sort fallback) used in many standard libraries to guarantee O(n log n).
        - Be ready to implement different pivot selection strategies (Lomuto vs. Hoare partitioning schemes).
    */
    public class QuickSort<T> : SortingBase<T> where T : IComparable<T>
    {
        // Sorts the array in ascending order using the quick sort algorithm.
        public override void Sort(T[] array)
        {
            // Early exit for null, empty, or single-element arrays (no sorting needed).
            if (array == null || array.Length < 2)
            {
                return;
            }

            // Start the recursive quick sort process.
            QuickSortRecursive(array, 0, array.Length - 1);
        }

        // Recursively partitions and sorts the array.
        private void QuickSortRecursive(T[] array, int low, int high)
        {
            // Base case: If low >= high, the subarray has 0 or 1 element and is already sorted.
            if (low >= high)
            {
                return;
            }

            // STEP 1: Partition - Rearrange elements so that elements smaller than pivot are on the left,
            // and elements greater than pivot are on the right. Returns the pivot's final position.
            int pivotIndex = Partition(array, low, high);

            // STEP 2: Recursively sort the left partition [low...pivotIndex-1]
            QuickSortRecursive(array, low, pivotIndex - 1);

            // STEP 3: Recursively sort the right partition [pivotIndex+1...high]
            QuickSortRecursive(array, pivotIndex + 1, high);

            // Visual representation for array [10, 7, 8, 9, 1, 5] with pivot = last element (5)
            // Initial:           [10, 7, 8, 9, 1, 5]  pivot=5
            // After partition:   [1, 5, 8, 9, 7, 10]  pivot now at index 1
            //                     /          \
            //               [1]              [8, 9, 7, 10]
            //                                 pivot=10
            // After partition:                [8, 9, 7, 10]
            //                                  /         \
            //                            [8, 9, 7]      [10]
            //                             pivot=7
            // After partition:            [7, 9, 8]
            //                               /    \
            //                             [7]   [9, 8]
            //                                    pivot=8
            // After partition:                  [8, 9]
            //                                    /  \
            //                                  [8] [9]
            // Final sorted:      [1, 5, 7, 8, 9, 10]
        }

        // Lomuto partition scheme: Places pivot at its correct position and returns the pivot index.
        // Elements smaller than pivot are moved to the left, larger to the right.
        private int Partition(T[] array, int low, int high)
        {
            // PIVOT SELECTION: Choose the last element as the pivot (simple but can lead to O(n^2) on sorted data).
            // Alternative strategies: randomized pivot, median-of-three for better average performance.
            T pivot = array[high];

            // Index of the smaller element, indicates the position where the next smaller element should be placed.
            int i = low - 1;

            // PARTITIONING PROCESS: Iterate through the array and rearrange elements.
            for (int j = low; j < high; j++)
            {
                // If current element is smaller than or equal to pivot, place it in the "smaller" section.
                if (array[j].CompareTo(pivot) <= 0)
                {
                    i++; // Move the boundary of the smaller section.
                    Swap(array, i, j); // Swap current element with element at boundary.
                }
                // Elements greater than pivot naturally stay on the right side.
            }

            // PLACE PIVOT: Move the pivot to its correct position (between smaller and larger elements).
            Swap(array, i + 1, high);

            // Return the pivot's final position.
            return i + 1;

            // Example partition for [10, 7, 8, 9, 1, 5] with pivot=5:
            // j=0: array[0]=10 > 5, no swap               → [10, 7, 8, 9, 1, 5] i=-1
            // j=1: array[1]=7 > 5, no swap                → [10, 7, 8, 9, 1, 5] i=-1
            // j=2: array[2]=8 > 5, no swap                → [10, 7, 8, 9, 1, 5] i=-1
            // j=3: array[3]=9 > 5, no swap                → [10, 7, 8, 9, 1, 5] i=-1
            // j=4: array[4]=1 <= 5, i++, swap(0,4)        → [1, 7, 8, 9, 10, 5] i=0
            // Place pivot: swap(i+1=1, high=5)            → [1, 5, 8, 9, 10, 7]
            // Return pivotIndex=1
        }

        // Helper method to swap two elements in the array.
        private void Swap(T[] array, int i, int j)
        {
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
