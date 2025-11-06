
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

## Example Grid Backtracking Problems
### 1. Maze Solver (Path Finding)Problem: Given a 2D grid maze with walls and open paths, find a path from start to end.

The classic maze problem demonstrates all grid backtracking concepts.

```csharp
public class MazeSolver
{
    // Maze: 0 = path, 1 = wall, S = start, E = end
    // Find if path exists from S to E

    private static readonly int[] rowDirs = { -1, 1, 0, 0 };
    private static readonly int[] colDirs = { 0, 0, -1, 1 };
    private static readonly string[] dirNames = { "UP", "DOWN", "LEFT", "RIGHT" };

    public static bool SolveMaze(int[][] maze,
                                (int row, int col) start,
                                (int row, int col) end)
    {
        int rows = maze.Length;
        int cols = maze[0].Length;

        bool[][] visited = new bool[rows][];
        for (int i = 0; i < rows; i++)
        {
            visited[i] = new bool[cols];
        }

        var path = new List<(int, int)>();

        bool found = Backtrack(maze, start.row, start.col, end, visited, path);

        if (found)
        {
            Console.WriteLine("Path found:");
            foreach (var (r, c) in path)
            {
                Console.WriteLine($"  ({r},{c})");
            }
        }
        else
        {
            Console.WriteLine("No path exists!");
        }

        return found;
    }

    private static bool Backtrack(int[][] maze, int row, int col,
                                 (int row, int col) end,
                                 bool[][] visited,
                                 List<(int, int)> path)
    {
        // BASE CASE 1: Out of bounds
        if (row < 0 || row >= maze.Length ||
            col < 0 || col >= maze[0].Length)
        {
            return false;
        }

        // BASE CASE 2: Wall (obstacle)
        if (maze[row][col] == 1)
        {
            return false;
        }

        // BASE CASE 3: Already visited (avoid cycles)
        if (visited[row][col])
        {
            return false;
        }

        // CHOOSE: Mark as visited and add to path
        visited[row][col] = true;
        path.Add((row, col));

        // BASE CASE 4: Reached destination
        if (row == end.row && col == end.col)
        {
            return true; // Success! Keep the path
        }

        // EXPLORE: Try all 4 directions
        for (int dir = 0; dir < 4; dir++)
        {
            int newRow = row + rowDirs[dir];
            int newCol = col + colDirs[dir];

            // Recursively search from new position
            if (Backtrack(maze, newRow, newCol, end, visited, path))
            {
                return true; // Found path through this direction
            }
        }

        // UNCHOOSE: Backtrack - this path didn't work
        visited[row][col] = false;
        path.RemoveAt(path.Count - 1);

        return false;
    }

    // Enhanced version with detailed trace
    public static bool SolveMazeWithTrace(int[][] maze,
                                         (int row, int col) start,
                                         (int row, int col) end)
    {
        Console.WriteLine("=== MAZE SOLVER WITH TRACE ===");
        Console.WriteLine("Maze:");
        PrintMaze(maze, start, end);
        Console.WriteLine();

        int rows = maze.Length;
        int cols = maze[0].Length;

        bool[][] visited = new bool[rows][];
        for (int i = 0; i < rows; i++)
        {
            visited[i] = new bool[cols];
        }

        var path = new List<(int, int)>();

        bool found = BacktrackTrace(maze, start.row, start.col, end,
                                   visited, path, 0);

        Console.WriteLine($"\n=== PATH {'(FOUND)' : 'NOT FOUND'} ===");

        return found;
    }

    private static bool BacktrackTrace(int[][] maze, int row, int col,
                                      (int row, int col) end,
                                      bool[][] visited,
                                      List<(int, int)> path,
                                      int depth)
    {
        string indent = new string(' ', depth * 2);

        Console.WriteLine($"{indent}Exploring ({row},{col})...");

        // Boundary check
        if (row < 0 || row >= maze.Length ||
            col < 0 || col >= maze[0].Length)
        {
            Console.WriteLine($"{indent}  ✗ Out of bounds");
            return false;
        }

        // Wall check
        if (maze[row][col] == 1)
        {
            Console.WriteLine($"{indent}  ✗ Wall (obstacle)");
            return false;
        }

        // Visited check
        if (visited[row][col])
        {
            Console.WriteLine($"{indent}  ✗ Already visited");
            return false;
        }

        // Mark visited and add to path
        visited[row][col] = true;
        path.Add((row, col));
        Console.WriteLine($"{indent}  ✓ Added to path (length: {path.Count})");

        // Goal check
        if (row == end.row && col == end.col)
        {
            Console.WriteLine($"{indent}  ★ GOAL REACHED!");
            return true;
        }

        // Try all 4 directions
        for (int dir = 0; dir < 4; dir++)
        {
            int newRow = row + rowDirs[dir];
            int newCol = col + colDirs[dir];

            Console.WriteLine($"{indent}  → Try {dirNames[dir]} to ({newRow},{newCol})");

            if (BacktrackTrace(maze, newRow, newCol, end, visited, path, depth + 1))
            {
                Console.WriteLine($"{indent}  ← {dirNames[dir]} succeeded!");
                return true;
            }

            Console.WriteLine($"{indent}  ← {dirNames[dir]} failed");
        }

        // Backtrack
        visited[row][col] = false;
        path.RemoveAt(path.Count - 1);
        Console.WriteLine($"{indent}  ⟲ BACKTRACK from ({row},{col})");

        return false;
    }

    private static void PrintMaze(int[][] maze, (int r, int c) start, (int r, int c) end)
    {
        for (int r = 0; r < maze.Length; r++)
        {
            for (int c = 0; c < maze[0].Length; c++)
            {
                if (r == start.r && c == start.c)
                    Console.Write("S ");
                else if (r == end.r && c == end.c)
                    Console.Write("E ");
                else if (maze[r][c] == 1)
                    Console.Write("█ ");
                else
                    Console.Write(". ");
            }
            Console.WriteLine();
        }
    }
}


public static class MazeExamples
{
    public static void DemonstrateSimpleMaze()
    {
        Console.WriteLine("=== SIMPLE MAZE EXAMPLE ===\n");

        // Maze representation: 0 = path, 1 = wall
        int[][] maze = new int[][]
        {
            new int[] { 0, 1, 0, 0, 0 },
            new int[] { 0, 1, 0, 1, 0 },
            new int[] { 0, 0, 0, 1, 0 },
            new int[] { 1, 1, 0, 0, 0 },
            new int[] { 0, 0, 0, 1, 0 }
        };

        var start = (row: 0, col: 0);  // Top-left
        var end = (row: 4, col: 4);    // Bottom-right

        Console.WriteLine("Maze (S=start, E=end, █=wall, .=path):");
        PrintMazeWithPath(maze, start, end, new List<(int, int)>());

        // Solve with trace
        MazeSolver.SolveMazeWithTrace(maze, start, end);
    }

    private static void PrintMazeWithPath(int[][] maze,
                                         (int r, int c) start,
                                         (int r, int c) end,
                                         List<(int r, int c)> path)
    {
        var pathSet = new HashSet<(int, int)>(path);

        for (int r = 0; r < maze.Length; r++)
        {
            for (int c = 0; c < maze[0].Length; c++)
            {
                if (r == start.r && c == start.c)
                    Console.Write("S ");
                else if (r == end.r && c == end.c)
                    Console.Write("E ");
                else if (pathSet.Contains((r, c)))
                    Console.Write("* ");
                else if (maze[r][c] == 1)
                    Console.Write("█ ");
                else
                    Console.Write(". ");
            }
            Console.WriteLine();
        }
    }
}
```

