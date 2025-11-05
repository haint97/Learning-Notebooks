
# Definition
Backtracking is a systematic search technique that builds candidate solutions incrementally and abandons a candidate ("backtracks") as soon as it determines the candidate cannot lead to a valid solution. It is built on recursion plus explicit state management and is commonly used for combinatorial search, constraint satisfaction, and puzzle solving.

# Key characteristics
- Explore all (or many) possible configurations by incremental construction.
- Choose → Explore → Unchoose (choose, recurse, undo choice).
- Heavy use of pruning to reduce search space.
- Finds all solutions or a single valid solution depending on stopping condition.
- Time complexity: often exponential in input size; Space: O(depth) for recursion stack + state.

# Typical pattern
1. Maintain current state (path, partial solution, visited markers, counters).
2. Check base case (complete/invalid).
3. Iterate over choices:
    - Make choice (modify state).
    - Recurse.
    - Undo choice (restore state).

# Common techniques
- Pruning: reject partial solutions early using constraints.
- Ordering/heuristics: try promising choices first.
- Memoization / DP hybrid: cache equivalent states where applicable.
- Constraint propagation: reduce available choices before recursion.

### When to use
- Enumerating combinations/permutations/subsets.
- Solving puzzles (N-Queens, Sudoku, word search).
- Backtracking + pruning outperforms brute force for constrained problems.
- Use DP/memoization when overlapping subproblems dominate.

# Simple C# template
```csharp
public class BacktrackingTemplate
{
    // THE UNIVERSAL BACKTRACKING TEMPLATE
    // This pattern applies to 90% of backtracking problems

    public static void Backtrack<T>(
        List<T> candidates,           // Available choices
        List<T> currentSolution,      // Current partial solution being built
        List<List<T>> allSolutions,   // Collection of all valid solutions
        Func<List<T>, bool> isValid,  // Function to check if solution is valid
        Func<List<T>, bool> isComplete) // Function to check if solution is complete
    {
        // BASE CASE: Check if we have a complete solution
        if (isComplete(currentSolution))
        {
            // Found a valid solution - add a COPY to results
            allSolutions.Add(new List<T>(currentSolution));
            return;
        }

        // RECURSIVE CASE: Try each candidate choice
        foreach (T candidate in candidates)
        {
            // CHOOSE: Make a choice by adding candidate to current solution
            currentSolution.Add(candidate);

            // CONSTRAINT CHECK: Only explore if current state is valid
            if (isValid(currentSolution))
            {
                // EXPLORE: Recursively search from this state
                Backtrack(candidates, currentSolution, allSolutions, isValid, isComplete);
            }

            // UNCHOOSE: Remove the choice to try other options (BACKTRACK!)
            currentSolution.RemoveAt(currentSolution.Count - 1);
        }
    }
}
```

# Visualizing Backtracking
Choose → Explore → Unchoose Visualization
```plaintext
          Start
            |
         [Choose]
            |
        +---+---+
        |       |
     Choice1  Choice2
        |       |
     [Explore][Explore]
        |       |
     +--+--+  +--+--+
     |     |  |     |
   Valid  Invalid  Valid
    |        |      |
 [Unchoose] [Unchoose]
    |        |      |
   Back    Back    Back
    |        |      |
   ...      ...    ...
```

```
State: []
  ↓
CHOOSE: Add 'A' → State: [A]
  ↓
EXPLORE: Recursively try all possibilities with [A]
  ↓
UNCHOOSE: Remove 'A' → State: []
  ↓
CHOOSE: Add 'B' → State: [B]
  ↓
EXPLORE: Recursively try all possibilities with [B]
  ↓
UNCHOOSE: Remove 'B' → State: []
  ↓
... continue with other choices
```


# Generate subsets
Problem: Given an array of distinct integers, generate all possible subsets
Example: [1,2,3] → [[], [1], [2], [3], [1,2], [1,3], [2,3], [1,2,3]]

