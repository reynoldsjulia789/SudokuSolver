namespace SudokuSolver;

/// <summary>
/// Solves a sudoku puzzle as a Constraint Satisfaction Problem (CSP), with optimizations inspired by Peter Norvig's 
/// 2026 Java implementation (naked pairs, MRV heuristic): https://github.com/norvig/pytudes/blob/main/ipynb/SudokuJava.ipynb
/// </summary>
public class Sudoku
{
    private const int Size = 9;
    private const int CellCount = Size * Size;
    private const int AllCandidates = 0b1_1111_1111; // digits 1..9


    /// <summary>The 27 units: 9 rows, 9 columns, 9 blocks. Each is a list of 9 cell indices.</summary>
    private static readonly int[][] _units = BuildUnits();

    /// <summary>The 3 units (row, column, block) that each cell belongs to.</summary>
    private static readonly int[][][] _unitsOf = BuildUnitsOf();
    
    /// <summary>The 20 peers of each cell (same row, column, or 3x3 block, excluding the cell itself).</summary>
    private static readonly int[][] _peers = BuildPeers();

    private readonly int[,] _givens;

    public Sudoku(int[,] givens)
    {
        if (givens.GetLength(0) != Size || givens.GetLength(1) != Size)
            throw new ArgumentException("Sudoku: puzzle must be 9x9");

        _givens = (int[,])givens.Clone();
    }

    /// <summary> Returns true if two given (non-zero) digits share a row, column, or block. Such a puzzle has no solution. </summary>
    public bool HasConflictingGivens()
    {
        for (int cell = 0; cell < CellCount; cell++)
        {
            int value = _givens[cell / Size, cell % Size];
            if (value == 0) continue;

            foreach (int peer in _peers[cell])
            {
                if (_givens[peer / Size, peer % Size] == value) return true;
            }
        }
        return false;
    }

    /// <summary> Finds every distinct solution to the puzzle. Returns an empty list if none exist. </summary>
    public List<int[,]> Solve()
    {
        List<int[,]> solutions = [];

        if (HasConflictingGivens()) return solutions;

        // Start every cell with the full domain, then assign each given value. Assigning a given runs constraint
        // propagation, so the domains are already reduced before any guessing happens. The givens are never
        // changed afterward: their domains stay a single candidate, which is that given digit.
        var domains = new int[CellCount];
        Array.Fill(domains, AllCandidates);

        for (int cell = 0; cell < CellCount; cell++)
        {
            int value = _givens[cell / Size, cell % Size];
            if (value != 0 && !Assign(domains, cell, value)) return solutions;
        }

        Search(domains, solutions);
        return solutions;
    }

    /// <summary>
    /// Recursive backtracking search. domains is already consistent
    /// </summary>
    private static void Search(int[] domains, List<int[,]> solutions)
    {
        // MRV heuristic: choose the unassigned cell with the fewest remaining candidates.
        int chosen = -1;
        int fewest = int.MaxValue;

        for (int cell = 0; cell < CellCount; cell++)
        {
            int count = CountCandidates(domains[cell]);
            if (count > 1 && count < fewest)
            {
                chosen = cell;
                fewest = count;
            }
        }

        // Every cell has exactly one candidate, so every variable is assigned and the grid is a solution.
        if (chosen == -1)
        {
            solutions.Add(ToGrid(domains));
            return;
        }

        // Guess each candidate of the chosen cell in turn. Each guess works on a copy of the domains, so
        // backtracking restores the previous state simply by discarding the copy
        // Every candidate is tried, even after a solution is found, so that all solutions are found.
        for (int digit = 1; digit <= Size; digit++)
        {
            if ((domains[chosen] & Bit(digit)) == 0) continue;

            var copy = (int[])domains.Clone();

            if (Assign(copy, chosen, digit)) Search(copy, solutions);
        }
    }

    /// <summary> Assigns digit to cell by eliminating every other candidate from its domain. </summary>
    private static bool Assign(int[] domains, int cell, int digit)
    {
        int others = domains[cell] & ~Bit(digit);

        for (int d = 1; d <= Size; d++)
        {
            if ((others & Bit(d)) != 0 && !Eliminate(domains, cell, d)) return false;
        }
        return true;
    }

