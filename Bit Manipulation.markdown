# Bit Manipulation in C#

## Definition
Bit manipulation involves performing logical operations on individual bits of data to achieve specific results. It’s a powerful technique in C# for tasks requiring low-level control, optimization, or compact data representation. Bitwise operations are **constant-time (O(1))** and directly supported by hardware, making them fast and efficient.

Bit manipulation is used in:
- **Low-level device control** (e.g., GPIO pins in .NET IoT).
- **Error detection/correction** (e.g., parity checks, CRC).
- **Data compression** (e.g., packing RGB colors).
- **Encryption** (e.g., XOR-based algorithms).
- **Optimization** (e.g., replacing arithmetic with shifts).

This document focuses on C# bitwise operators, their applications, and interview preparation tips.

## Bitwise Operators
C# supports six bitwise operators for integer types (`int`, `uint`, `byte`, etc.). Below is a summary:

| Operator | Name                | Description                                                                 | Example (a=47, b=19)                     |
|----------|---------------------|-----------------------------------------------------------------------------|------------------------------------------|
| `&`      | AND                 | Sets bit to 1 if both bits are 1. Used for masking or checking bits.         | `47 & 19` = `0b00101111 & 0b00010011` = `3` |
| `\|`     | OR                  | Sets bit to 1 if at least one bit is 1. Used to set or combine bits.        | `47 \| 19` = `0b00101111 \| 0b00010011` = `63` |
| `~`      | NOT                 | Inverts all bits (0 to 1, 1 to 0). Affected by signed/unsigned types.        | `~47` = `-48` (32-bit int)              |
| `^`      | XOR                 | Sets bit to 1 if bits differ. Used for toggling or detecting differences.    | `47 ^ 19` = `0b00101111 ^ 0b00010011` = `60` |
| `<<`     | Left Shift          | Shifts bits left, filling right with 0s. Multiplies by \(2^n\).             | `47 << 2` = `188`                       |
| `>>`     | Right Shift         | Shifts bits right. For `int`, sign bit is preserved; for `uint`, 0s are used. | `47 >> 2` = `11`                        |

## Operations

### Right Shift (`>>`)
Shifts bits right by \(n\) positions. For `int`, it’s an **arithmetic shift** (sign bit is copied to preserve sign). For `uint`, it’s a **logical shift** (0s are shifted in).

- **Effect**: Equivalent to integer division by \(2^n\).
- **Example**:
  ```csharp
  int num = 8; // 0b00001000
  int result = num >> 1; // 0b00000100 (4)
  Console.WriteLine(result); // 4
  uint unsignedNum = 8; // 0b00001000
  uint unsignedResult = unsignedNum >> 2; // 0b00000010 (2)
  Console.WriteLine(unsignedResult); // 2
  ```
- **Edge Case**: Right-shifting a negative `int` preserves the sign bit:
  ```csharp
  int neg = -8; // 0b111...11111000 (32-bit)
  Console.WriteLine(neg >> 1); // -4 (sign bit preserved)
  ```
- **Truth Table** (for single bit):
  | Input | Shift | Output |
  |-------|-------|--------|
  | 0     | >> 1  | 0      |
  | 1     | >> 1  | 0 (unsigned) / 1 (signed, negative) |

### Left Shift (`<<`)
Shifts bits left by \(n\) positions, filling right with 0s. Equivalent to multiplying by \(2^n\).

- **Example**:
  ```csharp
  int x = 10; // 0b00001010
  x = x << 3; // 0b01010000 (80)
  Console.WriteLine(x); // 80
  ```
- **Edge Case**: Shifting too far can cause overflow:
  ```csharp
  int x = 1 << 31; // 0b1000...0000 (int.MaxValue + 1)
  Console.WriteLine(x); // -2147483648 (overflow in signed int)
  ```
- **Truth Table** (for single bit):
  | Input | Shift | Output |
  |-------|-------|--------|
  | 0     | << 1  | 0      |
  | 1     | << 1  | 0 (shifted out, 0 filled) |

### AND (`&`)
Sets a bit to 1 only if both corresponding bits are 1.

- **Uses**:
  1. **Masking**: Extract specific bits.
     ```csharp
     int num = 0b11110101; // 245
     int mask = 0b00001111; // 15
     int result = num & mask; // 0b00000101 (5)
     Console.WriteLine(result); // 5
     ```
  2. **Check Flags**: Verify if a bit is set.
     ```csharp
     int flags = 0b1010; // 10
     bool isSecondBitOn = (flags & 0b0010) != 0;
     Console.WriteLine(isSecondBitOn); // True
     ```
  3. **Even/Odd Check**: Check the least significant bit.
     ```csharp
     int n = 5; // 0b101
     bool isOdd = (n & 1) != 0;
     Console.WriteLine(isOdd); // True
     ```
