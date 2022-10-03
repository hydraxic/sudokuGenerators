/* C# program for Sudoku generator */
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

public class Sudoku
{
	int[,] mat;
	int N; // number of columns/rows.
	int SRN; // square root of N
	int K; // No. Of missing digits
	int EO;

	// Constructor
	public Sudoku(int N, int K, int EO)
	{
		this.N = N;
		this.K = K;
		this.EO = EO;

		// Compute square root of N
		double SRNd = Math.Sqrt(N);
		SRN = (int)SRNd;

		mat = new int[N,N];
	}

	// Sudoku Generator
	public void fillValues(int actualIter)
	{
		// Fill the diagonal of SRN x SRN matrices
		fillDiagonal();

		// Fill remaining blocks
		fillRemaining(0, SRN);


		/*string path = @"OUTPUTSUDOKU\Solved\" + actualIter.ToString() + ".txt";
		using (StreamWriter sw = File.CreateText(path))
		{
			for (int i = 0; i<N; i++)
			{
				for (int j = 0; j<N; j++)
				{
					if (j == N-1)
					{
						sw.Write(mat[i, j]);
					}
					else
					{
						sw.Write(mat[i,j] + " ");
					}
				}
				if (i != N-1)
				{
					sw.Write("\n");
				}
			}
		sw.Flush();
		sw.Close();
		}
*/
		// Remove Randomly K digits to make game
		removeKDigits();
	}

	// Fill the diagonal SRN number of SRN x SRN matrices
	void fillDiagonal()
	{

		for (int i = 0; i<N; i=i+SRN)

			// for diagonal box, start coordinates->i==j
			fillBox(i, i);
	}

	// Returns false if given 3 x 3 block contains num.
	bool unUsedInBox(int rowStart, int colStart, int num)
	{
		for (int i = 0; i<SRN; i++)
			for (int j = 0; j<SRN; j++)
				if (mat[rowStart+i,colStart+j]==num)
					return false;

		return true;
	}

	// Fill a 3 x 3 matrix.
	void fillBox(int row,int col)
	{
		int num;
		for (int i=0; i<SRN; i++)
		{
			for (int j=0; j<SRN; j++)
			{
				do
				{
					num = randomGenerator(N);
				}
				while (!unUsedInBox(row, col, num));

				mat[row+i,col+j] = num;
			}
		}
	}

	// Random generator
	int randomGenerator(int num)
	{
		Random rand = new Random();
		return (int) Math.Floor((double)(rand.NextDouble()*num+1));
	}

	// Check if safe to put in cell
	bool CheckIfSafe(int i,int j,int num)
	{
		return (unUsedInRow(i, num) &&
				unUsedInCol(j, num) &&
				unUsedInBox(i-i%SRN, j-j%SRN, num));
	}

	// check in the row for existence
	bool unUsedInRow(int i,int num)
	{
		for (int j = 0; j<N; j++)
		if (mat[i,j] == num)
				return false;
		return true;
	}

	// check in the row for existence
	bool unUsedInCol(int j,int num)
	{
		for (int i = 0; i<N; i++)
			if (mat[i,j] == num)
				return false;
		return true;
	}

	// A recursive function to fill remaining
	// matrix
	bool fillRemaining(int i, int j)
	{
		// System.out.println(i+" "+j);
		if (j>=N && i<N-1)
		{
			i = i + 1;
			j = 0;
		}
		if (i>=N && j>=N)
			return true;

		if (i < SRN)
		{
			if (j < SRN)
				j = SRN;
		}
		else if (i < N-SRN)
		{
			if (j==(int)(i/SRN)*SRN)
				j = j + SRN;
		}
		else
		{
			if (j == N-SRN)
			{
				i = i + 1;
				j = 0;
				if (i>=N)
					return true;
			}
		}

		for (int num = 1; num<=N; num++)
		{
			if (CheckIfSafe(i, j, num))
			{
				mat[i,j] = num;
				if (fillRemaining(i, j+1))
					return true;

				mat[i,j] = 0;
			}
		}
		return false;
	}

	// Remove the K no. of digits to
	// complete game
	public void removeKDigits()
	{
		int count = K;
		while (count != 0)
		{
			int cellId = randomGenerator(N*N)-1;

			// System.out.println(cellId);
			// extract coordinates i and j
			int i = (cellId/N);
			int j = cellId%9;
			if (j != 0)
				j = j - 1;

			// System.out.println(i+" "+j);
			if (mat[i,j] != 0)
			{
				count--;
				mat[i,j] = 0;
			}
		}
	}