    /// <summary> Removes digit from the domain of cell and propagates the consequences. </summary>
    private static bool Eliminate(int[] domains, int cell, int digit)
    {
        int bit = Bit(digit);

        // Already eliminated: nothing to propagate.
        if ((domains[cell] & bit) == 0) return true;

        domains[cell] &= ~bit;

        // Constraint check: a cell with no candidates left cannot be given any value.
        if (domains[cell] == 0) return false;

        // The cell is down to one candidate, so that candidate is now the cell's value. No peer may share it,
        // so eliminate it from every peer. This may reduce peers to one candidate, which recursively propagates.
        if (CountCandidates(domains[cell]) == 1)
        {
            int value = ToDigit(domains[cell]);

            foreach (int peer in _peers[cell])
            {
                if (!Eliminate(domains, peer, value)) return false;
            }
        }

        // Only-place-left rule: every unit needs the digit somewhere. Now that it's gone from this cell, check where
        // else it can go in each of this cell's units. Nowhere is a contradiction; exactly one place means it must go there.
        foreach (int[] unit in _unitsOf[cell])
        {
            int places = 0;
            int lastPlace = -1;

            foreach (int other in unit)
            {
                if ((domains[other] & bit) != 0)
                {
                    places++;
                    lastPlace = other;
                }
            }

            if (places == 0) return false;
            if (places == 1 && !Assign(domains, lastPlace, digit)) return false;
        }

        // Naked pairs: if this cell is down to two candidates and another cell in the same unit has exactly the same two,
        // those two cells must hold those two digits between them, so no other cell in the unit can hold either one.
        if (CountCandidates(domains[cell]) == 2)
        {
            int pair = domains[cell]; // snapshot: recursive eliminations below may change domains[cell]

            foreach (int[] unit in _unitsOf[cell])
            {
                foreach (int partner in unit)
                {
                    if (partner == cell || domains[partner] != pair) continue;

                    foreach (int other in unit)
                    {
                        if (other == cell || other == partner) continue;

                        for (int d = 1; d <= Size; d++)
                        {
                            if ((pair & Bit(d)) != 0 && !Eliminate(domains, other, d)) return false;
                        }
                    }
                    break; // a unit can only hold one partner with an identical pair
                }
            }
        }

        return true;
    }

    /// <summary>Converts fully assigned domains into a 9x9 grid of digits.</summary>
    private static int[,] ToGrid(int[] domains)
    {
        var grid = new int[Size, Size];
        for (int cell = 0; cell < CellCount; cell++)
        {
            grid[cell / Size, cell % Size] = ToDigit(domains[cell]);
        }
        return grid;
    }

    private static int[][] BuildUnits()
    {
        var units = new int[27][];
        for (int i = 0; i < Size; i++)
        {
            units[i]      = new int[Size]; // row i
            units[9 + i]  = new int[Size]; // column i
            units[18 + i] = new int[Size]; // block i
            for (int j = 0; j < Size; j++)
            {
                units[i][j]      = i * Size + j;
                units[9 + i][j]  = j * Size + i;
                units[18 + i][j] = (i / 3 * 3 + j / 3) * Size + (i % 3 * 3 + j % 3);
            }
        }
        return units;
    }

    private static int[][][] BuildUnitsOf()
    {
        var unitsOf = new int[CellCount][][];
        for (int cell = 0; cell < CellCount; cell++)
        {
            unitsOf[cell] = [.. _units.Where(unit => unit.Contains(cell))];
        }
        return unitsOf;
    }

    private static int[][] BuildPeers()
    {
        var peers = new int[CellCount][];
        for (int cell = 0; cell < CellCount; cell++)
        {
            peers[cell] = [.. _unitsOf[cell].SelectMany(unit => unit).Distinct().Where(other => other != cell)];
        }
        return peers;
    }

    private static int Bit(int digit) => 1 << (digit - 1);

    private static int CountCandidates(int mask) => System.Numerics.BitOperations.PopCount((uint)mask);

    /// <summary>Returns the digit of a single-candidate mask.</summary>
    private static int ToDigit(int mask) => System.Numerics.BitOperations.TrailingZeroCount(mask) + 1;
}
