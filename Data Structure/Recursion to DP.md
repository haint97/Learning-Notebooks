# Recursion

## Definition
Recursion is a programming technique where a function calls itself to solve smaller instances of the same problem until it reaches a base case. It is often used for problems that can be broken down into smaller, similar subproblems.

## Components of Recursion
1. **Base Case**:

The condition under which the recursion stops. It prevents infinite recursion and eventually leads to a solution. A base case is like the foundation of a building - everything depends on it being solid. Without it → infinite recursion → stack overflow.

### The Three Laws of Base Cases

Every recursive function should satisfy these three rules to be safe and correct:

1. **Reachability** — Every possible input must eventually reach a base case.
    Ensure recursive progress (e.g., decreasing n, shrinking input size) so calls cannot loop forever.

2. **Simplicity** — Base cases should be trivially solvable without recursion.
    Keep them minimal: direct returns or simple computations (e.g., return 0, return 1, return input).

3. **Correctness** — Base cases must return the correct result for their inputs.
    Test edge inputs explicitly (empty list, zero, negative where applicable).

`Common base cases patterns`
```csharp
using System;

public class BaseCasePatterns
{
    // Pattern 1: Natural Numbers (countdown to 0 or 1)
    public static int Factorial(int n)
    {
        if (n <= 1) return 1;  // Base: both 0! and 1! equal 1
        return n * Factorial(n - 1);
    }

    // Pattern 2: Array/Collection Boundaries
    public static int SumArray(int[] arr, int index)
    {
        if (index >= arr.Length) return 0;  // Base: past end of array
        return arr[index] + SumArray(arr, index + 1);
    }

    // Pattern 3: String Length
    public static bool IsPalindrome(string s, int left, int right)
    {
        if (left >= right) return true;  // Base: single char or empty
        if (s[left] != s[right]) return false;  // Base: mismatch found
        return IsPalindrome(s, left + 1, right - 1);
    }

    // Pattern 4: Mathematical Properties
    public static int GCD(int a, int b)
    {
        if (b == 0) return a;  // Base: Euclidean algorithm endpoint
        return GCD(b, a % b);
    }
}
```


2. **Recursive Case**: The part of the function where the function calls itself with modified arguments, moving closer to the base case.

## Examples
```csharp
using System;

public class RecursionBasics
{
    // Simple factorial implementation
    public static int Factorial(int n)
    {
        // Base case: prevents infinite recursion
        if (n <= 1)
            return 1;

        // Recursive case: break down the problem
        return n * Factorial(n - 1);
    }

    // Visualization helper
    public static int FactorialWithTrace(int n, int depth = 0)
    {
        string indent = new string(' ', depth * 2);
        Console.WriteLine($"{indent}→ Factorial({n}) called");

        if (n <= 1)
        {
            Console.WriteLine($"{indent}← Base case: returning 1");
            return 1;
        }

        Console.WriteLine($"{indent}  Computing {n} * Factorial({n-1})");
        int result = n * FactorialWithTrace(n - 1, depth + 1);
        Console.WriteLine($"{indent}← Factorial({n}) = {result}");

        return result;
    }
}
```

```
Factorial(4):
Factorial(4) → 4 × Factorial(3)
              → 4 × (3 × Factorial(2))
                    → 4 × (3 × (2 × Factorial(1)))
                              → 4 × (3 × (2 × 1))
                                    → 24

Call Stack (Linear Chain):
┌─────────────────┐
│ Factorial(1)    │ ← Returns 1
├─────────────────┤
│ Factorial(2)    │
├─────────────────┤
│ Factorial(3)    │
├─────────────────┤
│ Factorial(4)    │
└─────────────────┘

Fibonacci(4):
                 fib(4)
               /        \
           fib(3)        fib(2)
          /      \      /      \
      fib(2)   fib(1) fib(1)  fib(0)
     /     \     │      │       │
  fib(1)  fib(0) 1      1       0
    │       │
    1       0

Result: 1 + 0 + 1 + 1 + 1 + 0 = 3

```

## Call Stack Visualization
Every time a recursive function is called, a new frame is added to the call stack. The call stack grows with each recursive call until the base case is reached, at which point the stack unwinds as each function call returns its result. Here's a simple visualization of the call stack for a recursive function that calculates the factorial of a number
```
Call Stack (grows downward):
┌─────────────────────┐
│ factorial(1) ← BASE │  Returns 1
├─────────────────────┤
│ factorial(2)        │  Waits for factorial(1)
├─────────────────────┤
│ factorial(3)        │  Waits for factorial(2)
├─────────────────────┤
│ factorial(4)        │  Waits for factorial(3)
└─────────────────────┘
```

## Memory Management
Recursion can lead to high memory usage due to the accumulation of function calls on the call stack. Each recursive call consumes stack space, and deep recursion can result in stack overflow errors. To mitigate this, techniques such as tail recursion optimization or converting recursive algorithms to iterative ones can be employed.

## Connect to Dynamic Programming
Recursion is often used in dynamic programming to solve problems that exhibit overlapping subproblems and optimal substructure. In dynamic programming, recursion is combined with memoization or tabulation to store the results of subproblems, avoiding redundant calculations and improving efficiency.

-Dynamic Programming is optimized recursion
-Backtracking uses recursive exploration
-Understanding call stack helps debug complex DP problems

## Examples
1. Binary Search
```csharp
using System;

public class BaseCasesSolutions
{
    // Solution 1: Binary Search - Complete Implementation
    public static int BinarySearch(int[] arr, int target, int left, int right)
    {
        // Base Case 1: Invalid search space (element not found)
        if (left > right)
            return -1;

        int mid = left + (right - left) / 2; // Avoid overflow

        // Base Case 2: Target found
        if (arr[mid] == target)
            return mid;

        // Recursive Case 1: Target is in left half
        if (target < arr[mid])
            return BinarySearch(arr, target, left, mid - 1);

        // Recursive Case 2: Target is in right half
        return BinarySearch(arr, target, mid + 1, right);
    }

    // Detailed trace for understanding
    public static int BinarySearchWithTrace(int[] arr, int target, int left, int right, int depth = 0)
    {
        string indent = new string(' ', depth * 2);
        Console.WriteLine($"{indent}→ Search [{left}..{right}] for {target}");

        if (left > right)
        {
            Console.WriteLine($"{indent}← Not found: left > right");
            return -1;
        }

        int mid = left + (right - left) / 2;
        Console.WriteLine($"{indent}  Mid index: {mid}, value: {arr[mid]}");

        if (arr[mid] == target)
        {
            Console.WriteLine($"{indent}← Found at index {mid}!");
            return mid;
        }

        if (target < arr[mid])
        {
            Console.WriteLine($"{indent}  Target < {arr[mid]}, searching left half");
            return BinarySearchWithTrace(arr, target, left, mid - 1, depth + 1);
        }
        else
        {
            Console.WriteLine($"{indent}  Target > {arr[mid]}, searching right half");
            return BinarySearchWithTrace(arr, target, mid + 1, right, depth + 1);
        }
    }
}
```

