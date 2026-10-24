using System.Reflection.PortableExecutable;

namespace SudokuSolver;

public class Program
{
    internal const string _inputDirectory = "../Puzzles/UnsolvedPuzzles";
    internal const string _outputDirectory = "../Puzzles/Solutions";

    internal static Dictionary<string, int[,]> _files = []; // filename, parsed file

    public static void Main(string[] args)
    {
        // start up
        PrintRunInstructions();

        // get filenames and parse file(s)
        foreach (string filename in GetFileNames(args))
        {
            try
            {
                var puzzle = ParseFile(filename);

                _files[filename] = puzzle;
            }
            catch (Exception exception)
            {
                PrintErrorMessage(exception.Message);
            }
        }

        if (_files.Count == 0)
        {
            PrintErrorMessage($"No usable files were found. " +
                $"Please double check that the puzzle file is in {_inputDirectory} " +
                $"and follows the specified formatting");

            return;
        }

        // solve puzzles

        // print solutions
        Console.WriteLine(); // formatting in console logs

        //PrintSolution(solutions, inputFileName);

        Console.WriteLine();
    }

    /// <summary>
    /// Returns a list of the files to read from the input puzzle directory.
    /// </summary>
    internal static List<string> GetFileNames(string[]? args = null, string inputDirectory = _inputDirectory)
    {
        List<string> filenames = [];

        if ((args is not null) && (args.Length == 2) && (args[0] == "--filename"))
        {
            filenames.Add(args[1].Trim());
        }
        else
        {
            foreach (var filepath in Directory.GetFiles(inputDirectory))
            {
                filenames.Add(Path.GetFileName(filepath));
            }
        }
        return filenames;
    }

    /// <summary>
    /// Parses the sudoku puzzle from the .txt file specified by the filename.
    /// </summary>
    internal static int[,] ParseFile(string filename, string inputDirectory = _inputDirectory)
    {
        var file = File.ReadLines($"{inputDirectory}/{filename}");

        if (file.Count() != 9) throw new ArgumentException(
            $"Program.ParseFile: {filename} has an incorrect number of lines");

        int lineNumber = 0;
        var puzzle = new int[9, 9];

        foreach (var line in file)
        {
            var chars = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

            if (chars is null || chars.Count() != 9) throw new ArgumentException(
                $"Program.ParseFile: {filename} has an incorrect number of characters in line {lineNumber}");

            int charNumber = 0;

            foreach (var character in chars)
            {
                if (int.TryParse(character, out int value) is false || value < 0 || value > 9)
                {
                    throw new ArgumentException(
                        $"Program.ParseFile: {filename}({lineNumber}, {charNumber}) failed to parse");
                }

                puzzle[lineNumber, charNumber] = value;
                charNumber++;
            }

            lineNumber++;
        }

        return puzzle;
    }

    /// <summary>
    /// Writes all provided solutions to a text file in the output directory
    /// </summary>
    /// <param name="solutions">A list of sudoku puzzle solutions</param>
    /// <param name="filename">The name of the input file associated with the set of solutions</param>
    internal static void PrintSolution(List<int[,]> solutions, string filename)
    {
        Directory.CreateDirectory(_outputDirectory);
        var solutionFilePath = $"{_outputDirectory}/{Path.GetFileNameWithoutExtension(filename)}_solutions.txt";

        using StreamWriter writer = new(solutionFilePath);

        writer.WriteLine($"Input file: {filename}");

        for (int solution = 0; solution < solutions.Count; solution++)
        {
            int charnum = 0;
            writer.WriteLine();
            writer.WriteLine($"Solution {solution + 1}:");

            foreach (var num in solutions[solution])
            {
                writer.Write(num);
                writer.Write(' ');

                charnum++;

                if (charnum == 9)
                {
                    writer.WriteLine();
                    charnum = 0;
                }
            }
        }

        writer.WriteLine($"\r\nTotal solutions: {solutions.Count}");
        writer.Close();

        Console.WriteLine($"Solutions for {filename} written to {solutionFilePath}");
    }

    /// <summary>
    /// Prints instructions on how to run the program
    /// </summary>
    private static void PrintRunInstructions()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;

        Console.WriteLine();
        Console.WriteLine("To run:");
        Console.WriteLine($"1) Place.txt files containing sudoku puzzles into the directory {_inputDirectory}");
        Console.WriteLine("    Each.txt file must contain exactly one sudoku puzzle, which meets the following specs:");
        Console.WriteLine("        * There must be nine rows, with nine integers per row");
        Console.WriteLine("        * Each integers must be separated by a space");
        Console.WriteLine("        * Values representing given numbers must be between 1 and 9");
        Console.WriteLine("        * 0 must be used to represent empty cells");
        Console.WriteLine($"2) If using the cli, navigate to the directory containing the .csproj file and run the following command:");
        Console.WriteLine("    dotnet run [--filename <filename.txt>]");
        Console.WriteLine("        * the `--filename <filename.txt>` argument is not required");
        Console.WriteLine("        * If included, the specified file must be located in the directory listed above");
        Console.WriteLine("        * If not included, the program will find the solution(s) to all puzzles in the directory");

        Console.ResetColor();
    }

    /// <summary>
    /// Prints the specified error message to the console
    /// </summary>
    private static void PrintErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;

        Console.WriteLine();
        Console.WriteLine(message);
        Console.WriteLine();

        Console.ResetColor();
    }
}