- **Truth Table**:
  | a (0/1) | b (0/1) | a & b | a (Bool) | b (Bool) | a & b (Bool) |
  |---------|---------|-------|----------|----------|--------------|
  | 0       | 0       | 0     | False    | False    | False        |
  | 0       | 1       | 0     | False    | True     | False        |
  | 1       | 0       | 0     | True     | False    | False        |
  | 1       | 1       | 1     | True     | True     | True         |

### OR (`|`)
Sets a bit to 1 if at least one corresponding bit is 1.

- **Uses**:
  1. **Set Bits**: Turn on specific bits.
     ```csharp
     int x = 0b1000; // 8
     x |= 0b0010; // 0b1010 (10)
     Console.WriteLine(x); // 10
     ```
  2. **Combine Flags**: Merge multiple settings.
     ```csharp
     int READ = 0b100, WRITE = 0b010, EXECUTE = 0b001;
     int permissions = READ | WRITE; // 0b110 (6)
     Console.WriteLine(permissions); // 6
     ```
- **Truth Table**:
  | a (0/1) | b (0/1) | a \| b | a (Bool) | b (Bool) | a \| b (Bool) |
  |---------|---------|--------|----------|----------|---------------|
  | 0       | 0       | 0      | False    | False    | False         |
  | 0       | 1       | 1      | False    | True     | True          |
  | 1       | 0       | 1      | True     | False    | True          |
  | 1       | 1       | 1      | True     | True     | True          |

### XOR (`^`)
Sets a bit to 1 if the bits differ, 0 if they’re the same.

- **Uses**:
  1. **Toggle Bits**: Flip specific bits.
     ```csharp
     int value = 0b1010; // 10
     int mask = 0b0100; // 4
     value ^= mask; // 0b1110 (14)
     Console Union.WriteLine(value); // 14
     ```
  2. **Swap Values**: Swap two integers without a temporary variable.
     ```csharp
     int x = 10, y = 20;
     x ^= y; y ^= x; x ^= y;
     Console.WriteLine($"x={x}, y={y}"); // x=20, y=10
     ```
  3. **Difference Detection**: Find differing bits.
     ```csharp
     int a = 0b1010, b = 0b1100;
     int diff = a ^ b; // 0b0110 (6)
     Console.WriteLine(diff); // 6
     ```
- **Truth Table**:
  | a (0/1) | b (0/1) | a ^ b | a (Bool) | b (Bool) | a ^ b (Bool) |
  |---------|---------|-------|----------|----------|--------------|
  | 0       | 0       | 0     | False    | False    | False        |
  | 0       | 1       | 1     | False    | True     | True         |
  | 1       | 0       | 1     | True     | False    | True         |
  | 1       | 1       | 0     | True     | True     | False        |

### NOT (`~`)
Inverts all bits. For `int`, this affects the sign bit, producing a negative number in two’s complement.

- **Example**:
  ```csharp
  int x = 47; // 0b00101111
  int result = ~x; // 0b11010000... (32-bit: -48)
  Console.WriteLine(result); // -48
  uint ux = 47; // 0b00101111
  uint uresult = ~ux; // 0b111...11010000 (4294967248)
  Console.WriteLine(uresult); // 4294967248
  ```
- **Use**: Clear bits when combined with AND.
  ```csharp
  int flags = 0b1010; // 10
  flags &= ~0b0010; // Clear second bit: 0b1000 (8)
  Console.WriteLine(flags); // 8
  ```
- **Truth Table**:
  | a (0/1) | ~a (0/1) |
  |---------|----------|
  | 0       | 1        |
  | 1       | 0        |

## Applications
Bitwise operators are widely used in C# for:

| Operation | Use Case | Example |
|-----------|----------|---------|
| `&`       | Test bits (e.g., check flags, even/odd) | Check if a permission is set. |
| `\|`      | Set bits (e.g., combine flags) | Grant multiple permissions. |
| `^`       | Toggle bits, swap values, detect differences | Toggle a flag or swap variables. |
| `~`       | Invert bits, clear bits with AND | Clear specific flags. |
| `<<`      | Multiply by \(2^n\), pack data | Scale numbers or encode RGB colors. |
| `>>`      | Divide by \(2^n\), extract bits | Parse network addresses or decode data. |

### C# Example: Managing Permissions
```csharp
[Flags]
public enum Permissions
{
    None = 0,
    Read = 1 << 0,    // 0b0001 (1)
    Write = 1 << 1,   // 0b0010 (2)
    Execute = 1 << 2, // 0b0100 (4)
    Delete = 1 << 3   // 0b1000 (8)
}

Permissions perms = Permissions.Read | Permissions.Execute; // 0b0101 (5)
Console.WriteLine(perms); // Read, Execute
// Check Write permission
bool hasWrite = (perms & Permissions.Write) != 0; // False
Console.WriteLine(hasWrite); // False
// Add Write
perms |= Permissions.Write; // 0b0111 (7)
Console.WriteLine(perms); // Read, Write, Execute
// Remove Write
perms &= ~Permissions.Write; // 0b0101 (5)
Console.WriteLine(perms); // Read, Execute
// Toggle Write
perms ^= Permissions.Write; // 0b0111 (7)
Console.WriteLine(perms); // Read, Write, Execute
```