```csharp
public class SubsetGeneration
{
    // Problem: Given an array of distinct integers, generate all possible subsets
    // Example: [1,2,3] → [[], [1], [2], [3], [1,2], [1,3], [2,3], [1,2,3]]

    public static List<List<int>> GenerateSubsets(int[] nums)
    {
        var allSubsets = new List<List<int>>();
        var currentSubset = new List<int>();

        BacktrackSubsets(nums, 0, currentSubset, allSubsets);

        return allSubsets;
    }

    private static void BacktrackSubsets(
        int[] nums,
        int startIndex,           // Current position in nums array
        List<int> currentSubset,  // Current subset being built
        List<List<int>> allSubsets) // All subsets found so far
    {
        // BASE CASE: Every state is a valid subset!
        // We add the current subset at EVERY recursive call
        allSubsets.Add(new List<int>(currentSubset));

        // RECURSIVE CASE: Try adding each remaining element
        for (int i = startIndex; i < nums.Length; i++)
        {
            // CHOOSE: Include nums[i] in the current subset
            currentSubset.Add(nums[i]);

            // EXPLORE: Recursively generate subsets that include nums[i]
            // Start from i+1 to avoid duplicates (we've already processed earlier elements)
            BacktrackSubsets(nums, i + 1, currentSubset, allSubsets);

            // UNCHOOSE: Remove nums[i] to try other possibilities
            currentSubset.RemoveAt(currentSubset.Count - 1);
        }
    }

    // Enhanced version with visualization
    public static List<List<int>> GenerateSubsetsWithTrace(int[] nums)
    {
        Console.WriteLine($"=== GENERATING SUBSETS FOR [{string.Join(",", nums)}] ===\n");

        var allSubsets = new List<List<int>>();
        var currentSubset = new List<int>();

        BacktrackSubsetsTrace(nums, 0, currentSubset, allSubsets, 0);

        Console.WriteLine($"\n=== TOTAL SUBSETS FOUND: {allSubsets.Count} ===");
        return allSubsets;
    }

    private static void BacktrackSubsetsTrace(
        int[] nums,
        int startIndex,
        List<int> currentSubset,
        List<List<int>> allSubsets,
        int depth)
    {
        string indent = new string(' ', depth * 2);

        // Add current subset to results
        Console.WriteLine($"{indent}✓ Added subset: [{string.Join(",", currentSubset)}]");
        allSubsets.Add(new List<int>(currentSubset));

        // Try each remaining element
        for (int i = startIndex; i < nums.Length; i++)
        {
            Console.WriteLine($"{indent}→ CHOOSE: Add {nums[i]} to subset");

            // Choose
            currentSubset.Add(nums[i]);

            // Explore
            BacktrackSubsetsTrace(nums, i + 1, currentSubset, allSubsets, depth + 1);

            // Unchoose
            currentSubset.RemoveAt(currentSubset.Count - 1);
            Console.WriteLine($"{indent}← UNCHOOSE: Remove {nums[i]} from subset");
        }
    }
}
```

```
BacktrackSubsets([1,2,3], 0, [])
├─ Add [] to results (subset #1)
├─ i=0: CHOOSE 1
│  ├─ BacktrackSubsets([1,2,3], 1, [1])
│  │  ├─ Add [1] to results (subset #2)
│  │  ├─ i=1: CHOOSE 2
│  │  │  ├─ BacktrackSubsets([1,2,3], 2, [1,2])
│  │  │  │  ├─ Add [1,2] to results (subset #3)
│  │  │  │  ├─ i=2: CHOOSE 3
│  │  │  │  │  ├─ BacktrackSubsets([1,2,3], 3, [1,2,3])
│  │  │  │  │  │  └─ Add [1,2,3] to results (subset #4)
│  │  │  │  │  └─ UNCHOOSE 3
│  │  │  │  └─ return
│  │  │  └─ UNCHOOSE 2
│  │  ├─ i=2: CHOOSE 3
│  │  │  ├─ BacktrackSubsets([1,2,3], 3, [1,3])
│  │  │  │  └─ Add [1,3] to results (subset #5)
│  │  │  └─ UNCHOOSE 3
│  │  └─ return
│  └─ UNCHOOSE 1
├─ i=1: CHOOSE 2
│  ├─ BacktrackSubsets([1,2,3], 2, [2])
│  │  ├─ Add [2] to results (subset #6)
│  │  ├─ i=2: CHOOSE 3
│  │  │  ├─ BacktrackSubsets([1,2,3], 3, [2,3])
│  │  │  │  └─ Add [2,3] to results (subset #7)
│  │  │  └─ UNCHOOSE 3
│  │  └─ return
│  └─ UNCHOOSE 2
└─ i=2: CHOOSE 3
   ├─ BacktrackSubsets([1,2,3], 3, [3])
   │  └─ Add [3] to results (subset #8)
   └─ UNCHOOSE 3
```