2. Tower of Hanoi

Move n-1 disks from A to B (using C as auxiliary)
Move the bottom disk from A to C
Move n-1 disks from B to C (using A as auxiliary)


```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class MultipleRecursion
{
    // Classic Tower of Hanoi - Pure Multiple Recursion
    public static void TowerOfHanoi(int n, char source, char destination, char auxiliary)
    {
        // Base case: only one disk
        if (n == 1)
        {
            Console.WriteLine($"Move disk 1 from {source} to {destination}");
            return;
        }

        // Step 1: Move n-1 disks to auxiliary peg
        TowerOfHanoi(n - 1, source, auxiliary, destination);

        // Step 2: Move the bottom disk to destination
        Console.WriteLine($"Move disk {n} from {source} to {destination}");

        // Step 3: Move n-1 disks from auxiliary to destination
        TowerOfHanoi(n - 1, auxiliary, destination, source);
    }

    // Enhanced version with move counting and detailed tracing
    private static int moveCount = 0;

    public static int TowerOfHanoiWithCount(int n, char source, char destination, char auxiliary, int depth = 0)
    {
        string indent = new string(' ', depth * 2);
        Console.WriteLine($"{indent}→ Move {n} disk(s) from {source} to {destination} (aux: {auxiliary})");

        if (n == 1)
        {
            moveCount++;
            Console.WriteLine($"{indent}  Move #{moveCount}: disk 1 from {source} to {destination}");
            return 1;
        }

        Console.WriteLine($"{indent}  Step 1: Move {n-1} disks to auxiliary");
        int moves1 = TowerOfHanoiWithCount(n - 1, source, auxiliary, destination, depth + 1);

        Console.WriteLine($"{indent}  Step 2: Move bottom disk");
        moveCount++;
        Console.WriteLine($"{indent}  Move #{moveCount}: disk {n} from {source} to {destination}");

        Console.WriteLine($"{indent}  Step 3: Move {n-1} disks to destination");
        int moves2 = TowerOfHanoiWithCount(n - 1, auxiliary, destination, source, depth + 1);

        int totalMoves = moves1 + 1 + moves2;
        Console.WriteLine($"{indent}← Total moves for {n} disks: {totalMoves}");

        return totalMoves;
    }
}
```

- Mathematical Analysis:

T(n) = 2 × T(n-1) + 1
T(1) = 1

- Solving step by step:

T(1) = 1
T(2) = 2×T(1) + 1 = 2×1 + 1 = 3
T(3) = 2×T(2) + 1 = 2×3 + 1 = 7
T(4) = 2×T(3) + 1 = 2×7 + 1 = 15

- Why This Formula Works:

Each level doubles the work of the previous level
Plus one additional move for the bottom disk
This creates the pattern: 1, 3, 7, 15, 31, ...
Each term is 2^n - 1


Pattern: T(n) = 2^n - 1

3. Fibonacci Optimization Progression
```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class TreeRecursionSolutions
{
    // Solution 1a: Naive Fibonacci - Classic Tree Recursion
    /*
    Time: O(2^n) due to exponential tree growth
    Space: O(n) recursion depth
    Problem: Recalculates same values repeatedly
    */
    public static long FibonacciNaive(int n)
    {
        // Base cases: F(0) = 0, F(1) = 1
        if (n <= 1) return n;

        // Tree recursion: F(n) = F(n-1) + F(n-2)
        return FibonacciNaive(n - 1) + FibonacciNaive(n - 2);
    }

    // Solution 1b: Fibonacci with Memoization
    /*
    Time: O(n) - each value calculated once
    Space: O(n) for memoization table + O(n) recursion stack
    Benefit: Eliminates overlapping subproblems
    */
    private static Dictionary<int, long> fibMemo = new Dictionary<int, long>();

    public static long FibonacciMemoized(int n)
    {
        // Check if result already computed
        if (fibMemo.ContainsKey(n))
            return fibMemo[n];

        // Base cases
        if (n <= 1)
        {
            fibMemo[n] = n;
            return n;
        }

        // Compute and store result
        long result = FibonacciMemoized(n - 1) + FibonacciMemoized(n - 2);
        fibMemo[n] = result;
        return result;
    }

    // Solution 1c: Iterative DP (Bottom-up)
    /*
    Time: O(n) - single pass calculation
    Space: O(1) - only stores last two values
    Optimal: No recursion overhead, minimal memory
    */
    public static long FibonacciIterative(int n)
    {
        if (n <= 1) return n;

        // Only need to track last two values
        long prev2 = 0; // F(0)
        long prev1 = 1; // F(1)

        for (int i = 2; i <= n; i++)
        {
            long current = prev1 + prev2;
            prev2 = prev1;
            prev1 = current;
        }

        return prev1;
    }
}
```

4. Binary Tree Diameter
```csharp
    public class TreeNode
    {
        public int val;
        public TreeNode left;
        public TreeNode right;
        public TreeNode(int val = 0, TreeNode left = null, TreeNode right = null)
        {
            this.val = val;
            this.left = left;
            this.right = right;
        }
    }
public class TreeRecursionSolutions
{
    // Solution 2a: Naive Approach - O(n^2)
    public static int DiameterOfBinaryTree(TreeNode root)
    {
        if (root == null) return 0;

        // Three possibilities for diameter:
        // 1. Diameter is in left subtree
        int leftDiameter = DiameterOfBinaryTree(root.left);

        // 2. Diameter is in right subtree
        int rightDiameter = DiameterOfBinaryTree(root.right);

        // 3. Diameter passes through root (left_height + right_height)
        int heightThroughRoot = GetHeight(root.left) + GetHeight(root.right);

        return Math.Max(Math.Max(leftDiameter, rightDiameter), heightThroughRoot);
    }

    private static int GetHeight(TreeNode node)
    {
        if (node == null) return 0;
        return 1 + Math.Max(GetHeight(node.left), GetHeight(node.right));
    }

    // Solution 2b: Optimized Approach - O(n)
    private static int maxDiameter = 0;

    public static int DiameterOptimized(TreeNode root)
    {
        maxDiameter = 0;
        GetHeightAndUpdateDiameter(root);
        return maxDiameter;
    }

    private static int GetHeightAndUpdateDiameter(TreeNode node)
    {
        if (node == null) return 0;

        // Get heights of left and right subtrees
        int leftHeight = GetHeightAndUpdateDiameter(node.left);
        int rightHeight = GetHeightAndUpdateDiameter(node.right);

        // Update global diameter if path through current node is longer
        int currentDiameter = leftHeight + rightHeight;
        maxDiameter = Math.Max(maxDiameter, currentDiameter);

        // Return height of current subtree
        return 1 + Math.Max(leftHeight, rightHeight);
    }

    // Enhanced version with detailed tracing
    public static int DiameterWithTrace(TreeNode root)
    {
        maxDiameter = 0;
        Console.WriteLine("=== COMPUTING TREE DIAMETER ===");
        int height = GetHeightWithTrace(root, 0);
        Console.WriteLine($"Final diameter: {maxDiameter}");
        return maxDiameter;
    }

    private static int GetHeightWithTrace(TreeNode node, int depth)
    {
        string indent = new string(' ', depth * 2);

        if (node == null)
        {
            Console.WriteLine($"{indent}→ null node: height = 0");
            return 0;
        }

        Console.WriteLine($"{indent}→ Node {node.val}");

        int leftHeight = GetHeightWithTrace(node.left, depth + 1);
        int rightHeight = GetHeightWithTrace(node.right, depth + 1);

        int currentDiameter = leftHeight + rightHeight;
        int oldMaxDiameter = maxDiameter;
        maxDiameter = Math.Max(maxDiameter, currentDiameter);

        int height = 1 + Math.Max(leftHeight, rightHeight);

        Console.WriteLine($"{indent}← Node {node.val}: height={height}, diameter_through_node={currentDiameter}");
        if (maxDiameter > oldMaxDiameter)
            Console.WriteLine($"{indent}  ★ New max diameter: {maxDiameter}");

        return height;
    }
}
```


