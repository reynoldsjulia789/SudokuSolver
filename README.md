# SudokuSolver

This is a program that solves Sudoku puzzles using a Constraint Satisfaction Problem (CSP) approach.
The program reads the specified puzzle from a text file in the directory specified below, finds all the unique valid solutions, and prints the results to a separate text file in the output directory.
This assignment provides practice with constraint modeling, recursive backtracking, file input/output, and systematic testing.

### Specs

* **Language Used:** C#
* **CSP Model:**
* **Search Algorithm:**
* **Additional Heuristics Used:**

## Test Cases

### <ins>Case 1</ins>: puzzle1.txt, 1 Solution

| 5 | 3 |   |   | 7 |   |   |   |   |
|---|---|---|---|---|---|---|---|---|
| 6 |   |   | 1 | 9 | 5 |   |   |   |
|   | 9 | 8 |   |   |   |   | 6 |   |
| 8 |   |   |   | 6 |   |   |   | 3 |
| 4 |   |   | 8 |   | 3 |   |   | 1 |
| 7 |   |   |   | 2 |   |   |   | 6 |
|   | 6 |   |   |   |   | 2 | 8 |   |
|   |   |   | 4 | 1 | 9 |   |   | 5 |
|   |   |   |   | 8 |   |   | 7 | 9 |

### <ins>Case 2</ins>: _ Solutions

|   |   |   |   |   |   |   |   |   |
|---|---|---|---|---|---|---|---|---|
|   |   |   |   |   |   |   |   |   |
|   |   |   |   |   |   |   |   |   |
|   |   |   |   |   |   |   |   |   |
|   |   |   |   |   |   |   |   |   |
|   |   |   |   |   |   |   |   |   |
|   |   |   |   |   |   |   |   |   |
|   |   |   |   |   |   |   |   |   |
|   |   |   |   |   |   |   |   |   |

## How to Run
1. Place .txt files containing coordinates into the directory `.\UnsolvedPuzzles`
    Each .txt file must contain exactly **one** sudoku puzzle, which meets the following specs:
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
        * If not included, the 

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