# Permutation generation
Problem: Given an array of distinct integers, generate all possible permutations
Example: [1,2,3] → [[1,2,3], [1,3,2], [2,1,3], [2,3,1], [3,1,2], [3,2,1]]

```csharp
public class PermutationGeneration
{
    // Problem: Generate all permutations of distinct integers
    // Example: [1,2,3] → [[1,2,3], [1,3,2], [2,1,3], [2,3,1], [3,1,2], [3,2,1]]

    public static List<List<int>> GeneratePermutations(int[] nums)
    {
        var allPermutations = new List<List<int>>();
        var currentPermutation = new List<int>();
        var used = new bool[nums.Length]; // Track which elements are used

        BacktrackPermutations(nums, currentPermutation, used, allPermutations);

        return allPermutations;
    }

    private static void BacktrackPermutations(
        int[] nums,
        List<int> currentPermutation,
        bool[] used,
        List<List<int>> allPermutations)
    {
        // BASE CASE: Permutation is complete when we've used all elements
        if (currentPermutation.Count == nums.Length)
        {
            allPermutations.Add(new List<int>(currentPermutation));
            return;
        }

        // RECURSIVE CASE: Try each unused element
        for (int i = 0; i < nums.Length; i++)
        {
            // Skip if already used in current permutation
            if (used[i])
                continue;

            // CHOOSE: Add nums[i] to current permutation
            currentPermutation.Add(nums[i]);
            used[i] = true; // Mark as used

            // EXPLORE: Recursively build rest of permutation
            BacktrackPermutations(nums, currentPermutation, used, allPermutations);

            // UNCHOOSE: Remove nums[i] and mark as unused
            currentPermutation.RemoveAt(currentPermutation.Count - 1);
            used[i] = false; // BACKTRACK!
        }
    }

    // Enhanced version with visualization
    public static List<List<int>> GeneratePermutationsWithTrace(int[] nums)
    {
        Console.WriteLine($"=== GENERATING PERMUTATIONS FOR [{string.Join(",", nums)}] ===\n");

        var allPermutations = new List<List<int>>();
        var currentPermutation = new List<int>();
        var used = new bool[nums.Length];

        BacktrackPermutationsTrace(nums, currentPermutation, used, allPermutations, 0);

        Console.WriteLine($"\n=== TOTAL PERMUTATIONS: {allPermutations.Count} (Expected: {Factorial(nums.Length)}) ===");
        return allPermutations;
    }

    private static void BacktrackPermutationsTrace(
        int[] nums,
        List<int> currentPermutation,
        bool[] used,
        List<List<int>> allPermutations,
        int depth)
    {
        string indent = new string(' ', depth * 2);

        // Check if permutation is complete
        if (currentPermutation.Count == nums.Length)
        {
            Console.WriteLine($"{indent}✓ COMPLETE PERMUTATION: [{string.Join(",", currentPermutation)}]");
            allPermutations.Add(new List<int>(currentPermutation));
            return;
        }

        Console.WriteLine($"{indent}Current: [{string.Join(",", currentPermutation)}], Used: [{string.Join(",", GetUsedElements(nums, used))}]");

        // Try each unused element
        for (int i = 0; i < nums.Length; i++)
        {
            if (used[i])
            {
                Console.WriteLine($"{indent}  Skip {nums[i]} (already used)");
                continue;
            }

            Console.WriteLine($"{indent}→ CHOOSE: {nums[i]}");

            // Choose
            currentPermutation.Add(nums[i]);
            used[i] = true;

            // Explore
            BacktrackPermutationsTrace(nums, currentPermutation, used, allPermutations, depth + 1);

            // Unchoose
            currentPermutation.RemoveAt(currentPermutation.Count - 1);
            used[i] = false;

            Console.WriteLine($"{indent}← UNCHOOSE: {nums[i]}");
        }
    }

    private static List<int> GetUsedElements(int[] nums, bool[] used)
    {
        var result = new List<int>();
        for (int i = 0; i < used.Length; i++)
        {
            if (used[i])
                result.Add(nums[i]);
        }
        return result;
    }

    private static int Factorial(int n)
    {
        if (n <= 1) return 1;
        return n * Factorial(n - 1);
    }
}

```
```
                              []
                    /         |         \
                   /          |          \
                  1           2           3
                 / \         / \         / \
                /   \       /   \       /   \
               2     3     1     3     1     2
               |     |     |     |     |     |
               3     2     3     1     2     1
               |     |     |     |     |     |
             [1,2,3][1,3,2][2,1,3][2,3,1][3,1,2][3,2,1]

Key Differences from Subsets:
• Every path must reach depth n (use all elements)
• Need 'used' array to track which elements are in current path
• Total permutations = n! = 6 for n=3
• Each level explores ALL unused elements (not just forward)

```