```
Initial Maze:
S █ . . .
. █ . █ .
. . . █ .
█ █ . . .
. . . █ E

Step-by-step exploration:

Step 1: Start at (0,0)
Current: (0,0)
* █ . . .      Try UP → out of bounds
. █ . █ .      Try DOWN → (1,0) valid
. . . █ .      Try LEFT → out of bounds
█ █ . . .      Try RIGHT → (0,1) wall
. . . █ E

Step 2: Move to (1,0)
* █ . . .      Try UP → (0,0) visited
* █ . █ .      Try DOWN → (2,0) valid
. . . █ .      Try LEFT → out of bounds
█ █ . . .      Try RIGHT → (1,1) wall
. . . █ E

Step 3: Move to (2,0)
* █ . . .      Try UP → (1,0) visited
* █ . █ .      Try DOWN → (3,0) wall
* . . █ .      Try LEFT → out of bounds
█ █ . . .      Try RIGHT → (2,1) valid
. . . █ E

Step 4: Move to (2,1)
* █ . . .      Try UP → (1,1) wall
* █ . █ .      Try DOWN → (3,1) wall
* * . █ .      Try LEFT → (2,0) visited
█ █ . . .      Try RIGHT → (2,2) valid
. . . █ E

Step 5: Move to (2,2)
* █ . . .      Try UP → (1,2) path, explore...
* █ . █ .      Try DOWN → (3,2) path, explore...
* * * █ .      Try LEFT → (2,1) visited
█ █ . . .      Try RIGHT → (2,3) wall
. . . █ E

... (exploration continues) ...

Final successful path marked with *:
* █ . . .
* █ . █ .
* * * █ .
█ █ * * *
. . . █ *
```