5. Generate All Binary Strings

```csharp
public class TreeRecursionSolutions
{
    // Solution 3: Binary Strings with No Consecutive 1's - Tree Recursion with Constraints
    public static List<string> GenerateValidBinaryStrings(int n)
    {
        var result = new List<string>();
        GenerateBinaryHelper("", n, result);
        return result;
    }

    private static void GenerateBinaryHelper(string current, int remaining, List<string> result)
    {
        // Base case: string is complete
        if (remaining == 0)
        {
            result.Add(current);
            return;
        }

        // Always can add '0'
        GenerateBinaryHelper(current + "0", remaining - 1, result);

        // Can add '1' only if last character wasn't '1' (or string is empty)
        if (current.Length == 0 || current[current.Length - 1] != '1')
        {
            GenerateBinaryHelper(current + "1", remaining - 1, result);
        }
    }

    // Enhanced version with detailed tracing
    public static List<string> GenerateValidBinaryStringsTrace(int n)
    {
        var result = new List<string>();
        Console.WriteLine($"=== GENERATING BINARY STRINGS OF LENGTH {n} ===");
        GenerateBinaryTrace("", n, result, 0);
        Console.WriteLine($"Generated {result.Count} valid strings");
        return result;
    }

    private static void GenerateBinaryTrace(string current, int remaining, List<string> result, int depth)
    {
        string indent = new string(' ', depth * 2);
        Console.WriteLine($"{indent}→ Current: \"{current}\", Remaining: {remaining}");

        // Base case
        if (remaining == 0)
        {
            Console.WriteLine($"{indent}← Complete string: \"{current}\"");
            result.Add(current);
            return;
        }

        // Branch 1: Add '0' (always allowed)
        Console.WriteLine($"{indent}  Branch 1: Adding '0'");
        GenerateBinaryTrace(current + "0", remaining - 1, result, depth + 1);

        // Branch 2: Add '1' (conditional)
        bool canAddOne = current.Length == 0 || current[current.Length - 1] != '1';
        if (canAddOne)
        {
            Console.WriteLine($"{indent}  Branch 2: Adding '1'");
            GenerateBinaryTrace(current + "1", remaining - 1, result, depth + 1);
        }
        else
        {
            Console.WriteLine($"{indent}  Branch 2: PRUNED (last char is '1')");
        }

        Console.WriteLine($"{indent}← Backtrack from \"{current}\"");
    }

    // Mathematical analysis: Count without generating
    public static int CountValidBinaryStrings(int n)
    {
        if (n <= 0) return 1; // Empty string
        if (n == 1) return 2; // "0" and "1"

        // Let f(n) = number of valid strings of length n
        // f(n) = f(n-1) + f(n-2)
        // This is because:
        // - If string ends with '0', previous part can be any valid string of length n-1
        // - If string ends with '1', previous part must end with '0', so it's any valid string of length n-2 + '0'

        int prev2 = 1; // f(0) = 1
        int prev1 = 2; // f(1) = 2

        for (int i = 2; i <= n; i++)
        {
            int current = prev1 + prev2;
            prev2 = prev1;
            prev1 = current;
        }

        return prev1;
    }
}


Tree Structure:
                    ""
                   /  \
                 "0"   "1"
                / \     / \
              "00" "01" "10" "11" (PRUNED)
             / |   / |   / |
           "000""001""010""011""100""101"
                     (PRUNED)      (PRUNED)

Valid strings: ["000", "001", "010", "100", "101"]
Count: 5
```

6. Expression Tree Evaluation
```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class IndirectRecursion
{
    // Example 1: Expression Tree Evaluation (Indirect Recursion)
    public class ExpressionNode
    {
        public string Value { get; set; }
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }

        public ExpressionNode(string value, ExpressionNode left = null, ExpressionNode right = null)
        {
            Value = value;
            Left = left;
            Right = right;
        }
    }

    // Indirect recursion: The data structure (tree) determines the recursive pattern
    public static double EvaluateExpression(ExpressionNode node)
    {
        if (node == null)
            throw new ArgumentNullException("Expression node cannot be null");

        // Base case: leaf node (number)
        if (node.Left == null && node.Right == null)
        {
            if (double.TryParse(node.Value, out double number))
                return number;
            throw new ArgumentException($"Invalid number: {node.Value}");
        }

        // Recursive case: operator node
        // The recursion is "indirect" - driven by tree structure, not explicit function calls
        double leftValue = EvaluateExpression(node.Left);   // Recursion through LEFT child
        double rightValue = EvaluateExpression(node.Right); // Recursion through RIGHT child

        // Apply operator
        switch (node.Value)
        {
            case "+": return leftValue + rightValue;
            case "-": return leftValue - rightValue;
            case "*": return leftValue * rightValue;
            case "/":
                if (rightValue == 0) throw new DivideByZeroException();
                return leftValue / rightValue;
            default:
                throw new ArgumentException($"Unknown operator: {node.Value}");
        }
    }

    // Enhanced version with tracing to show indirect recursion
    public static double EvaluateExpressionTrace(ExpressionNode node, int depth = 0)
    {
        string indent = new string(' ', depth * 2);
        Console.WriteLine($"{indent}→ Evaluating node: '{node?.Value ?? "null"}'");

        if (node == null)
        {
            Console.WriteLine($"{indent}← Error: null node");
            throw new ArgumentNullException("Expression node cannot be null");
        }

        // Base case: leaf node (number)
        if (node.Left == null && node.Right == null)
        {
            if (double.TryParse(node.Value, out double number))
            {
                Console.WriteLine($"{indent}← Leaf node result: {number}");
                return number;
            }
            throw new ArgumentException($"Invalid number: {node.Value}");
        }

        // Recursive case: operator node
        Console.WriteLine($"{indent}  Operator '{node.Value}' - evaluating children");

        Console.WriteLine($"{indent}  Evaluating LEFT child:");
        double leftValue = EvaluateExpressionTrace(node.Left, depth + 1);

        Console.WriteLine($"{indent}  Evaluating RIGHT child:");
        double rightValue = EvaluateExpressionTrace(node.Right, depth + 1);

        double result;
        switch (node.Value)
        {
            case "+":
                result = leftValue + rightValue;
                Console.WriteLine($"{indent}← {leftValue} + {rightValue} = {result}");
                return result;
            case "-":
                result = leftValue - rightValue;
                Console.WriteLine($"{indent}← {leftValue} - {rightValue} = {result}");
                return result;
            case "*":
                result = leftValue * rightValue;
                Console.WriteLine($"{indent}← {leftValue} * {rightValue} = {result}");
                return result;
            case "/":
                if (rightValue == 0) throw new DivideByZeroException();
                result = leftValue / rightValue;
                Console.WriteLine($"{indent}← {leftValue} / {rightValue} = {result}");
                return result;
            default:
                throw new ArgumentException($"Unknown operator: {node.Value}");
        }
    }
}
```