# Combinations (Fixed Size)
Combinations are like subsets but with a fixed size constraint.
```csharp
public class CombinationGeneration
{
    // Problem: Generate all combinations of k numbers chosen from 1 to n
    // Example: n=4, k=2 → [[1,2], [1,3], [1,4], [2,3], [2,4], [3,4]]

    public static List<List<int>> GenerateCombinations(int n, int k)
    {
        var allCombinations = new List<List<int>>();
        var currentCombination = new List<int>();

        BacktrackCombinations(n, k, 1, currentCombination, allCombinations);

        return allCombinations;
    }

    private static void BacktrackCombinations(
        int n,                          // Range: 1 to n
        int k,                          // Target size
        int start,                      // Current starting number
        List<int> currentCombination,   // Current combination being built
        List<List<int>> allCombinations) // All valid combinations
    {
        // BASE CASE: Combination is complete when size equals k
        if (currentCombination.Count == k)
        {
            allCombinations.Add(new List<int>(currentCombination));
            return;
        }

        // PRUNING OPTIMIZATION: Stop if we can't reach size k
        // Remaining elements needed: k - currentCombination.Count
        // Available elements: n - start + 1
        int needed = k - currentCombination.Count;
        int available = n - start + 1;

        if (available < needed)
            return; // Not enough elements left to complete combination

        // RECURSIVE CASE: Try each number from start to n
        for (int i = start; i <= n; i++)
        {
            // CHOOSE: Add number i to current combination
            currentCombination.Add(i);

            // EXPLORE: Recursively build rest of combination
            // Start from i+1 to avoid duplicates (combinations don't care about order)
            BacktrackCombinations(n, k, i + 1, currentCombination, allCombinations);

            // UNCHOOSE: Remove number i to try other options
            currentCombination.RemoveAt(currentCombination.Count - 1);
        }
    }

    // Enhanced version with visualization
    public static List<List<int>> GenerateCombinationsWithTrace(int n, int k)
    {
        Console.WriteLine($"=== GENERATING COMBINATIONS C({n},{k}) ===\n");

        var allCombinations = new List<List<int>>();
        var currentCombination = new List<int>();

        BacktrackCombinationsTrace(n, k, 1, currentCombination, allCombinations, 0);

        Console.WriteLine($"\n=== TOTAL COMBINATIONS: {allCombinations.Count} ===");
        Console.WriteLine($"Expected: C({n},{k}) = {CalculateCombinations(n, k)}");

        return allCombinations;
    }

    private static void BacktrackCombinationsTrace(
        int n, int k, int start,
        List<int> currentCombination,
        List<List<int>> allCombinations,
        int depth)
    {
        string indent = new string(' ', depth * 2);

        // Check if combination is complete
        if (currentCombination.Count == k)
        {
            Console.WriteLine($"{indent}✓ COMPLETE COMBINATION: [{string.Join(",", currentCombination)}]");
            allCombinations.Add(new List<int>(currentCombination));
            return;
        }

        // Pruning check
        int needed = k - currentCombination.Count;
        int available = n - start + 1;

        if (available < needed)
        {
            Console.WriteLine($"{indent}✗ PRUNE: Need {needed} more, only {available} available");
            return;
        }

        Console.WriteLine($"{indent}Current: [{string.Join(",", currentCombination)}], Need: {needed}, Available: {available}");

        // Try each number from start to n
        for (int i = start; i <= n; i++)
        {
            Console.WriteLine($"{indent}→ CHOOSE: {i}");

            // Choose
            currentCombination.Add(i);

            // Explore
            BacktrackCombinationsTrace(n, k, i + 1, currentCombination, allCombinations, depth + 1);

            // Unchoose
            currentCombination.RemoveAt(currentCombination.Count - 1);

            Console.WriteLine($"{indent}← UNCHOOSE: {i}");
        }
    }

    private static long CalculateCombinations(int n, int k)
    {
        if (k > n) return 0;
        if (k == 0 || k == n) return 1;

        // C(n,k) = n! / (k! * (n-k)!)
        // Optimized: C(n,k) = C(n,k-1) * (n-k+1) / k
        long result = 1;
        for (int i = 0; i < k; i++)
        {
            result = result * (n - i) / (i + 1);
        }
        return result;
    }
}
```

