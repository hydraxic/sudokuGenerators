using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperSudokuGenerator
{
    class Program
    {
        // Define constants for the size of the grid and sub-grids
        const int GRID_SIZE = 9;
        const int SUBGRID_SIZE = 3;

        // Define a 2D array to represent the sudoku grid
        static int[,] grid = new int[GRID_SIZE, GRID_SIZE];

        static void Main(string[] args)
        {
            // Generate a valid hyper sudoku puzzle
            if (GenerateHyperSudoku(grid, 0, 0))
            {
                // Print the grid to the console
                PrintGrid(grid);
            }
            else
            {
                Console.WriteLine("No valid solution found.");
            }
        }

        // Define a function to generate a valid hyper sudoku puzzle
        static bool GenerateHyperSudoku(int[,] grid, int row, int col)
        {
            // If we have reached the end of the grid, return true
            if (row == GRID_SIZE)
            {
                return true;
            }

            // If the current cell is not empty, move to the next cell
            if (grid[row, col] != 0)
            {
                return (col == GRID_SIZE - 1) ? GenerateHyperSudoku(grid, row + 1, 0) : GenerateHyperSudoku(grid, row, col + 1);
            }

            // Try filling in the current cell with a number from 1-9
            for (int num = 1; num <= 9; num++)
            {
                // Check if the number is valid for the current position
                if (IsValid(grid, row, col, num))
                {
                    // Fill in the cell and recursively call the function with the updated grid and next position
                    grid[row, col] = num;
                    if (GenerateHyperSudoku(grid, row, col))
                    {
                        return true;
                    }
                }
            }

            // If we have tried all numbers and none of them lead to a valid puzzle, undo the last move and return false
            grid[row, col] = 0;
            return false;
        }

        // Define a function to check if a number is valid for a given position in the grid
        bool IsValid(int[,] grid, int row, int col, int num)
        {
            // Check if the number appears in the same row, column, 3x3 block, or hyper-region
            for (int i = 0; i < GRID_SIZE; i++)
            {
                if (grid[row, i] == num || grid[i, col] == num || grid[(row / SUBGRID_SIZE) * SUBGRID_SIZE + i / SUBGRID_SIZE, (col / SUBGRID_SIZE) * SUBGRID_SIZE + i % SUBGRID_SIZE] == num)
                {
                    return false;
                }
            }
            return true;
        }

        // Define a function to print the grid to the console
        static void PrintGrid(int[,] grid)
        {
            for (int row = 0; row < GRID_SIZE; row++)
            {
                for (int col = 0; col < GRID_SIZE; col++)
                {
                    Console.Write(grid[row, col]);
                }
                Console.WriteLine();
            }
        }
    }
}