```

       *
      / \
     +   2
    / \
   3   4

EvaluateExpression("*")
├── EvaluateExpression("+")     ← Recursion through LEFT pointer
│   ├── EvaluateExpression("3") ← Base case: return 3
│   └── EvaluateExpression("4") ← Base case: return 4
│   └── Result: 3 + 4 = 7
└── EvaluateExpression("2")     ← Recursion through RIGHT pointer
    └── Base case: return 2
└── Result: 7 * 2 = 14
```

7. Grid Path Counting

```csharp
public class MultipleParameterRecursion
{
    // Example 1: Grid Path Counting (Multiple Parameters)
    public static int CountPaths(int[][] grid, int row, int col, int targetRow, int targetCol)
    {
        // Base cases
        if (row < 0 || row >= grid.Length || col < 0 || col >= grid[0].Length)
            return 0; // Out of bounds

        if (grid[row][col] == 1)
            return 0; // Obstacle

        if (row == targetRow && col == targetCol)
            return 1; // Reached destination

        // Recursive case: sum paths from all possible moves
        // Multiple parameters: (row, col) represents current state
        return CountPaths(grid, row + 1, col, targetRow, targetCol) +     // Down
               CountPaths(grid, row - 1, col, targetRow, targetCol) +     // Up
               CountPaths(grid, row, col + 1, targetRow, targetCol) +     // Right
               CountPaths(grid, row, col - 1, targetRow, targetCol);      // Left
    }

    // Enhanced version with path tracking and visited state
    public static List<List<(int, int)>> FindAllPaths(int[][] grid, int startRow, int startCol,
                                                      int targetRow, int targetCol)
    {
        var allPaths = new List<List<(int, int)>>();
        var currentPath = new List<(int, int)>();
        var visited = new bool[grid.Length, grid[0].Length];

        FindAllPathsHelper(grid, startRow, startCol, targetRow, targetCol,
                          currentPath, allPaths, visited);

        return allPaths;
    }

    private static void FindAllPathsHelper(int[][] grid, int row, int col,
                                          int targetRow, int targetCol,
                                          List<(int, int)> currentPath,
                                          List<List<(int, int)>> allPaths,
                                          bool[,] visited)
    {
        // Boundary checks
        if (row < 0 || row >= grid.Length || col < 0 || col >= grid[0].Length)
            return;

        // Obstacle or already visited
        if (grid[row][col] == 1 || visited[row, col])
            return;

        // Add current position to path
        currentPath.Add((row, col));
        visited[row, col] = true;

        // Check if we reached the target
        if (row == targetRow && col == targetCol)
        {
            // Found a complete path - add copy to results
            allPaths.Add(new List<(int, int)>(currentPath));
        }
        else
        {
            // Continue exploring in all directions
            // Multiple recursive calls with different parameters
            FindAllPathsHelper(grid, row + 1, col, targetRow, targetCol, currentPath, allPaths, visited);
            FindAllPathsHelper(grid, row - 1, col, targetRow, targetCol, currentPath, allPaths, visited);
            FindAllPathsHelper(grid, row, col + 1, targetRow, targetCol, currentPath, allPaths, visited);
            FindAllPathsHelper(grid, row, col - 1, targetRow, targetCol, currentPath, allPaths, visited);
        }

        // Backtrack: remove current position and mark as unvisited
        currentPath.RemoveAt(currentPath.Count - 1);
        visited[row, col] = false;
    }

    // Example 2: Matrix Region Processing (3D-like recursion with state)
    public static void FloodFill(int[][] matrix, int row, int col, int newColor, int originalColor)
    {
        // Base cases
        if (row < 0 || row >= matrix.Length || col < 0 || col >= matrix[0].Length)
            return; // Out of bounds

        if (matrix[row][col] != originalColor)
            return; // Not the target color

        if (matrix[row][col] == newColor)
            return; // Already the new color

        // Change current cell
        matrix[row][col] = newColor;

        // Recursively fill connected cells (4-directional)
        FloodFill(matrix, row + 1, col, newColor, originalColor); // Down
        FloodFill(matrix, row - 1, col, newColor, originalColor); // Up
        FloodFill(matrix, row, col + 1, newColor, originalColor); // Right
        FloodFill(matrix, row, col - 1, newColor, originalColor); // Left
    }

    // Enhanced flood fill with area counting
    public static int FloodFillWithCount(int[][] matrix, int row, int col, int newColor, int originalColor)
    {
        // Base cases
        if (row < 0 || row >= matrix.Length || col < 0 || col >= matrix[0].Length)
            return 0;

        if (matrix[row][col] != originalColor || matrix[row][col] == newColor)
            return 0;

        // Change current cell
        matrix[row][col] = newColor;
        int count = 1; // Count current cell

        // Recursively count and fill connected cells
        count += FloodFillWithCount(matrix, row + 1, col, newColor, originalColor);
        count += FloodFillWithCount(matrix, row - 1, col, newColor, originalColor);
        count += FloodFillWithCount(matrix, row, col + 1, newColor, originalColor);
        count += FloodFillWithCount(matrix, row, col - 1, newColor, originalColor);

        return count;
    }
}
```