### 2. Word Search Problem: Given a 2D grid of letters and a word, determine if the word exists in the grid by moving horizontally or vertically.

Problem: Given a 2D board and a word, find if the word exists in the grid.
The word can be constructed from letters of sequentially adjacent cells,where "adjacent" cells are horizontally or vertically neighboring.
The same letter cell may not be used more than once.
```csharp
public class WordSearch
{
    // Problem: Given a 2D board and a word, find if the word exists in the grid.
    // The word can be constructed from letters of sequentially adjacent cells,
    // where "adjacent" cells are horizontally or vertically neighboring.
    // The same letter cell may not be used more than once.

    private static readonly int[] rowDirs = { -1, 1, 0, 0 };
    private static readonly int[] colDirs = { 0, 0, -1, 1 };

    public static bool ExistWord(char[][] board, string word)
    {
        if (board == null || board.Length == 0 || word == null || word.Length == 0)
            return false;

        int rows = board.Length;
        int cols = board[0].Length;

        // Try starting from every cell
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                // If first character matches, start backtracking from this cell
                if (board[r][c] == word[0])
                {
                    bool[][] visited = new bool[rows][];
                    for (int i = 0; i < rows; i++)
                    {
                        visited[i] = new bool[cols];
                    }

                    if (Backtrack(board, word, 0, r, c, visited))
                    {
                        return true; // Found the word
                    }
                }
            }
        }

        return false; // Word not found
    }

    private static bool Backtrack(char[][] board, string word, int index,
                                 int row, int col, bool[][] visited)
    {
        // BASE CASE 1: Found complete word
        if (index == word.Length)
        {
            return true;
        }

        // BASE CASE 2: Out of bounds
        if (row < 0 || row >= board.Length ||
            col < 0 || col >= board[0].Length)
        {
            return false;
        }

        // BASE CASE 3: Already visited
        if (visited[row][col])
        {
            return false;
        }

        // BASE CASE 4: Character doesn't match
        if (board[row][col] != word[index])
        {
            return false;
        }

        // CHOOSE: Mark current cell as visited
        visited[row][col] = true;

        // EXPLORE: Try all 4 directions for next character
        for (int dir = 0; dir < 4; dir++)
        {
            int newRow = row + rowDirs[dir];
            int newCol = col + colDirs[dir];

            // Recursively search for next character
            if (Backtrack(board, word, index + 1, newRow, newCol, visited))
            {
                return true; // Found path to complete word
            }
        }

        // UNCHOOSE: Backtrack - unmark visited
        visited[row][col] = false;

        return false; // No path from this cell
    }

    // Enhanced version with visualization
    public static bool ExistWordWithTrace(char[][] board, string word)
    {
        Console.WriteLine($"=== WORD SEARCH: '{word}' ===\n");
        Console.WriteLine("Board:");
        PrintBoard(board);
        Console.WriteLine();

        if (board == null || board.Length == 0 || word == null || word.Length == 0)
            return false;

        int rows = board.Length;
        int cols = board[0].Length;

        // Try starting from every cell
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (board[r][c] == word[0])
                {
                    Console.WriteLine($"Starting search from ({r},{c}) - matches '{word[0]}'");

                    bool[][] visited = new bool[rows][];
                    for (int i = 0; i < rows; i++)
                    {
                        visited[i] = new bool[cols];
                    }

                    if (BacktrackTrace(board, word, 0, r, c, visited, 0))
                    {
                        Console.WriteLine($"\n★ WORD FOUND starting from ({r},{c})!");
                        return true;
                    }

                    Console.WriteLine($"Word not found from ({r},{c})\n");
                }
            }
        }

        Console.WriteLine("✗ Word not found anywhere in the board");
        return false;
    }

    private static bool BacktrackTrace(char[][] board, string word, int index,
                                      int row, int col, bool[][] visited, int depth)
    {
        string indent = new string(' ', depth * 2);

        // Check bounds
        if (row < 0 || row >= board.Length ||
            col < 0 || col >= board[0].Length)
        {
            Console.WriteLine($"{indent}✗ ({row},{col}) out of bounds");
            return false;
        }

        // Check visited
        if (visited[row][col])
        {
            Console.WriteLine($"{indent}✗ ({row},{col}) already visited");
            return false;
        }

        // Check character match
        if (board[row][col] != word[index])
        {
            Console.WriteLine($"{indent}✗ ({row},{col})='{board[row][col]}' doesn't match '{word[index]}'");
            return false;
        }

        Console.WriteLine($"{indent}✓ ({row},{col})='{board[row][col]}' matches word[{index}]='{word[index]}'");

        // Check if word complete
        if (index == word.Length - 1)
        {
            Console.WriteLine($"{indent}★ COMPLETE WORD FOUND!");
            return true;
        }

        // Mark visited
        visited[row][col] = true;
        Console.WriteLine($"{indent}  Searching for next char '{word[index + 1]}'...");

        // Try all 4 directions
        string[] dirNames = { "UP", "DOWN", "LEFT", "RIGHT" };
        for (int dir = 0; dir < 4; dir++)
        {
            int newRow = row + rowDirs[dir];
            int newCol = col + colDirs[dir];

            Console.WriteLine($"{indent}  → Try {dirNames[dir]} to ({newRow},{newCol})");

            if (BacktrackTrace(board, word, index + 1, newRow, newCol, visited, depth + 1))
            {
                return true;
            }
        }

        // Backtrack
        visited[row][col] = false;
        Console.WriteLine($"{indent}  ⟲ BACKTRACK from ({row},{col})");

        return false;
    }

    private static void PrintBoard(char[][] board)
    {
        for (int r = 0; r < board.Length; r++)
        {
            for (int c = 0; c < board[0].Length; c++)
            {
                Console.Write(board[r][c] + " ");
            }
            Console.WriteLine();
        }
    }
}
```