```
                              []
                    /    /    |    \
                   /    /     |     \
                  1    2      3      4
                 /|\   |\     |
                / | \  | \    |
               2  3  4 3  4   4
               |  |  | |  |   |
              [1,2][1,3][1,4][2,3][2,4][3,4]

Key Observations:
• Each path must reach depth k=2
• Numbers increase left to right (startIndex prevents [2,1])
• Total valid combinations = C(4,2) = 6
• Pruning occurs when available < needed
```

Pruning Optimization Visualization:
```
When building combination from [1,2,...,n=5] with k=3:

Current: [1,4], Need: 1 more element
Available numbers: 5 (just one number left: 5)
Can we complete? YES → Continue

Current: [3], Need: 2 more elements
Available numbers: 4,5 (only 2 numbers left)
Can we complete? YES → Continue

Current: [4], Need: 2 more elements
Available numbers: 5 (only 1 number left)
Can we complete? NO → PRUNE this branch!

Pruning saves: All recursive calls that would fail anyway
```

# Core Backtracking Patterns
## 1. SUBSETS (All possible sizes)
- Use startIndex to avoid duplicates.
- Every state is a valid subset (add to results at each call).
- Total subsets = 2^n.

## 2. COMBINATIONS (Fixed size k)
- Use startIndex to avoid duplicates.
- Only add to results when current size == k.
- Prune branches where remaining elements < needed.
- Total combinations = C(n,k).

## 3. PERMUTATIONS (All elements used, order matters)
- Use a 'used' array to track elements in current path.
- Only add to results when current size == n.
- Explore all unused elements at each level.
- Total permutations = n!.

## Decision tree for pattern selection
```
Is ORDER important?
├── NO (Order doesn't matter)
│   │
│   └── Do you need ALL elements or a SPECIFIC SIZE?
│       ├── ALL SIZES → SUBSETS pattern
│       │   • startIndex: YES
│       │   • used array: NO
│       │   • Add at: EVERY node
│       │
│       └── SPECIFIC SIZE k → COMBINATIONS pattern
│           • startIndex: YES
│           • used array: NO
│           • Add at: When size = k
│           • Pruning: available < needed
│
└── YES (Order matters)
    │
    └── PERMUTATIONS pattern
        • startIndex: NO (try all each time)
        • used array: YES
        • Add at: When all elements used
        • Try: ALL unused elements at each level
```





# 🎯 KEY DISTINCTION: Combinations vs Permutations vs Subsets

Understanding when order matters is **fundamental** to choosing the right backtracking pattern.

### **Comparison Table:**

| Aspect | **Subsets** | **Combinations** | **Permutations** |
|--------|-------------|------------------|------------------|
| **Order Matters?** | No | No | **Yes** |
| **Use All Elements?** | No (any size) | No (fixed size k) | **Yes** |
| **Duplicates Allowed?** | No | No | No |
| **Count Formula** | 2^n | C(n,k) = n!/(k!(n-k)!) | n! |
| **Example [1,2,3]** | 8 subsets | C(3,2)=3 combos | 6 permutations |
| **Need startIndex?** | **Yes** | **Yes** | No |
| **Need used array?** | No | No | **Yes** |

### **Visual Comparison:**

```
INPUT: [1, 2, 3]

SUBSETS (2^3 = 8):
[], [1], [2], [3], [1,2], [1,3], [2,3], [1,2,3]
Order doesn't matter: [1,2] and [2,1] are the SAME subset

COMBINATIONS size k=2 (C(3,2) = 3):
[1,2], [1,3], [2,3]
Order doesn't matter: [1,2] and [2,1] are the SAME combination

PERMUTATIONS (3! = 6):
[1,2,3], [1,3,2], [2,1,3], [2,3,1], [3,1,2], [3,2,1]
Order MATTERS: [1,2,3] and [3,2,1] are DIFFERENT permutations
```