	public void chooseEvenOdd(int actualIter, string diff)
	{

		List<(int, int)> evenArray = new List<(int, int)>();
		List<(int, int)> oddArray = new List<(int, int)>();

		Random rnd = new Random();

		List<(int, int)> randomVals = new List<(int, int)>();

		for (int num = 0; num < 50; num++)
		{
			(int, int) randomTuple = (rnd.Next(0, 9), rnd.Next(0, 9));

			if (!randomVals.Contains(randomTuple))
			{
				randomVals.Add(randomTuple);
			}
		}

		foreach ((int, int) val in randomVals)
		{
			//Console.WriteLine(val);

			if (mat[val.Item1, val.Item2] % 2 == 0)
			{
				if (evenArray.Count < EO + 1)
				{
					evenArray.Add(val);
				}
			}
			else
			{
				if (oddArray.Count < EO + 1)
				{
					oddArray.Add(val);
				}
			}
		}

		string path = string.Format(@"OUTPUTSUDOKU\EvenOddSudoku\{0}\EvenOdd\{1}.txt", diff, actualIter);
		using (StreamWriter sw = File.CreateText(path))
		{
			foreach ((int, int) even in evenArray)
			{
				sw.WriteLine(string.Format("{0}, {1}", even.Item1, even.Item2));
			}
			sw.WriteLine(";");
			foreach ((int, int) odd in oddArray)
			{
				sw.WriteLine(string.Format("{0}, {1}", odd.Item1, odd.Item2));
			}
		}


		
		//int count = 0;


		/*do
		{
			if (mat[randomI, randomJ] % 2 == 0)
			{
				if (evenArray.Count(s => s != (0, 0)) < EO)
				{
					//Console.WriteLine((randomI, randomJ));
					evenArray[Array.FindIndex(evenArray, i => i == (0, 0))] = (randomI, randomJ);
				}
			}
			else
			{
				if (oddArray.Count(s => s != (0, 0)) < EO)
				{
					//Console.WriteLine((randomI, randomJ));
					oddArray[Array.FindIndex(oddArray, i => i == (0, 0))] = (randomI, randomJ);
				}
			}

			count++;
		} while (evenArray.Count(s => s != (0, 0)) < EO && oddArray.Count(s => s != (0, 0)) < EO);
	*/
		foreach ((int, int) tup in evenArray)
		{
			Console.WriteLine(tup);
			Console.WriteLine(mat[tup.Item1, tup.Item2]);
		}

		

	}

	// Print sudoku
	public void printSudoku()
	{
		for (int i = 0; i<N; i++)
		{
			for (int j = 0; j<N; j++)
				Console.Write(mat[i,j] + " ");
			Console.WriteLine();
		}
		Console.WriteLine();
	}

	public void unsolvedPrintSudoku(int actualIter, string diff)
    {
        string path = string.Format(@"OUTPUTSUDOKU\EvenOddSudoku\{0}\Unsolved\{1}.txt", diff, actualIter);
        using (StreamWriter sw = File.CreateText(path))
        {
            for (int i = 0; i<N; i++)
            {
                for (int j = 0; j<N; j++)
                {
                    if (j == N-1)
                    {
                        sw.Write(mat[i, j]);
                    }
                    else
                    {
                        sw.Write(mat[i,j] + " ");
                    }
                }
                if (i != N-1)
                {
                    sw.Write("\n");
                }
            }
        sw.Flush();
        sw.Close();
        }
    }

	// Driver code
	public static void Main(string[] args)
	{
		Random rnd = new Random();

		int N = 9, K = rnd.Next(30, 35), EO = 10;
		string dif = "Very Easy";

		for (int gen = 0; gen < 1; gen++)
		{
			Sudoku sudoku = new Sudoku(N, K, EO);
			sudoku.fillValues(gen + 1, dif);
			sudoku.chooseEvenOdd(gen + 1);
			sudoku.unsolvedPrintSudoku(gen + 1, dif);
			sudoku.printSudoku();
		}
	}
}

// andahfanfknfdf812moiro2f42i f34if89rfcataraqui3getg dr. 1209asnbdfoyw8 ashfordf3ihg78fh r32place

// This code is contributed by rrrtnx. Edited by hydraxic to make Even and Odd Sudoku.
