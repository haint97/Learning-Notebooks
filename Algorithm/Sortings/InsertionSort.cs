using System;

namespace Algorithm
{
    /*
        Algorithm overview: Insertion Sort is a comparison-based sorting algorithm that builds a sorted array one element at a time.
        It iterates through the array, taking each element and inserting it into its correct position within the already sorted portion
        of the array. This process continues until the entire array is sorted.
        Why is it called "Insertion": The algorithm "inserts" each element into its appropriate place in the sorted portion.

        Characteristics:
        - Comparison-based: Uses the IComparable<T> interface to compare elements.
        - Stable: Preserves the relative order of equal elements, making it suitable for multi-key sorting.
        - In-place: Requires no additional data structures, only a constant amount of extra memory (O(1)).
        - Adaptive: Performs better on partially sorted arrays, with a best-case time complexity of O(n).
        - Iterative: Uses a loop to process each element, avoiding recursion for simplicity.
        - Deterministic: Produces the same output for the same input with predictable performance.

        Time complexity:
        - Best case: O(n) – occurs when the array is already sorted, requiring only comparisons and no shifts.
        - Average case: O(n^2) – for randomly ordered arrays, requires comparisons and shifts for most elements.
        - Worst case: O(n^2) – occurs when the array is sorted in reverse order, requiring maximum comparisons and shifts.

        Space complexity: O(1) – in-place algorithm, using only a constant amount of extra memory (for temporary variables).

        When to use:
        - Small datasets (e.g., fewer than 100 elements) where simplicity and stability are important.
        - Partially sorted arrays, as it performs well due to its adaptive nature (O(n) for nearly sorted data).
        - Resource-constrained environments (e.g., embedded systems) where memory usage must be minimized.
        - Educational purposes to teach sorting concepts due to its intuitive "insert into sorted portion" approach.
        - As a building block in hybrid algorithms (e.g., used in QuickSort for small partitions).
        - Scenarios requiring stable sorting, such as multi-key sorting in databases or UI applications.

        Compare to Selection Sort and Bubble Sort:
        - Unlike Selection Sort (unstable, non-adaptive), Insertion Sort is stable and adaptive, making it more efficient for partially sorted data.
        - Compared to Bubble Sort (stable, semi-adaptive), Insertion Sort typically performs fewer operations for partially sorted arrays and is more efficient in practice due to fewer swaps.
        - Insertion Sort is preferred when stability is required and the dataset is small or partially sorted.

        Advantages:
        - Simple to implement and understand, ideal for beginners or quick prototyping.
        - In-place sorting: No additional memory allocation beyond the input array.
        - Stable sorting: Preserves the relative order of equal elements, useful for multi-key sorting.
        - Adaptive: Efficient for partially sorted arrays, with O(n) best-case performance.
        - Minimal code footprint, suitable for systems with limited storage.

        Disadvantages:
        - Inefficient for large datasets due to O(n^2) time complexity in average and worst cases.
        - Outperformed by advanced algorithms like QuickSort or MergeSort for large or random datasets.
        - Requires shifting elements, which can be costly for large objects compared to algorithms with fewer data movements.

        Use Case Examples:
        - Sorting a small list of customer names in a CRM system where stability is needed for multi-key sorting.
        - Ordering sensor data in an IoT device with partially sorted inputs and limited memory.
        - Educational tools or coding interviews to demonstrate understanding of stable, adaptive sorting.
        - Sorting small arrays in real-time applications where data is often nearly sorted (e.g., updating a leaderboard).
        - As a subroutine in hybrid algorithms like TimSort or QuickSort for handling small partitions.

        Common Pitfalls:
        - Not handling null or empty arrays, leading to potential NullReferenceExceptions or redundant iterations.
        - Assuming Insertion Sort is efficient for large datasets, which can lead to performance issues.
        - Incorrect loop bounds or improper shifting logic, causing incorrect sorting or infinite loops.
        - Overlooking stability requirements, though Insertion Sort is naturally stable.
        - Forgetting to leverage its adaptive nature in scenarios with partially sorted data.

        Interview Tips:
        - Highlight its stability and adaptive nature, especially for nearly sorted arrays (O(n) best case).
        - Compare it to Bubble Sort (more swaps) and Selection Sort (unstable, non-adaptive) to show trade-offs.
        - Walk through an example array (e.g., [5, 2, 8, 1, 9]) step-by-step, showing how elements are inserted into the sorted portion.
        - Emphasize its use in hybrid algorithms (e.g., TimSort uses Insertion Sort for small runs).
        - Discuss its efficiency for small or partially sorted datasets and its minimal memory usage.
        - If asked to optimize, mention its natural optimization for nearly sorted data or suggest hybrid approaches for larger datasets.
        - Be ready to explain why it’s stable (e.g., equal elements are not swapped past each other during insertion).
    */
    public class InsertionSort<T> : SortingBase<T> where T : IComparable<T>
    {
        // Sorts the array in ascending order using the insertion sort algorithm.
        public override void Sort(T[] array)
        {
            // Early exit for null, empty, or single-element arrays (no sorting needed).
            if (array == null || array.Length < 2)
            {
                return;
            }

            int n = array.Length; // Cache length for efficiency.

            // Iterate through the array starting from the second element (index 1).
            // Loop invariant: At the start of interation i, the subarray arr[0...i-1] consists of the original elements arr[0...i-1] but in sorted order
            for (int i = 1; i < n; i++)
            {
                // STEP1: pick the current element to insert
                // Store the current element that we want to insert into the sorted portion. This is like picking up a new card from the deck

                T key = array[i];


                //STEP 2: Find the correct position for insertion
                // Start comparing from the rightmost element of the sorted portion and move leftward until we find the correct insertion point
                int j = i - 1;


                // Find ther correct position to insert key/current element
                // Shift larger elements one position to the right
                while (j >= 0 && array[j].CompareTo(key) > 0)
                {
                    // SHIFT: move the larger element one position to the right
                    // This creates space for the key/current elements to be inserted.
                    // It's l;ike moving cards to the right to make space for the new card
                    array[j + 1] = array[j]; // Shift element one position to the right.

                    // Move to the next element on the left
                    j--;

                    // Performamce note: This shifting operation is what makes insertion sort efficient on nearly sorted arrays - if elements are already in place, minial shifting occurs
                }

                //STEP 3: Place the key in its correct position.
                array[j + 1] = key;

                // Visual representation for array [5, 2, 4, 6, 1, 3]
                // Initial:  [5, 2, 4, 6, 1, 3]  – Sorted portion: [5]
                // i = 1:   [2, 5, 4, 6, 1, 3]  – Sorted portion: [2, 5]
                // i = 2:   [2, 4, 5, 6, 1, 3]  – Sorted portion: [2, 4, 5]
                // i = 3:   [2, 4, 5, 6, 1, 3]  – Sorted portion: [2, 4, 5, 6]
                // i = 4:   [1, 2, 4, 5, 6, 3]  – Sorted portion: [1, 2, 4, 5, 6]
                // i = 5:   [1, 2, 3, 4, 5, 6]  – Sorted portion: [1, 2, 3, 4, 5, 6]
                /*
                    STABILITY NOTE: By comparing with > (not >=), we ensure that equal elements
                    are not swapped, preserving their relative order and maintaining stability.
                */
            }
        }
    }
}