8. Longest Common Subsequence
```csharp
using System;
using System.Collections.Generic;

public class RecursionOptimizationSolutions
{
    // NAIVE SOLUTION: O(2^(m+n)) - Exponential Disaster
    public static int LongestCommonSubsequenceNaive(string text1, string text2)
    {
        return LongestCommonSubsequenceHelper(text1, text2, 0, 0);
    }

    private static int LongestCommonSubsequenceHelper(string text1, string text2, int i, int j)
    {
        // Base cases: if either string is exhausted, no more common characters
        if (i >= text1.Length || j >= text2.Length)
            return 0;

        // If characters match, include this character and continue with both strings
        if (text1[i] == text2[j])
        {
            return 1 + LongestCommonSubsequenceHelper(text1, text2, i + 1, j + 1);
        }

        // If characters don't match, try skipping from either string and take maximum
        int skipText1 = LongestCommonSubsequenceHelper(text1, text2, i + 1, j);
        int skipText2 = LongestCommonSubsequenceHelper(text1, text2, i, j + 1);

        return Math.Max(skipText1, skipText2);
    }

    // MEMOIZED SOLUTION: O(m×n) - Polynomial Optimization
    private static Dictionary<string, int> lcsMemo = new Dictionary<string, int>();

    public static int LongestCommonSubsequenceMemoized(string text1, string text2)
    {
        lcsMemo.Clear(); // Reset memoization cache
        return LongestCommonSubsequenceMemoizedHelper(text1, text2, 0, 0);
    }

    private static int LongestCommonSubsequenceMemoizedHelper(string text1, string text2, int i, int j)
    {
        // Create cache key from both indices
        string key = $"{i}_{j}";

        // Check if result already computed
        if (lcsMemo.ContainsKey(key))
            return lcsMemo[key];

        int result;

        // Same logic as naive version, but with caching
        if (i >= text1.Length || j >= text2.Length)
        {
            result = 0;
        }
        else if (text1[i] == text2[j])
        {
            result = 1 + LongestCommonSubsequenceMemoizedHelper(text1, text2, i + 1, j + 1);
        }
        else
        {
            int skipText1 = LongestCommonSubsequenceMemoizedHelper(text1, text2, i + 1, j);
            int skipText2 = LongestCommonSubsequenceMemoizedHelper(text1, text2, i, j + 1);
            result = Math.Max(skipText1, skipText2);
        }

        // Store result in cache before returning
        lcsMemo[key] = result;
        return result;
    }
}
```