### 3. Constraint Satisfaction Problems (CSP)

While maze and word search involve path finding, Constraint Satisfaction Problems involve configuration finding - arranging elements to satisfy multiple constraints simultaneously.

- **Key Characteristics**:
  - Variables: What we need to assign (e.g., queen positions)
  - Domains: Possible values for each variable (e.g., column positions)
  - Constraints: Rules that must be satisfied (e.g., no two queens attack each other)

#### N-Queens Problem

```csharp
public class NQueensVisualization
{
    public static List<List<string>> SolveNQueensWithTrace(int n)
    {
        Console.WriteLine($"=== SOLVING {n}-QUEENS PROBLEM ===\n");

        var solutions = new List<List<string>>();
        var board = new char[n][];

        for (int i = 0; i < n; i++)
        {
            board[i] = new char[n];
            for (int j = 0; j < n; j++)
            {
                board[i][j] = '.';
            }
        }

        BacktrackTrace(board, 0, solutions, 0);

        Console.WriteLine($"\n=== TOTAL SOLUTIONS FOUND: {solutions.Count} ===");

        return solutions;
    }

    private static void BacktrackTrace(char[][] board, int row,
                                      List<List<string>> solutions, int depth)
    {
        int n = board.Length;
        string indent = new string(' ', depth * 2);

        // Check if all queens placed
        if (row == n)
        {
            Console.WriteLine($"{indent}★ SOLUTION FOUND!");
            PrintBoard(board, indent + "  ");
            solutions.Add(BoardToStringList(board));
            return;
        }

        Console.WriteLine($"{indent}Processing Row {row}:");

        // Try placing queen in each column
        for (int col = 0; col < n; col++)
        {
            Console.WriteLine($"{indent}  Trying column {col}...");

            if (IsSafeTrace(board, row, col, indent + "    "))
            {
                Console.WriteLine($"{indent}    ✓ Safe! Placing queen at ({row},{col})");

                // Place queen
                board[row][col] = 'Q';
                PrintBoard(board, indent + "    ");

                // Explore next row
                BacktrackTrace(board, row + 1, solutions, depth + 1);

                // Remove queen (backtrack)
                board[row][col] = '.';
                Console.WriteLine($"{indent}    ⟲ BACKTRACK: Removed queen from ({row},{col})");
            }
            else
            {
                Console.WriteLine($"{indent}    ✗ Unsafe! Cannot place queen at ({row},{col})");
            }
        }
    }

    private static bool IsSafeTrace(char[][] board, int row, int col, string indent)
    {
        int n = board.Length;

        // Check column
        for (int r = 0; r < row; r++)
        {
            if (board[r][col] == 'Q')
            {
                Console.WriteLine($"{indent}Conflict: Queen at ({r},{col}) - same column");
                return false;
            }
        }

        // Check upper-left diagonal
        for (int r = row - 1, c = col - 1; r >= 0 && c >= 0; r--, c--)
        {
            if (board[r][c] == 'Q')
            {
                Console.WriteLine($"{indent}Conflict: Queen at ({r},{c}) - upper-left diagonal");
                return false;
            }
        }

        // Check upper-right diagonal
        for (int r = row - 1, c = col + 1; r >= 0 && c < n; r--, c++)
        {
            if (board[r][c] == 'Q')
            {
                Console.WriteLine($"{indent}Conflict: Queen at ({r},{c}) - upper-right diagonal");
                return false;
            }
        }

        Console.WriteLine($"{indent}No conflicts found - position is safe");
        return true;
    }

    private static void PrintBoard(char[][] board, string indent)
    {
        foreach (var row in board)
        {
            Console.WriteLine($"{indent}{new string(row)}");
        }
    }

    private static List<string> BoardToStringList(char[][] board)
    {
        var result = new List<string>();
        foreach (var row in board)
        {
            result.Add(new string(row));
        }
        return result;
    }
}
```



