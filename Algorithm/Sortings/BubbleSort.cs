
/*
    Algorithm overview: Bubble Sort is a simple comparison-based sorting algorithm that repeatedly steps through the list,
    compares adjacent elements, and swaps them if they are in the wrong order (e.g., for ascending order, if the left element
    is greater than the right). This process continues until no more swaps are needed, indicating the list is sorted.
    Why is it called "Bubble": Smaller elements "bubble" to the top (start) of the list, while larger elements "sink" to the bottom (end).

    Characteristics:
    - Comparison-based: Relies on comparing elements using the IComparable<T> interface.
    - Stable: Preserves the relative order of equal elements, making it suitable for multi-key sorting.
    - In-place: Requires no additional data structures, only a constant amount of extra memory (O(1)).
    - Non-adaptive: Does not take advantage of existing order in the input, though the swapped flag optimization helps.
    - Iterative: Uses nested loops rather than recursion, making it simple but less efficient for large datasets.
    - Deterministic: Always produces the same output for the same input, with predictable performance.

    Time complexity:
    - Best case: O(n) when the array is already sorted (due to the swapped flag optimization).
    - Average case: O(n^2) for randomly ordered arrays, as it must compare and potentially swap elements in multiple passes.
    - Worst case: O(n^2) when the array is sorted in reverse order, requiring the maximum number of comparisons and swaps.

    Space complexity: O(1) as it is an in-place algorithm, using only a constant amount of extra memory (for the temp variable in Swap).

    When to use:
    - Small datasets (e.g., fewer than 100 elements) where simplicity is preferred over performance.
    - Educational purposes or teaching sorting concepts due to its straightforward implementation.
    - Scenarios where memory usage must be minimal, as it requires no additional data structures.
    - Systems with expensive write operations (e.g., flash memory or networked databases), as it performs at most O(n) swaps.
    - When stability is required (preserving the relative order of equal elements).
    - Debugging or verifying sorted output in systems where a simple, reliable algorithm is needed.
    - Embedded systems or resource-constrained environments where memory and computational power are limited.

    Advantages:
    - Simple to understand and implement, making it ideal for beginners or quick prototyping.
    - In-place sorting: No additional memory allocation is needed beyond the input array.
    - Stable sorting: Maintains the relative order of equal elements, which is crucial in some applications (e.g., sorting records by multiple keys).
    - Optimized best-case performance (O(n)) with the swapped flag, making it efficient for nearly sorted arrays.
    - Minimal code footprint, suitable for systems with limited storage.

    Disadvantages:
    - Poor performance for large datasets due to O(n^2) time complexity in average and worst cases.
    - Inefficient compared to advanced algorithms like QuickSort or MergeSort for most practical applications.
    - Not suitable for modern high-performance systems handling large or complex datasets.
    - Requires multiple passes even for partially sorted arrays, unlike adaptive algorithms like Insertion Sort.

    Use Case Examples:
    - Sorting a small list of student grades in a classroom application where simplicity is prioritized.
    - Sorting a small array of sensor readings in an IoT device with limited memory and processing power.
    - Educational tools or coding interviews to demonstrate understanding of sorting mechanics.
    - Sorting data in a database with expensive write operations, where minimizing swaps is beneficial.
    - Quick sorting tasks in scripts or prototypes where performance is not critical.

    Common Pitfalls:
    - Forgetting the swapped flag optimization, which can degrade best-case performance to O(n^2).
    - Not checking for null or empty arrays, leading to potential NullReferenceExceptions or unnecessary iterations.
    - Assuming Bubble Sort is efficient for large datasets, which can lead to performance issues.
    - Incorrect loop bounds (e.g., not reducing the inner loop range), causing unnecessary comparisons of already sorted elements.
    - Overlooking stability requirements in applications where it matters, though Bubble Sort is naturally stable.

    Interview tips:
    - Highlight the swapped flag optimization to demonstrate understanding of performance improvements (O(n) best case).
    - Explain why Bubble Sort is rarely used in production for large datasets, comparing it to faster algorithms like QuickSort (O(n log n)).
    - Discuss stability and why it matters in scenarios like multi-key sorting (e.g., sorting by name then age).
    - Mention real-world use cases, such as in resource-constrained environments or educational contexts.
    - Be prepared to walk through the algorithm step-by-step with an example array (e.g., [5, 2, 8, 1, 9]).
    - If asked to optimize, suggest checking for early termination or combining with other algorithms for specific cases.
    - Emphasize the algorithm’s simplicity and minimal memory footprint as key selling points for niche applications.
    - Be ready to discuss trade-offs, such as when to choose Bubble Sort over Insertion Sort or Selection Sort.
*/
public class BubbleSort<T> : SortingBase<T> where T : IComparable<T>
{
    // Implements bubble sort algorithm to sort the array in ascending order.
    public override void Sort(T[] array)
    {
        // Early exit for null, empty, or single-element arrays (no sorting needed).
        if (array == null || array.Length < 2)
        {
            return;
        }

        int n = array.Length;  // Cache length for efficiency.

        // Outer loop: Controls the number of passes. Each pass places the largest unsorted element at the end.
        for (int i = 0; i < n - 1; i++)
        {
            bool swapped = false;  // Tracks swaps to optimize for already sorted arrays.

            // Inner loop: Compares adjacent elements in the unsorted portion of the array.
            // After each pass, the largest element in the unsorted portion "bubbles" to its correct position.
            for (int j = 0; j < n - i - 1; j++)
            {
                // Compare adjacent elements; swap if they are in the wrong order (for ascending sort).
                if (array[j].CompareTo(array[j + 1]) > 0)
                {
                    Swap(array, j, j + 1);  // Perform swap using base class method.
                    swapped = true;         // Indicate a swap occurred.
                }
            }

            /*
                EARLY TERMINATION OPTIMIZATION:
                If no swaps occured during this entire pass, it means all adjacent elements are already in correct order, so the entire array is sorted and we can exit early
            */
            if (!swapped)
            {
                // This is optimization is curcial for interview discussions as it shows understanding of algorithm efficiency
                break;
            }
        }
    }
}