9.Island Counting
```csharp
public class GridAlgorithmsSolutions
{
    // Solution 2a: Island Counting with Multiple Parameter Recursion
    public static int CountIslands(int[][] grid, bool includeDiagonal = false)
    {
        if (grid == null || grid.Length == 0 || grid[0].Length == 0)
            return 0;

        int rows = grid.Length;
        int cols = grid[0].Length;
        bool[,] visited = new bool[rows, cols];
        int islandCount = 0;

        // Scan every cell in the grid
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // If we find an unvisited land cell, it's a new island
                if (grid[i][j] == 1 && !visited[i, j])
                {
                    islandCount++;
                    ExploreIsland(grid, i, j, visited, includeDiagonal);
                }
            }
        }

        return islandCount;
    }

    private static void ExploreIsland(int[][] grid, int row, int col, bool[,] visited, bool includeDiagonal)
    {
        int rows = grid.Length;
        int cols = grid[0].Length;

        // Base cases
        if (row < 0 || row >= rows || col < 0 || col >= cols)
            return; // Out of bounds

        if (visited[row, col] || grid[row][col] != 1)
            return; // Already visited or water

        // Mark as visited
        visited[row, col] = true;

        // Explore all connected land cells (multiple parameter recursion)
        // 4-directional movement
        ExploreIsland(grid, row + 1, col, visited, includeDiagonal); // Down
        ExploreIsland(grid, row - 1, col, visited, includeDiagonal); // Up
        ExploreIsland(grid, row, col + 1, visited, includeDiagonal); // Right
        ExploreIsland(grid, row, col - 1, visited, includeDiagonal); // Left

        // 8-directional movement (include diagonals)
        if (includeDiagonal)
        {
            ExploreIsland(grid, row + 1, col + 1, visited, includeDiagonal); // Down-Right
            ExploreIsland(grid, row + 1, col - 1, visited, includeDiagonal); // Down-Left
            ExploreIsland(grid, row - 1, col + 1, visited, includeDiagonal); // Up-Right
            ExploreIsland(grid, row - 1, col - 1, visited, includeDiagonal); // Up-Left
        }
    }
}
```
10.  Maximal Rectangle ad World Search
```csharp
// ...existing code...

public class GridAlgorithmsSolutions
{
    // Solution 2a: Island Counting with Multiple Parameter Recursion
    public static int CountIslands(int[][] grid, bool includeDiagonal = false)
    {
        if (grid == null || grid.Length == 0 || grid[0].Length == 0)
            return 0;

        int rows = grid.Length;
        int cols = grid[0].Length;
        bool[,] visited = new bool[rows, cols];
        int islandCount = 0;

        // Scan every cell in the grid
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // If we find an unvisited land cell, it's a new island
                if (grid[i][j] == 1 && !visited[i, j])
                {
                    islandCount++;
                    ExploreIsland(grid, i, j, visited, includeDiagonal);
                }
            }
        }

        return islandCount;
    }

    private static void ExploreIsland(int[][] grid, int row, int col, bool[,] visited, bool includeDiagonal)
    {
        int rows = grid.Length;
        int cols = grid[0].Length;

        // Base cases
        if (row < 0 || row >= rows || col < 0 || col >= cols)
            return; // Out of bounds

        if (visited[row, col] || grid[row][col] != 1)
            return; // Already visited or water

        // Mark as visited
        visited[row, col] = true;

        // Explore all connected land cells (multiple parameter recursion)
        // 4-directional movement
        ExploreIsland(grid, row + 1, col, visited, includeDiagonal); // Down
        ExploreIsland(grid, row - 1, col, visited, includeDiagonal); // Up
        ExploreIsland(grid, row, col + 1, visited, includeDiagonal); // Right
        ExploreIsland(grid, row, col - 1, visited, includeDiagonal); // Left

        // 8-directional movement (include diagonals)
        if (includeDiagonal)
        {
            ExploreIsland(grid, row + 1, col + 1, visited, includeDiagonal); // Down-Right
            ExploreIsland(grid, row + 1, col - 1, visited, includeDiagonal); // Down-Left
            ExploreIsland(grid, row - 1, col + 1, visited, includeDiagonal); // Up-Right
            ExploreIsland(grid, row - 1, col - 1, visited, includeDiagonal); // Up-Left
        }
    }

    // Solution 2b: Largest Rectangle in Grid
    public static int FindLargestRectangle(int[][] grid)
    {
        if (grid == null || grid.Length == 0 || grid[0].Length == 0)
            return 0;

        int rows = grid.Length;
        int cols = grid[0].Length;
        int maxArea = 0;

        // Try every possible rectangle starting position
        for (int startRow = 0; startRow < rows; startRow++)
        {
            for (int startCol = 0; startCol < cols; startCol++)
            {
                if (grid[startRow][startCol] == 1)
                {
                    // Find largest rectangle starting from this position
                    int area = FindLargestRectangleFrom(grid, startRow, startCol);
                    maxArea = Math.Max(maxArea, area);
                }
            }
        }

        return maxArea;
    }

    private static int FindLargestRectangleFrom(int[][] grid, int startRow, int startCol)
    {
        int rows = grid.Length;
        int cols = grid[0].Length;
        int maxArea = 0;

        // Try all possible rectangle sizes from this starting position
        for (int endRow = startRow; endRow < rows; endRow++)
        {
            for (int endCol = startCol; endCol < cols; endCol++)
            {
                // Check if rectangle from (startRow, startCol) to (endRow, endCol) contains all 1s
                if (IsValidRectangle(grid, startRow, startCol, endRow, endCol))
                {
                    int width = endCol - startCol + 1;
                    int height = endRow - startRow + 1;
                    int area = width * height;
                    maxArea = Math.Max(maxArea, area);
                }
                else
                {
                    break; // No need to extend further in this row
                }
            }
        }

        return maxArea;
    }

    private static bool IsValidRectangle(int[][] grid, int startRow, int startCol, int endRow, int endCol)
    {
        for (int i = startRow; i <= endRow; i++)
        {
            for (int j = startCol; j <= endCol; j++)
            {
                if (grid[i][j] != 1)
                    return false;
            }
        }
        return true;
    }

    // Solution 2c: Grid Word Search with Backtracking
    public static bool WordSearch(char[][] board, string word)
    {
        if (board == null || board.Length == 0 || board[0].Length == 0)
            return false;

        if (string.IsNullOrEmpty(word))
            return true;

        int rows = board.Length;
        int cols = board[0].Length;

        // Try starting from every cell
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (board[i][j] == word[0])
                {
                    bool[,] visited = new bool[rows, cols];
                    if (WordSearchHelper(board, word, i, j, 0, visited))
                        return true;
                }
            }
        }

        return false;
    }

    private static bool WordSearchHelper(char[][] board, string word, int row, int col,
                                        int wordIndex, bool[,] visited)
    {
        // Base case: found complete word
        if (wordIndex == word.Length)
            return true;

        // Base cases: invalid position or constraints
        if (row < 0 || row >= board.Length || col < 0 || col >= board[0].Length)
            return false;

        if (visited[row, col])
            return false;

        if (board[row][col] != word[wordIndex])
            return false;

        // Mark current cell as visited
        visited[row, col] = true;

        // Try all 4 directions (multiple parameter recursion with backtracking)
        bool found = WordSearchHelper(board, word, row + 1, col, wordIndex + 1, visited) || // Down
                     WordSearchHelper(board, word, row - 1, col, wordIndex + 1, visited) || // Up
                     WordSearchHelper(board, word, row, col + 1, wordIndex + 1, visited) || // Right
                     WordSearchHelper(board, word, row, col - 1, wordIndex + 1, visited);   // Left

        // Backtrack: unmark current cell
        visited[row, col] = false;

        return found;
    }

    // Enhanced version with path tracking
    public static List<(int, int)> WordSearchWithPath(char[][] board, string word)
    {
        if (board == null || board.Length == 0 || board[0].Length == 0)
            return new List<(int, int)>();

        if (string.IsNullOrEmpty(word))
            return new List<(int, int)>();

        int rows = board.Length;
        int cols = board[0].Length;

        // Try starting from every cell
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (board[i][j] == word[0])
                {
                    bool[,] visited = new bool[rows, cols];
                    var path = new List<(int, int)>();

                    if (WordSearchHelperWithPath(board, word, i, j, 0, visited, path))
                        return path;
                }
            }
        }

        return new List<(int, int)>();
    }

    private static bool WordSearchHelperWithPath(char[][] board, string word, int row, int col,
                                                int wordIndex, bool[,] visited, List<(int, int)> path)
    {
        // Base case: found complete word
        if (wordIndex == word.Length)
            return true;

        // Base cases: invalid position or constraints
        if (row < 0 || row >= board.Length || col < 0 || col >= board[0].Length)
            return false;

        if (visited[row, col])
            return false;

        if (board[row][col] != word[wordIndex])
            return false;

        // Mark current cell as visited and add to path
        visited[row, col] = true;
        path.Add((row, col));

        // Try all 4 directions
        bool found = WordSearchHelperWithPath(board, word, row + 1, col, wordIndex + 1, visited, path) ||
                     WordSearchHelperWithPath(board, word, row - 1, col, wordIndex + 1, visited, path) ||
                     WordSearchHelperWithPath(board, word, row, col + 1, wordIndex + 1, visited, path) ||
                     WordSearchHelperWithPath(board, word, row, col - 1, wordIndex + 1, visited, path);

        // Backtrack if path not found
        if (!found)
        {
            visited[row, col] = false;
            path.RemoveAt(path.Count - 1);
        }

        return found;
    }
}
```

11. Collect All Valid Combinations with Global Results
```csharp

    // Solution 3b: Collect All Valid Combinations with Global Results
    public static List<List<int>> GenerateAllCombinations(int[] nums, int target)
    {
        globalResults = new List<List<int>>(); // Reset global results
        GenerateCombinationsHelper(nums, target, 0, 0, new List<int>());
        return globalResults;
    }

    private static void GenerateCombinationsHelper(int[] nums, int target, int index,
                                                  int currentSum, List<int> currentCombination)
    {
        // Base case: sum equals target - found valid combination
        if (currentSum == target)
        {
            globalResults.Add(new List<int>(currentCombination)); // Add copy to global results
            return;
        }

        // Base case: sum exceeds target or no more numbers
        if (currentSum > target || index >= nums.Length)
            return;

        // Choice 1: Include current number (multiple parameter recursion)
        currentCombination.Add(nums[index]);
        GenerateCombinationsHelper(nums, target, index + 1, currentSum + nums[index], currentCombination);
        currentCombination.RemoveAt(currentCombination.Count - 1); // Backtrack

        // Choice 2: Skip current number
        GenerateCombinationsHelper(nums, target, index + 1, currentSum, currentCombination);
    }

    // Alternative: Allow repeated use of numbers
    public static List<List<int>> GenerateAllCombinationsWithRepetition(int[] nums, int target)
    {
        globalResults = new List<List<int>>();
        GenerateCombinationsWithRepetitionHelper(nums, target, 0, 0, new List<int>());
        return globalResults;
    }

    private static void GenerateCombinationsWithRepetitionHelper(int[] nums, int target, int index,
                                                               int currentSum, List<int> currentCombination)
    {
        if (currentSum == target)
        {
            globalResults.Add(new List<int>(currentCombination));
            return;
        }

        if (currentSum > target || index >= nums.Length)
            return;

        // For each remaining number, try using it multiple times
        for (int i = index; i < nums.length; i++)
        {
            // Include current number and stay at same index (allow repetition)
            currentCombination.Add(nums[i]);
            GenerateCombinationsWithRepetitionHelper(nums, target, i, currentSum + nums[i], currentCombination);
            currentCombination.RemoveAt(currentCombination.Count - 1); // Backtrack
        }
    }
```