- Optiomize solution

```csharp
using System;
using System.Collections.Generic;

public class NQueensOptimized
{
    // Optimized N-Queens using sets for O(1) constraint checking
    // Key insight: Track columns and diagonals using mathematical formulas

    public static List<List<string>> SolveNQueensOptimized(int n)
    {
        var solutions = new List<List<string>>();
        var board = new char[n][];

        // Initialize empty board
        for (int i = 0; i < n; i++)
        {
            board[i] = new char[n];
            for (int j = 0; j < n; j++)
            {
                board[i][j] = '.';
            }
        }

        // Use sets to track attacked positions - O(1) lookup
        var columns = new HashSet<int>();           // Attacked columns
        var diagonals1 = new HashSet<int>();        // Attacked \ diagonals (row - col)
        var diagonals2 = new HashSet<int>();        // Attacked / diagonals (row + col)

        Backtrack(board, 0, columns, diagonals1, diagonals2, solutions);

        return solutions;
    }

    private static void Backtrack(
        char[][] board,
        int row,
        HashSet<int> columns,
        HashSet<int> diagonals1,    // row - col is constant for \ diagonals
        HashSet<int> diagonals2,    // row + col is constant for / diagonals
        List<List<string>> solutions)
    {
        int n = board.Length;

        // BASE CASE: All queens placed
        if (row == n)
        {
            solutions.Add(BoardToStringList(board));
            return;
        }

        // Try each column in current row
        for (int col = 0; col < n; col++)
        {
            int diag1 = row - col;  // Identifier for \ diagonal
            int diag2 = row + col;  // Identifier for / diagonal

            // CONSTRAINT CHECK: O(1) instead of O(N)
            if (columns.Contains(col) ||
                diagonals1.Contains(diag1) ||
                diagonals2.Contains(diag2))
            {
                continue; // Position is under attack
            }

            // CHOOSE: Place queen and mark attacked positions
            board[row][col] = 'Q';
            columns.Add(col);
            diagonals1.Add(diag1);
            diagonals2.Add(diag2);

            // EXPLORE: Move to next row
            Backtrack(board, row + 1, columns, diagonals1, diagonals2, solutions);

            // UNCHOOSE: Remove queen and unmark positions
            board[row][col] = '.';
            columns.Remove(col);
            diagonals1.Remove(diag1);
            diagonals2.Remove(diag2);
        }
    }

    private static List<string> BoardToStringList(char[][] board)
    {
        var result = new List<string>();
        foreach (var row in board)
        {
            result.Add(new string(row));
        }
        return result;
    }
}
```

```
For an N×N board, understanding diagonal patterns:

POSITION (row, col):

\ DIAGONAL (top-left to bottom-right):
  row - col is CONSTANT along each diagonal

  Example 4×4 board (showing row-col values):
  (0,0)=0   (0,1)=-1  (0,2)=-2  (0,3)=-3
  (1,0)=1   (1,1)=0   (1,2)=-1  (1,3)=-2
  (2,0)=2   (2,1)=1   (2,2)=0   (2,3)=-1
  (3,0)=3   (3,1)=2   (3,2)=1   (3,3)=0

  Range: -(n-1) to (n-1)

/ DIAGONAL (top-right to bottom-left):
  row + col is CONSTANT along each diagonal

  Example 4×4 board (showing row+col values):
  (0,0)=0   (0,1)=1   (0,2)=2   (0,3)=3
  (1,0)=1   (1,1)=2   (1,2)=3   (1,3)=4
  (2,0)=2   (2,1)=3   (2,2)=4   (2,3)=5
  (3,0)=3   (3,1)=4   (3,2)=5   (3,3)=6

  Range: 0 to 2(n-1)

By storing these values in sets, we get O(1) diagonal checking!
```