# Grid Backtracking Fundamentals
When backtracking on a 2D grid (matrix), the core principles remain the same, but we need to manage two-dimensional coordinates and often a visited state for each cell.
## Key Considerations
- **Coordinates**: Use (row, col) to track position.
- **Directions**: Define possible moves (up, down, left, right, diagonals).
- **Visited Tracking**: Use a 2D boolean array to mark cells as visited.
- **Boundary Checks**: Ensure moves stay within grid bounds.
- **Base Cases**: Define when to stop (found solution, out of bounds, cell already visited).
- **State Management**: Update visited state when choosing/unchoosing cells.
- **Path Tracking**: Optionally maintain the current path for solutions that require it.
- **Pruning**: Apply constraints to skip invalid paths early.
- **Recursion Depth**: Be mindful of recursion depth in large grids; consider iterative approaches if necessary.

## Example Template for Grid Backtracking
```csharp
using System;
using System.Collections.Generic;

public class GridBacktrackingTemplate
{
    // UNIVERSAL GRID BACKTRACKING TEMPLATE
    // This pattern applies to most grid exploration problems

    // Direction vectors for 4-directional movement (up, down, left, right)
    private static readonly int[] rowDirections = { -1, 1, 0, 0 };
    private static readonly int[] colDirections = { 0, 0, -1, 1 };

    // Direction names for debugging/visualization
    private static readonly string[] directionNames = { "UP", "DOWN", "LEFT", "RIGHT" };

    public static bool ExploreGrid(
        int[][] grid,                      // The grid to explore
        int startRow, int startCol,        // Starting position
        int targetRow, int targetCol,      // Target/goal position
        bool[][] visited,                  // Track visited cells
        List<(int, int)> currentPath,      // Current path being built
        Func<int, int, bool> isValid,      // Function to check if cell is valid
        Func<int, int, bool> isGoal)       // Function to check if we reached goal
    {
        // BASE CASE 1: Out of bounds
        if (startRow < 0 || startRow >= grid.Length ||
            startCol < 0 || startCol >= grid[0].Length)
        {
            return false;
        }

        // BASE CASE 2: Already visited (cycle detection)
        if (visited[startRow][startCol])
        {
            return false;
        }

        // BASE CASE 3: Invalid cell (obstacle, constraint violation)
        if (!isValid(startRow, startCol))
        {
            return false;
        }

        // BASE CASE 4: Goal reached
        if (isGoal(startRow, startCol))
        {
            currentPath.Add((startRow, startCol));
            return true; // Found valid path
        }

        // CHOOSE: Mark current cell as visited and add to path
        visited[startRow][startCol] = true;
        currentPath.Add((startRow, startCol));

        // EXPLORE: Try all 4 directions
        for (int dir = 0; dir < 4; dir++)
        {
            int newRow = startRow + rowDirections[dir];
            int newCol = startCol + colDirections[dir];

            // Recursively explore from new position
            if (ExploreGrid(grid, newRow, newCol, targetRow, targetCol,
                          visited, currentPath, isValid, isGoal))
            {
                return true; // Found path, propagate success
            }
        }

        // UNCHOOSE: Backtrack - unmark visited and remove from path
        visited[startRow][startCol] = false;
        currentPath.RemoveAt(currentPath.Count - 1);

        return false; // No valid path from this cell
    }
}
```

## Visualization of Grid Backtracking
```
Current cell at (row, col):

           (row-1, col)
                ↑
                |
(row, col-1) ← (row, col) → (row, col+1)
                |
                ↓
           (row+1, col)

Direction vectors explained:
UP:    rowDir = -1, colDir =  0  → (row-1, col)
DOWN:  rowDir = +1, colDir =  0  → (row+1, col)
LEFT:  rowDir =  0, colDir = -1  → (row, col-1)
RIGHT: rowDir =  0, colDir = +1  → (row, col+1)


(-1,-1)  (-1,0)  (-1,+1)
   ↖      ↑      ↗
(-0,-1) ← (0,0) → (0,+1)
   ↙      ↓      ↘
(+1,-1)  (+1,0)  (+1,+1)

For 8-direction movement:
int[] rowDirs = {-1,-1,-1, 0, 0, 1, 1, 1};
int[] colDirs = {-1, 0, 1,-1, 1,-1, 0, 1};
```
