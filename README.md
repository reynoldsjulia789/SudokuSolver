# SudokuSolver

This is a program that solves Sudoku puzzles using a Constraint Satisfaction Problem (CSP) approach.
The program reads the specified puzzle from a text file in the directory specified below, finds all the unique valid solutions, and prints the results to a separate text file in the output directory.
While a standard sudoku puzzle is designed to only have one solution, this puzzle solver is designed to find all possible solutions if there does happen to multiple possible solutions.
This assignment provides practice with constraint modeling, recursive backtracking, file input/output, and systematic testing.

### Specs

* **Language Used:** C#
* **Search Algorithm:** Recursive Backtracking
* **Additional Heuristics Used:** MRV
* **Optimizations:** Naked Pairs

## Test Cases

### <ins>Case 1</ins>: puzzle1.txt, 1 solution
```
5 3   |   7   |      
6     | 1 9 5 |  
  9 8 |       |   6  
------+-------+------
8     |   6   |     3
4     | 8   3 |     1
7     |   2   |     6
------+-------+------
  6   |       | 2 8  
      | 4 1 9 |     5
      |   8   |   7 9
```

### <ins>Case 2</ins>: puzzle2.txt, 2 solutions
```
2 9 5 | 7 4 3 | 8 6 1
4 3 1 | 8 6 5 | 9    
8 7 6 | 1 9 2 | 5 4 3
------+-------+------
3 8 7 | 4 5 9 | 2 1 6
6 1 2 | 3 8 7 | 4 9 5 
5 4 9 | 2 1 6 | 7 3 8
------+-------+------ 
7 6 3 | 5 2 4 | 1 8 9
9 2 8 | 6 7 1 | 3 5 4
1 5 4 | 9 3 8 | 6    
```
## How to Run
1. Place .txt files containing sudoku puzzles into the directory `.\Puzzles\UnsolvedPuzzles`
    * Each .txt file must contain exactly **one** sudoku puzzle, which meets the following specs:
        * There must be nine rows, with nine integers per row
        * Each integers must be separated by a space
        * Values representing given numbers must be between 1 and 9
        * 0 must be used to represent empty cells
2. Use your preferred run method to run the program

### Command Line:

1. Navigate to the folder containing the .csproj file (currently, it is in the inner "SudokuSolver" folder)
2. Run the following command: `dotnet run [--filename <filename.txt>]`
    * note that c# and dotnet 10 are required
    * the `--filename <filename.txt>` argument is not required 
        * If included, the specified file must be located in the `UnsolvedPuzzles` directory
        * If not included, the program will find the solution(s) to all puzzles in the directory
    * example command `dotnet run --filename puzzle1.txt`

### Visual Studio:

1. Navigate to the folder containing the solution file (.slnx)
2. Open the file in Visual Studio
3. Locate the play button
4. Alter the run settings
    * Click the dropdown next to the play button and click the menu item that says "BrokenKeypad Debug Properties"
    * Enter arguments in the section labeled "Command line arguments" (see the Command Line run instructions for args)
    * Click the Browse... button next to the "Working directory" section and choose the directory with the .csproj file in it
       * Without it, the files containing the unsolved puzzles will not be able to be found
5. Exit out of the Launch Profiles popup. It should save automatically.
6. Click the play button