#### Sudoku Solver

```csharp
using System;
using System.Collections.Generic;

public class SudokuSolver
{
    // Sudoku rules:
    // 1. Each row must contain digits 1-9 without repetition
    // 2. Each column must contain digits 1-9 without repetition
    // 3. Each 3×3 box must contain digits 1-9 without repetition

    public static void SolveSudoku(char[][] board)
    {
        Backtrack(board);
    }

    private static bool Backtrack(char[][] board)
    {
        // Find next empty cell
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board[row][col] == '.')
                {
                    // Try digits 1-9
                    for (char digit = '1'; digit <= '9'; digit++)
                    {
                        // CONSTRAINT CHECK: Can we place this digit?
                        if (IsValid(board, row, col, digit))
                        {
                            // CHOOSE: Place digit
                            board[row][col] = digit;

                            // EXPLORE: Continue solving
                            if (Backtrack(board))
                            {
                                return true; // Solution found
                            }

                            // UNCHOOSE: Backtrack
                            board[row][col] = '.';
                        }
                    }

                    return false; // No valid digit for this cell
                }
            }
        }

        return true; // All cells filled - solution found
    }

    private static bool IsValid(char[][] board, int row, int col, char digit)
    {
        // CHECK 1: Row constraint
        for (int c = 0; c < 9; c++)
        {
            if (board[row][c] == digit)
            {
                return false; // Digit already in row
            }
        }

        // CHECK 2: Column constraint
        for (int r = 0; r < 9; r++)
        {
            if (board[r][col] == digit)
            {
                return false; // Digit already in column
            }
        }

        // CHECK 3: 3×3 box constraint
        int boxRow = (row / 3) * 3;  // Top-left row of box
        int boxCol = (col / 3) * 3;  // Top-left col of box

        for (int r = boxRow; r < boxRow + 3; r++)
        {
            for (int c = boxCol; c < boxCol + 3; c++)
            {
                if (board[r][c] == digit)
                {
                    return false; // Digit already in box
                }
            }
        }

        return true; // All constraints satisfied
    }

    // Optimized version using sets
    public static void SolveSudokuOptimized(char[][] board)
    {
        // Initialize constraint tracking sets
        var rows = new HashSet<char>[9];
        var cols = new HashSet<char>[9];
        var boxes = new HashSet<char>[9];

        for (int i = 0; i < 9; i++)
        {
            rows[i] = new HashSet<char>();
            cols[i] = new HashSet<char>();
            boxes[i] = new HashSet<char>();
        }

        // Pre-populate sets with existing digits
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (board[r][c] != '.')
                {
                    char digit = board[r][c];
                    int boxIndex = (r / 3) * 3 + (c / 3);

                    rows[r].Add(digit);
                    cols[c].Add(digit);
                    boxes[boxIndex].Add(digit);
                }
            }
        }

        BacktrackOptimized(board, rows, cols, boxes);
    }

    private static bool BacktrackOptimized(char[][] board,
                                          HashSet<char>[] rows,
                                          HashSet<char>[] cols,
                                          HashSet<char>[] boxes)
    {
        // Find next empty cell
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board[row][col] == '.')
                {
                    int boxIndex = (row / 3) * 3 + (col / 3);

                    // Try digits 1-9
                    for (char digit = '1'; digit <= '9'; digit++)
                    {
                        // CONSTRAINT CHECK: O(1) using sets
                        if (!rows[row].Contains(digit) &&
                            !cols[col].Contains(digit) &&
                            !boxes[boxIndex].Contains(digit))
                        {
                            // CHOOSE: Place digit and update sets
                            board[row][col] = digit;
                            rows[row].Add(digit);
                            cols[col].Add(digit);
                            boxes[boxIndex].Add(digit);

                            // EXPLORE
                            if (BacktrackOptimized(board, rows, cols, boxes))
                            {
                                return true;
                            }

                            // UNCHOOSE: Remove digit and update sets
                            board[row][col] = '.';
                            rows[row].Remove(digit);
                            cols[col].Remove(digit);
                            boxes[boxIndex].Remove(digit);
                        }
                    }

                    return false;
                }
            }
        }

        return true;
    }
}
```