## Advanced Memoization Techniques
### Pattern 1: Multi-Dimensional Memoization
``` csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class MultiDimensionalMemoization
{
    // Example 1: Grid Path Counting - From O(4^(m×n)) to O(m×n)

    // NAIVE RECURSION: Exponential disaster
    public static long CountPathsNaive(int[][] grid, int row, int col, int targetRow, int targetCol)
    {
        // Base cases
        if (row < 0 || row >= grid.Length || col < 0 || col >= grid[0].Length)
            return 0; // Out of bounds

        if (grid[row][col] == 1)
            return 0; // Obstacle

        if (row == targetRow && col == targetCol)
            return 1; // Reached destination

        // Exponential explosion: 4 recursive calls per cell
        return CountPathsNaive(grid, row + 1, col, targetRow, targetCol) +     // Down
               CountPathsNaive(grid, row - 1, col, targetRow, targetCol) +     // Up
               CountPathsNaive(grid, row, col + 1, targetRow, targetCol) +     // Right
               CountPathsNaive(grid, row, col - 1, targetRow, targetCol);      // Left
    }

    // MEMOIZED VERSION: Linear optimization
    private static Dictionary<string, long> pathMemo = new Dictionary<string, long>();

    public static long CountPathsMemoized(int[][] grid, int row, int col, int targetRow, int targetCol)
    {
        // Create cache key from multiple parameters
        string key = $"{row}_{col}_{targetRow}_{targetCol}";

        // Check cache first
        if (pathMemo.ContainsKey(key))
            return pathMemo[key];

        long result;

        // Same logic as naive version
        if (row < 0 || row >= grid.Length || col < 0 || col >= grid[0].Length)
        {
            result = 0;
        }
        else if (grid[row][col] == 1)
        {
            result = 0;
        }
        else if (row == targetRow && col == targetCol)
        {
            result = 1;
        }
        else
        {
            // Same recursive structure, but results are cached
            result = CountPathsMemoized(grid, row + 1, col, targetRow, targetCol) +
                     CountPathsMemoized(grid, row - 1, col, targetRow, targetCol) +
                     CountPathsMemoized(grid, row, col + 1, targetRow, targetCol) +
                     CountPathsMemoized(grid, row, col - 1, targetRow, targetCol);
        }

        // Cache result before returning
        pathMemo[key] = result;
        return result;
    }

    public static void ResetPathMemo()
    {
        pathMemo.Clear();
    }

    // Alternative: 2D Array Memoization (when parameters are bounded integers)
    private static long[,] pathMemo2D;
    private static bool[,] computed;

    public static long CountPathsArray2D(int[][] grid, int row, int col, int targetRow, int targetCol)
    {
        // Initialize memo arrays on first call
        if (pathMemo2D == null)
        {
            int maxRows = grid.Length;
            int maxCols = grid[0].Length;
            pathMemo2D = new long[maxRows, maxCols];
            computed = new bool[maxRows, maxCols];
        }

        // Check if already computed
        if (computed[row, col])
            return pathMemo2D[row, col];

        long result;

        // Base cases (same as before)
        if (row < 0 || row >= grid.Length || col < 0 || col >= grid[0].Length)
        {
            result = 0;
        }
        else if (grid[row][col] == 1)
        {
            result = 0;
        }
        else if (row == targetRow && col == targetCol)
        {
            result = 1;
        }
        else
        {
            result = CountPathsArray2D(grid, row + 1, col, targetRow, targetCol) +
                     CountPathsArray2D(grid, row - 1, col, targetRow, targetCol) +
                     CountPathsArray2D(grid, row, col + 1, targetRow, targetCol) +
                     CountPathsArray2D(grid, row, col - 1, targetRow, targetCol);
        }

        // Store result and mark as computed
        pathMemo2D[row, col] = result;
        computed[row, col] = true;

        return result;
    }
}
```

### Pattern 2: Complex State Memoization
```csharp
public class ComplexStateMemoization
{
    // Example: Combination Sum with Complex State
    public class CombinationState
    {
        public int Index { get; set; }
        public int RemainingSum { get; set; }
        public bool CanRepeat { get; set; }

        // Override Equals and GetHashCode for Dictionary keys
        public override bool Equals(object obj)
        {
            if (obj is CombinationState other)
            {
                return Index == other.Index &&
                       RemainingSum == other.RemainingSum &&
                       CanRepeat == other.CanRepeat;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Index, RemainingSum, CanRepeat);
        }

        public override string ToString()
        {
            return $"({Index},{RemainingSum},{CanRepeat})";
        }
    }

    // Memoization with complex state objects
    private static Dictionary<CombinationState, int> complexMemo = new Dictionary<CombinationState, int>();

    public static int CountCombinations(int[] nums, int target, bool canRepeat = false)
    {
        complexMemo.Clear();
        return CountCombinationsHelper(nums, 0, target, canRepeat);
    }

    private static int CountCombinationsHelper(int[] nums, int index, int remainingSum, bool canRepeat)
    {
        var state = new CombinationState
        {
            Index = index,
            RemainingSum = remainingSum,
            CanRepeat = canRepeat
        };

        // Check memoization
        if (complexMemo.ContainsKey(state))
            return complexMemo[state];

        int result;

        // Base cases
        if (remainingSum == 0)
        {
            result = 1; // Found valid combination
        }
        else if (remainingSum < 0 || index >= nums.Length)
        {
            result = 0; // Invalid state
        }
        else
        {
            // Include current number
            int include = CountCombinationsHelper(nums, canRepeat ? index : index + 1,
                                                 remainingSum - nums[index], canRepeat);

            // Skip current number
            int skip = CountCombinationsHelper(nums, index + 1, remainingSum, canRepeat);

            result = include + skip;
        }

        // Store in memoization table
        complexMemo[state] = result;
        return result;
    }

    public static void DisplayMemoStats()
    {
        Console.WriteLine($"Complex memo entries: {complexMemo.Count}");
        foreach (var kvp in complexMemo.Take(5))
        {
            Console.WriteLine($"  State {kvp.Key} => {kvp.Value}");
        }
    }
}
```