### Networking Example: Subnet Mask
Calculate the network address from an IP and subnet mask:
```csharp
uint ip = 0xC0A80064; // 192.168.0.100
uint mask = 0xFFFFFF00; // 255.255.255.0
uint network = ip & mask; // 192.168.0.0 (0xC0A80000)
Console.WriteLine($"Network: {network:X8}"); // C0A80000
```

### BitArray Example
Use `System.Collections.BitArray` for high-level bit manipulation:
```csharp
using System.Collections;
BitArray bits = new BitArray(new[] { true, false, true, true }); // 0b1011 (11)
bits[1] = true; // Set bit 1: 0b1111 (15)
Console.WriteLine(bits.Cast<bool>().Aggregate(0, (acc, b) => (acc << 1) | (b ? 1 : 0))); // 15
```

### Advanced Example: Bit Counting
Count the number of 1s in a number (Hamming weight):
```csharp
int CountSetBits(int n)
{
    int count = 0;
    while (n != 0)
    {
        count += n & 1;
        n >>= 1;
    }
    return count;
}
Console.WriteLine(CountSetBits(47)); // 6 (0b00101111 has 6 ones)
```

### Unity Example: Player States
In Unity, manage player states efficiently:
```csharp
[Flags]
public enum PlayerState
{
    None = 0,
    IsAlive = 1 << 0,    // 1
    IsJumping = 1 << 1,  // 2
    HasPowerUp = 1 << 2  // 4
}

PlayerState state = PlayerState.IsAlive | PlayerState.IsJumping; // 0b0011
bool isJumping = (state & PlayerState.IsJumping) != 0; // True
state &= ~PlayerState.IsJumping; // Remove jumping: 0b0001
Console.WriteLine(state); // IsAlive
```

## Common Interview Questions
1. **Check if a Number is a Power of 2**:
   ```csharp
   bool IsPowerOfTwo(int n)
   {
       return n > 0 && (n & (n - 1)) == 0;
   }
   Console.WriteLine(IsPowerOfTwo(8)); // True (0b1000)
   Console.WriteLine(IsPowerOfTwo(10)); // False (0b1010)
   ```
   - **Explanation**: A power of 2 has one bit set. `n & (n-1)` clears the lowest set bit; if 0, it’s a power of 2.

2. **Swap Two Numbers Without a Temporary Variable**:
   ```csharp
   int x = 10, y = 20;
   x ^= y; y ^= x; x ^= y;
   Console.WriteLine($"x={x}, y={y}"); // x=20, y=10
   ```

3. **Reverse Bits of a Number**:
   ```csharp
   uint ReverseBits(uint n)
   {
       uint result = 0;
       for (int i = 0; i < 32; i++)
       {
           result = (result << 1) | (n & 1);
           n >>= 1;
       }
       return result;
   }
   Console.WriteLine(ReverseBits(0b1011)); // 0b1101
   ```

4. **Find the Single Number**:
   Given an array where every number appears twice except one, find the single number:
   ```csharp
   int SingleNumber(int[] nums)
   {
       int result = 0;
       foreach (int num in nums) result ^= num;
       return result;
   }
   Console.WriteLine(SingleNumber(new[] { 4, 1, 2, 1, 2 })); // 4
   ```

## Edge Cases and Interview Tips
- **Signed vs. Unsigned**: Use `uint` for logical operations to avoid sign-bit issues (e.g., `~47` is negative in `int`).
- **Overflow**: Left shifts can overflow `int` or `uint`, causing unexpected results.
- **Common Mistake**: Don’t confuse `&` (bitwise) with `&&` (logical). Bitwise operates on all bits; logical evaluates to a boolean.
- **Interview Tip**: Explain bitwise operations step-by-step with binary examples:
  - Write the binary representation (e.g., `47 = 0b00101111`).
  - Show each bit operation (e.g., `47 & 19 = 0b00101111 & 0b00010011 = 0b00000011`).
  - Verify the decimal result (3).
- **C# Tip**: Use binary literals (`0b`) and `[Flags]` enums for readability. Avoid `unsafe` code unless necessary (e.g., hardware access).

## Cheat Sheet
- **AND (`&`)**: Mask bits, check flags, even/odd (`n & 1`).
- **OR (`|`)**: Set bits, combine flags (`flag1 | flag2`).
- **XOR (`^`)**: Toggle bits, swap values, find unique number.
- **NOT (`~`)**: Invert bits, clear with AND (`n & ~mask`).
- **Left Shift (`<<`)**: Multiply by \(2^n\), pack data.
- **Right Shift (`>>`)**: Divide by \(2^n\), extract bits.
- **Practice**: Test edge cases (negative numbers, large shifts) in a C# console app.
