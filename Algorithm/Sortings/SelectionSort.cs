using System;

namespace Algorithm
{
    /*
        Algorithm overview: Selection Sort is a comparison-based sorting algorithm that divides the array into a sorted and an unsorted portion.
        In each iteration, it finds the minimum element in the unsorted portion and swaps it with the first element of the unsorted portion,
        effectively growing the sorted portion by one element. This process repeats until the entire array is sorted.
        Why is it called "Selection": The algorithm repeatedly "selects" the smallest (or largest, for descending order) element from the unsorted portion.

        Characteristics:
        - Comparison-based: Uses the IComparable<T> interface to compare elements.
        - Unstable: Does not preserve the relative order of equal elements, as swaps may reorder them.
        - In-place: Requires no additional data structures, only a constant amount of extra memory (O(1)).
        - Non-adaptive: Performs the same number of comparisons regardless of the input's initial order.
        - Iterative: Uses nested loops, avoiding recursion, which keeps it simple but less efficient for large datasets.
        - Deterministic: Produces the same output for the same input with predictable performance.
        - Make fewer swaps than bubble sort (at most n-1 swaps)
        - Good for situations where memory write is costly
        - Performance is not affected by initial order of elements
        Time complexity:
        - Best case: O(n^2) – always scans the entire unsorted portion to find the minimum.
        - Average case: O(n^2) – for randomly ordered arrays, requires full scans and comparisons.
        - Worst case: O(n^2) – same as average case, as the algorithm doesn't benefit from input order.

        Space complexity: O(1) – in-place algorithm, using only a constant amount of extra memory (for temporary variables in Swap).

        When to use:
        - Small datasets (e.g., fewer than 50-100 elements) where simplicity is more important than performance.
        - Resource-constrained environments (e.g., embedded systems) where memory usage must be minimized.
        - Scenarios where minimizing swaps is critical (e.g., expensive write operations like flash memory or database updates), as it performs at most O(n) swaps.
        - Educational purposes to teach sorting concepts due to its intuitive "select the minimum" approach.
        - Prototyping or quick scripts where performance is not a priority and code simplicity is valued.
        - When memory write operations are expensive
        - Minimize memory write important (only n swaps maximum)

        Compare to Bubble sort:
        - Selection sort makes fewer swaps O(n) and O(n*n)
        - Selection sort has no best case optimization like bubble sort

        Advantages:
        - Simple to implement and understand, making it ideal for beginners or quick prototyping.
        - In-place sorting: No need for additional memory allocation beyond the input array.
        - Minimal swaps: Performs at most n-1 swaps, which is beneficial when swapping is expensive (e.g., large objects or network operations).
        - Predictable performance: Always O(n^2) comparisons, regardless of input, which can be useful in systems requiring consistent timing.
        - Small code footprint, suitable for systems with limited storage.

        Disadvantages:
        - Inefficient for large datasets due to O(n^2) time complexity in all cases.
        - Unstable, which can be problematic for applications requiring preservation of equal elements' relative order (e.g., multi-key sorting).
        - Non-adaptive, so it doesn’t benefit from partially sorted inputs, unlike Insertion Sort.
        - Outperformed by more advanced algorithms like QuickSort or MergeSort in most practical scenarios.
        - Requires full scans of the unsorted portion, even if the array is nearly sorted.

        Use Case Examples:
        - Sorting a small array of product prices in a point-of-sale system with limited memory.
        - Ordering a short list of tasks by priority in a resource-constrained embedded device.
        - Teaching sorting algorithms in computer science courses due to its clear "select minimum" logic.
        - Sorting database records where write operations are costly, and the dataset is small.
        - Quick prototyping in scripts where sorting performance is not critical.

        Common Pitfalls:
        - Forgetting to check if the minimum index is different from the current index before swapping, leading to unnecessary swaps.
        - Not handling null or empty arrays, which can cause NullReferenceExceptions or redundant iterations.
        - Assuming Selection Sort is stable, which can lead to issues in applications requiring stability.
        - Incorrect loop bounds (e.g., scanning the entire array instead of the unsorted portion), reducing efficiency.
        - Using Selection Sort for large datasets, resulting in poor performance compared to O(n log n) algorithms.

        Interview Tips:
        - Emphasize the minimal number of swaps (O(n)) compared to Bubble Sort’s potential O(n^2) swaps, highlighting its advantage in write-heavy scenarios.
        - Explain why Selection Sort is unstable (e.g., demonstrate with an array containing equal elements and show how swaps disrupt their order).
        - Compare it to Bubble Sort (stable, adaptive) and Insertion Sort (stable, adaptive) to show trade-offs.
        - Walk through an example array (e.g., [5, 2, 8, 1, 9]) step-by-step, showing how the minimum is selected and swapped.
        - Discuss the lack of early termination, unlike Bubble Sort’s swapped flag optimization, and why this makes it less efficient for nearly sorted arrays.
        - If asked to optimize, suggest hybrid approaches (e.g., switching to Insertion Sort for small unsorted portions) or using a library sort for production code.
        - Highlight its simplicity and predictability as strengths in niche scenarios, such as embedded systems or educational contexts.
    */
    public class SelectionSort<T> : SortingBase<T> where T : IComparable<T>
    {
        // Sorts the array in ascending order using the selection sort algorithm.
        public void Sort(T[] array)
        {
            // Early exit for null, empty, or single-element arrays (no sorting needed).
            if (array == null || array.Length < 2)
            {
                return;
            }

            int n = array.Length; // Cache length for efficiency.

            // Outer loop: Iterates through each position in the array, growing the sorted portion.
            for (int i = 0; i < n - 1; i++)
            {
                // Assume the current position holds the minimum element.
                int minIndex = i;

                // Inner loop: Scan the unsorted portion (from i+1 to n-1) to find the minimum element.
                for (int j = i + 1; j < n; j++)
                {
                    // Update minIndex if a smaller element is found.
                    if (array[j].CompareTo(array[minIndex]) < 0)
                    {
                        minIndex = j;
                    }
                }

                // Swap the found minimum element with the first element of the unsorted portion, if necessary.
                if (minIndex != i)
                {
                    // Perform the swap using a temporary variable.
                    Swap(array, i, minIndex);

                    // IMPORTANT: This is where stability is lost. If we have duplicate elements, this swap might change their relative order
                }
            }
        }
    }
}