## 🎯 Optimization Progression

### 1. Naive Recursion

* ⏰ **Time:** Often `O(2^n)` or worse
* 💾 **Space:** `O(depth)` recursion stack
* ✅ **Pros:** Easy to understand and write
* ❌ **Cons:** Exponential complexity, stack overflow risk

---

### 2. Memoization (Top-down DP)

* ⏰ **Time:** `O(states)` — polynomial
* 💾 **Space:** `O(states)` + `O(depth)` recursion stack
* ✅ **Pros:** Easy conversion from recursion, only computes needed states
* ❌ **Cons:** Recursion overhead, more complex cache management

---

### 3. Iterative DP (Bottom-up)

* ⏰ **Time:** `O(states)` — same as memoization
* 💾 **Space:** `O(states)` — no recursion stack
* ✅ **Pros:** No recursion overhead, predictable space usage
* ❌ **Cons:** Must compute all states, less intuitive

---

### 4. Space-Optimized DP

* ⏰ **Time:** `O(states)` — same complexity
* 💾 **Space:** `O(1)` to `O(smaller dimension)`
* ✅ **Pros:** Minimal memory usage, production-ready
* ❌ **Cons:** Most complex to implement and debug

---

### 🧭 Optimization Decision Framework

## 🤔 When to Use Each Approach

### 🧩 Use Naive Recursion When:

* Understanding the problem structure
* Input size is small (`n < 20`)
* Prototyping and initial problem solving
* Teaching or explaining recursion

---

### 💡 Use Memoization When:

* Converting existing recursive solution
* Not all states are needed (sparse state space)
* Complex state relationships
* Easier to implement than bottom-up

---

### ⚙️ Use Iterative DP When:

* All states need to be computed anyway
* Want predictable performance
* Avoiding recursion stack limitations
* Production systems with performance requirements

---

### 🧮 Use Space-Optimized DP When:

* Memory is constrained
* Only need the final result, not intermediate states
* Processing very large inputs
* Competitive programming optimization

---

# 🌉 Bridge: Recursion → Dynamic Programming

## 🎓 Recursion Mastery Achieved

* ✅ Direct recursion patterns and complexity analysis
* ✅ Mutual recursion and interdependent function calls
* ✅ Indirect recursion through data structures
* ✅ Multiple parameter recursion for complex state spaces
* ✅ Global state management in recursive algorithms
* ✅ Systematic optimization through memoization

---

## 🔄 The Optimization Journey

`Recursion → Identify overlapping subproblems`
`Memoization → Add caching layer`
`Iterative DP → Remove recursion overhead`
`Optimized DP → Minimize space complexity`

---

## 🎯 Dynamic Programming Readiness

* 📚 Understand optimal substructure principle
* 📚 Identify overlapping subproblems instantly
* 📚 Know when memoization transforms complexity
* 📚 Convert top-down to bottom-up approaches
* 📚 Recognize classic DP problem patterns

---



## 🔗 DP Preview Connections

* **Fibonacci** → Linear DP fundamentals
* **Grid Paths** → 2D DP and coordinate systems
* **LCS / Edit Distance** → String DP patterns
* **Coin Change** → Unbounded knapsack family
* **House Robber** → State machine DP

---

## ✨ Mastery Indicators

* 🎯 Can write any recursive solution with proper base cases
* 🎯 Can identify exponential complexity patterns instantly
* 🎯 Can add memoization to any recursive function
* 🎯 Can analyze when problems have optimal substructure
* 🎯 Can predict which problems benefit from DP





# BACKTRACKING

## Definition
Backtracking is a systematic search technique that builds candidate solutions incrementally and abandons a candidate ("backtracks") as soon as it determines the candidate cannot lead to a valid solution. It is built on recursion plus explicit state management and is commonly used for combinatorial search, constraint satisfaction, and puzzle solving.

### Key characteristics
- Explore all (or many) possible configurations by incremental construction.
- Choose → Explore → Unchoose (choose, recurse, undo choice).
- Heavy use of pruning to reduce search space.
- Finds all solutions or a single valid solution depending on stopping condition.
- Time complexity: often exponential in input size; Space: O(depth) for recursion stack + state.

### Typical pattern
1. Maintain current state (path, partial solution, visited markers, counters).
2. Check base case (complete/invalid).
3. Iterate over choices:
    - Make choice (modify state).
    - Recurse.
    - Undo choice (restore state).

### Common techniques
- Pruning: reject partial solutions early using constraints.
- Ordering/heuristics: try promising choices first.
- Memoization / DP hybrid: cache equivalent states where applicable.
- Constraint propagation: reduce available choices before recursion.

### When to use
- Enumerating combinations/permutations/subsets.
- Solving puzzles (N-Queens, Sudoku, word search).
- Backtracking + pruning outperforms brute force for constrained problems.
- Use DP/memoization when overlapping subproblems dominate.

### Simple C# template
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

## Visualizing Backtracking
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


## Pattern
```csharp
public static class DuplicateHandlingPatterns
{
    // PATTERN 1: Subsets with duplicates
    public static void SubsetsPattern(int[] nums, int start)
    {
        // Key: Sort first, then skip consecutive duplicates
        // Skip condition: i > start && nums[i] == nums[i-1]

        for (int i = start; i < nums.Length; i++)
        {
            if (i > start && nums[i] == nums[i - 1])
                continue; // Skip duplicate

            // Process nums[i]...
        }
    }

    // PATTERN 2: Combinations with duplicates
    public static void CombinationsPattern(int[] nums, int k, int start)
    {
        // Same duplicate handling as subsets
        // Plus size constraint

        for (int i = start; i < nums.Length; i++)
        {
            if (i > start && nums[i] == nums[i - 1])
                continue; // Skip duplicate

            // Process nums[i]...
        }
    }

    // PATTERN 3: Permutations with duplicates
    public static void PermutationsPattern(int[] nums, bool[] used)
    {
        // Different! Need used array AND duplicate check

        for (int i = 0; i < nums.Length; i++)
        {
            if (used[i])
                continue;

            // Skip if same value and previous occurrence not used
            if (i > 0 && nums[i] == nums[i - 1] && !used[i - 1])
                continue; // Skip duplicate

            used[i] = true;
            // Process nums[i]...
            used[i] = false;
        }
    }
}
```

When to Use Each Pattern:
- **Subsets with Duplicates:** When generating all unique subsets from a list that may contain duplicates. Sort the input and skip consecutive duplicates during iteration.
- **Combinations with Duplicates:** When generating combinations of a specific size from a list with duplicates. Similar to subsets, sort and skip duplicates, while also enforcing size constraints.
- **Permutations with Duplicates:** When generating all unique permutations from a list with duplicates. Use a 'used' array to track which elements are included, and skip duplicates based on usage of previous identical elements.


